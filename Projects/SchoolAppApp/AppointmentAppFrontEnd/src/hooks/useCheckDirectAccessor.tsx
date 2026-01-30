import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAppDispatch } from "./useReduxHook";
import { checkAuthorized } from "../redux/login";

// check directly access to route user: not-valid cookies, not-valid login => redirect to login pages
function useCheckDirectAccessor() {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  useEffect(() => {
    let cancelled = false;

    (async () => {
      try {
        await dispatch(checkAuthorized()).unwrap();
      } catch {
        if (!cancelled) navigate("/Login");
      }
    })();

    return () => { cancelled = true; };
  }, [dispatch, navigate]);
}

export default useCheckDirectAccessor;
