import React from "react";
import useLoRContext from "../hooks/useContext";

function LoginInput() {
  const { id, setId, password, setPassword } = useLoRContext();

  return (
    <>
      <div className="loginInput_container">
        <label htmlFor="Id">Id</label>
        <input
          className="LoR__input"
          type="text"
          name="Id"
          id="Id"
          value={id}
          required
          placeholder="Ex: F123456"
          onChange={(e) => {
            setId(e.target.value);
          }}
        />
      </div>
      <div className="loginInput_container">
        <label htmlFor="Password">Password</label>
        <input
          className="LoR__input"
          type="password"
          name="Password"
          id="Password"
          value={password}
          required
          onChange={(e) => {
            setPassword(e.target.value);
          }}
        />
      </div>
    </>
  );
}

export default LoginInput;
