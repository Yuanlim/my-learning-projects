import React, { useState } from "react";
import { PostType } from "./types/posts";
import { useNavigate } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "./hooks/useReduxHooks";
import { savePost } from "./redux/post";

function PostPage() {
  const [postTitle, setPostTitle] = useState("");
  const [postContent, setPostContent] = useState("");
  // const { posts, setPosts } = useDataContext();
  const dispatch = useAppDispatch();
  const { posts } = useAppSelector((state) => state.posts);
  const navigate = useNavigate();

  const setNewPost = () => {
    const newId = posts.length ? Number(posts[posts.length - 1].id) + 1 : 1;
    const newPost: PostType = {
      id: String(newId),
      topic: postTitle,
      content: postContent,
      postDate: new Date().toLocaleString(),
    };
    dispatch(savePost(newPost));
    navigate("/");
  };

  return (
    <main className="main">
      <form className="main__form" onSubmit={(e) => e.preventDefault()}>
        <label htmlFor="Post_Title">Post Title</label>
        <textarea
          name="PostTitle"
          id="Post_Title"
          className="form__textarea"
          required
          onChange={(e) => setPostTitle(e.target.value)}
        />
        <label htmlFor="Post_Content">Post Content</label>
        <textarea
          name="PostContent"
          id="Post_Content"
          className="form__textarea"
          rows={6}
          required
          onChange={(e) => setPostContent(e.target.value)}
        />
        <button type="submit" onClick={setNewPost} className="postButton">
          Post
        </button>
      </form>
    </main>
  );
}

export default PostPage;
