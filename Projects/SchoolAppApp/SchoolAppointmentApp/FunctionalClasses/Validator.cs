using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.FunctionalClasses;

// All Validator must have method: IsValid()
public interface IValidator
{
  bool IsValid(string Email);
}
public interface IRoleValidator
{
  Roles? IsValid(string role);
}

public interface IProcessValidator
{
  ValueTask<string?> IsResults<T>(VerifyResultsDto<T> dto);
}

public interface IAuthorizedValidator
{
  Task<(bool, R?)> IsResults<R>(
      Roles expectedRole,
      CancellationToken ct,
      ClaimsPrincipal? user = null,
      string? id = null
  ) where R : class;
}

internal sealed class EmailValidator : IValidator
{
  public bool IsValid(string email)
  {
    if (email.EndsWith("@gmail.com")) return true;
    if (email.EndsWith("@nkust.edu.tw")) return true;
    return false;
  }
}

internal sealed class NameValidator : IValidator
{

  public bool IsValid(string name) => name.Length > 3 || string.IsNullOrWhiteSpace(name);
}

public enum Roles { admin, student, teacher, schoolPrincipal };

internal sealed class RoleValidator : IRoleValidator
{
  public Roles? IsValid(string role)
  {
    foreach (var eachRole in Enum.GetValues<Roles>())
    {
      if (role.Equals(eachRole.ToString(), StringComparison.InvariantCultureIgnoreCase))
        return eachRole;
    }
    return null;
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
  public async ValueTask<string?> IsResults<T>(VerifyResultsDto<T> dto)
  {
    if (dto.ClassValidation[0] == true)
    {
      return await Task.FromResult(
          dto.ValidateValue is null ? "User points is empty" : null
      );
    }
    else if (dto.ClassValidation[1] == true)
    {
      return await Task.FromResult(string.IsNullOrWhiteSpace(
          dto.ValidateValue as string) ? "The message is empty" : null
      );
    }
    else // must be userId ??
    {
      return await Task.FromResult(string.IsNullOrWhiteSpace(
          dto.ValidateValue as string) ? "UserId not found" : null
      );

    }

  }
}

public class UnAuthorizedValidator(MyAppDbContext dbContext)
    : DataBaseService(dbContext), IAuthorizedValidator
{
  public async Task<(bool, R?)> IsResults<R>(
    Roles expectedRole,
    CancellationToken ct,
    ClaimsPrincipal? user = null,
    string? id = null
  ) where R : class
  {
    if (expectedRole == Roles.teacher)
    {
      Teacher? teacher = await IsTeacherResults(id: user?.FindFirstValue("TeacherId") ?? id, ct);
      if (teacher is null) return (false, default);
      return (true, typeof(R) == typeof(Teacher) ? (R)(object)teacher : default);
    }

    if (expectedRole == Roles.student)
    {
      Student? student = await IsStudentResults(id: user?.FindFirstValue("StudentId") ?? id, ct);
      if (student is null) return (false, default);
      return (true, typeof(R) == typeof(Student) ? (R)(object)student : default);
    }

    if (expectedRole == Roles.schoolPrincipal)
    {
      SchoolPrincipal? sp = await IsPrincipalResults(id: user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? id, ct);
      if (sp is null) return (false, default);
      return (true, typeof(R) == typeof(SchoolPrincipal) ? (R)(object)sp : default);
    }

    return (false, default);
  }

  private async Task<Teacher?> IsTeacherResults(string? id, CancellationToken ct)
  => await dbContext.Teachers.Include(t => t.User)
                              .FirstOrDefaultAsync(s => s.TeacherId == id, ct);
  private async Task<Student?> IsStudentResults(string? id, CancellationToken ct)
  => await dbContext.Students.Include(s => s.User)
                              .FirstOrDefaultAsync(s => s.StudentId == id, ct);
  private async Task<SchoolPrincipal?> IsPrincipalResults(string? id, CancellationToken ct)
  => await dbContext.SchoolPrincipal.FirstOrDefaultAsync(s => s.PrincipalId == id, ct);
}