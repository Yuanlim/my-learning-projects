import React, { useEffect, useState } from 'react'
import { checkAuthorized, requestLogin, RoleType, setAuth, UserDataType } from '../redux/login';
import { LoR, registerPayload } from '../types/LoR';
import { useAppDispatch, useAppSelector } from './useReduxHook';
import { useNavigate } from 'react-router-dom';
import { setPrompText } from '../redux/generic';
import { fetchData } from '../API/FetchAPI';


function useLoR() {
  // Login or Register. Login as true
  const [LoR, setLoR] = useState<LoR>("login");
  const [id, setId] = useState<string>("");
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [role, setRole] = useState<RoleType>(null);
  const [name, setName] = useState<string>("");
  const [phoneNumber, setPhoneNumber] = useState<string>("");
  const [schoolClass, setSchoolClass] = useState<string>("電通一甲");
  const loginState: UserDataType = useAppSelector((state) => state.login);
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  useEffect(() => {
    console.log(loginState);
    if (loginState.authorized) {
      navigate("/");
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [loginState.authorized])

  const userAndInputReset = () => {
    dispatch(setAuth(false));
    setId("");
    setEmail("");
    setPassword("");
    setRole(null);
    setName("");
    setPhoneNumber("");
    setSchoolClass("電通一甲");
  };

  const handleRoleButtonStyle = (buttonRole: RoleType): React.CSSProperties | undefined => {
    // Style defination
    const highLight = {
      border: "5px solid red",
    } as React.CSSProperties;
    const disappear = {
      display: "none",
    } as React.CSSProperties;

    // Login => All button appear, when button is targeted border to red to hightlight
    if (LoR === "login") {
      return buttonRole === role ? highLight : undefined;
    }

    // Register => Other than "student" "teacher" button should disappear.
    if (buttonRole !== "student" && buttonRole !== "teacher") return disappear;
    return buttonRole === role ? highLight : undefined;
  };

  const handleLoRButtonStyle = (callAs: LoR): React.CSSProperties => {
    // Style defination
    const selected = {
      color: "#333",
      backgroundColor: "whitesmoke",
      width: "50%"
    } as React.CSSProperties;
    const notSelected = {
      color: "whitesmoke",
      backgroundColor: "#333",
      width: "50%"
    } as React.CSSProperties;

    return callAs === LoR ? selected : notSelected
  };

  const handleLogin = async () => {
    // Safety reset auth
    dispatch(setAuth(false));

    try {
      // Request login
      const result = await dispatch(requestLogin({ id: id, role: role, password: password })).unwrap();

      // Auth Guard
      if (!result.success) { return; }
      userAndInputReset();
      dispatch(checkAuthorized());

    } catch (error) {
      const err = error as { errorMessage: string }
      dispatch(setPrompText(err.errorMessage));
      userAndInputReset();
    }

  };

  const handleRegister = async () => {
    // Safety reset auth
    dispatch(setAuth(false));

    // Request new registeration
    const registerApiUrl = "register";
    const { success } = await fetchData<registerPayload>(
      {
        URL: registerApiUrl, method: "post", credentials: false, payload: {
          role: role,
          id: id,
          password: password,
          email: email,
          name: name,
          phoneNumber: phoneNumber,
          Class: schoolClass
        }
      }
    );

    // Result success return to login page and reset input
    if (success) {
      userAndInputReset();
      setLoR("login");
    };
  };

  return {
    LoR, setLoR,
    id, setId,
    email, setEmail,
    password, setPassword,
    role, setRole,
    name, setName,
    phoneNumber, setPhoneNumber,
    schoolClass, setSchoolClass,
    handleRoleButtonStyle,
    handleLoRButtonStyle,
    handleLogin,
    handleRegister
  }
}

export default useLoR
