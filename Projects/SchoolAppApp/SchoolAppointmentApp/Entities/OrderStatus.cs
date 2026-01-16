namespace SchoolAppointmentApp.Entities;

public enum OrderPossibleStatus { pending, received, cancelled, mix }
public enum OrderItemPossibleStatus { pending, received, cancelled }
public record class OrderStatus
{
    public int StatusId { get; set; }
    public OrderPossibleStatus Status { get; set; }
}

public record class OrderItemStatus
{
    public int StatusId { get; set; }
    public OrderItemPossibleStatus Status { get; set; }
}
