import React from "react";
import { useNavigate } from "react-router-dom";
import { ShoppingReturn } from "../types/Product";

function ShowProduct({ p }: { p: ShoppingReturn }) {

  const navigate = useNavigate();

  return (
    <button
      className="flex"
      style={{
        flexFlow: "column",
        color: "white",
        backgroundColor: "#555",
        justifyContent: "center",
        alignItems: "center",
        width: "300px",
        height: "300px",
        gap: "20px"
      }}
      key={p.productId}
      onClick={() => {
        navigate(`/ProductPage/${p.productId}`)
      }}
    >
      <p>{p.productName}</p>
      <img
        style={{ display: "block", width: "80px", height: "80px" }}
        src={`http://localhost:5095/productImg/${p.productImageRoot}`}
        alt="ShoppingItem"
      />
      <p>Available Qty: {p.availableQuantity}</p>
      <p>{p.description}</p>
      <p>Price: {p.pointCost}</p>
    </button>
  );
}

export default ShowProduct;