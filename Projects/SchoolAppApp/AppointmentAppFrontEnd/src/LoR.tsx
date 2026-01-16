import LoginInput from "./component/LoginInput";
import RegisterInput from "./component/RegisterInput";
import RoleChangeButton from './component/RoleChangeButton';
import useLoRContext from "./hooks/useContext";


export default function Login() {

  const { LoR, setLoR, handleLoRButtonStyle, handleLogin, handleRegister } = useLoRContext();

  return (
    <main className="main card contextCenter">
      <form onSubmit={(e) => e.preventDefault()} className="login__form">
        <div className="login__container">
          <div className="flex contextCenter" style={{ padding: "10px" }}>
            <button type="button"
              style={handleLoRButtonStyle("login")}
              onClick={() => setLoR("login")}
            >
              Login
            </button>
            <button type="button"
              style={handleLoRButtonStyle("register")}
              onClick={() => setLoR("register")}
            >
              Register
            </button>
          </div>
          <div
            className="LoR__input__container"
            style={LoR === "login" ? {} : {
              display: "grid",
              gridTemplateColumns: "repeat(2, 1fr)",
              gap: "5px",
            }}
          >
            <LoginInput />
            {LoR === "register" && (<RegisterInput />)}
          </div>
          <button
            type="submit" className="login__button"
            onClick={() => LoR === "login" ? handleLogin() : handleRegister()}
          >
            {LoR === "login" ? "Login" : "Register"}
          </button>
          {/* <h4>-Your Role-</h4> */}
          {/* <p className="role_des">
            {role === "schoolPrinciple" ? "School Principle" : role}
            {!role && ""}
          </p> */}
          <div className="role__change__container">
            <RoleChangeButton />
          </div>
        </div>
      </form>
    </main>
  );
}
