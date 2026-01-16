using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolAppointmentApp.Entities;

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public required string ProductImageRoot { get; set; }
    public required string Description { get; set; }
    [Range(0, int.MaxValue)] public required int PointCost { get; set; }
    [Range(0, int.MaxValue)] public required int AvailableQuantity { get; set; }
}
