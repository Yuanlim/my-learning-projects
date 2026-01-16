export type ShoppingInitialType = {
  pointsInfo: PointsReturn
  cartInfo: WishListReturn
}

export type PointsReturn = {
  points: number;
  todaysEarning: number;
};

export type WishListPayload = {
  productId: number;
  quantity: number;
}

export type WishListReturn = {
  totalCost: number;
  cartProductList: CartItemType[];
}

export type CartItemType = {
  productName: string;
  productImageRoot: string;
  quantity: number;
  pointCost: number;
}