using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;

namespace SchoolAppointmentApp.Mapping;

public static class CommunityPostMapping
{
    public static MainPostDto ToMainPostDto(this MainPost mainPost)
    {   
        ICollection<ReplyDto> replyDtos = [];
        foreach (var item in mainPost.Replies)
        {
            replyDtos.Add(item.ToReplyDto());
        }
        
        return new MainPostDto
        (
            PostId: mainPost.MainPostId,
            StudentId: mainPost.StudentId,
            StudentName: mainPost.Student.User!.Name,
            Content: mainPost.Content,
            Replies: replyDtos,
            PostDateTime: mainPost.PostDateTime,
            ThumbsUp: mainPost.NumOfThumbsUp
        );
    }

    public static ReplyDto ToReplyDto(this Reply reply)
    {
        System.Console.WriteLine($"\n\n\n\npass\n\n\n\n");
        string id = reply.Role == Roles.teacher ? reply.User.Teacher!.TeacherId
                                                : reply.User.Student!.StudentId;
            
        return new ReplyDto
        (
            ReplyId: reply.ReplyId,
            UserId: id,
            UserName: reply.User.Name,
            Content: reply.Content,
            PostDateTime: reply.PostDateTime,
            ThumbsUp: reply.NumOfThumbsUp
        );
    }

    public static ICollection<MainPostDto> ToMainPostListDto (this ICollection<MainPost> mainPosts)
    {
        ICollection<MainPostDto> mainPostDtos = [];
        foreach (var item in mainPosts)
        {
            MainPostDto mainPostDto = item.ToMainPostDto();
            mainPostDtos.Add(mainPostDto);
        }

        return mainPostDtos;
    }

}
