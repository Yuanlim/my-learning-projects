import React, { useEffect } from "react";
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
import { setPosts } from "./redux/post";
import useAxiosFetch from "./hooks/useAxiosFetch";
import { useAppDispatch } from "./hooks/useReduxHooks";

function App() {
  const API_URL = "http://localhost:3500/posts?_sort=-id";
  const { data, fetchError, isLoading } = useAxiosFetch(API_URL);
  const dispatch = useAppDispatch();
  console.log("Axios data:", data);

  useEffect(() => {
    if (!data) return;
    dispatch(setPosts(data));
  }, [data, dispatch]);

  return (
    <div className="App">
      <DataProvider>
        <Header title="ReactTs BlogPosts" />
        <Routes>
          <Route
            path="/"
            element={<Home fetchError={fetchError} isLoading={isLoading} />}
          />
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
