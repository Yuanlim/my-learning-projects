using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;

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

  public static ResponseChatMessageDto ToChatBetweenDto
  (this List<Message> messages, User ThisUser)
  {
    if (messages.Capacity != 0)
    {
      ResponseChatMessageDto response = new(LastMessageId: messages.Last().MessageId, []);

      foreach (var item in messages)
      {
        response.ChatMessages.Add(
        new(
          ISended: ThisUser.UserId == item.SenderId,
          Content: item.Content ?? "Error Message coundn't been recover",
          PostDateTime: item.MessageDateTime
        )
      );
      }

      return response;
    }
    else return new(LastMessageId: 0, []);
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