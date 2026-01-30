import React, { useState } from "react";
import { PostType, ReplyPayload, ThumbUpPayload } from "../types/Community";
import { FaRegThumbsUp } from "react-icons/fa6";
import { MdExpand } from "react-icons/md";
import Reply from "./Reply";

type Props = {
  post: PostType;
  toDate: (date: string) => string;
  handleThumbs: (payload: ThumbUpPayload) => Promise<void>;
  handleReply: (payload: ReplyPayload) => Promise<void>;
};

const Post = ({ post, toDate, handleThumbs, handleReply }: Props) => {
  const [openReplies, setOpenReplies] = useState<boolean>(false);
  const [replyContent, setReplyContent] = useState<string>("");

  return (
    <div
      className="card MainPost__container"
      style={{ flexFlow: "column", flexGrow: "1" }}
    >
      {/* Show content */}
      <p>
        User: {post.studentId}, {post.studentName}
      </p>
      <h3>{post.content}</h3>
      <p>{toDate(post.postDateTime)}</p>

      <div id="PostFuncButton" className="flex gap">
        <button type="button" className="flex"
          onClick={() => handleThumbs({ MainPostId: post.postId, ReplyId: null })}>
          <FaRegThumbsUp /> {post.thumbsUp}
        </button>
        <button
          type="button"
          className="flex"
          onClick={() => setOpenReplies(!openReplies)}
        >
          <MdExpand /> Reply
        </button>
      </div>

      <div id="ReplyPost" className="card ReplyPost__container">
        <input type="text" name="ReplyContent" value={replyContent}
          onChange={(e) => { setReplyContent(e.target.value) }} />
        <button type="button" className="flex"
          onClick={() => {
            handleReply({ RepliedMainPostId: post.postId, Content: replyContent });
            setReplyContent("");
          }}>
          Reply
        </button>
      </div>

      {openReplies && (
        <div>
          <h3 style={{ textDecoration: "underline" }}>Replies</h3>
          {post.replies.length ?
            post.replies.map((r) =>
              <Reply reply={r} handleThumbs={handleThumbs} toDate={toDate} key={r.replyId} />
            ) : <p>No reply to be shown.</p>
          }
        </div>
      )}
    </div>
  );
};

export default Post;
