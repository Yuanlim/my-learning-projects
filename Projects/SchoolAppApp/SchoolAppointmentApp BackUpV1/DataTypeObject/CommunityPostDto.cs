namespace SchoolAppointmentApp.DataTypeObject;

public enum OrderingTypes { asc, desc }

public record class MainPostDto
(
    int PostId,
    string StudentId,
    string StudentName,
    string Content,
    ICollection<ReplyDto> Replies,
    DateTime PostDateTime,
    int ThumbsUp
);

public record class ReplyDto
(
    int ReplyId,
    string UserId,
    string UserName,
    string Content,
    DateTime PostDateTime,
    int ThumbsUp
);

public record class GetMainPostDto
(
    int StepAmount,
    string OrderBy,
    OrderingTypes Ordering,
    string? SearchString
);

public record class PostMainPostDto
(
    string Content
);

public record class ReplyMainPostDto
(
    int RepliedMainPostId,
    string Content
);
public record class ThumbsUpDto
(
    int? MainPostId,
    int? ReplyId
);
