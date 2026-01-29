import { createSlice, createAsyncThunk, PayloadAction } from "@reduxjs/toolkit";
import { PostType } from "../types/posts";
import postsApi from "../api/posts";
import { RootState } from "./store";

/* 
Key takeaway: base dispatch function/async should be only calling api alter everthing outside, never influence state inside. reducer is only a state changer and only a state changer, extra reducer is really a eventListner for base dispatch function/async act when after the event specific status.
*/

interface PostState {
  posts: PostType[];
  searchText: string;
}

interface EditPostPayload {
  id: string;
  editTitle: string;
  editContent: string;
}

const initialState: PostState = {
  posts: [],
  searchText: "",
};

export const savePost = createAsyncThunk<
  PostType,
  PostType,
  { state: RootState }
>("posts/save", async (newPost) => {
  const response = await postsApi.post("/posts", newPost);
  return response.data as PostType;
});

const toPostState = (state: RootState) => state.posts;

export const editPost = createAsyncThunk<
  PostType,
  EditPostPayload,
  { state: RootState }
>("posts/edit", async (newEditPost, thunkAPI) => {
  const targetPost = getPostById(thunkAPI.getState(), newEditPost.id);
  if (!targetPost) {
    throw new Error("Post not found");
  }
  const updatePost = {
    ...targetPost,
    topic: newEditPost.editTitle,
    content: newEditPost.editContent,
  };

  const response = await postsApi.put(`/posts/${newEditPost.id}`, updatePost);
  return response.data as PostType;
});

export const deletePost = createAsyncThunk<
  string,
  string,
  { state: RootState }
>("post/delete", async (id) => {
  await postsApi.delete(`/posts/${id}`);
  return id;
});

export const getPostById = (state: RootState, id: string) =>
  toPostState(state).posts.find((p) => p.id === id);

export const searchResult = (state: RootState) => {
  const postState = toPostState(state);
  return postState.searchText
    ? postState.posts.filter(
        (p) =>
          p.content.includes(postState.searchText) ||
          p.topic.includes(postState.searchText)
      )
    : postState.posts;
};

export const postsSlice = createSlice({
  name: "posts",
  initialState,
  reducers: {
    setPosts(state, action: PayloadAction<PostType[]>) {
      state.posts = action.payload;
    },
    setSearchText(state, action: PayloadAction<string>) {
      state.searchText = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(savePost.fulfilled, (state, action) => {
        state.posts.unshift(action.payload);
      })
      .addCase(deletePost.fulfilled, (state, action) => {
        state.posts = state.posts.filter((p) => p.id !== action.payload);
      })
      .addCase(editPost.fulfilled, (state, action) => {
        state.posts = state.posts.map((p) =>
          p.id === action.payload.id ? { ...action.payload } : p
        );
      });
  },
});

export const { setPosts, setSearchText } = postsSlice.actions;

export default postsSlice.reducer;
