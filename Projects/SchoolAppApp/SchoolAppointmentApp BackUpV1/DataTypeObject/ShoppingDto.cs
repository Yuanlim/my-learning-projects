using SchoolAppointmentApp.Entities;

public record ShoppingInitialDto(
    IReadOnlyCollection<ProductListDto> Products,
    int Points,
    int TodaysEarning
);

public record ProductListDto(
    int ProductId,
    string ProductImageRoot,
    string Description,
    int PointCost,
    int AvailableQuantity
);

public record OrderItemDto(
    int OrderItemId,
    int ProductId,
    string ProductName,
    string ProductImageRoot,
    OrderItemPossibleStatus OrderStatus,
    int PointCost,
    int Quantity
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
