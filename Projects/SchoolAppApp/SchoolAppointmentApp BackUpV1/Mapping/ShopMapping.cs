using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Mapping;

public static class ShopMapping
{
    public static ICollection<OrderItemDto> ToOrderListDto(this ICollection<OrderItem> orderItems)
    {
        ICollection<OrderItemDto> productListDtos = [];
        foreach (var item in orderItems)
        {
            OrderItemDto newProductListDto = new
            (
                OrderItemId: item.OrderItemId,
                ProductId: item.ProductId,
                ProductName: item.Product.ProductName,
                ProductImageRoot: item.Product.ProductImageRoot,
                OrderStatus: item.OrderItemStatus.Status,
                PointCost: item.Product.PointCost,
                Quantity: item.Quantity
            );

            productListDtos.Add(newProductListDto);
        }

        return productListDtos;
    }

    public static ICollection<OrderDto> ToOrderDtos(this ICollection<Order> orders)
    {
        ICollection<OrderDto> orderDtos = [];
        foreach (var order in orders)
        {
            ICollection<OrderItemDto> orderItemDtos = order.OrderItems.ToOrderListDto();
            OrderDto orderDto = new
            (
                OrderId: order.OrderId,
                OrderItems: orderItemDtos,
                Status: order.OrderStatus.Status
            );

            orderDtos.Add(orderDto);
        }

        return orderDtos;
    }
} 

