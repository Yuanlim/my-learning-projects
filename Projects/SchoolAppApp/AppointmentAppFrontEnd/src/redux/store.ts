import { configureStore } from "@reduxjs/toolkit";
import { loginSlice } from "./login";
import { genericSlice } from "./generic";
import { relationSlice } from "./relation";
import { productSlice } from "./product";
import { shoppingSlice } from "./shopping";

export const store = configureStore({
  reducer: {
    login: loginSlice.reducer,
    relation: relationSlice.reducer,
    generic: genericSlice.reducer,
    product: productSlice.reducer,
    shopping: shoppingSlice.reducer
  },
});

// Get the type of our store variable
export type AppStore = typeof store;
// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<AppStore["getState"]>;
// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = AppStore["dispatch"];
