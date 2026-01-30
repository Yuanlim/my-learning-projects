using SchoolAppointmentApp.DataTypeObject;
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

    public static CartDto ToCartDto(this Cart? cart)
    {
        if (cart is not null)
        {
            CartDto cartDto = new(TotalCost: cart.TotalCost, CartProductList: []);
            foreach (var item in cart.CartProductList)
            {
                cartDto.CartProductList.Add(
                    item.ToCartItemDto()
                );
            }
            return cartDto;
        }
        else return new(TotalCost: 0, CartProductList: []);
    }

    public static CartItemDto ToCartItemDto(this CartItem cartItem)
    => new(
            ProductId: cartItem.Product.ProductId,
            ProductName: cartItem.Product.ProductName,
            ProductImageRoot: cartItem.Product.ProductImageRoot,
            Quantity: cartItem.Quantity,
            PointCost: cartItem.Product.PointCost
        );

    public static ProductDto ToProductDto(this Product product)
    => new(
            ProductId: product.ProductId,
            ProductImageRoot: product.ProductImageRoot,
            ProductName: product.ProductName,
            Description: product.Description,
            PointCost: product.PointCost,
            AvailableQuantity: product.AvailableQuantity
        );
}

