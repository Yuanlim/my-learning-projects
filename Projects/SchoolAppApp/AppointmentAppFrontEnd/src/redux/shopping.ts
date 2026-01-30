import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { ShoppingInitialType, PointsReturn, WishListPayload, WishListReturn, CartItemType } from '../types/Shopping';
import { fetchData } from "../API/FetchAPI";

const initialState: ShoppingInitialType = {
  pointsInfo: { points: 0, todaysEarning: 0 },
  cartInfo: { totalCost: 0, cartProductList: [] }
}

export const PlaceOrder = createAsyncThunk<
  { success: boolean },
  void,
  { rejectValue: { errorMessage: string } }
>("Order/Placed", async (_, thunkAPI) => {
  const URL = "Shopping/Order/Placed";
  const { data, success } = await fetchData<void, void>({
    URL: URL,
    method: "post",
    credentials: true,
  });
  if (success) return { success };
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
  { data: WishListReturn, success: boolean },
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
  if (success) return { data: data as WishListReturn, success: success };
  return thunkAPI.rejectWithValue({ errorMessage: data as string });
})

// Patch Quantity
export const UpdateQuantity = createAsyncThunk<
  { id: number, cartItem: CartItemType, success: boolean },
  WishListPayload,
  { rejectValue: { errorMessage: string } }
>("Cart/Update", async (payload, thunkAPI) => {
  const URL = `Shopping/Cart/Product/Patch`;
  const { data, success } = await fetchData<WishListPayload, CartItemType>(
    {
      URL: URL,
      method: "patch",
      credentials: true,
      payload: payload
    }
  );
  if (success)
    return { id: payload.productId, cartItem: data as CartItemType, success: success };
  return thunkAPI.rejectWithValue({ errorMessage: data as string });
})

// Delete cart item
export const DeleteCartItem = createAsyncThunk<
  { productId: number, success: boolean },
  { productId: number },
  { rejectValue: { errorMessage: string } }
>("routeName", async (payload, thunkAPI) => {
  const URL = `Shopping/Cart/Product/Delete/${payload.productId}`;
  const { data, success } = await fetchData<void, void>(
    {
      URL: URL,
      method: "delete",
      credentials: true,
    }
  );
  if (success) return { productId: payload.productId, success: success };
  return thunkAPI.rejectWithValue({ errorMessage: data as string });
})

export const shoppingSlice = createSlice(
  {
    name: "shopping",
    initialState,
    reducers: {
      setEmptyCart(
        state
      ) {
        state.cartInfo = {
          totalCost: 0,
          cartProductList: []
        }
      }
    },
    extraReducers(builder) {
      builder
        .addCase(LoadInPoints.fulfilled, (state, action) => {
          state.pointsInfo = action.payload;
        })
        .addCase(AddWishList.fulfilled, (state, action) => {
          state.cartInfo = action.payload.data;
        })
        .addCase(LoadInCart.fulfilled, (state, action) => {
          state.cartInfo = action.payload;
        })
        .addCase(DeleteCartItem.fulfilled, (state, action) => {
          // Get target index
          let index = state.cartInfo.cartProductList.findIndex(
            cpl => cpl.productId === action.payload.productId
          );

          // Get total cost * quantity and minus the cost
          state.cartInfo.totalCost -= state.cartInfo.cartProductList[index].pointCost * state.cartInfo.cartProductList[index].quantity

          // Filter out cart item from cart
          state.cartInfo.cartProductList.filter(
            cpl => cpl.productId !== action.payload.productId
          )
        })
        .addCase(UpdateQuantity.fulfilled, (state, action) => {
          // Get target index
          let index = state.cartInfo.cartProductList.findIndex(
            cpl => cpl.productId === action.payload.id
          );

          // Update the cart item state
          state.cartInfo.cartProductList[index] = action.payload.cartItem;

          // Get total cost * quantity and minus the cost
          state.cartInfo.totalCost -= state.cartInfo.cartProductList[index].pointCost * action.payload.cartItem.quantity
        })
    },
  })

export const { setEmptyCart } = shoppingSlice.actions;

export default shoppingSlice.reducer 