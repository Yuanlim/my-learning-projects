import React, { Dispatch, SetStateAction, useEffect, useState } from "react";
import { createContext } from "react";
import { useAppDispatch, useAppSelector } from "../hooks/useReduxHook";
import { getRelationList, setLoading } from "../redux/relation";


interface PendingContextType {
  handleReFetchPending: () => void;
  setPendingFlags: Dispatch<SetStateAction<number>>;
  isLoading: boolean;
}

type Props = { children: React.ReactNode };
export const PendingContext = createContext<PendingContextType | undefined>(undefined);

export const PendingDataProvider = ({ children }: Props) => {
  const [PendingFlags, setPendingFlags] = useState<number>(0);
  const dispatch = useAppDispatch();
  const isLoading = useAppSelector((state) => state.relation["Loading"]);

  const handleReFetchPending = () => {
    dispatch(setLoading(true));
    dispatch(getRelationList({ AskFor: "Pending" }));
    dispatch(setLoading(false));
  }

  useEffect(() => {
    handleReFetchPending();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [PendingFlags, setPendingFlags])

  return (
    <PendingContext.Provider value={{ handleReFetchPending, setPendingFlags, isLoading }}>
      {children}
    </PendingContext.Provider>
  );
};
