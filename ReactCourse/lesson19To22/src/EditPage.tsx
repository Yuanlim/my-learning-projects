import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import useDataContext from "./hooks/useDataContext";
import postApi from "./api/posts";
import { useNavigate } from "react-router-dom";
import LogError from "./functions/LogError";

const EditPage = () => {
  const { posts, setPosts } = useDataContext();
  const { id } = useParams();
  const targetPost = posts.find((p) => p.id === id);
  const [editTitle, setEditTitle] = useState("");
  const [editContent, setEditContent] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    if (targetPost) {
      setEditTitle(targetPost.topic);
      setEditContent(targetPost.content);
    }
  }, [targetPost]);

  const handleEdit = async (id: string): Promise<void> => {
    let updatePost = posts.find((post) => post.id === id);
    if (!updatePost) return;
    updatePost = { ...updatePost, topic: editTitle, content: editContent };

    try {
      const reponse = await postApi.put(`/posts/${id}`, updatePost);
      setPosts(posts.map((post) => (post.id === id ? reponse.data : post)));
      navigate("/");
    } catch (error) {
      LogError(error);
    }
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
