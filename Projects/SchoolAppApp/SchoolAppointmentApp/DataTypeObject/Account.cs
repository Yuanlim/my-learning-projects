using SchoolAppointmentApp.FunctionalClasses;

namespace SchoolAppointmentApp.DataTypeObject;

public record class CreateAccount(
    string Role,
    string Id,
    string Name,
    string Password,
    string Email,
    string PhoneNumber,
    string? Class
);

public record class LoginDto(
    string Role,
    string Id,
    string Password
);
