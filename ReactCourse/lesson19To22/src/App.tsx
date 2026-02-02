import React from "react";
import Header from "./Header";
import Footer from "./Footer";
import Home from "./Home";
import PostPage from "./PostPage";
import Post from "./Post";
import Missing from "./Missing";
import About from "./About";
import EditPage from "./EditPage";
import { Routes, Route } from "react-router-dom";
import { DataProvider } from "./context/DataContext";

function App() {
  return (
    <div className="App">
      <DataProvider>
        <Header title="ReactTs BlogPosts" />
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/post" element={<PostPage />} />
          <Route path="/post/:id" element={<Post />} />
          <Route path="/edit/:id" element={<EditPage />} />
          <Route path="/about" element={<About />} />
          <Route path="*" element={<Missing />} />
        </Routes>
      </DataProvider>
      <Footer />
    </div>
  );
}

export default App;
