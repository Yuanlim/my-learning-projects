using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.DataTypeObject;

public record class PostChatDto
(
  int ToUserId,
  string Content
);

public record class SendedChatDto
(
  int ReceiverId,
  string ReceiverName,
  string Content
);

public record class ChatMessageDto
(
  string Content,
  DateTime PostDateTime
);

public record class GetChatBetweenDto
(
  string ToId,
  ICollection<ChatMessageDto> ChatMessages
);

public record class GetPersonDto
(
  string Id,
  int UserId,
  string Name
);