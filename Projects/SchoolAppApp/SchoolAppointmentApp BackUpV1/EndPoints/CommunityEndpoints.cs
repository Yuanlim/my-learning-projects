using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;
using SchoolAppointmentApp.Mapping;

namespace SchoolAppointmentApp.EndPoints;

public static class Community
{
    public static RouteGroupBuilder CommunityEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/Community");

        group.MapGet("/GetMainDiscussion", async (
            [AsParameters] GetMainPostDto dto,
            CancellationToken ct,
            MyAppDbContext dbContext
        ) =>
        {
            // System.Console.WriteLine("Pass");
            IQueryable<MainPost> mainPostsQuery = dbContext.MainPosts;
            ICollection<MainPost> mainPosts = [];

            if (!string.IsNullOrWhiteSpace(dto.SearchString))
                mainPostsQuery = mainPostsQuery.AsNoTracking()
                                               .Where(
                                                    mp => EF.Functions.Like(mp.Content,
                                                    $"%{dto.SearchString}%")
                                               );

            // defualt order by thumbs up
            mainPostsQuery = mainPostsQuery.Include(mp => mp.Replies)
                                            .ThenInclude(r => r.User)
                                            .ThenInclude(u => u.Teacher)
                                           .Include(mp => mp.Replies)
                                            .ThenInclude(r => r.User)
                                            .ThenInclude(u => u.Student)
                                           .Include(mp => mp.Student)
                                            .ThenInclude(s => s.User);

            if (dto.OrderBy == "Date")
            {
                mainPostsQuery = dto.Ordering == OrderingTypes.asc ?
                                 mainPostsQuery.OrderBy(mp => mp.PostDateTime) :
                                 mainPostsQuery.OrderByDescending(mp => mp.PostDateTime);
            }
            else
            {
                mainPostsQuery = dto.Ordering == OrderingTypes.asc ?
                                 mainPostsQuery.OrderBy(mp => mp.NumOfThumbsUp) :
                                 mainPostsQuery.OrderByDescending(mp => mp.NumOfThumbsUp);
            }

            mainPosts = await mainPostsQuery.Skip((dto.StepAmount - 1) * 5)
                                            .ToListAsync(ct);

            return Results.Ok(mainPosts.ToMainPostListDto());

        }).RequireAuthorization("TeacherOrStudentAllowed")
          .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

        // I Post Something in the community for discussion.
        group.MapPost("/MainPost/Post", async (
            PostMainPostDto dto,
            ClaimsPrincipal user,
            CancellationToken ct,
            IGetUser userHandler,
            IGetUserId idHandler,
            MyAppDbContext dbContext,
            NullValidator validator
        ) =>
        {
            // content to a MainPost object
            // validation => userId, content
            IResult? contentResult = await validator.IsResults<string?>(
                new(
                    ValidateValue: dto.Content,
                    ClassValidation: ToArray.ToBooleanArray(3, [0])
                )
            );
            if (contentResult is not null) return contentResult;

            (User? user1, IResult? userResult) = await userHandler.GetUserBySomething(new(UserPrincipal: user), ct);
            if (userResult is not null) return userResult;
            // (string? userId, IResult? idResult) = await idHandler.GetIdByUser(new(User: user1, Roles: Roles.student));
            // System.Console.WriteLine($"\n\n\n\nPass\n\n\n\n");
            // if (idResult is not null) return idResult;

            MainPost mainPost = new()
            {
                StudentId = user1!.Student!.StudentId,
                Student = user1!.Student!,
                Content = dto.Content,
                Replies = [],
                PostDateTime = DateTime.Now,
                ThumbsUpInfos = [],
                NumOfThumbsUp = 0
            };

            await dbContext.MainPosts.AddAsync(mainPost, ct);
            await dbContext.SaveChangesAsync(ct);

            return Results.Created("New MainPost is created", null);
        }).RequireAuthorization("StudentAllowed")
          .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

        group.MapPost("/ReplyPost/Post", async (
            ReplyMainPostDto dto,
            ClaimsPrincipal user,
            CancellationToken ct,
            IGetUser userHandler,
            MyAppDbContext dbContext,
            NullValidator validator
        ) =>
        {
            IResult? contentResult = await validator.IsResults<string?>(
                new(
                    ValidateValue: dto.Content,
                    ClassValidation: ToArray.ToBooleanArray(3, [0])
                )
            );
            if (contentResult is not null) return contentResult;

            (User? user1, IResult? userResult) = await userHandler.GetUserBySomething(new(UserPrincipal: user), ct);
            if (userResult is not null) return userResult;

            Roles role = user.IsInRole("teacher") ? Roles.teacher : Roles.student;

            var theMainPost = await dbContext.MainPosts.FirstOrDefaultAsync(
                mp => mp.MainPostId == dto.RepliedMainPostId, ct
            );
            if (theMainPost is null) return Results.BadRequest("The main post never existed");

            theMainPost.Replies.Add(
                new()
                {
                    MainPostId = dto.RepliedMainPostId,
                    Role = role,
                    UserId = user1!.UserId,
                    User = user1,
                    Content = dto.Content,
                    PostDateTime = DateTime.Now,
                    ThumbsUpInfos = [],
                    NumOfThumbsUp = 0
                }
            );

            await dbContext.SaveChangesAsync(ct);

            return Results.Ok();
        }).RequireAuthorization("TeacherOrStudentAllowed")
          .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

        group.MapPost("/ThumbsUp/MainPost/", async (
            ThumbsUpDto dto,
            ClaimsPrincipal user,
            CancellationToken ct,
            MyAppDbContext dbContext,
            IGetUser userHandler,
            IGetPost postHandler
        ) =>
        {
            // Get User -> Verify user exist
            // Get MainPost => Verify it exist
            (User? User, IResult? userResults) = await userHandler.GetUserBySomething(new(UserPrincipal: user), ct);
            if (userResults is not null) return userResults;

            MainPost? mainPost;
            Reply? reply;
            ThumbsUpInfo? pastThumbsUpInfo;

            if (dto.ReplyId is null) // User thumb up mainPost?
            {
                (mainPost, IResult? postResults) = await postHandler.GetMainPostByItsIds(new(IndexId: dto.MainPostId), ct);
                if (postResults is not null) return postResults;
                pastThumbsUpInfo = mainPost!.ThumbsUpInfos.FirstOrDefault(tui => tui.UserId == User!.UserId)!;

                if (pastThumbsUpInfo is null)
                {
                    ThumbsUpInfo thumbsUpInfo = new()
                    {
                        MainPostId = dto.MainPostId,
                        UserId = User!.UserId,
                        User = User,
                        Thumbed = true
                    };

                    mainPost.ThumbsUpInfos?.Add(thumbsUpInfo);
                    mainPost.NumOfThumbsUp += 1;

                    await dbContext.SaveChangesAsync(ct);
                    return Results.Ok($"You Tumbed up UserName:{User!.Name} main post.");
                }

                pastThumbsUpInfo.Thumbed = !pastThumbsUpInfo.Thumbed;

                if (pastThumbsUpInfo.Thumbed) mainPost.NumOfThumbsUp += 1;
                else mainPost.NumOfThumbsUp -= 1;

                await dbContext.SaveChangesAsync(ct);
                return Results.Ok($"You have reverse your thumb up decision UserName:{User!.Name} main post.");
            }
            else if (dto.MainPostId is null) // User thumb up reply?
            {
                Console.WriteLine(dto.ReplyId);
                (reply, IResult? postResults) = await postHandler.GetReplyByItsIds(new(IndexId: dto.ReplyId), ct);
                if (postResults is not null) return postResults;
                pastThumbsUpInfo = reply!.ThumbsUpInfos.FirstOrDefault(tui => tui.UserId == User!.UserId)!;

                if (pastThumbsUpInfo is null)
                {
                    ThumbsUpInfo thumbsUpInfo = new()
                    {
                        ReplyId = dto.ReplyId,
                        UserId = User!.UserId,
                        User = User,
                        Thumbed = true
                    };

                    reply.ThumbsUpInfos?.Add(thumbsUpInfo);
                    reply.NumOfThumbsUp += 1;

                    await dbContext.SaveChangesAsync(ct);
                    return Results.Ok($"You Tumbed up UserName:{User!.Name} reply post.");
                }

                pastThumbsUpInfo.Thumbed = !pastThumbsUpInfo.Thumbed;

                if (pastThumbsUpInfo.Thumbed) reply.NumOfThumbsUp += 1;
                else reply!.NumOfThumbsUp -= 1;
                await dbContext.SaveChangesAsync(ct);
                return Results.Ok($"You have reverse your thumb up decision UserName:{User!.Name} reply post.");
            }
            else
                return Results.BadRequest("Must provided at least one id: ReplyId or MainId");

        }).RequireAuthorization("StudentAllowed")
          .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

        return group;
    }
}
