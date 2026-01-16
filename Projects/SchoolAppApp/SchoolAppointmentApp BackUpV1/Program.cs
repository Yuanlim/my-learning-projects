using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.EndPoints;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;
using SchoolAppointmentApp.Jwt;
using System.Text.Json.Serialization;
using static SchoolAppointmentApp.FunctionalClasses.BlockChecker;


Console.WriteLine(new PasswordHasher<object>()
       .HashPassword(default!, "Hello world"));

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("SchoolAppointmentDB");
builder.Services.AddSqlite<MyAppDbContext>(connString);

// When someone asks for JwtProvider, fill the parameter that is already been known.
builder.Services.AddOptions<JwtConfiguration>()
                .Bind(builder.Configuration.GetSection("Jwt"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

// Transient, Scope, Singeton DI registeration
builder.Services.AddSingleton<JwtProvider>();
builder.Services.AddSingleton<IPasswordHasher<object>, PasswordHasher<object>>();
builder.Services.AddScoped<IDuplicateChecker, DuplicateChecker>();
builder.Services.AddScoped<IProductListClasses, ProductListClasses>();
builder.Services.AddScoped<IOrderItemList, OrderItemListClasses>();
builder.Services.AddScoped<IOrderStatus, GetStatus>();
builder.Services.AddScoped<IGetUserId, GetUserId>();
builder.Services.AddScoped<IGetUser, GetUser>();
builder.Services.AddScoped<IGetPost, GetPost>();
builder.Services.AddScoped<IGetFriend, GetFriend>();
builder.Services.AddScoped<IBlock, BlockChecker>();
builder.Services.AddScoped<IRelationship, RelationHandler>();
builder.Services.AddScoped<IProcessValidator, NullValidator>();
builder.Services.AddScoped<IProcessValidator, UnAuthorizedValidator>();
builder.Services.AddScoped<NullValidator>();
builder.Services.AddScoped<UnAuthorizedValidator>();
builder.Services.AddTransient<EmailValidator>();
builder.Services.AddTransient<NameValidator>();
builder.Services.AddTransient<RoleValidator>();


// Jwt Bearer
// IF appsetting.json Jwt doesn't exist, get default obj.
var Jwt = builder.Configuration.GetSection("Jwt").Get<JwtConfiguration>()
          ?? throw new InvalidOperationException("JWT configuration missing");

var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.SecretKey ?? ""));


// CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            // .AllowAnyOrigin()
            .AllowCredentials();
    });
});


// Treat passed in string able to convert to enum
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});


// Jwt validation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // All Register as "Bearer" validation endpoints
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = Jwt.Issuer,
                        ValidAudience = Jwt.Audience,
                        IssuerSigningKey = SecurityKey,
                        ClockSkew = TimeSpan.FromMinutes(1),
                        RoleClaimType = ClaimTypes.Role
                    };
                })
                .AddCookie("Cookie", c =>
                {
                    c.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = 401; return Task.CompletedTask; };
                    c.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = 403; return Task.CompletedTask; };
                    c.LoginPath = "/login";
                    c.ExpireTimeSpan = TimeSpan.FromHours(8);
                }); ;

// Endpoint role restriction setup
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "AdminAllowed", policy => policy.RequireRole("admin")
                                        .RequireClaim(ClaimTypes.NameIdentifier)
    );
    options.AddPolicy(
        "TeacherAllowed", policy => policy.RequireRole("teacher")
                                          .RequireClaim("TeacherId")
    );
    options.AddPolicy(
        "StudentAllowed", policy => policy.RequireRole("student")
                                          .RequireClaim("StudentId")
    );
    options.AddPolicy(
        "PrincipalAllowed", policy => policy.RequireRole("schoolPrincipal")
                                            .RequireClaim(ClaimTypes.NameIdentifier)
    );
    options.AddPolicy(
        "TeacherOrStudentAllowed", policy => policy.RequireRole("student", "teacher")
    );
    options.AddPolicy(
        "AllRoleAllowed", policy => policy.RequireRole("student", "teacher", "admin", "schoolPrincipal")
    );
});

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async httpContext =>
    {
        var pds = httpContext.RequestServices.GetService<IProblemDetailsService>();
        if (pds == null
            || !await pds.TryWriteAsync(new() { HttpContext = httpContext }))
        {
            // Fallback behavior
            await httpContext.Response.WriteAsync("Fallback: An error occurred.");
        }
    });
});

// login endpoints
app.MapPost("/login", async (
    MyAppDbContext dbContext, LoginDto dto, IPasswordHasher<object> passwordHasher,
    JwtProvider jwtProvider, RoleValidator roleValidator, HttpContext hc
) =>
{
    string? role = dto.Role?.ToLowerInvariant();
    ClaimsPrincipal? claimsPrincipal;

    // Exception handler
    if (role is null) return Results.BadRequest("Role is required");
    if (!roleValidator.IsValid(role))
        return Results.BadRequest("Unexpected role");

    if (role == "admin")
    {
        // Query SQL admin by login id

        var admin = await dbContext.Admins
                                   .AsNoTracking() // do not track changes
                                   .SingleOrDefaultAsync(a => a.AdminLoginId == dto.Id);  // Find match id admin

        if (admin is null) return Results.Unauthorized(); // No match

        var verified = passwordHasher.VerifyHashedPassword(
            admin, admin.PasswordHash!, dto.Password
        );

        // Verify password
        bool success = verified == PasswordVerificationResult.Success;

        // IF Success setup JWT
        if (!success) return Results.Unauthorized();

        var adminClaims = new[] {
            new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
            new Claim(ClaimTypes.Email, admin.Email ?? ""),
            new Claim(ClaimTypes.Role, "admin"),
        };

        var token = jwtProvider.Create(adminClaims);

        claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(adminClaims, "Cookie"));

        await hc.SignInAsync("Cookie", claimsPrincipal);

        return Results.Ok(token);
    }

    if (role == "schoolPrincipal")
    {
        SchoolPrincipal? sp = await dbContext.SchoolPrincipal.AsNoTracking()
                                                             .FirstOrDefaultAsync(
                                                                sp => sp.PrincipalId == dto.Id
                                                             );
        if (sp is null) return Results.Unauthorized();

        var success3 = passwordHasher.VerifyHashedPassword(sp, sp.PasswordHash, dto.Password);
        if (success3 != PasswordVerificationResult.Success) return Results.Unauthorized();

        var principalClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, sp.PrincipalId.ToString()),
            new Claim(ClaimTypes.Email, sp.Email ?? ""),
            new Claim(ClaimTypes.Role, "schoolPrincipal")
        };

        claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(principalClaims, "Cookie"));

        var token = jwtProvider.Create(principalClaims);

        await hc.SignInAsync("Cookie", claimsPrincipal);

        return Results.Ok(token);
    }

    User? user = role == "student"
                 ? await dbContext.Users.AsNoTracking()
                                        .Include(u => u.Student)
                                        .FirstOrDefaultAsync(u => u.Student != null && u.Student.StudentId == dto.Id)
                 : await dbContext.Users.AsNoTracking()
                                        .Include(u => u.Teacher)
                                        .FirstOrDefaultAsync(u => u.Teacher != null && u.Teacher.TeacherId == dto.Id);

    System.Console.WriteLine(user);

    if (user is null) return Results.Unauthorized();

    var verified2 = passwordHasher.VerifyHashedPassword(
        user, user.PasswordHash!, dto.Password
    );

    bool success2 = verified2 == PasswordVerificationResult.Success;

    // IF Success setup JWT
    if (!success2) return Results.Unauthorized();

    var claims = new List<Claim> {
        new (ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new (ClaimTypes.Role, role),
        new (ClaimTypes.Email, user.Email!)
    };

    if (role == "student") claims.Add(new Claim("StudentId", user.Student!.StudentId!));
    if (role == "teacher") claims.Add(new Claim("TeacherId", user.Teacher!.TeacherId!));

    var token2 = jwtProvider.Create(claims);

    claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookie"));

    await hc.SignInAsync("Cookie", claimsPrincipal);

    return Results.Ok(token2);
}).AllowAnonymous();

// Logout user
app.MapPost("/logout", async (HttpContext hc) =>
{
    await hc.SignOutAsync("Cookie");
    return Results.Ok();
}).RequireAuthorization("AllRoleAllowed")
  .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

// Register user
app.MapPost("/register", async (
    MyAppDbContext dbContext, CreateAccount dto,
    IDuplicateChecker duplicateDetector,
    EmailValidator emailValidator,
    NameValidator nameValidator,
    IPasswordHasher<object> passwordHasher,
    RoleValidator roleValidator
) =>
{
    var Role = dto.Role.ToLower();
    // Input checker
    if (!emailValidator.IsValid(dto.Email)) // Check format
        return Results.BadRequest("We only supported @gmail and @nkust.edu.tw registeration");
    if (!nameValidator.IsValid(dto.Name))
        return Results.BadRequest("Invalid name");
    if (!roleValidator.IsValid(Role) || Role is "admin" || Role is "SchoolPrincipal")
        return Results.BadRequest("Unexpected Role");

    // Databse validation
    if (await duplicateDetector.IsDuplicateAsync(dto.Role, dto.Email, dto.Id)) // Check database if email or id existed
        return Results.BadRequest("Email or Student/TeacherId has been register");

    if (string.Equals(dto.Role, "student", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("!!!!" + dto.Class);
        var cls = await dbContext.SchoolClasses.AsNoTracking()
                                               .Where(s => s.ClassName == dto.Class)
                                               .SingleOrDefaultAsync(s => s.ClassName == dto.Class);

        if (cls is null) return Results.BadRequest("Class not found");

        // CREATE NEW STUDENT
        Student student = new()
        {
            StudentId = dto.Id,
            ClassId = cls.ClassId,
            User = new()
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber ?? null,
                Email = dto.Email,
                PasswordHash = passwordHasher.HashPassword(default!, dto.Password)
            }
        };

        await dbContext.Students.AddAsync(student);
        await dbContext.SaveChangesAsync();

        StudentDto newStudent = new
        (
            dto.Id,
            dto.Name,
            dto.Class!,
            dto.PhoneNumber ?? "",
            dto.Email
        );

        return Results.Created($"/students/{student.StudentId}", newStudent);
    }
    else if (string.Equals(dto.Role, "teacher", StringComparison.OrdinalIgnoreCase)) // role is teacher 
    {
        Teacher teacher = new()
        {
            TeacherId = dto.Id,
            Points = 0,
            User = new()
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber ?? null,
                Email = dto.Email,
                PasswordHash = passwordHasher.HashPassword(default!, dto.Password)
            }
        };

        await dbContext.Teachers.AddAsync(teacher);
        await dbContext.SaveChangesAsync();

        TeacherDto newTeacher = new
        (
            dto.Id,
            dto.Name,
            0,
            dto.PhoneNumber ?? "",
            dto.Email
        );

        return Results.Created($"/teacher/{teacher.TeacherId}", newTeacher);
    }
    else
        return Results.BadRequest("The role is not allowed to register.");
}).AllowAnonymous();

// Refresh page requesting current user data by claim 
app.MapGet("/auth/me", (ClaimsPrincipal user) =>
{
    if (user.Identity?.IsAuthenticated != true) return Results.Unauthorized();

    var role = user.FindFirstValue(ClaimTypes.Role)!.ToLowerInvariant();
    var email = user.FindFirstValue(ClaimTypes.Email);
    var id = role == "student" ? user.FindFirstValue("StudentId")
             : role == "teacher" ? user.FindFirstValue("TeacherId")
             : user.FindFirstValue(ClaimTypes.NameIdentifier);
    Console.WriteLine(role, email, id);

    return Results.Ok(new { id = id, role = role, email = email });
}).RequireAuthorization("AllRoleAllowed")
  .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

// Todo: Return all possible school classes (no harm data)

app.MapGet("/", () => "Hello World");

app.UseCors("FrontendCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.ShoppingEndpoints().RequireAuthorization();
app.CommunityEndpoints().RequireAuthorization();
app.CommonEndpoints().RequireAuthorization();
app.ChatEndpoints().RequireAuthorization();
app.FriendShipEndpoints().RequireAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();



