namespace SchoolAppointmentApp.Entities;

using SchoolAppointmentApp.FunctionalClasses;

public record class MainPost
{
    public int MainPostId { get; init; }
    public string StudentId { get; init; } = null!;
    public Student Student { get; init; } = null!;
    public string Content { get; set; } = null!;
    public ICollection<Reply> Replies { get; set; } = [];
    public DateTime PostDateTime { get; set; }
    public ICollection<ThumbsUpInfo> ThumbsUpInfos { get; set; } = [];
    public int NumOfThumbsUp { get; set; }
}

public record class Reply
{
    public int ReplyId { get; init; }
    public int MainPostId { get; init; }
    public MainPost MainPost { get; init; } = null!;
    public Roles Role { get; init; }
    public int UserId { get; init; }
    public User User { get; init; } = null!;
    public string Content { get; set; } = null!;
    public DateTime PostDateTime { get; init; }
    public ICollection<ThumbsUpInfo> ThumbsUpInfos { get; set; } = [];
    public int NumOfThumbsUp { get; set; }
}

// Duplicate user TumbsUp same post
public record class ThumbsUpInfo
{
    public int ThumbsUpInfoId { get; init; }
    public int? MainPostId { get; init; }
    public MainPost? MainPost { get; init; }
    public int? ReplyId { get; init; }
    public Reply? Reply { get; init; }
    public int UserId { get; init; }
    public User User { get; init; } = null!;
    public bool Thumbed { get; set; }
};