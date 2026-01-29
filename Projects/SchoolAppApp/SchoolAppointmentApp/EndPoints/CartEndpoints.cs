using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;
using SchoolAppointmentApp.Mapping;

namespace SchoolAppointmentApp.EndPoints;

public static class MyCart
{
  public static RouteGroupBuilder CartEndpoints(this RouteGroupBuilder shoppingRoute)
  {
    var group = shoppingRoute.MapGroup("/Cart");

    // Get my own cart 
    group.MapGet("/Get", async (
      ClaimsPrincipal user,
      HttpContext hc,
      UnAuthorizedValidator validator,
      IErrorResults errorHandler,
      CancellationToken ct,
      MyAppDbContext dbContext
    ) =>
    {
      // Validation guard
      (bool auth, Teacher? teacher) = await validator.IsResults<Teacher>(
        expectedRole: Roles.teacher,
        user: user,
        ct: ct
      );

      if (teacher is null)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: $"Unautherized, user doesnt existed",
          hc: hc
        );

      // Get chart
      Cart? cart = await dbContext.Carts.AsNoTracking()
                                        .Include(c => c.CartProductList)
                                          .ThenInclude(cpl => cpl.Product)
                                        .Where(
                                          c =>
                                          c.CustomerId == teacher.TeacherId &&
                                          !c.Ordered
                                        )
                                        .SingleOrDefaultAsync(ct);

      return Results.Ok(cart.ToCartDto());

    }).RequireAuthorization("TeacherAllowed");


    // I delete something from cart
    group.MapDelete("/Product/Delete/{productId}", async (
      int productId,
      ClaimsPrincipal user,
      IErrorResults errorHandler,
      HttpContext hc,
      CancellationToken ct,
      UnAuthorizedValidator validator,
      IGetCartItem cartItemHandler,
      MyAppDbContext dbContext
    ) =>
    {
      // Validation of user
      (bool auth, Teacher? teacher) = await validator.IsResults<Teacher>(
        expectedRole: Roles.teacher,
        user: user,
        ct: ct
      );

      if (teacher is null)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: "Unautherized, user doesnt existed",
          hc: hc
        );

      // Find Cart as tracking
      IQueryable<CartItem>? cartItemQuery = await cartItemHandler.GetCartItemQueryAsync(
        teacher: teacher,
        productId: productId,
        cancellationToken: ct
      );

      if (cartItemQuery is null)
        return errorHandler.NotFoundResult(
          title: "Get request reported empty.",
          message: "Teacher has no unordered cart.",
          hc: hc,
          user: teacher.User
        );

      await cartItemQuery.ExecuteDeleteAsync();

      await dbContext.SaveChangesAsync(ct);

      return Results.NoContent();
    }).RequireAuthorization("TeacherAllowed");


    // I change quantity
    group.MapPatch("/Product/Patch", async (
      WishItemDto dto,
      ClaimsPrincipal user,
      IErrorResults errorHandler,
      HttpContext hc,
      CancellationToken ct,
      UnAuthorizedValidator validator,
      IGetCartItem cartItemHandler,
      IProductListClasses productHandler,
      MyAppDbContext dbContext
    ) =>
    {
      (bool auth, Teacher? teacher) = await validator.IsResults<Teacher>(
        expectedRole: Roles.teacher,
        user: user,
        ct: ct
      );

      if (teacher is null)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: "Unautherized, user doesnt existed",
          hc: hc
        );

      // Get Product quantity， check ask for quantity larger than available quantity
      Product? product = await productHandler.GetProductAsync(productId: dto.ProductId, ct);
      if (product is null)
        return errorHandler.NotFoundResult(
          title: "Get Product Failed",
          message: "Product didn't exist",
          hc
        );
      if (product.AvailableQuantity < dto.Quantity)
        return errorHandler.BadReqResult(
          title: "Insufficient",
          message: "No enough stock",
          hc: hc,
          user: teacher.User
        );

      // Find Cart as tracking
      IQueryable<CartItem>? cartItemQuery = await cartItemHandler.GetCartItemQueryAsync(
        teacher: teacher,
        productId: dto.ProductId,
        cancellationToken: ct
      );
      if (cartItemQuery is null)
        return errorHandler.NotFoundResult(
          title: "Get request reported empty.",
          message: "Teacher has no unordered cart.",
          hc: hc,
          user: teacher.User
        );

      // Treat 0 as delete
      if (dto.Quantity == 0)
      {
        await cartItemQuery.ExecuteDeleteAsync(ct);
        return Results.NoContent();
      }
      else
      {
        CartItem? cartItem = await cartItemQuery.FirstOrDefaultAsync(ct);

        if (cartItem is null)
          return errorHandler.NotFoundResult(
            title: "Get request reported empty.",
            message: "Teacher has no such item in cart.",
            hc: hc,
            user: teacher.User
          );

        cartItem.Quantity = dto.Quantity;
        await dbContext.SaveChangesAsync(ct);
        return Results.Ok(cartItem.ToCartItemDto());
      }
    }).RequireAuthorization("TeacherAllowed");

    return group;
  }
}