using System.Security.Claims;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;

namespace SchoolAppointmentApp.DataTypeObject;

public record class VerifyResultsDto<T>
(
    T ValidateValue,
    bool[] ClassValidation,
    Roles? Roles = null
);
