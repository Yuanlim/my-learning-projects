import React, { useState } from "react";
import useCheckDirectAccessor from "./hooks/useCheckDirectAccessor";
import useCommunity from "./hooks/useCommunity";
import { IoMdAdd } from "react-icons/io";
import {
  FaSearch,
  FaSortAmountDown,
  FaSortAmountDownAlt,
} from "react-icons/fa";
import Post from "./component/Post";

export const toDate = (date: string) => new Date(date).toLocaleString();

function Community() {
  // TODO: organize css
  useCheckDirectAccessor();
  const {
    posts, orderBy, handleOrderBy, handleAddPost,
    postContent, setPostContent, orderingBy,
    handleOrderingBy, handleSearch, handleReply,
    handleThumbs
  } = useCommunity();
  const [searchString, setSearchString] = useState<string>("");

  return (
    <main
      className="main"
      style={{
        flexFlow: "column nowrap",
        justifyContent: "center",
        width: "clamp(375px, 80vw, 800px)",
      }}
    >
      <div id="SearchPost" className="card SearchPost__container">
        <button type="button" className="card withTip" onClick={handleOrderingBy}>
          {orderingBy === "asc" ? (
            <FaSortAmountDownAlt />
          ) : (
            <FaSortAmountDown />
          )}
          <span className="tooltip left">Ordering By:{orderingBy}</span>
        </button>
        <button type="button" className="card" onClick={handleOrderBy}>
          {orderBy}
        </button>
        <input
          type="text"
          name="SearchContent"
          id="SearchContent"
          value={searchString}
          placeholder="Search dedicate post..."
          onChange={(e) => setSearchString(e.target.value)}
        />
        <button
          type="button"
          className="flex withTip"
          onClick={() => {
            handleSearch(searchString);
            setSearchString("");
          }}
        >
          <FaSearch />
          <span className="tooltip right">search</span>
        </button>
      </div>

      <div id="AddPost" className="card PostPost__container">
        <input
          type="text"
          name="PostContent"
          id="PostContent"
          value={postContent}
          placeholder="Share your thoughts..."
          onChange={(e) => setPostContent(e.target.value)}
        />
        <button type="button" className="flex" onClick={handleAddPost}>
          <IoMdAdd /> Post
        </button>
      </div>

      {posts && posts.map((p) => (
        <Post
          post={p}
          handleThumbs={handleThumbs}
          handleReply={handleReply}
          toDate={toDate}
          key={p.postId}
        />
      ))}

      {!posts.length &&
        <div className="card contextCenter" style={{ flexGrow: "1" }}>
          Not Match Post to be shown.
        </div>
      }

    </main>
  );
}

export default Community;
