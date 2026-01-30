using Microsoft.Extensions.Configuration.UserSecrets;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Mapping;

public interface IRelation
{
  int InitiatorId { get; set; }
  User Initiator { get; set; }
  User Receiver { get; set; }
}

public static class FriendMapping
{
  public static ICollection<GetPersonDto> ToFriendListDto<T>(
    this ICollection<T> list, User user
  ) where T : IRelation
  {
    ICollection<GetPersonDto> newList = [];
    foreach (var item in list)
    {
      User withUser = item.InitiatorId == user.UserId ?
                      item.Receiver : item.Initiator;
      string id = withUser.Student is null ?
                  withUser.Teacher!.TeacherId :
                  withUser.Student.StudentId;

      newList.Add(new
      (
        Id: id,
        UserId: withUser.UserId,
        Name: withUser.Name
      ));
    }

    return newList;
  }
}