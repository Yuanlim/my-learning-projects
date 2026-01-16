import React from "react";
import useCheckDirectAccessor from "./hooks/useCheckDirectAccessor";

function Nothingness() {
  useCheckDirectAccessor();

  return (
    <main className="main contextCenter">
      <div>
        <p style={{ textAlign: "center" }}>You successfully login.</p>
        <p style={{ textAlign: "center" }}>Click something on nav menu to continue.</p>
      </div>
    </main>
  );
}

export default Nothingness;
