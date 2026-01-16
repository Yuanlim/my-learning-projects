using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;

namespace SchoolAppointmentApp.FunctionalClasses;

// All Validator must have method: IsValid()
public interface IValidator
{
    bool IsValid(string Email);
}

public interface IProcessValidator
{
    ValueTask<IResult?> IsResults<T>(VerifyResultsDto<T> dto);
}

internal sealed class EmailValidator : IValidator
{
    public Guid Id { get; } = Guid.NewGuid(); // For logging

    public bool IsValid(string email)
    {
        if (email.EndsWith("@gmail.com")) return true;
        if (email.EndsWith("@nkust.edu.tw")) return true;
        return false;
    }
}

internal sealed class NameValidator : IValidator
{
    public Guid Id { get; } = Guid.NewGuid();

    public bool IsValid(string name) => name.Length > 3 || string.IsNullOrWhiteSpace(name);
}

public enum Roles { admin, student, teacher, schoolPrincipal };

internal sealed class RoleValidator : IValidator
{
    public Guid Id { get; } = Guid.NewGuid();

    public bool IsValid(string role)
    {
        foreach (var eachRole in Enum.GetValues<Roles>())
        {
            if (role == Convert.ToString(eachRole))
                return true;
        }

        return false;
    }
}

// For endpoint results return consistencies when error
internal sealed class NullValidator : IProcessValidator
{
    /// <summary>
    /// Check proccess returns nothing, which is an error.
    /// </summary>
    /// <param name="dto.ValidateValue">The value that is begin checked</param>
    /// <param name="dto.ClassValidation"> 
    /// A boolean array with different indicies indecate that checking:
    /// 0: User point is empty
    /// 1: Posted content is empty or white space
    /// defualt: UserId is not valid, indicate user does not existed
    /// if true
    /// </param>
    /// <returns>The sum of the two integers.</returns>
    public ValueTask<IResult?> IsResults<T>(VerifyResultsDto<T> dto)
    {
        if (dto.ClassValidation[0] == true)
        {
            return ValueTask.FromResult(
                    dto.ValidateValue is null ? Results.BadRequest("User points is empty")
                                              : null
            );
        }
        else if (dto.ClassValidation[1] == true)
        {
            return ValueTask.FromResult(
                    string.IsNullOrWhiteSpace(dto.ValidateValue as string) ? Results.BadRequest("The message is empty")
                                                                           : null
            );
        }
        else // must be userId ??
        {
            return ValueTask.FromResult(
                    string.IsNullOrWhiteSpace(dto.ValidateValue as string) ? Results.BadRequest("UserId not found")
                                                                           : null
            );
        }

    }
}

public class UnAuthorizedValidator(MyAppDbContext dbContext)
    : DataBaseService(dbContext), IProcessValidator
{
    public async ValueTask<IResult?> IsResults<T>(VerifyResultsDto<T> dto)
    {
        if (dto.Roles == Roles.teacher)
            return
                await dbContext.Teachers.FirstOrDefaultAsync(t => t.TeacherId == dto.ValidateValue!.ToString())
                      is null ? Results.Unauthorized() : null;
        else
            return
                await dbContext.Students.FirstOrDefaultAsync(s => s.StudentId == dto.ValidateValue!.ToString())
                      is null ? Results.Unauthorized() : null;
    }
}