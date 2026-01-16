using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;
using SchoolAppointmentApp.Mapping;

namespace SchoolAppointmentApp.EndPoints;

public static class MyOrder
{
  public static RouteGroupBuilder OrderEndpoints(this RouteGroupBuilder shoppingRoute)
  {
    var group = shoppingRoute.MapGroup("/Order");

    // I order something endpoint
    group.MapPost("/Placed", async (
        ClaimsPrincipal user,
        MyAppDbContext dbContext,
        CancellationToken ct,
        IOrderStatus statusHandler,
        IErrorResults errorHandler,
        HttpContext hc,
        UnAuthorizedValidator validator
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
          message: $"Unautherized, user doesnt existed",
          hc: hc
        );

      Cart? cartInfo = await dbContext.Carts.Where(
                                              c =>
                                              c.CustomerId == teacher.TeacherId
                                              && c.Ordered == false
                                            )
                                            .Include(c => c.CartProductList)
                                              .ThenInclude(ci => ci.Product)
                                            .FirstOrDefaultAsync(ct);

      if (cartInfo is null)
        return errorHandler.NotFoundResult(
            title: "Doesn't exist",
            message: "You have no unorder cart.",
            hc: hc,
            user: teacher.User
          );

      // Insufficient points detection Or
      // Insufficient Quantity(Wished, has quantity but order too slow, other user got it.)
      int total = 0;
      foreach (var item in cartInfo.CartProductList)
      {
        total += item.Quantity * item.Product.PointCost;
        if (item.Product.AvailableQuantity < item.Quantity)
          return errorHandler.BadReqResult(
            title: "Quantity issue",
            message: $"{item.Product.ProductName} has insufficient stock",
            hc: hc,
            user: teacher.User
          );
        item.Product.AvailableQuantity -= item.Quantity;
      }
      if (total > teacher.Points)
        return errorHandler.BadReqResult(
            title: "Points issue",
            message: "Product is sufficient but user did not have enough points.",
            hc: hc,
            user: teacher.User
          );

      teacher.Points -= total;

      // Get id 1 orderstatus type(pending)
      int pendingOrderStatusId = await statusHandler.GetOrderStatus(OrderPossibleStatus.pending, ct);
      if (pendingOrderStatusId == 0)
        return errorHandler.ProblemResult(
          title: "Caution: programming issue",
          message: "No such order status",
          hc: hc,
          user: teacher.User // Prob dont need
        );

      int pendingOrderItemStatusId = await statusHandler.GetOrderItemStatus(OrderItemPossibleStatus.pending, ct);
      if (pendingOrderItemStatusId == 0)
        return errorHandler.ProblemResult(
          title: "Caution: programming issue",
          message: "No such order item status",
          hc: hc,
          user: teacher.User // Prob dont need
        );

      Order newOrder = new()
      {
        CustomerId = teacher.TeacherId,
        // When an order is placed it is always pending status
        StatusId = pendingOrderStatusId,
        OrderItems = [.. cartInfo.CartProductList.Select(ci => new OrderItem
        {
          ProductId = ci.ProductId,
          Quantity = ci.Quantity,
          StatusId = pendingOrderItemStatusId
        })],
        TotalCost = cartInfo.TotalCost
      };

      cartInfo.Ordered = true;

      await dbContext.Orders.AddAsync(newOrder, ct);
      await dbContext.SaveChangesAsync(ct);

      return Results.Created($"/Order/Placed/{newOrder.OrderId}", newOrder.OrderId);
    }).RequireAuthorization("TeacherAllowed");

    // I request what I order.
    group.MapGet("/Check", async (
        ClaimsPrincipal user,
        CancellationToken ct,
        IOrderItemList orderListHandler,
        IErrorResults errorHandler,
        HttpContext hc,
        UnAuthorizedValidator validator
    ) =>
    {
      // Validation of user
      (bool auth, _) = await validator.IsResults<Teacher>(
        expectedRole: Roles.teacher,
        user: user,
        ct: ct
      );

      if (!auth)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: $"Unautherized, user doesnt existed",
          hc: hc
        );

      var items = await orderListHandler.GetOrderItemList(user, ct);
      return Results.Ok(orderListHandler is null ? [] : items.ToOrderListDto());
    }).RequireAuthorization("TeacherAllowed");

    // I request cancelling an order "item"
    group.MapPost("/Cancel/{productId}&{orderId}", async (
        int productId,
        int orderId,
        ClaimsPrincipal user,
        CancellationToken ct,
        MyAppDbContext dbContext,
        IOrderItemList orderListHandler,
        IOrderStatus statusHandler,
        IErrorResults errorHandler,
        HttpContext hc,
        UnAuthorizedValidator validator
    ) =>
    {
      var id = user.FindFirstValue("TeacherId");

      // Validation of user
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

      OrderItem? theItem = await orderListHandler.GetSpecificOrderItem(productId, orderId, ct);
      if (theItem is null)
        return errorHandler.NotFoundResult(
            title: "Doesnt exist issue",
            message: "No such item in your order",
            hc: hc,
            user: teacher.User
          );

      int orderItemStatusId = await statusHandler.GetOrderItemStatus(OrderItemPossibleStatus.cancelled, ct);
      if (orderItemStatusId == 0)
        return errorHandler.ProblemResult(
          title: "Caution: programming issue",
          message: "No such order item status",
          hc: hc,
          user: teacher.User // Prob dont need
        );

      theItem.StatusId = orderItemStatusId;

      int itemCost = theItem.Quantity * theItem.Product.PointCost;
      // Reduce totalCost of teachers order
      theItem.Order.TotalCost -= itemCost;

      // Added back item quantity
      theItem.Product.AvailableQuantity += theItem.Quantity;

      // Add back the points
      theItem.Order.Teacher.Points += itemCost;

      // All teacher order, order items contains only cancel status product
      var allCancelled = theItem.Order.OrderItems.All(oi => oi.StatusId == orderItemStatusId);

      // Status Update
      int orderStatusId = allCancelled ? await statusHandler.GetOrderStatus(OrderPossibleStatus.cancelled, ct)
                                    : await statusHandler.GetOrderStatus(OrderPossibleStatus.mix, ct);

      if (orderStatusId == 0)
        return errorHandler.ProblemResult(
          title: "Caution: programming issue",
          message: "No such order status",
          hc: hc,
          user: teacher.User // Prob dont need
        );

      theItem.Order.StatusId = orderStatusId;

      await dbContext.SaveChangesAsync(ct);

      return Results.Ok(
          $"Your order product name:{theItem.Product.ProductName} has been {theItem.Order.OrderStatus.Status}."
      );
    }).RequireAuthorization("TeacherAllowed");

    // When received, school principle change state.
    group.MapPatch("/Received", async (
        CancellationToken ct,
        ClaimsPrincipal user,
        MyAppDbContext dbContext,
        TeacherOrderStatusDto dto,
        IOrderStatus statusHandler,
        IErrorResults errorHandler,
        HttpContext hc,
        UnAuthorizedValidator validator
    ) =>
    {
      // Validation of user
      (bool auth, SchoolPrincipal? schoolPrincipal) = await validator.IsResults<SchoolPrincipal>(
        expectedRole: Roles.schoolPrincipal,
        user: user,
        ct: ct
      );

      if (schoolPrincipal is null)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: $"Unautherized, user doesnt existed",
          hc: hc
        );

      Order? teacherOrder = await dbContext.Orders.Where(o => o.OrderId == dto.OrderId)
                                                  .Include(o => o.OrderItems)
                                                    .ThenInclude(oi => oi.OrderItemStatus)
                                                  .Include(o => o.OrderItems)
                                                    .ThenInclude(oi => oi.Product)
                                                  .Include(o => o.OrderStatus)
                                                  .FirstOrDefaultAsync(ct);

      if (teacherOrder is null)
        return errorHandler.NotFoundResult(
          title: "Doesn't exists",
          message: "No such order",
          hc: hc
        );

      // Get teacher propreties
      string customerId = teacherOrder.CustomerId;

      // Update
      int orderStatusId;
      int orderItemStatusId;

      if (dto.EntireOrder)
      {
        ICollection<OrderItem> orderItems = teacherOrder.OrderItems;
        orderItemStatusId = await statusHandler.GetOrderItemStatus(OrderItemPossibleStatus.received, ct);
        if (orderItemStatusId == 0)
          return errorHandler.ProblemResult(
            title: "Caution: programming issue",
            message: "No such order status",
            hc: hc
          );

        foreach (var item in orderItems)
        {
          if (item.OrderItemStatus.Status == OrderItemPossibleStatus.pending)
            item.StatusId = orderItemStatusId;
        }

        // The order previously cancelled before but some other product is suffice.
        // But cancel before still has pending product the status already in mix status.
        // Thus, if not all received does nothing to status, vice versa.
        if (teacherOrder.OrderItems.All(oi => oi.StatusId == orderItemStatusId))
        {
          orderStatusId = await statusHandler.GetOrderStatus(OrderPossibleStatus.received, ct);
          if (orderStatusId == 0)
            return errorHandler.ProblemResult(
              title: "Caution: programming issue",
              message: "No such order status",
              hc: hc
            );
          // All Received
          teacherOrder.StatusId = orderStatusId;
        }
      }
      else
      {
        OrderItem? orderItem = teacherOrder.OrderItems.FirstOrDefault(
                oi => oi.ProductId == dto.ProductId
            );
        if (orderItem is null)
          return errorHandler.NotFoundResult(
            title: "Doesn't exists",
            message: "No such order item",
            hc: hc
          );

        // Update received
        orderItemStatusId = await statusHandler.GetOrderItemStatus(OrderItemPossibleStatus.received, ct);
        if (orderItemStatusId == 0)
          return errorHandler.ProblemResult(
            title: "Caution: programming issue",
            message: "No such order item status",
            hc: hc
          );
        orderItem.StatusId = orderItemStatusId;

        // any unreceived or all received
        if (
          teacherOrder.OrderItems.Any(
          oi => oi.OrderItemStatus.Status == OrderItemPossibleStatus.cancelled
          || oi.OrderItemStatus.Status == OrderItemPossibleStatus.pending
        ))
        {
          orderStatusId = await statusHandler.GetOrderStatus(OrderPossibleStatus.mix, ct);
        }
        else // all received
        {
          orderStatusId = await statusHandler.GetOrderStatus(OrderPossibleStatus.received, ct);
        }

        if (orderStatusId == 0)
          return errorHandler.ProblemResult(
            title: "Caution: programming issue",
            message: "No such order status",
            hc: hc
          );

        teacherOrder.StatusId = orderStatusId;
      }

      await dbContext.SaveChangesAsync(ct);

      return Results.Ok($"Id:{customerId} teacher received order id item/items");
    }).RequireAuthorization("PrincipalAllowed");

    group.MapGet("/AllList/Get/{customerId}", async (
        string customerId,
        CancellationToken ct,
        MyAppDbContext dbContext,
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

      ICollection<Order> orderList = await dbContext.Orders.AsNoTracking()
                                                            .Where(o => o.CustomerId == id)
                                                            .Include(o => o.OrderStatus)
                                                            .Include(o => o.OrderItems)
                                                              .ThenInclude(oi => oi.Product)
                                                            .Include(o => o.OrderItems)
                                                              .ThenInclude(oi => oi.OrderItemStatus)
                                                            .ToListAsync(ct);

      if (orderList is null)
        return errorHandler.NotFoundResult(
              title: "Doesn't exists",
              message: "Customer id didnt exist or Customer didnt order anything before",
              hc: hc
            );

      return Results.Ok(orderList.ToOrderDtos());
    }).RequireAuthorization("PrincipalAllowed");

    return group;
  }
}