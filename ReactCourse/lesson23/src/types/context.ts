import { PostType } from "./posts";
import { PostArrayStateFuncType, StringStateFuncType } from "./state";

export type DataContextType = {
  searchText: string;
  setSearchText: StringStateFuncType;
  searchResult: PostType[];
  posts: PostType[];
  setPosts: PostArrayStateFuncType;
  fetchError: string | null;
  isLoading: boolean;
};
