using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Data;

public class MyAppDbContext(DbContextOptions<MyAppDbContext> options) : DbContext(options)
{
    // User's table
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<SchoolPrincipal> SchoolPrincipal => Set<SchoolPrincipal>();

    // User to user propreties table
    public DbSet<User> Users => Set<User>();
    public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();
    public DbSet<FriendRequestStatus> FriendRequestStatuses => Set<FriendRequestStatus>();
    public DbSet<Block> Blocks => Set<Block>();
    public DbSet<Report> Reports => Set<Report>();

    // User personal properties table
    public DbSet<SchoolClass> SchoolClasses => Set<SchoolClass>();
    public DbSet<Message> Messages => Set<Message>();

    // Shopping Table
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
    public DbSet<OrderItemStatus> OrderItemStatuses => Set<OrderItemStatus>();

    // Community table
    public DbSet<Reply> Replies => Set<Reply>();
    public DbSet<MainPost> MainPosts => Set<MainPost>();
    public DbSet<ThumbsUpInfo> TumbsUpInfos => Set<ThumbsUpInfo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            // Setting Primary key
            entity.HasKey(eachEntity => eachEntity.ProductId);

            // (Often used) Faster Search
            entity.HasIndex(eachEntity => eachEntity.ProductId);

            // Default to 0 if empty
            entity.Property(eachEntity => eachEntity.PointCost).HasDefaultValue(0);

            entity.Property(eachEntity => eachEntity.ProductName).IsRequired();

            // Initialize some data
            entity.HasData(
                new
                {
                    ProductId = 1,
                    ProductName = "item",
                    ProductImageRoot = "../Example1",
                    Description = "Just a item",
                    AvailableQuantity = 5,
                    PointCost = 1
                },
                new
                {
                    ProductId = 2,
                    ProductName = "snack",
                    ProductImageRoot = "../Example2",
                    Description = "Just a snack",
                    AvailableQuantity = 30,
                    PointCost = 2
                },
                new
                {
                    ProductId = 3,
                    ProductName = "tool",
                    ProductImageRoot = "../Example3",
                    Description = "Just a tool ",
                    AvailableQuantity = 10,
                    PointCost = 3
                }
            );
        });

        modelBuilder.Entity<Student>(entity =>
        {
            // Setting Primary key
            entity.HasKey(eachEntity => eachEntity.StudentId);

            // (Often used) Faster Search
            entity.HasIndex(eachEntity => eachEntity.StudentId);

            // StudentId is required, maximum length under 30 
            // and should not be auto generated
            entity.Property(eachEntity => eachEntity.StudentId)
                  .IsRequired()
                  .HasMaxLength(10)
                  .ValueGeneratedNever();

            // Each student represent one class
            entity.HasOne(eachEntity => eachEntity.SchoolClass)
                  .WithMany()
                  .HasForeignKey(s => s.ClassId);

            // Student must have one class
            entity.Property(eachEntity => eachEntity.ClassId)
                  .IsRequired();

            // One to one with user table
            entity.HasOne(eachEntity => eachEntity.User)
                  .WithOne(user => user.Student)
                  .HasForeignKey<Student>(eachEntity => eachEntity.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Each student must required a UserId
            entity.Property(eachEntity => eachEntity.UserId)
                  .IsRequired();
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(eachEntity => eachEntity.TeacherId);

            entity.HasIndex(eachEntity => eachEntity.TeacherId);

            entity.Property(eachEntity => eachEntity.TeacherId)
                  .IsRequired()
                  .HasMaxLength(10)
                  .ValueGeneratedNever();

            // Defualt to 0 if null
            entity.Property(eachEntity => eachEntity.Points)
                  .HasDefaultValue(0);

            entity.HasOne(eachEntity => eachEntity.User)
                  .WithOne(user => user.Teacher)
                  .HasForeignKey<Teacher>(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(eachEntity => eachEntity.UserId)
                  .IsRequired();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(eachEntity => eachEntity.UserId);

            // User must have password and email
            entity.Property(eachEntity => eachEntity.PasswordHash)
                  .IsRequired();
            entity.Property(eachEntity => eachEntity.Email)
                  .IsRequired();

            // User phone and email is uniquely different
            entity.HasIndex(eachEntity => eachEntity.PhoneNumber)
                  .IsUnique();
            entity.HasIndex(eachEntity => eachEntity.Email)
                  .IsUnique();
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(eachEntity => eachEntity.MessageId);

            entity.HasIndex(eachEntity => new { eachEntity.SenderId, eachEntity.ReceiverId });

            entity.HasOne(eachEntity => eachEntity.Receiver)
                  .WithMany()
                  .HasForeignKey(eachEntity => eachEntity.SenderId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(eachEntity => eachEntity.Receiver)
                  .WithMany()
                  .HasForeignKey(eachEntity => eachEntity.ReceiverId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Content || Audio || Image must have at least one
            entity.ToTable(t => t.HasCheckConstraint(
                "CheckMessageHasExactlyOneTypeOfContent", // Name
                                                          // SQL script check content is null and less then 0 charaters
                @"(CASE WHEN ""Content"" IS NOT NULL AND length(trim(""Content"")) > 0 THEN 1 ELSE 0 END) +" +
                @"(CASE WHEN ""AudioMessageRoot"" IS NOT NULL THEN 1 ELSE 0 END) +" +
                @"(CASE WHEN ""ImageMessageRoot"" IS NOT NULL THEN 1 ELSE 0 END) = 1"
            ));
        });

        modelBuilder.Entity<SchoolClass>(entity =>
        {
            entity.HasKey(eachEntity => eachEntity.ClassId);

            entity.Property(eachEntity => eachEntity.ClassName)
                  .IsRequired();

            entity.HasData(
                new { ClassId = 1, ClassName = "電通一甲" },
                new { ClassId = 2, ClassName = "電通二甲" },
                new { ClassId = 3, ClassName = "電通三甲" }
            );
        });

        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.HasKey(eachEntity => eachEntity.RequestId);

            entity.HasIndex(eachEntity => new { eachEntity.ReceiverId, eachEntity.InitiatorId });

            entity.HasOne(eachEntity => eachEntity.Receiver)
                  .WithMany() // Receiver possibilly has many friend request
                  .HasForeignKey(eachEntity => eachEntity.ReceiverId) // FK keys of FriendStatus Receiver id
                  .IsRequired();

            entity.HasOne(eachEntity => eachEntity.Initiator)
                  .WithMany()
                  .HasForeignKey(eachEntity => eachEntity.InitiatorId)
                  .IsRequired();

            entity.HasOne(eachEntity => eachEntity.FriendRequestStatus)
                  .WithMany()
                  .HasForeignKey(eachEntity => eachEntity.StatusId)
                  .IsRequired();
        });

        modelBuilder.Entity<FriendRequestStatus>(fs =>
        {
            fs.HasKey(fs => fs.StatusId);

            fs.Property(fs => fs.FriendRequestPossibleStatus)
              .HasConversion<string>();

            fs.HasData(
                new { StatusId = 1, FriendRequestPossibleStatus = FriendRequestPossibleStatus.Pending },
                new { StatusId = 2, FriendRequestPossibleStatus = FriendRequestPossibleStatus.Denied },
                new { StatusId = 3, FriendRequestPossibleStatus = FriendRequestPossibleStatus.Accepted }
            );
        });

        modelBuilder.Entity<Block>(entity =>
        {
            entity.HasKey(eachEntity => eachEntity.RequestId);

            entity.HasIndex(eachEntity => new { eachEntity.ReceiverId, eachEntity.InitiatorId });

            entity.HasOne(eachEntity => eachEntity.Receiver)
                  .WithMany() // Receiver possibilly has many Block request
                  .HasForeignKey(eachEntity => eachEntity.ReceiverId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(eachEntity => eachEntity.Initiator)
                  .WithMany()
                  .HasForeignKey(eachEntity => eachEntity.InitiatorId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(eachEntity => eachEntity.RequestId);

            entity.HasIndex(eachEntity => new { eachEntity.ReceiverId, eachEntity.InitiatorId });

            entity.HasOne(eachEntity => eachEntity.Receiver)
                  .WithMany() // Receiver possibilly has many Report request
                  .HasForeignKey(eachEntity => eachEntity.ReceiverId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(eachEntity => eachEntity.Initiator)
                  .WithMany()
                  .HasForeignKey(eachEntity => eachEntity.InitiatorId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(eachEntity => eachEntity.AdminId);

            // Avoid computing error
            entity.Property(eachEntity => eachEntity.PasswordHash).IsRequired();
            entity.Property(eachEntity => eachEntity.AdminLoginId).IsRequired();
            entity.HasIndex(eachEntity => eachEntity.AdminLoginId).IsUnique();

            entity.HasData(
                new
                {
                    AdminId = 1,
                    AdminLoginId = "iwueowsakd62981sksai",
                    PasswordHash = "AQAAAAIAAYagAAAAEPkLFJ63cyGZh4YEMMBflj7olrKjkCRfswg70N4NWZyONPxcarnHnhuX2zozI1OGAg=="
                },
                new
                {
                    AdminId = 2,
                    AdminLoginId = "84u232fhfehw889d0ufd",
                    PasswordHash = "AQAAAAIAAYagAAAAEF5EzKnMIKp0aWrnmYAxClS2aiFfz0dDljh38TEU1KdwOcJnzpjiSK6Hczvs53pM1Q=="
                }
            );
        });

        modelBuilder.Entity<Cart>(c =>
        {
            c.HasKey(x => x.CartId);

            c.Property(x => x.CustomerId)
             .IsRequired();

            c.HasIndex(x => x.CustomerId)
             .IsUnique()
             .HasFilter("[Ordered] = 0");

            c.Property(x => x.Ordered)
             .IsRequired();

            // 1 Cart has many CartItem
            c.HasMany(x => x.CartProductList)
             .WithOne(x => x.Cart)
             .HasForeignKey(x => x.CartId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Cascade);

            // 1 customer & its teacher has many carts
            c.HasOne(x => x.Teacher)
             .WithMany()
             .HasForeignKey(x => x.CustomerId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Restrict);

        });

        modelBuilder.Entity<CartItem>(ci =>
        {
            ci.HasKey(x => x.CartItemId);

            // Many Cart product all of them has one Product
            ci.HasOne(x => x.Product)
              .WithMany()
              .HasForeignKey(x => x.ProductId)
              .OnDelete(DeleteBehavior.Restrict);

            // Table requirement
            ci.Property(x => x.CartId)
              .IsRequired();
            ci.Property(x => x.ProductId)
              .IsRequired();
        });

        modelBuilder.Entity<OrderStatus>(os =>
        {
            os.HasKey(os => os.StatusId);

            // Saved it as status string
            os.Property(x => x.Status)
              .HasConversion<string>();

            os.HasData(
                new { StatusId = 1, Status = OrderPossibleStatus.pending },
                new { StatusId = 2, Status = OrderPossibleStatus.cancelled },
                new { StatusId = 3, Status = OrderPossibleStatus.received },
                new { StatusId = 4, Status = OrderPossibleStatus.mix }// Received some but cancelled some.
            );
        });

        modelBuilder.Entity<OrderItemStatus>(ois =>
        {
            ois.HasKey(ois => ois.StatusId);

            ois.Property(x => x.Status)
               .HasConversion<string>();

            ois.HasData(
                new { StatusId = 1, Status = OrderItemPossibleStatus.pending },
                new { StatusId = 2, Status = OrderItemPossibleStatus.received },
                new { StatusId = 3, Status = OrderItemPossibleStatus.cancelled }
            );
        });

        modelBuilder.Entity<Order>(o =>
        {
            o.HasKey(x => x.OrderId);

            o.Property(x => x.CustomerId)
             .IsRequired();

            o.HasIndex(x => x.CustomerId);

            // Many order maybe place by one teacher(as customer)
            // Relation in customerId
            o.HasOne(x => x.Teacher)
             .WithMany()
             .HasForeignKey(x => x.CustomerId)
             .OnDelete(DeleteBehavior.Restrict);

            // 1 order may have many items, with Fk of OrderId
            o.HasMany(x => x.OrderItems)
             .WithOne(x => x.Order)
             .HasForeignKey(x => x.OrderId)
             .OnDelete(DeleteBehavior.Restrict);

            // 1 order status can be in pending, cancelled or received.
            // Many Orders can have its own independent status
            o.HasOne(x => x.OrderStatus)
             .WithMany()
             .HasForeignKey(x => x.StatusId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(oi =>
        {
            oi.HasKey(x => x.OrderItemId);

            oi.HasOne(x => x.Product)
              .WithMany()
              .HasForeignKey(x => x.ProductId)
              .OnDelete(DeleteBehavior.Restrict);

            oi.HasOne(x => x.Order)
              .WithMany(x => x.OrderItems)
              .HasForeignKey(x => x.OrderId)
              .OnDelete(DeleteBehavior.Restrict);

            oi.HasOne(x => x.OrderItemStatus)
              .WithMany()
              .HasForeignKey(x => x.StatusId)
              .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SchoolPrincipal>(sp =>
        {
            sp.HasKey(sp => sp.Id);
            sp.Property(x => x.PasswordHash).IsRequired();
            sp.Property(x => x.PrincipalId).IsRequired();

            sp.HasData(
                new
                {
                    Id = 1,
                    PrincipalId = "L123456",
                    PasswordHash = "AQAAAAIAAYagAAAAEOe6noMHGXGrXzbCSkir9wB2m2z8GwLZTUp69XY2CT9Bpe4dwpTh29iOYbVBPp2dNw=="
                }
            );
        });

        modelBuilder.Entity<MainPost>(mp =>
        {
            mp.HasKey(x => x.MainPostId);

            mp.HasOne(x => x.Student)
              .WithMany()
              .HasForeignKey(mps => mps.StudentId)
              .OnDelete(DeleteBehavior.Restrict);

            mp.HasMany(x => x.Replies)
              .WithOne()
              .HasForeignKey(r => r.MainPostId)
              .OnDelete(DeleteBehavior.Restrict);

            mp.Property(x => x.Content)
              .IsRequired();
            mp.Property(x => x.PostDateTime)
              .IsRequired();

        });

        modelBuilder.Entity<Reply>(r =>
        {
            r.HasKey(x => x.ReplyId);

            r.HasOne(x => x.MainPost)
             .WithMany(mp => mp.Replies)
             .HasForeignKey(x => x.MainPostId)
             .OnDelete(DeleteBehavior.Restrict);

            r.HasOne(x => x.User)
             .WithMany()
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            r.Property(x => x.Content)
             .IsRequired();
            r.Property(x => x.PostDateTime)
             .IsRequired();
            r.Property(x => x.UserId)
             .IsRequired();
            r.Property(x => x.MainPostId)
             .IsRequired();
        });

        modelBuilder.Entity<ThumbsUpInfo>(tui =>
        {
            tui.HasKey(x => x.ThumbsUpInfoId);

            tui.ToTable(t => t.HasCheckConstraint(
                "CK_ReplyIdAndPostIdCoExist",
                @"(""MainPostId"" IS NULL AND ""ReplyId"" IS NOT NULL)" +
                @"OR (""ReplyId"" IS NULL AND ""MainPostId"" IS NOT NULL)"
            ));

            // Uniquely indentify
            tui.HasIndex(x => new { x.MainPostId, x.UserId });
            tui.HasIndex(x => new { x.ReplyId, x.UserId });

            tui.HasOne(x => x.MainPost)
               .WithMany(x => x.ThumbsUpInfos)
               .HasForeignKey(x => x.MainPostId)
               .OnDelete(DeleteBehavior.Restrict);

            tui.HasOne(x => x.Reply)
               .WithMany(x => x.ThumbsUpInfos)
               .HasForeignKey(x => x.ReplyId)
               .OnDelete(DeleteBehavior.Restrict);

            // Is Required
            tui.Property(x => x.UserId).IsRequired();
            tui.Property(x => x.Thumbed).IsRequired();
        });

    }

}
