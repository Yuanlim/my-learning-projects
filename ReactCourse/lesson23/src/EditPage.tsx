import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "./hooks/useReduxHooks";
import { editPost, getPostById } from "./redux/post";

const EditPage = () => {
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const targetPost = useAppSelector((state) =>
    id ? getPostById(state, id) : undefined
  );
  const [editTitle, setEditTitle] = useState("");
  const [editContent, setEditContent] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    if (targetPost) {
      setEditTitle(targetPost.topic);
      setEditContent(targetPost.content);
    }
  }, [targetPost]);

  const handleEdit = (id: string): void => {
    dispatch(
      editPost({ id: id, editTitle: editTitle, editContent: editContent })
    );

    navigate("/");
  };

  return (
    <main className="main">
      {targetPost && (
        <form className="main__form" onSubmit={(e) => e.preventDefault()}>
          <label htmlFor="Edit_Title">Post Title</label>
          <textarea
            name="EditTitle"
            id="Edit_Title"
            className="form__textarea"
            value={editTitle}
            required
            onChange={(e) => setEditTitle(e.target.value)}
          />
          <label htmlFor="Post_Content">Post Content</label>
          <textarea
            name="PostContent"
            id="Post_Content"
            className="form__textarea"
            rows={6}
            value={editContent}
            required
            onChange={(e) => setEditContent(e.target.value)}
          />
          <button
            type="submit"
            onClick={() => handleEdit(targetPost.id)}
            className="postButton"
          >
            Edit
          </button>
        </form>
      )}
      {!targetPost && (
        <>
          <h2>404 Not Found</h2> <p>No such post existed</p>
        </>
      )}
    </main>
  );
};

export default EditPage;
