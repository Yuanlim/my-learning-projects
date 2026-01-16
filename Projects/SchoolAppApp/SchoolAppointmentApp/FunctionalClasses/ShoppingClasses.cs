using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.FunctionalClasses;

public interface IProductListClasses
{
	public Task<IReadOnlyCollection<ProductDto>> GetProductList(
			string? searchString,
			int limitStep,
			int maximumProductListedEachExpand,
			CancellationToken ct
	);

	public Task<Product?> GetProductAsync(int productId, CancellationToken ct);
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

public interface IGetCart
{
	public IQueryable<Cart> GetCartQueryAsync(
		Teacher teacher,
		CancellationToken cancellationToken,
		bool AsTracking = false
	);

	public IQueryable<Cart> GetCartQueryAsync(
		ClaimsPrincipal user,
		CancellationToken cancellationToken,
		bool AsTracking = false
	);
}

public interface IGetCartItem
{
	public Task<IQueryable<CartItem>?> GetCartItemQueryAsync(
		Teacher teacher,
		int productId,
		CancellationToken cancellationToken,
		bool AsTracking = false
	);

	public Task<IQueryable<CartItem>?> GetCartItemQueryAsync(
		ClaimsPrincipal user,
		int productId,
		CancellationToken cancellationToken,
		bool AsTracking = false
	);
}

internal sealed class ProductListClasses(MyAppDbContext dbContext) :
		DataBaseService(dbContext), IProductListClasses
{
	public async Task<IReadOnlyCollection<ProductDto>> GetProductList(
			string? searchString, int limitStep, int maximumProductListedEachExpand,
			CancellationToken ct
	)
	{
		// Set up database query of Products table
		IQueryable<Product> productListQuery = dbContext.Products;

		// IF searchString is null or only white space => intention: get product by id (random scrolling)
		if (!string.IsNullOrWhiteSpace(searchString))
		{
			System.Console.WriteLine(searchString);
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
		IReadOnlyCollection<ProductDto> productListDtos = await productList.Select(
				p => new ProductDto(
						p.ProductId,
						p.ProductImageRoot,
						p.ProductName,
						p.Description,
						p.PointCost,
						p.AvailableQuantity
				)
		).ToListAsync(ct);

		return productListDtos;
	}

	public async Task<Product?> GetProductAsync(int productId, CancellationToken ct)
	=> await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId, ct);
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

internal sealed class GetCartHandler(MyAppDbContext dbContext)
		: DataBaseService(dbContext), IGetCart
{
	/// <summary>
	/// 	Get teacher's cart query with Teacher obj or ClaimsPriciple obj
	/// 	With FirstOrDefualt to get cart result.
	/// 	With ExecuteDelete to delete cart.
	/// </summary>
	/// <param name="teacher">Object of teacher info, to get its id</param>
	/// <param name="cancellationToken">Immediate cancelling the operation, once logout or so on.</param>
	/// <param name="AsNoTracking">defualt as false, when changes need to be applied set as "true"</param>
	/// <returns>Type: Cart, Teacher's cart that has not been order</returns>
	public IQueryable<Cart> GetCartQueryAsync(
		Teacher teacher,
		CancellationToken cancellationToken,
		bool AsNoTracking = false
	)
	{
		IQueryable<Cart> cartQuery = dbContext.Carts;

		// Track when need changes
		if (AsNoTracking) cartQuery = cartQuery.AsNoTracking();

		return cartQuery.Include(c => c.CartProductList)
											.ThenInclude(cpl => cpl.Product)
										.Where(
											c =>
												c.CustomerId == teacher.TeacherId &&
												!c.Ordered
										);
	}

	public IQueryable<Cart> GetCartQueryAsync(
		ClaimsPrincipal claimsPrincipal,
		CancellationToken cancellationToken,
		bool AsNoTracking = false
	)
	{
		var id = claimsPrincipal.FindFirstValue("TeacherId");

		IQueryable<Cart> cartQuery = dbContext.Carts;

		// Track when need changes
		if (AsNoTracking) cartQuery = cartQuery.AsNoTracking();

		return cartQuery.Include(c => c.CartProductList)
											.ThenInclude(cpl => cpl.Product)
										.Where(
											c =>
												c.CustomerId == id &&
												!c.Ordered
										);
	}
}


internal sealed class GetCartItemHandler(MyAppDbContext dbContext, GetCartHandler cartHandler)
		: DataBaseService(dbContext), IGetCartItem
{
	private readonly GetCartHandler _cartHandler = cartHandler;
	/// <summary>
	/// 	Get teacher's cart item query with Teacher obj or ClaimsPriciple obj
	/// </summary>
	/// <param name="teacher">Object of teacher info, to get its id</param>
	/// <param name="cancellationToken">Immediate cancelling the operation, once logout or so on.</param>
	/// <param name="AsNoTracking">defualt as false, when changes need to be applied set as "true"</param>
	/// <returns>Teacher's cart item query</returns>
	public async Task<IQueryable<CartItem>?> GetCartItemQueryAsync(
		Teacher teacher,
		int productId,
		CancellationToken cancellationToken,
		bool AsNoTracking = false
	)
	{
		Cart? cart = await _cartHandler.GetCartQueryAsync(
			teacher: teacher,
			cancellationToken: cancellationToken,
			AsNoTracking: AsNoTracking
		).FirstOrDefaultAsync(cancellationToken);

		if (cart is null) return null;

		// Search cart item
		IQueryable<CartItem> cartItemQuery = AsNoTracking ? dbContext.CartItems.AsNoTracking()
																											: dbContext.CartItems;

		return cartItemQuery.Where(
													ci => ci.ProductId == productId &&
													ci.CartId == cart.CartId
												);
	}

	public async Task<IQueryable<CartItem>?> GetCartItemQueryAsync(
		ClaimsPrincipal claimsPrincipal,
		int productId,
		CancellationToken cancellationToken,
		bool AsNoTracking = false
	)
	{
		Cart? cart = await _cartHandler.GetCartQueryAsync(
			claimsPrincipal: claimsPrincipal,
			cancellationToken: cancellationToken,
			AsNoTracking: AsNoTracking
		).FirstOrDefaultAsync(cancellationToken);

		if (cart is null) return null;

		IQueryable<CartItem> cartItemQuery = AsNoTracking ? dbContext.CartItems.AsNoTracking()
																											: dbContext.CartItems;
		return cartItemQuery.Where(
													ci => ci.ProductId == productId &&
													ci.CartId == cart.CartId
												);
	}
}

