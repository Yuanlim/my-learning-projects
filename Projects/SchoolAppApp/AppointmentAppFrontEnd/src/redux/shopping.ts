import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { ShoppingInitialType, PointsReturn, WishListPayload, WishListReturn } from '../types/Shopping';
import { fetchData } from "../API/FetchAPI";

const initialState: ShoppingInitialType = {
  pointsInfo: {points: 0, todaysEarning: 0},
  cartInfo: {totalCost: 0, cartProductList: []}
}

export const PlaceOrder = createAsyncThunk<
  void,
  void,
  { rejectValue: { errorMessage: string } }
>("Order/Placed", async (_, thunkAPI) => {
  const URL = "Shopping/Order/Placed";
  const { data, success } = await fetchData<void, void>({
    URL: URL,
    method: "get",
    credentials: true,
  });
  if (success) return;
  return thunkAPI.rejectWithValue({ errorMessage: data as string });
});


// Load User Points
export const LoadInPoints = createAsyncThunk<
  PointsReturn,
  void,
  { rejectValue: { errorMessage: string } }
>("Points/Get", async (_, thunkAPI) => {
  const URL = "Shopping/Points/Get";
  const { data, success } = await fetchData<void, PointsReturn>({
    URL: URL,
    method: "get",
    credentials: true,
  });
  if (success) return data as PointsReturn;
  return thunkAPI.rejectWithValue({ errorMessage: data as string });
});

// Load in initial cart 
export const LoadInCart = createAsyncThunk<
  WishListReturn,
  void,
  { rejectValue: { errorMessage: string } }
>("Cart/Get", async (_, thunkAPI) => {
  const URL = "Shopping/Cart/Get";
  const { data, success } = await fetchData<void, WishListReturn>({
    URL: URL,
    method: "get",
    credentials: true,
  });
  if (success) return data as WishListReturn;
  return thunkAPI.rejectWithValue({ errorMessage: data as string });
});

// Put something in cart.
export const AddWishList = createAsyncThunk<
  WishListReturn,
  WishListPayload,
  { rejectValue: { errorMessage: string } }
>("WishList/Get", async (payload, thunkAPI) => {
  const URL = `Shopping/WishList`;
  const { data, success } = await fetchData<WishListPayload, WishListReturn>(
    {
      URL: URL,
      method: "post",
      credentials: true,
      payload: { productId: payload.productId, quantity: payload.quantity }
    }
  );
  if (success) return data as WishListReturn;
  return thunkAPI.rejectWithValue({ errorMessage: data as string });
})

export const shoppingSlice = createSlice(
{
  name: "shopping",
  initialState,
  reducers: {},
  extraReducers(builder) {
    builder
    .addCase(LoadInPoints.fulfilled, (state, action) => {
      state.pointsInfo = action.payload;
    })
    .addCase(AddWishList.fulfilled, (state, action) => {
      state.cartInfo = action.payload;
    })
    .addCase(LoadInCart.fulfilled, (state, action) => {
      console.log(action.payload);
      state.cartInfo = action.payload;
    });
  },
})

export default shoppingSlice.reducer 