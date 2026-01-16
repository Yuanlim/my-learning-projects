export type OrderByType = "Date" | "ThumbsUp";

export type ReplyType = {
  replyId: number;
  userId: string;
  userName: string;
  content: string;
  postDateTime: string;
  thumbsUp: number;
};

export type PostType = {
  postId: number;
  studentId: string;
  studentName: string;
  content: string;
  replies: ReplyType[];
  postDateTime: string;
  thumbsUp: number;
};

export type PostPayload = {
  Content: string;
};

export type ReplyPayload = {
  RepliedMainPostId: number;
  Content: string;
};

export type ThumbUpPayload = {
  MainPostId: number | null;
  ReplyId: number | null;
};
