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
