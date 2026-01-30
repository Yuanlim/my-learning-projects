import React from "react";
import { createContext } from "react";
import { LoRContextType } from "../types/Context";
import useLoR from "../hooks/useLoR";

type Props = { children: React.ReactNode };
export const LoRContext = createContext<LoRContextType | undefined>(undefined);

export const LoRDataProvider = ({ children }: Props) => {
  // Load state once share with children component.
  const value = useLoR();

  return <LoRContext.Provider value={value}>{children}</LoRContext.Provider>;
};
