using System;

namespace SchoolAppointmentApp.Entities;

public class Message
{
    public int MessageId { set; get; }
    public required int SenderId { set; get; }
    public required int ReceiverId { set; get; }
    public string? Content { set; get; }
    public string? AudioMessageRoot { set; get; }
    public string? ImageMessageRoot { set; get; }
    public DateTime MessageDateTime { get; init; }
    public required User Receiver { get; set; }
    public required User Sender { get; set; }
}
