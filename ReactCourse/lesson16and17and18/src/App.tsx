import React, { useState } from "react";
import Header from "./Header";
import Footer from "./Footer";
import { Routes, Route, useNavigate } from "react-router-dom";
import Home from "./Home";
import PostPage from "./PostPage";
import Post from "./Post";
import Missing from "./Missing";
import About from "./About";

export type PostType = {
  id: number;
  topic: string;
  content: string;
  postDate: Date;
};

function App() {
  const [posts, setPosts] = useState<PostType[]>([
    {
      id: 1,
      topic:
        "Constantly losing interest when I start coding — how do I fix this?",
      content:
        "Hi everyone, I have a problem. I really love programming, and I enjoy diving deep into concepts and understanding programming terms. I also love writing code and I want to create a game in Unity. Everything seems clear in theory, but the problem is that I don't understand what to do next. I have the desire and the idea, but I struggled with procrastination, and for the whole year I was just dreaming about making a game and learning. But whenever I sat down to write code, I would completely lose interest. Now I finally feel motivated again and I have hope that I can do it. Can you give me some advice?",
      postDate: new Date(2025, 11, 24, 7, 11, 23),
    },
    {
      id: 2,
      topic: "Implementing the Pipe Operator in C# 14",
      content:
        "Inspired by one of the previous posts that created a Result monad, I decided to experiment a bit and to create an F#-like pipe operator using extension members.",
      postDate: new Date(2025, 11, 21, 7, 11, 23),
    },
    {
      id: 3,
      topic: "Figma and WPF",
      content:
        "I'm responsible for a software development project at my company. It will be a C# desktop app with WPF UI, but for the first time we will involve a 3rd party to design the UI. I want to make the job of my developer as easy as possible with the UI so it came to my mind if it is possible to export the design from figma into XAML which could be directly imported into the C# project in Visual Studio.",
      postDate: new Date(2025, 11, 2, 7, 11, 23),
    },
  ]);
  const [searchText, setSearchText] = useState("");
  const [postTitle, setPostTitle] = useState("");
  const [postContent, setPostContent] = useState("");
  let navigate = useNavigate();

  const handleDelete = (id: number): void => {
    const filteredPost = posts.filter((post) => post.id !== id);
    setPosts(filteredPost);
    navigate("/");
  };

  const setNewPost = (): void => {
    const newId = posts.length ? posts[posts.length - 1].id + 1 : 1;
    const newPost: PostType = {
      id: newId,
      topic: postTitle,
      content: postContent,
      postDate: new Date(),
    };
    setPosts([...posts, newPost]);
    navigate("/");
  };

  return (
    <div className="App">
      <Header
        title="ReactTs BlogPosts"
        searchText={searchText}
        setSearchText={setSearchText}
      />
      {/* <Nav /> */}
      <Routes>
        <Route
          path="/"
          element={
            <Home
              posts={posts.filter(
                (post) =>
                  post.topic.includes(searchText) ||
                  post.content.includes(searchText)
              )}
            />
          }
        />
        <Route
          path="/post"
          element={
            <PostPage
              postTitle={postTitle}
              postContent={postContent}
              setPostTitle={setPostTitle}
              setPostContent={setPostContent}
              setNewPost={setNewPost}
            />
          }
        />
        <Route
          path="/post/:id"
          element={<Post posts={posts} handleDelete={handleDelete} />}
        />
        <Route path="/about" element={<About />} />
        <Route path="*" element={<Missing />} />
      </Routes>
      <Footer />
    </div>
  );
}

export default App;
