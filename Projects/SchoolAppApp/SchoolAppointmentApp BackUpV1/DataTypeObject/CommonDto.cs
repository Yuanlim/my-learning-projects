using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.DataTypeObject;

public record BlockRequestDto
(
    int ToUserId
);

public record BlockDto
(
    int InitiatorId,
    int ReceiverId,
    bool Blocked
);

public record RelationDto
(
    int ReceiverId,
    int StatusIds
);

public record NewFriendRequestDto
(
    int ReceiverId,
    int InitiatorId,
    FriendRequestPossibleStatus FriendRequestPossibleStatus
);