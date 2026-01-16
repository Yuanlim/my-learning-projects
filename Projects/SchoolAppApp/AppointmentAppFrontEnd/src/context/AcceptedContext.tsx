import React, { Dispatch, SetStateAction, useEffect, useState } from "react";
import { createContext } from "react";
import { useAppDispatch, useAppSelector } from "../hooks/useReduxHook";
import { getRelationList, setLoading } from "../redux/relation";


interface AcceptedContextType {
  handleReFetchAccepted: () => void;
  setAcceptedFlags: Dispatch<SetStateAction<number>>;
  isLoading: boolean;
}

type Props = { children: React.ReactNode };
export const AcceptedContext = createContext<AcceptedContextType | undefined>(undefined);

export const AcceptedDataProvider = ({ children }: Props) => {
  const [AcceptedFlags, setAcceptedFlags] = useState<number>(0);
  const dispatch = useAppDispatch();
  const isLoading = useAppSelector((state) => state.relation["Loading"]);

  const handleReFetchAccepted = () => {
    dispatch(setLoading(true));
    dispatch(getRelationList({ AskFor: "Accepted" }));
    dispatch(setLoading(false));
  }

  useEffect(() => {
    handleReFetchAccepted();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [AcceptedFlags, setAcceptedFlags])

  return (
    <AcceptedContext.Provider value={{ handleReFetchAccepted, setAcceptedFlags, isLoading }}>
      {children}
    </AcceptedContext.Provider>
  );
};
