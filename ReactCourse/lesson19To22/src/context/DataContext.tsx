import React, { useEffect, useState } from "react";
import { createContext } from "react";
import useAxiosFetch from "../hooks/useAxiosFetch";
import { PostType } from "../types/posts";
import { DataContextType } from "../types/context";

type Props = { children: React.ReactNode };
const DataContext = createContext<DataContextType | undefined>(undefined);

export const DataProvider = ({ children }: Props) => {
  const [posts, setPosts] = useState<PostType[]>([]);
  const [searchText, setSearchText] = useState("");
  const [searchResult, setSearchResult] = useState<PostType[]>([]);

  const { data, fetchError, isLoading } = useAxiosFetch(
    "http://localhost:3500/posts"
  );

  useEffect(() => {
    setPosts(data);
  }, [data]);

  useEffect(() => {
    setSearchResult(
      posts.filter(
        (p) => p.content.includes(searchText) || p.topic.includes(searchText)
      )
    );
  }, [searchText, posts]);

  return (
    <DataContext.Provider
      value={{
        searchText: searchText,
        setSearchText: setSearchText,
        searchResult: searchResult,
        posts: posts,
        setPosts: setPosts,
        isLoading: isLoading,
        fetchError: fetchError,
      }}
    >
      {children}
    </DataContext.Provider>
  );
};

export default DataContext;
