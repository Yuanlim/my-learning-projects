import React, { Dispatch, SetStateAction, useEffect, useState } from "react";
import { createContext } from "react";
import { useAppDispatch, useAppSelector } from "../hooks/useReduxHook";
import { getRelationList, setLoading } from "../redux/relation";


interface BlockContextType {
  handleReFetchBlock: () => void;
  setBlockFlags: Dispatch<SetStateAction<number>>;
  isLoading: boolean;
}

type Props = { children: React.ReactNode };
export const BlockContext = createContext<BlockContextType | undefined>(undefined);

export const BlockDataProvider = ({ children }: Props) => {
  const [blockFlags, setBlockFlags] = useState<number>(0);
  const dispatch = useAppDispatch();
  const isLoading = useAppSelector((state) => state.relation["Loading"])

  const handleReFetchBlock = () => {
    dispatch(setLoading(true));
    dispatch(getRelationList({ AskFor: "Pending" }));
    dispatch(setLoading(false));
  }

  useEffect(() => {
    handleReFetchBlock();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [blockFlags, setBlockFlags])

  return (
    <BlockContext.Provider value={{ handleReFetchBlock, setBlockFlags, isLoading }}>
      {children}
    </BlockContext.Provider>
  );
};
