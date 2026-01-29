import React from "react";
import { ReplyType, ThumbUpPayload } from "../types/Community";
import { FaRegThumbsUp } from "react-icons/fa6";

type Props = {
  reply: ReplyType;
  toDate: (date: string) => string;
  handleThumbs: (payload: ThumbUpPayload) => Promise<void>;
};

const Reply = ({ reply, toDate, handleThumbs }: Props) => {
  return (
    <div className="card Reply__container" style={{ flexFlow: "column" }}>
      <p> User: {reply.userId}, {reply.userName} </p>
      <h3>{reply.content} </h3>
      <p> {toDate(reply.postDateTime)}</p>

      <div className="flex gap">
        <button type="button" className="flex"
          onClick={() => { handleThumbs({ MainPostId: null, ReplyId: reply.replyId }) }}>
          <FaRegThumbsUp /> {reply.thumbsUp}
        </button>
      </div>
    </div>
  );
};

export default Reply;
