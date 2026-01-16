using System.Text.Json.Serialization;
using SchoolAppointmentApp.Mapping;

namespace SchoolAppointmentApp.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FriendRequestPossibleStatus { Accepted, Denied, Pending }
public class FriendRequest : IRelation
{
    public int RequestId { set; get; } // PK
    public int ReceiverId { set; get; } // FK of a user
    public int InitiatorId { set; get; } // FK of another user 
    public int StatusId { get; init; }
    public FriendRequestStatus FriendRequestStatus { set; get; } = null!;
    public DateTime CreatedTime { set; get; } = DateTime.Now;

    // check Reciever/initialtor => name/classid/...
    public User Receiver { get; set; } = null!;
    public User Initiator { get; set; } = null!;
}

public class FriendRequestStatus
{
    public int StatusId { get; init; }
    public FriendRequestPossibleStatus FriendRequestPossibleStatus { get; init; }
}