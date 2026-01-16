using SchoolAppointmentApp.Mapping;

namespace SchoolAppointmentApp.Entities;

public class Block : IRelation // Block table entities
{
    public int RequestId { get; set; } // Pk
    public int ReceiverId { get; set; } // Fk
    public int InitiatorId { get; set; } // Fk
    public DateTime CreatedTime { set; get; } = DateTime.Now;

    // check Reciever/initialtor => name/classid/...
    public required User Receiver { get; set; }
    public required User Initiator { get; set; }
    public bool Blocked { get; set; }
}
