import React, { useEffect, useState } from "react";
import { CiViewTimeline } from "react-icons/ci";
import { useAppDispatch, useAppSelector } from "./hooks/useReduxHook";
import { UserDataType } from "./redux/login";
import StudentNav from "./component/StudentNav";
import TeacherNav from "./component/TeacherNav";
import { setPrompText } from "./redux/generic";

function Header() {
  const loginState: UserDataType = useAppSelector((state) => state.login);

  const promptText = useAppSelector((state) => state.generic.prompText);
  const [showPromptText, setShowPromptText] = useState<boolean>(false);
  const dispatch = useAppDispatch();

  useEffect(() => {
    // Change text state but not a prompt => earlier return 
    if (!promptText) {
      setShowPromptText(false);
      return;
    }

    // On prompt
    setShowPromptText(true);

    // Delay 4sec and off
    const showPrompt = setTimeout(() => {
      setShowPromptText(false);
      dispatch(setPrompText(""));
    }, 4000);

    //clean up
    return () => clearTimeout(showPrompt);
  }, [dispatch, promptText])

  return (
    <header className="header">
      <div className="web__title__container">
        <CiViewTimeline className="header__icon" />
        <p>Sch.App.App</p>
      </div>

      <div>{showPromptText && <p style={{ backgroundColor: "red" }}>{promptText}</p>}</div>

      {loginState.authorized &&
        (
          loginState.role === "student" ? <StudentNav /> :
            loginState.role === "teacher" ? <TeacherNav /> : <></>
        )
      }

    </header>
  );
}

export default Header;
