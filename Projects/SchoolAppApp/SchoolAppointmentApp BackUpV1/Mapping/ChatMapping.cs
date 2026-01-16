using System.Security.Claims;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;

namespace SchoolAppointmentApp.Mapping;

public static class ChatMapping
{
  public static SendedChatDto ToPostChatDto(this Message message)
  {
    return new
    (
        ReceiverId: message.ReceiverId,
        ReceiverName: message.Receiver.Name,
        Content: message.Content ?? ""
    );
  }

  public static async Task<GetChatBetweenDto> ToChatBetweenDto
  (this ICollection<Message> messages, ClaimsPrincipal userPriciple, IGetUserId idHandler)
  {
    (string? toId, _) = await idHandler.GetIdByClaims(new(UserPrincipal: userPriciple));
    GetChatBetweenDto dto = new(toId!, []);

    foreach (var item in messages)
    {
      dto.ChatMessages.Add(
        new(
          Content: item.Content ?? "Error Message coundn't been recover",
          PostDateTime: item.MessageDateTime
        )
      );
    }

    return dto;
  }

  public static GetPersonDto ToGetPersonDto(this User user, string id)
  {
    return new(
      Id: id,
      UserId: user.UserId,
      Name: user.Name
    );
  }
}