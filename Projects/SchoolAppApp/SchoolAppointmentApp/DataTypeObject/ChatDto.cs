namespace SchoolAppointmentApp.DataTypeObject;

public record class PostChatDto
(
  int ToUserId,
  string? Content
);

public record class SendedChatDto
(
  int ReceiverId,
  string ReceiverName,
  string Content
);

public record class ChatMessageDto
(
  bool ISended,
  string Content,
  DateTime PostDateTime
);

public record class ResponseChatMessageDto
(
  int LastMessageId,
  ICollection<ChatMessageDto> ChatMessages
);

public record class GetChatBetweenDto
(
  int OthersId,
  int LastMessageId
);

public record class GetPersonDto
(
  string Id,
  int UserId,
  string Name
);