import React, { Dispatch, SetStateAction } from 'react'
import { useAppSelector } from '../hooks/useReduxHook'
import { PiCoinLight, PiHandCoinsLight } from 'react-icons/pi';
import { RiShoppingCartLine } from 'react-icons/ri';
import { LuSquareMenu } from 'react-icons/lu';

type Props = { setShowCart: Dispatch<SetStateAction<boolean>> }

const ShowPoints = ({ setShowCart }: Props) => {
  const { pointsInfo, cartInfo } = useAppSelector((state) => state.shopping);
  return (
    <div className="flex"
      style={{ justifyContent: "space-between", padding: "10px" }}
    >
      <div className='flex withTip contextCenter'>
        <PiHandCoinsLight />
        <p>: {pointsInfo.todaysEarning}</p>
        <span className='tooltip left'>Todays Earning</span>
      </div>
      <div className="flex" style={{ gap: "30px", paddingRight: "5px" }}>
        <div className='flex contextCenter withTip'>
          <PiCoinLight />
          <p>: {pointsInfo.points}</p>
          <span className='tooltip right'>Available Points</span>
        </div>
        <div
          className='contextCenter asButton withTip'
        >
          <LuSquareMenu />
          <span className='tooltip down'>
            Check order
          </span>
        </div>
        <div
          className='contextCenter asButton withTip'
          onClick={() => {
            setShowCart(true);
          }}
        >
          <p
            className="flex contextCenter"
            style={{
              backgroundColor: "red", width: "22px", height: "22px",
              position: "absolute", top: "-5px", right: "-10px",
              borderRadius: "50%", fontSize: "1rem"
            }}
          >
            {cartInfo.cartProductList.length > 9 ? "9+" : cartInfo.cartProductList.length}
          </p>
          <RiShoppingCartLine />
          <span
            className='tooltip down'
            style={{ right: "0%" }}
          >
            Check Cart
          </span>
        </div>
      </div>
    </div>
  )
}

export default ShowPoints