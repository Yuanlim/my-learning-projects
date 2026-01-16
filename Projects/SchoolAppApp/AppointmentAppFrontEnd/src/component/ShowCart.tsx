import React, { Dispatch, SetStateAction } from 'react'
import { RxCross2 } from "react-icons/rx";
import { useAppDispatch, useAppSelector } from '../hooks/useReduxHook';
import { PlaceOrder } from '../redux/shopping';

type Props = { setShowCart: Dispatch<SetStateAction<boolean>> }

const ShowCart = ({ setShowCart }: Props) => {
  const { cartInfo } = useAppSelector((state) => state.shopping);
  const dispatch = useAppDispatch();

  return (
    <div className='showCartBackground'>
      <div className='showCartContainer'>
        <div className='showCartTopBar'>
          <RxCross2
            className='showCartExitButton asButton'
            onClick={() => setShowCart(false)}
          />
        </div>
        <div className='showProductContainer'>
          {cartInfo.cartProductList.map(cpl =>
            <div className='flex' key={cpl.productImageRoot}>
              <div className='showCartImgBg'>
                <img
                  className='showCartImg'
                  src={`http://localhost:5095/productImg/${cpl.productImageRoot}`}
                  alt="ProductIcon"
                />
              </div>
              <div className='productInfoContainer'>
                <h3>{cpl.productName}</h3>
                <h4>Quantity: {cpl.quantity}</h4>
                <h4>Cost:{cpl.pointCost}</h4>
                <h4>Total Price: {cpl.quantity * cpl.pointCost}</h4>
              </div>
            </div>
          )}
        </div>
        <div className='flex placeOrderContainer contextCenter'>
          <h4 className='totalCostLabel'>Total Cost: {cartInfo.totalCost}</h4>
          <button
            className='placeOrderButton'
            onClick={() => {
              dispatch(PlaceOrder());
            }}
          >
            Place Order
          </button>
        </div>
      </div>
    </div>
  )
}

export default ShowCart