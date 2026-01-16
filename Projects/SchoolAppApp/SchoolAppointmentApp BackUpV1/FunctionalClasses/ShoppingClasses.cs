using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.FunctionalClasses;

public interface IProductListClasses
{
    public Task<IReadOnlyCollection<ProductListDto>> GetProductLists(
        string? searchString, int limitStep, int maximumProductListedEachExpand,
        CancellationToken ct
    );
}

public interface IOrderItemList
{
    public Task<ICollection<OrderItem>> GetOrderItemList(
        ClaimsPrincipal user, CancellationToken ct
    );
    public Task<OrderItem?> GetSpecificOrderItem(
        int orderId, int productId, CancellationToken ct
    );
}

public interface IOrderStatus
{
    public Task<int> GetOrderItemStatus(OrderItemPossibleStatus oips, CancellationToken ct);
    public Task<int> GetOrderStatus(OrderPossibleStatus ops, CancellationToken ct);
}

internal sealed class ProductListClasses(MyAppDbContext dbContext) : 
    DataBaseService(dbContext), IProductListClasses
{
    public Guid guid = Guid.NewGuid();
    public async Task<IReadOnlyCollection<ProductListDto>> GetProductLists(
        string? searchString, int limitStep, int maximumProductListedEachExpand,
        CancellationToken ct
    )
    {
        // Set up database query of Products table
        IQueryable<Product> productListQuery = dbContext.Products;

        // IF searchString is null or only white space => intention: get product by id (random scrolling)
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            productListQuery = productListQuery.Where(
                p => EF.Functions.Like(p.Description, $"%{searchString}%")
            );
        }

        // Take: as SQL LIMIT statement
        // Each expand button pressed it takes the next 5 product
        var productList = productListQuery.AsNoTracking()
                                          .OrderBy(p => p.ProductId)
                                          .Skip((limitStep - 1) * maximumProductListedEachExpand)
                                          .Take(maximumProductListedEachExpand);

        // To table field to dtos
        IReadOnlyCollection<ProductListDto> productListDtos = await productList.Select(
            p => new ProductListDto(
                p.ProductId,
                p.ProductImageRoot,
                p.Description,
                p.PointCost,
                p.AvailableQuantity
            )
        ).ToListAsync(ct);

        return productListDtos;
    }
}

internal sealed class OrderItemListClasses(MyAppDbContext dbContext)
    : DataBaseService(dbContext), IOrderItemList
{
    public Guid guid = Guid.NewGuid();
    public async Task<ICollection<OrderItem>> GetOrderItemList(ClaimsPrincipal user, CancellationToken ct)
    {
        var TeacherId = user.FindFirstValue("TeacherId");

        ICollection<OrderItem> teacherOrderItemList =
            await dbContext.OrderItems.Where(o => o.Order.CustomerId == TeacherId)
                                      .Include(o => o.Product)
                                      .Include(o => o.OrderItemStatus)
                                      .Include(o => o.Order)
                                      .ToListAsync(ct);

        return teacherOrderItemList;
    }

    public async Task<OrderItem?> GetSpecificOrderItem(int productId, int orderId, CancellationToken ct)
    {
        OrderItem? teacherOrderItem =
            await dbContext.OrderItems.Where(
                                            oi => oi.Order.OrderId == orderId
                                         && oi.ProductId == productId
                                      )
                                      .Include(oi => oi.Order)
                                        .ThenInclude(o => o.OrderItems)
                                      .Include(oi => oi.Order)
                                        .ThenInclude(o => o.OrderStatus)
                                      .Include(oi => oi.Order)
                                        .ThenInclude(o => o.Teacher)
                                      .Include(oi => oi.Product)
                                      .Include(oi => oi.OrderItemStatus)
                                      .FirstOrDefaultAsync(ct);

        return teacherOrderItem;
    }
}

internal sealed class GetStatus(MyAppDbContext dbContext) :
    DataBaseService(dbContext), IOrderStatus
{
    public async Task<int> GetOrderItemStatus(OrderItemPossibleStatus oips, CancellationToken ct)
    => await dbContext.OrderItemStatuses.AsNoTracking()
                                        .Where(ois => ois.Status == oips)
                                        .Select(ois => ois.StatusId)
                                        .FirstOrDefaultAsync(ct);

    public async Task<int> GetOrderStatus(OrderPossibleStatus ops, CancellationToken ct)
    => await dbContext.OrderStatuses.AsNoTracking()
                                    .Where(os => os.Status == ops)
                                    .Select(os => os.StatusId)
                                    .FirstOrDefaultAsync(ct);
}


