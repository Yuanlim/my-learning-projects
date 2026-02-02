import React from "react";
import { useParams } from "react-router-dom";
import { Link } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "./hooks/useReduxHooks";
import { deletePost, getPostById } from "./redux/post";

function Header() {
  // const { posts, setPosts } = useDataContext();
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const targetPost = useAppSelector((state) =>
    id ? getPostById(state, id) : undefined
  );
  const navigate = useNavigate();

  const handleDelete = (id: string) => {
    dispatch(deletePost(id));
    navigate("/");
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
