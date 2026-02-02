import React from "react";
import { PostType } from "./App";
import Feed from "./Feed";

type Props = { posts: PostType[] };

function Home({ posts }: Props) {
  return (
    <main className="main">
      {posts.map((post) => (
        <Feed post={post} key={post.id} />
      ))}
    </main>
  );
}

export default Home;
