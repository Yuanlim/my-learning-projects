using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.DataTypeObject;

public record class GetIdBundleDto
(   
    int UserId,
    User User,
    string UserSchoolId
);