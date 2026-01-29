import React from "react";
import { useParams } from "react-router-dom";
import { Link } from "react-router-dom";
import useDataContext from "./hooks/useDataContext";
import postApi from "./api/posts";
import { useNavigate } from "react-router-dom";
import LogError from "./functions/LogError";

function Header() {
  const { posts, setPosts } = useDataContext();
  const { id } = useParams();
  const targetPost = posts.find((post) => post.id === id);
  const navigate = useNavigate();

  const handleDelete = async (id: string): Promise<void> => {
    try {
      await postApi.delete(`/posts/${id}`);
      const filteredPost = posts.filter((post) => post.id !== id);
      setPosts(filteredPost);
      navigate("/");
    } catch (error) {
      LogError(error);
    }
  };

  return (
    <main className="main">
      {targetPost && (
        <div className="post">
          <h2 className="post__title">{targetPost.topic}</h2>
          <p className="post__date">{targetPost.postDate}</p>
          <p>{targetPost.content}</p>
          <div>
            <Link to={`http://localhost:3000/edit/${id}`}>
              <button type="submit" className="editButton">
                Edit
              </button>
            </Link>
            <button
              className="deleteButton"
              type="button"
              onClick={() => handleDelete(targetPost.id)}
            >
              Delete
            </button>
          </div>
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
