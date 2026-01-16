using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.DataTypeObject;

public record FriendRequestDto
(
  int ToUserId,
  FriendRequestPossibleStatus Frps
);

public record GetWithStatusDto
(
  FriendRequestPossibleStatus Status
);

public record FindFriendDto
(
  string Id
);

