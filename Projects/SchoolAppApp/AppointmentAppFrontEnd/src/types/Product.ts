export type ProductInitialType = {
  products: ShoppingReturn[];
  targetProducts?: ShoppingReturn;
};

export type ShoppingReturn = {
  productId: number;
  productImageRoot: string;
  productName: string;
  description: string;
  pointCost: number;
  availableQuantity: number;
};

export type LoadProductsPayload = {
  SearchString?: string;
  RequestExpandTimes: number;
};



