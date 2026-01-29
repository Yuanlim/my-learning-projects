import React from "react";
import { PostType } from "./types/posts";
import { Link } from "react-router-dom";

type Props = { post: PostType };

function Feed({ post }: Props) {
  return (
    <article id="post" className="post">
      <Link className="post__postLink" to={`post/${post.id}`}>
        <h2 className="post__title">{post.topic}</h2>
        <p className="post__date">{post.postDate}</p>
        <p>{post.content.slice(0, 25)}...</p>
      </Link>
    </article>
  );
}

export default Feed;
