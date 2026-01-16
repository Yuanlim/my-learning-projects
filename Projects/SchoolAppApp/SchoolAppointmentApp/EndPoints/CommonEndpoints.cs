using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;
using SchoolAppointmentApp.Mapping;

namespace SchoolAppointmentApp.EndPoints;

public static class Common
{
	public static RouteGroupBuilder CommonEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("/Common");

		group.MapPost("/Block", async (
				BlockRequestDto dto,
				ClaimsPrincipal user,
				IGetUser userHandler,
				IGetUserId idHandler,
				IGetFriend friendHandler,
				IErrorResults errorHandler,
				MyAppDbContext dbContext,
				CancellationToken ct,
				HttpContext hc
		) =>
		{
			IGetReturn? result;

			(User? user1, result) = await userHandler.GetUser(user, ct);
			if (user1 is null)
				return errorHandler.UnauthorizedResult(
					title: result!.Title,
					message: result.Message!,
					hc: hc
				);

			(User? user2, result) = await userHandler.GetUser(dto.ToUserId, ct);
			if (user2 is null)
				return errorHandler.NotFoundResult(
					title: result!.Title,
					message: result.Message!,
					hc: hc
				);

			// Find match
			Block? block = await dbContext.Blocks.FirstOrDefaultAsync(
				b => b.InitiatorId == user1!.UserId && user2!.UserId == b.ReceiverId,
				ct
			);

			if (block is not null)
			{
				block.Blocked = !block.Blocked;

				await dbContext.SaveChangesAsync(ct);
				string returnMessage = block.Blocked ? "Blocked" : "Unblocked";
				return Results.Ok($"You reversed your block decision to {returnMessage}.");
			}

			block = new()
			{
				InitiatorId = user1.UserId,
				ReceiverId = user2.UserId,
				Initiator = user1,
				Receiver = user2,
				Blocked = true
			};

			await dbContext.Blocks.AddAsync(block, ct);

			// Also if friendship exist between the two rebind it to denied
			(FriendRequest? friendRequest, _) = await friendHandler.GetBetweenRelation(
																									userId1: user1!.UserId,
																									userId2: user2!.UserId,
																									ct: ct
																							);

			if (friendRequest is not null)
			{
				FriendRequestStatus? deniedStatus =
								await dbContext.FriendRequestStatuses.FirstOrDefaultAsync(
										frs => frs.FriendRequestPossibleStatus == FriendRequestPossibleStatus.Denied
								);
				friendRequest.FriendRequestStatus = deniedStatus!;
			}

			await dbContext.SaveChangesAsync(ct);
			return Results.Created(
							"Your block request has been created",
							block.ToBlockDto()
					);

		}).RequireAuthorization("TeacherOrStudentAllowed")
			.RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

		return group;
	}
}