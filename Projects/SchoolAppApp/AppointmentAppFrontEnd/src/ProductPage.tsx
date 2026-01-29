import React, { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import ShowPoints from './component/ShowPoints';
import useCheckDirectAccessor from './hooks/useCheckDirectAccessor';
import { LoadInTarget } from './redux/product';
import { useAppDispatch, useAppSelector } from './hooks/useReduxHook';
import { FiMinusCircle, FiPlusCircle } from 'react-icons/fi';
import { BsCartPlus } from 'react-icons/bs';
import { setPrompText } from './redux/generic';
import { AddWishList, LoadInCart, LoadInPoints } from './redux/shopping';
import ShowCart from './component/ShowCart';

function ProductPage() {
  useCheckDirectAccessor();
  const { id } = useParams();
  const { targetProducts } = useAppSelector((state) => state.product);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [buyingAmount, setBuyingAmount] = useState<number>(0);
  const [showCart, setShowCart] = useState<boolean>(false);
  const dispatch = useAppDispatch();

  const AddToCartHandler = async () => {
    if (buyingAmount === 0) return dispatch(setPrompText("No Quantity was added"));

    try {
      dispatch(AddWishList({ productId: Number(id), quantity: buyingAmount }))
    } catch (error) {
      if (error as { errorMessage: string } !== undefined)
        dispatch(setPrompText((error as { errorMessage: string }).errorMessage))
      else if (error instanceof Error)
        dispatch(setPrompText(error.message));
      else
        dispatch(setPrompText("Unexpected Error"));
    }
  }

  // Initial
  useEffect(() => {
    const Initiallize = async () => {
      setIsLoading(true);
      dispatch(LoadInTarget(Number(id)));
      dispatch(LoadInPoints());
      dispatch(LoadInCart());
    }

    Initiallize();

    setTimeout(() => {
      setIsLoading(false)
    }, 1000);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])


  return (
    <main
      className='main card'
      style={{ width: "100%", flexDirection: 'column', height: "80vh" }}
    >
      {!isLoading && targetProducts &&
        <>
          <ShowPoints setShowCart={setShowCart} />
          <div className='flex productPageTopContainer'>
            <img
              className='productPageImg'
              src={`http://localhost:5095/productImg/${targetProducts?.productImageRoot}`}
              alt="Product"
            />
            <div className='productInfoContainer'>
              <h1>{targetProducts.productName}</h1>
              <h2>Price: {targetProducts?.pointCost}</h2>
              <h2>Available: {targetProducts?.availableQuantity}</h2>
            </div>
          </div>
          <div style={{ flexGrow: 1, overflow: "auto" }}>
            <span style={{ fontWeight: "bolder", fontStyle: "oblique" }}>Description:</span> {targetProducts?.description} damdal jajdsaoij doajd ajdqh duoqhd9uqh dh hdaoshdosa hdoahod ahodha odhaoa dhahd hdhqdw qhqhfe fajfo haifh damdal jajdsaoij doajd ajdqh duoqhd9uqh dh hdaoshdosa hdoahod ahodha odhaoa dhahd hdhqdw qhqhfe fajfo haifh damdal jajdsaoij doajd ajdqh duoqhd9uqh dh hdaoshdosa hdoahod ahodha odhaoa dhahd hdhqdw qhqhfe fajfo haifh damdal jajdsaoij doajd ajdqh duoqhd9uqh dh hdaoshdosa hdoahod ahodha odhaoa dhahd hdhqdw qhqhfe fajfo haifh damdal jajdsaoij doajd ajdqh duoqhd9uqh dh hdaoshdosa hdoahod ahodha odhaoa dhahd hdhqdw qhqhfe fajfo haifh damdal jajdsaoij doajd ajdqh duoqhd9uqh dh hdaoshdosa hdoahod ahodha odhaoa dhahd hdhqdw qhqhfe fajfo haifh
          </div>
          <div className="flex productPageBottomContainer contextCenter">
            <div
              className='flex contextCenter productPageButton'
            >
              <FiMinusCircle
                role="button"
                className='asButton'
                onClick={() => {
                  if (buyingAmount === 0) return;
                  setBuyingAmount(buyingAmount - 1)
                }}
              />
              <p>{buyingAmount}</p>
              <FiPlusCircle
                role="button"
                className='asButton'
                onClick={() => {
                  if (buyingAmount === targetProducts.availableQuantity) return;
                  setBuyingAmount(buyingAmount + 1)
                }}
              />
              <button
                className='withTip addCartButton'
                type="button"
                onClick={async () => {
                  await AddToCartHandler()
                }}
              >
                <BsCartPlus />
                <span
                  className='tooltip up'
                  style={{ right: "0%" }}
                >
                  Add To Cart
                </span>
              </button>
            </div>
            <div>
              <p>Total amount: {targetProducts.pointCost * buyingAmount} points</p>
            </div>
          </div>
        </>}
      {isLoading && <p>Loading...</p>}
      {!isLoading && !targetProducts && <p>404 Not Found: Product is deleted or doesn't existed.</p>}
      {showCart && <ShowCart setShowCart={setShowCart} />}
    </main>
  )
}

export default ProductPage

