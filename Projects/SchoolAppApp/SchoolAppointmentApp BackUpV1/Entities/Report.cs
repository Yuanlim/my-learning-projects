using System;

namespace SchoolAppointmentApp.Entities;

public class Report
{
    public int RequestId { get; set; }
    public int ReceiverId { get; set; }
    public int InitiatorId { get; set; }
    public string? ReportedContent { get; set; }
    public DateTime ReportedTime { get; set; } = DateTime.UtcNow;
    public required User Receiver { get; set; }
    public required User Initiator { get; set; }
}
