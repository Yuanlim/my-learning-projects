import React, { useEffect, useState } from "react";
import { useAppDispatch, useAppSelector } from "./hooks/useReduxHook";
import useCheckDirectAccessor from "./hooks/useCheckDirectAccessor";
import ShowProduct from "./component/ShowProduct";
import { LoadIn } from "./redux/product";
import ShowPoints from "./component/ShowPoints";
import { setPrompText } from "./redux/generic";
import { LoadInCart, LoadInPoints } from "./redux/shopping";
import ShowCart from "./component/ShowCart";
import "../src/shopping.css";

function Shopping() {
  useCheckDirectAccessor();
  const { products } = useAppSelector((state) => state.product);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [showCart, setShowCart] = useState<boolean>(false);
  const dispatch = useAppDispatch();


  // Load shopping list
  useEffect(() => {

    const Initiallize = async () => {
      try {
        setIsLoading(true);
        dispatch(LoadIn({ RequestExpandTimes: 0 })); // defualt
        dispatch(LoadInPoints()); // Load teacher points
        dispatch(LoadInCart());
      } catch (error) {
        const errorMsg = error as { errorMessage: string }
        dispatch(setPrompText(errorMsg.errorMessage))
      }
    };

    Initiallize();

    setTimeout(() => {
      setIsLoading(false)
    }, 1000);

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])


  return (
    <main
      className='main card'
      style={{ width: "100%", height: "80vh", flexDirection: "column" }}
    >
      <ShowPoints setShowCart={setShowCart} />
      {!isLoading &&
        <div
          style={{
            display: "grid",
            gridAutoRows: "auto",
            gridAutoColumns: "max-content",
            gridTemplateColumns: "repeat(auto-fill, minmax(300px, 1fr))",
            gap: "20px",
            width: "100%",
            padding: "20px",
            justifyItems: "center",
            overflowY: "auto"
          }}
        >
          {products.map(p =>
            <ShowProduct p={p} key={p.productId} />
          )}
        </div>
      }
      {isLoading && <p>Loading...</p>}
      {showCart && <ShowCart setShowCart={setShowCart} />}
    </main>
  );
}

export default Shopping;
