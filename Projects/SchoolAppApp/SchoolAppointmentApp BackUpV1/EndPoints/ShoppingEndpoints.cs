using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;
using SchoolAppointmentApp.Mapping;

namespace SchoolAppointmentApp.EndPoints;
public static class Shopping
{
    public static RouteGroupBuilder ShoppingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/Shopping");

        // initial shopping list + teacher points
        group.MapGet("/Initial", async (
            ClaimsPrincipal user,
            MyAppDbContext dbContext,
            CancellationToken ct,
            IProductListClasses GetProductListHandler
        ) =>
        {
            var productList = await GetProductListHandler.GetProductLists(null, 1, 5, ct);

            var id = user.FindFirstValue("TeacherId");

            if (string.IsNullOrWhiteSpace(id)) return Results.BadRequest("Unautherized, invalid teacher id");

            var aboutPoints = await dbContext.Teachers.AsNoTracking()
                                                      .Where(t => t.TeacherId == id)
                                                      .Select(t => new { t.Points, t.TodaysEarning })
                                                      .FirstOrDefaultAsync(ct);

            if (aboutPoints is null) return Results.NoContent();

            // To return shopping initial dto
            var shoppingInitial = new ShoppingInitialDto(
                Products: productList,
                Points: aboutPoints.Points,
                TodaysEarning: aboutPoints.TodaysEarning
            );

            return Results.Ok(shoppingInitial);

        }).Produces<ShoppingInitialDto>()
          .RequireAuthorization("TeacherAllowed");

        // Expand product request
        // requestExpandTimes : frontend counter
        group.MapGet("/Expand/{requestExpandTimes}", async (
            int requestExpandTimes,
            MyAppDbContext dbContext,
            CancellationToken ct,
            IProductListClasses GetProductListHandler
        ) =>
        {
            var productList = await GetProductListHandler.GetProductLists(
                null, requestExpandTimes, 5, ct
            );

            // No more product available to expand
            if (productList.Count == 0) return Results.NoContent();

            return Results.Ok(productList);
        }).RequireAuthorization("TeacherAllowed", "PrincipalAllowed");

        group.MapGet("/Search/{searchString}&{requestExpandTimes}", async (
            CancellationToken ct, IProductListClasses GetProductHandler,
            MyAppDbContext dbContext, string? searchString, int requestExpandTimes
        ) =>
        {
            // User input nothing or white space only, nothing happen
            if (string.IsNullOrWhiteSpace(searchString)) return Results.NoContent();

            var productList = await GetProductHandler.GetProductLists(
                searchString, requestExpandTimes, 5, ct
            );

            // No such product exception
            if (productList.Count == 0) return Results.NotFound();

            return Results.Ok(productList);
        }).RequireAuthorization("TeacherAllowed");

        // I put something in my cart endpoint
        group.MapPost("/WishList", async (
            WishItemDto dto,
            MyAppDbContext dbContext,
            CancellationToken ct,
            ClaimsPrincipal user
        ) =>
        {
            string? teacherId = user.FindFirstValue("TeacherId");
            if (string.IsNullOrWhiteSpace(teacherId)) return Results.BadRequest("Unautherized, invalid teacher id");

            Product? product = await dbContext.Products.AsNoTracking()
                                                       .FirstOrDefaultAsync(
                p => p.ProductId == dto.ProductId, ct
            );
            if (product is null) return Results.NotFound("Product doesn't exist");
            if (product.AvailableQuantity < dto.Quantity) return Results.BadRequest("Product doesn't have enough stock");

            // Check wether there is cart hasn't been ordered
            Cart? teacherCart = await dbContext.Carts.Include(c => c.CartProductList)
                                                        .ThenInclude(c => c.Product)
                                                     .Where(
                                                        c => c.CustomerId == teacherId
                                                        && c.Ordered == false
                                                     )
                                                     .FirstOrDefaultAsync(ct);

            if (teacherCart is null)
            {
                // Nothing new was wished in the list, but trying to add item (new Cart)
                teacherCart = new()
                {
                    CustomerId = teacherId!,
                    Ordered = false,
                    TotalCost = product!.PointCost * dto.Quantity,
                    CartProductList = [new() { ProductId = dto.ProductId, Quantity = dto.Quantity }]
                };

                await dbContext.Carts.AddAsync(teacherCart, ct);
                await dbContext.SaveChangesAsync(ct);

                return Results.Created("Product is added to the cart", null);
            }

            // Check cart has the same product
            CartItem? sameCartProduct = teacherCart.CartProductList.FirstOrDefault(
                cpl => cpl.ProductId == dto.ProductId
            );

            // Existed add quantity, 
            if (sameCartProduct is null)
            {
                teacherCart.CartProductList.Add(
                    new()
                    {
                        ProductId = dto.ProductId,
                        Product = product, // For Total calculation 
                        Quantity = dto.Quantity
                    }
                );
            }
            else
            {
                sameCartProduct.Quantity += dto.Quantity;
            }

            // Recompute total cost
            int totalCost = 0;
            foreach (var item in teacherCart.CartProductList)
                totalCost += item.Quantity * item.Product.PointCost;

            teacherCart.TotalCost = totalCost;

            await dbContext.SaveChangesAsync(ct);
            return Results.Created("Product is added to the cart", null);
        }).RequireAuthorization("TeacherAllowed");

        // I order something endpoint
        group.MapPost("/Order/Placed/", async (
            ClaimsPrincipal user,
            MyAppDbContext dbContext,
            CancellationToken ct,
            IOrderStatus statusHandler
        ) =>
        {
            var Id = user.FindFirstValue("TeacherId");
            if (Id is null) return Results.Unauthorized();

            Cart? cartInfo =
                await dbContext.Carts.Where(c => c.CustomerId == Id && c.Ordered == false)
                                     .Include(c => c.CartProductList)
                                        .ThenInclude(ci => ci.Product)
                                     .FirstOrDefaultAsync(ct);

            if (cartInfo is null) return Results.NotFound("There is no unordered cart");

            // Insufficient points detection Or
            // Insufficient Quantity(Wished, has quantity but order too slow, other user got it.)
            int total = 0;
            foreach (var item in cartInfo.CartProductList)
            {
                total += item.Quantity * item.Product.PointCost;
                if (item.Product.AvailableQuantity < item.Quantity)
                    return Results.BadRequest($"{item.Product.ProductName} has insufficient stock");
                item.Product.AvailableQuantity -= item.Quantity;
            }
            var teacher = await dbContext.Teachers.FirstOrDefaultAsync(t => t.TeacherId == Id, ct);
            if (teacher == null) return Results.BadRequest("Teacher not found");
            if (total > teacher.Points) return Results.BadRequest("Insufficient points");

            teacher.Points -= total;

            // Get id 1 orderstatus type(pending)
            int pendingOrderStatusId = await statusHandler.GetOrderStatus(OrderPossibleStatus.pending, ct);

            if (pendingOrderStatusId == 0) return Results.NotFound("No such order status");

            int pendingOrderItemStatusId = await statusHandler.GetOrderItemStatus(OrderItemPossibleStatus.pending, ct);

            if (pendingOrderItemStatusId == 0) return Results.NotFound("No such order item status");

            Order newOrder = new()
            {
                CustomerId = Id,
                // When an order is placed it is always pending status
                StatusId = pendingOrderStatusId,
                OrderItems = cartInfo.CartProductList.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    StatusId = pendingOrderItemStatusId
                }).ToList(),
                TotalCost = cartInfo.TotalCost
            };

            cartInfo.Ordered = true;

            for (int i = 0; i < 100; i++)
            {
                System.Console.WriteLine("Hi");
            }

            await dbContext.Orders.AddAsync(newOrder, ct);
            await dbContext.SaveChangesAsync(ct);

            return Results.Created($"/Order/Placed/{newOrder.OrderId}", newOrder.OrderId);
        }).RequireAuthorization("TeacherAllowed");

        // I request what I order.
        group.MapGet("/Order/Check", async (
            ClaimsPrincipal user,
            CancellationToken ct,
            IOrderItemList orderListHandler
        ) =>
        {
            var items = await orderListHandler.GetOrderItemList(user, ct);
            return Results.Ok(items.ToOrderListDto());
        }).RequireAuthorization("TeacherAllowed");

        // I request cancelling an order "item"
        group.MapPost("/Order/Cancel/{productId}&{orderId}", async (
            int productId,
            int orderId,
            ClaimsPrincipal user,
            CancellationToken ct,
            MyAppDbContext dbContext,
            IOrderItemList orderListHandler,
            IOrderStatus statusHandler
        ) =>
        {
            OrderItem? theItem = await orderListHandler.GetSpecificOrderItem(productId, orderId, ct);

            if (theItem is null) return Results.NotFound("No such item in your order");

            int orderItemStatusId = await statusHandler.GetOrderItemStatus(OrderItemPossibleStatus.cancelled, ct);

            if (orderItemStatusId == 0) return Results.NotFound("No such status");

            theItem.StatusId = orderItemStatusId;

            int itemCost = theItem.Quantity * theItem.Product.PointCost;
            // Reduce totalCost of teachers order
            theItem.Order.TotalCost -= itemCost;

            // Added back item quantity
            theItem.Product.AvailableQuantity += theItem.Quantity;

            // Add back the points
            theItem.Order.Teacher.Points += itemCost;

            // Status Update handler
            int orderStatusId;

            // All teacher order, order items contains only cancel status product
            var allCancelled = theItem.Order.OrderItems.All(oi => oi.StatusId == orderItemStatusId);
            orderStatusId = allCancelled ? await statusHandler.GetOrderStatus(OrderPossibleStatus.cancelled, ct)
                                         : await statusHandler.GetOrderStatus(OrderPossibleStatus.mix, ct);

            if (orderStatusId == 0) return Results.NotFound("No such order status");

            theItem.Order.StatusId = orderStatusId;

            await dbContext.SaveChangesAsync(ct);

            return Results.Ok(
                $"Your order product name:{theItem.Product.ProductName} has been {theItem.Order.OrderStatus.Status}."
            );
        }).RequireAuthorization("TeacherAllowed");

        // When received, school principle change state.
        group.MapPatch("/Order/Received", async (
            CancellationToken ct,
            MyAppDbContext dbContext,
            TeacherOrderStatusDto dto,
            IOrderStatus statusHandler
        ) =>
        {
            Order? teacherOrder =
                await dbContext.Orders.Where(o => o.OrderId == dto.OrderId)
                                      .Include(o => o.OrderItems)
                                        .ThenInclude(oi => oi.OrderItemStatus)
                                      .Include(o => o.OrderItems)
                                        .ThenInclude(oi => oi.Product)
                                      .Include(o => o.OrderStatus)
                                      .FirstOrDefaultAsync(ct);

            if (teacherOrder is null) return Results.NotFound("No such order");

            // Get teacher propreties
            string customerId = teacherOrder.CustomerId;

            // Update
            int orderStatusId;
            if (dto.EntireOrder)
            {
                ICollection<OrderItem> orderItems = teacherOrder.OrderItems;
                int orderItemStatusId = await statusHandler.GetOrderItemStatus(OrderItemPossibleStatus.received, ct);

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
                }
            }
            else
            {
                OrderItem? orderItem = teacherOrder.OrderItems.FirstOrDefault(
                    oi => oi.ProductId == dto.ProductId
                );
                if (orderItem is null) return Results.NotFound("No such order item");

                // Update received
                int orderItemStatusId = await statusHandler.GetOrderItemStatus(OrderItemPossibleStatus.received, ct);
                orderItem.StatusId = orderItemStatusId;

                // any unreceived or all received
                if
                (
                    teacherOrder.OrderItems.Any(
                    oi => oi.OrderItemStatus.Status == OrderItemPossibleStatus.cancelled
                    || oi.OrderItemStatus.Status == OrderItemPossibleStatus.pending
                    )
                )
                {
                    orderStatusId = await statusHandler.GetOrderStatus(OrderPossibleStatus.mix, ct);
                }
                else // all received
                {
                    orderStatusId = await statusHandler.GetOrderStatus(OrderPossibleStatus.received, ct);
                }

                if (orderStatusId == 0) return Results.NotFound("No such order status");

                teacherOrder.StatusId = orderStatusId;
            }

            await dbContext.SaveChangesAsync(ct);

            return Results.Ok($"Id:{customerId} teacher received order id item/items");
        }).RequireAuthorization("PrincipalAllowed");


        group.MapPost("/newProduct/", async (
            CreateProductDto dto,
            MyAppDbContext dbContext,
            CancellationToken ct
        ) =>
        {
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

        group.MapPatch("/ProductChange/{productId}", async (
            PatchProductDto dto, MyAppDbContext dbContext,
            CancellationToken ct, int productId
        ) =>
        {
            var theProduct = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId, ct);
            if (theProduct is null) return Results.NotFound("No such product");

            theProduct.ProductName = dto.ProductName ?? theProduct.ProductName;
            theProduct.ProductImageRoot = dto.ProductImageRoot ?? theProduct.ProductImageRoot;
            theProduct.Description = dto.Description ?? theProduct.Description;
            theProduct.PointCost = dto.PointCost ?? theProduct.PointCost;
            theProduct.AvailableQuantity = dto.Quantity ?? theProduct.AvailableQuantity;

            await dbContext.SaveChangesAsync(ct);

            return Results.Ok($"Product id {theProduct.ProductId} propreties Changed");
        }).RequireAuthorization("PrincipalAllowed");

        group.MapDelete("/Product/Remove/{productId}", async (
            int productId, MyAppDbContext dbContext,
            CancellationToken ct
        ) =>
        {
            await dbContext.Products.Where(p => p.ProductId == productId)
                                    .ExecuteDeleteAsync(ct);

            return Results.NoContent();
        }).RequireAuthorization("PrincipalAllowed");

        group.MapGet("/AllIdOrderList/{id}", async (
            string id,
            CancellationToken ct,
            MyAppDbContext dbContext
        ) =>
        {
            ICollection<Order> orderList = await dbContext.Orders.AsNoTracking()
                                                                 .Where(o => o.CustomerId == id)
                                                                 .Include(o => o.OrderStatus)
                                                                 .Include(o => o.OrderItems)
                                                                    .ThenInclude(oi => oi.Product)
                                                                 .Include(o => o.OrderItems)
                                                                    .ThenInclude(oi => oi.OrderItemStatus)
                                                                 .ToListAsync(ct);

            if (orderList is null)
                return Results.NotFound(
                    "Customer id didnt exist or Customer didnt order anything before"
                );

            return Results.Ok(orderList.ToOrderDtos());
        }).RequireAuthorization("PrincipalAllowed");

        return group;
    }
}