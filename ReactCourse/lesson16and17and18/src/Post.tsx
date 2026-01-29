import React from "react";
import { useParams } from "react-router-dom";
import { PostType } from "./App";

type Props = { posts: PostType[]; handleDelete: (id: number) => void };

function Header({ posts, handleDelete }: Props) {
  const { id } = useParams();
  console.log(Number(id));
  const targetPost = posts.find((post) => post.id === Number(id));
  console.log(targetPost);
  return (
    <main className="main">
      {targetPost && (
        <div className="post">
          <h2 className="post__title">{targetPost.topic}</h2>
          <p className="post__date">{targetPost.postDate.toUTCString()}</p>
          <p>{targetPost.content}...</p>
          <button
            className="deleteButton"
            type="button"
            onClick={() => handleDelete(targetPost.id)}
          >
            Delete
          </button>
        </div>
      )}
      {!targetPost && (
        <div>
          <h2>Post is not available</h2>
        </div>
      )}
    </main>
  );
}

export default Header;
