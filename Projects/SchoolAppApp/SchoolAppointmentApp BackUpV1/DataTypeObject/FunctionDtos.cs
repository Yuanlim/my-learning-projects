using System.Security.Claims;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;

namespace SchoolAppointmentApp.DataTypeObject;

public record class GetSomethingDto
(
    int? IndexId = null,
    string? StringId = null,
    ClaimsPrincipal? UserPrincipal = null,
    User? User = null,
    Roles? Roles = null
);

public record class VerifyResultsDto<T>
(
    T ValidateValue,
    bool[] ClassValidation,
    Roles? Roles = null
);
