namespace SchoolAppointmentApp.DataTypeObject;

// TODO
public record class PatchTeacherDto(
    string? Name,
    decimal? Points,
    string? Password,
    string? Email,
    string? PhoneNumber
);

public record class TeacherOrderStatusDto(
    int? ProductId,
    int OrderId,
    bool EntireOrder
);
