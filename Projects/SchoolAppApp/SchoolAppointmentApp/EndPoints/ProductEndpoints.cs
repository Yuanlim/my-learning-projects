using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;
using SchoolAppointmentApp.Mapping;

namespace SchoolAppointmentApp.EndPoints;

public static class Products
{
  public static RouteGroupBuilder ProductEndpoints(this RouteGroupBuilder shoppingRoute)
  {
    var group = shoppingRoute.MapGroup("/Product");

    group.MapPost("/New", async (
        CreateProductDto dto,
        ClaimsPrincipal user,
        MyAppDbContext dbContext,
        CancellationToken ct,
        IErrorResults errorHandler,
        HttpContext hc,
        UnAuthorizedValidator validator
    ) =>
    {
      var id = user.FindFirstValue(ClaimTypes.NameIdentifier);

      // Validation of user
      (bool auth, _) = await validator.IsResults<Teacher>(
        expectedRole: Roles.schoolPrincipal,
        user: user,
        ct: ct
      );

      if (!auth)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: $"Unautherized, user doesnt existed",
          hc: hc
        );

      Product product = new()
      {
        ProductName = dto.ProductName,
        ProductImageRoot = dto.ProductImageRoot,
        Description = dto.Description,
        PointCost = dto.PointCost,
        AvailableQuantity = dto.Quantity
      };

      await dbContext.AddAsync(product, ct);
      await dbContext.SaveChangesAsync(ct);

      return Results.Created($"/Product/{dto.ProductName}", dto.ProductName);
    }).RequireAuthorization("PrincipalAllowed");

    group.MapPatch("/Change", async (
        PatchProductDto dto,
        ClaimsPrincipal user,
        MyAppDbContext dbContext,
        CancellationToken ct,
        IErrorResults errorHandler,
        HttpContext hc,
        UnAuthorizedValidator validator
    ) =>
    {
      var id = user.FindFirstValue(ClaimTypes.NameIdentifier);

      // Validation of user
      (bool auth, _) = await validator.IsResults<Teacher>(
        expectedRole: Roles.schoolPrincipal,
        user: user,
        ct: ct
      );

      if (!auth)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: $"Unautherized, user doesnt existed",
          hc: hc
        );

      var theProduct = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId, ct);
      if (theProduct is null) return Results.NotFound("No such product");

      theProduct.ProductName = dto.ProductName ?? theProduct.ProductName;
      theProduct.ProductImageRoot = dto.ProductImageRoot ?? theProduct.ProductImageRoot;
      theProduct.Description = dto.Description ?? theProduct.Description;
      theProduct.PointCost = dto.PointCost ?? theProduct.PointCost;
      theProduct.AvailableQuantity = dto.Quantity ?? theProduct.AvailableQuantity;

      await dbContext.SaveChangesAsync(ct);

      return Results.Ok($"Product id {theProduct.ProductId} propreties Changed");
    }).RequireAuthorization("PrincipalAllowed");

    group.MapDelete("/Remove/{productId}", async (
        int productId,
        MyAppDbContext dbContext,
        CancellationToken ct,
        ClaimsPrincipal user,
        IErrorResults errorHandler,
        HttpContext hc,
        UnAuthorizedValidator validator
    ) =>
    {
      var id = user.FindFirstValue(ClaimTypes.NameIdentifier);

      // Validation of user
      (bool auth, _) = await validator.IsResults<SchoolPrincipal>(
        expectedRole: Roles.schoolPrincipal,
        user: user,
        ct: ct
      );

      if (!auth)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: $"Unautherized, user doesnt existed",
          hc: hc
        );

      await dbContext.Products.Where(p => p.ProductId == productId)
                              .ExecuteDeleteAsync(ct);

      await dbContext.SaveChangesAsync();

      return Results.NoContent();
    }).RequireAuthorization("PrincipalAllowed");

    group.MapGet("/Get/{id}", async (
      int id,
      UnAuthorizedValidator validator,
      ClaimsPrincipal user,
      CancellationToken ct,
      IErrorResults errorHandler,
      HttpContext hc,
      MyAppDbContext dbContext,
      IProductListClasses productHandler
    ) =>
    {
      (bool isTeacher, _) = await validator.IsResults<Teacher>(
        expectedRole: Roles.teacher,
        user: user,
        ct: ct
      );
      (bool isPrincipal, _) = await validator.IsResults<SchoolPrincipal>(
        expectedRole: Roles.schoolPrincipal,
        user: user,
        ct: ct
      );

      if (!isTeacher && !isPrincipal)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: $"Unautherized, user doesnt existed",
          hc: hc
        );

      Product? product = await productHandler.GetProductAsync(productId: id, ct: ct);
      if (product is null)
        return errorHandler.NotFoundResult(
          title: "Get Product Failed",
          message: "Product didn't exist",
          hc
        );

      return Results.Ok(product.ToProductDto());
    }).RequireAuthorization("TeacherOrPrincipalAllowed");

    group.MapGet("/GetList/{SearchString}&{RequestExpandTimes}", async (
      string? SearchString,
      int RequestExpandTimes,
      CancellationToken ct,
      IProductListClasses GetProductHandler,
      MyAppDbContext dbContext,
      ClaimsPrincipal user,
      IErrorResults errorHandler,
      HttpContext hc,
      UnAuthorizedValidator validator
    ) =>
    {
      // User input nothing or white space only, nothing happen
      // if (string.IsNullOrWhiteSpace(searchString)) return Results.NoContent();

      // Validation of user
      (bool auth, _) = await validator.IsResults<Teacher>
      (
        expectedRole: Roles.teacher,
        user: user,
        ct: ct
      );

      if (!auth)
        return errorHandler.UnauthorizedResult
        (
          title: "Reported fake user",
          message: "Unautherized, user doesnt existed",
          hc: hc
        );

      var productList = await GetProductHandler.GetProductList
      (
        searchString: SearchString,
        limitStep: RequestExpandTimes,
        maximumProductListedEachExpand: 5,
        ct: ct
      );

      return Results.Ok(productList);
    }).RequireAuthorization("TeacherOrPrincipalAllowed");

    return group;
  }
}