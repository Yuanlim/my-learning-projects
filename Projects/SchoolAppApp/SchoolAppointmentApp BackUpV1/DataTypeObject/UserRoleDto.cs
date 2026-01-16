namespace SchoolAppointmentApp.DataTypeObject;

public record class StudentDto(
    string StudentId,
    string Name,
    string ClassName,
    string PhoneNumber,
    string Email
);

public record class TeacherDto(
    string TeacherId,
    string Name,
    int Points,
    string PhoneNumber,
    string Email
);

