using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;
using SchoolAppointmentApp.Mapping;

namespace SchoolAppointmentApp.EndPoints;

public static class Shopping
{
	public static RouteGroupBuilder ShoppingEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("/Shopping");

		group.PointsEndpoints()
					.RequireAuthorization()
					.RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

		group.CartEndpoints()
					.RequireAuthorization()
					.RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

		group.OrderEndpoints()
					.RequireAuthorization()
					.RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

		group.ProductEndpoints()
					.RequireAuthorization()
					.RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

		// I put something in my cart endpoint
		group.MapPost("/WishList", async (
			WishItemDto dto,
			MyAppDbContext dbContext,
			CancellationToken ct,
			ClaimsPrincipal user,
			IErrorResults errorHandler,
			HttpContext hc,
			UnAuthorizedValidator validator
		) =>
		{
			// Validation of user
			(bool auth, Teacher? teacher) = await validator.IsResults<Teacher>(
				expectedRole: Roles.teacher,
				user: user,
				ct: ct
			);

			if (teacher is null)
				return errorHandler.UnauthorizedResult(
					title: "Reported fake user",
					message: "Unautherized, user doesnt existed",
					hc: hc
				);


			Product? product = await dbContext.Products.AsNoTracking()
																								.FirstOrDefaultAsync(
																									p => p.ProductId == dto.ProductId,
																									ct
																								);
			if (product is null)
				return errorHandler.NotFoundResult(
						title: "I dont exist issue",
						message: "Product doesn't exist",
						hc: hc

					);

			if (product.AvailableQuantity < dto.Quantity)
				return errorHandler.BadReqResult(
						title: "Quantity issue",
						message: "Product insufficient quantity to suffice user request",
						hc: hc
					);

			// Check wether there is cart hasn't been ordered
			Cart? teacherCart = await dbContext.Carts.Include(c => c.CartProductList)
																									.ThenInclude(c => c.Product)
																								.Where(
																									c => c.CustomerId == teacher.TeacherId
																									&& c.Ordered == false
																								)
																								.FirstOrDefaultAsync(ct);

			if (teacherCart is null)
			{
				// Nothing new was wished in the list, but trying to add item (new Cart)
				teacherCart = new()
				{
					CustomerId = teacher.TeacherId,
					Ordered = false,
					TotalCost = product!.PointCost * dto.Quantity,
					CartProductList = [new CartItem
						{
							ProductId = product.ProductId,
							Product = product,
							Quantity = dto.Quantity
						}
					]
				};

				await dbContext.Carts.AddAsync(teacherCart, ct);
				await dbContext.SaveChangesAsync(ct);

				return Results.Created("Product is added to the cart", teacherCart.ToCartDto());
			}

			// Check cart has the same product
			CartItem? sameCartProduct = teacherCart.CartProductList.FirstOrDefault(
				cpl => cpl.ProductId == dto.ProductId
			);

			// Existed add quantity, 
			if (sameCartProduct is null)
			{
				teacherCart.CartProductList.Add(
					new()
					{
						ProductId = dto.ProductId,
						Product = product, // For Total calculation 
						Quantity = dto.Quantity
					}
				);
			}
			else
			{
				sameCartProduct.Quantity += dto.Quantity;
			}

			// Recompute total cost
			int totalCost = 0;
			foreach (var item in teacherCart.CartProductList)
				totalCost += item.Quantity * item.Product.PointCost;

			teacherCart.TotalCost = totalCost;

			await dbContext.SaveChangesAsync(ct);
			return Results.Created("Product is added to the cart", teacherCart.ToCartDto());
		}).RequireAuthorization("TeacherAllowed")
			.RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

		return group;
	}
}