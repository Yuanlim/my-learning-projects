using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;

namespace SchoolAppointmentApp.FunctionalClasses;

// Scoped Operation
public interface IDuplicateChecker
{
    Task<bool> IsDuplicateAsync(
        Roles? Role, string email,
        string Id
    );
}

internal sealed class DuplicateChecker(MyAppDbContext dbContext)
    : DataBaseService(dbContext), IDuplicateChecker
{
    public async Task<bool> IsDuplicateAsync(
        Roles? Role, string email,
        string id
    )
    {
        bool duplicated = Role.ToString()!.ToLowerInvariant() switch
        {
            "student" =>
                await dbContext.Students.AsNoTracking()
                                  .FirstOrDefaultAsync(s => s.StudentId == id)
                                  is not null ||
                await dbContext.Users.AsNoTracking()
                               .FirstOrDefaultAsync(u => u.Email == email)
                               is not null,

            "teacher" =>
                await dbContext.Teachers.AsNoTracking()
                                  .FirstOrDefaultAsync(s => s.TeacherId == id)
                                  is not null ||
                await dbContext.Users.AsNoTracking()
                               .FirstOrDefaultAsync(u => u.Email == email)
                               is not null,

            _ =>
                throw new Exception("Unexpected Role")
        };

        return duplicated;
    }
}
