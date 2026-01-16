using SchoolAppointmentApp.Entities;
namespace SchoolAppointmentApp.DataTypeObject;

public record PointsDto
(
	int Points,
	int TodaysEarning
);

public record GetProductListDto
(
	string? SearchString,
	int RequestExpandTimes
);

public record ProductDto
(
	int ProductId,
	string ProductImageRoot,
	string ProductName,
	string Description,
	int PointCost,
	int AvailableQuantity
);

public record OrderItemDto
(
	int OrderItemId,
	int ProductId,
	string ProductName,
	string ProductImageRoot,
	OrderItemPossibleStatus OrderStatus,
	int PointCost,
	int Quantity
);

public record CartItemDto
(
	string ProductName,
	string ProductImageRoot,
	int Quantity,
	int PointCost
);

public record CartDto
(
	ICollection<CartItemDto> CartProductList,
	int TotalCost
);

public record class CreateProductDto
(
	string ProductName,
	string ProductImageRoot,
	string Description,
	int PointCost,
	int Quantity
);

public record class PatchProductDto
(
	int ProductId,
	string? ProductName,
	string? ProductImageRoot,
	string? Description,
	int? PointCost,
	int? Quantity
);

public record class WishItemDto
(
	int ProductId,
	int Quantity
);

public record OrderDto
(
	int OrderId,
	ICollection<OrderItemDto> OrderItems,
	OrderPossibleStatus Status
);
