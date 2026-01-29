import React from "react";

type Props = {
  postTitle: string;
  postContent: string;
  setPostTitle: React.Dispatch<React.SetStateAction<string>>;
  setPostContent: React.Dispatch<React.SetStateAction<string>>;
  setNewPost: () => void;
};

function PostPage({
  postTitle,
  postContent,
  setPostTitle,
  setPostContent,
  setNewPost,
}: Props) {
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
