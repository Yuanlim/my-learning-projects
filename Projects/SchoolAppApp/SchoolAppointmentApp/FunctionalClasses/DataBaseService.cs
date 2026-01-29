using SchoolAppointmentApp.Data;

namespace SchoolAppointmentApp.FunctionalClasses;

public class DataBaseService
{
    protected readonly MyAppDbContext dbContext;
    protected DataBaseService(MyAppDbContext DbContext) => dbContext = DbContext;

}
