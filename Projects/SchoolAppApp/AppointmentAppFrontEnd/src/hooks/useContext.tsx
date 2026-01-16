import { useContext } from "react";
import { LoRContext } from "../context/LoRContext";
import { PendingContext } from "../context/PendingContext";
import { AcceptedContext } from "../context/AcceptedContext";
import { BlockContext } from "../context/BlockContext";

export const useLoRContext = () => {
  const dataContext = useContext(LoRContext);
  if (!dataContext) {
    throw Error("Data context is undefined");
  }
  return dataContext;
};

export const usePendingContext = () => {
  const dataContext = useContext(PendingContext);
  if (!dataContext) {
    throw Error("Data context is undefined");
  }
  return dataContext;
};

export const useAcceptedContext = () => {
  const dataContext = useContext(AcceptedContext);
  if (!dataContext) {
    throw Error("Data context is undefined");
  }
  return dataContext;
};

export const useBlockContext = () => {
  const dataContext = useContext(BlockContext);
  if (!dataContext) {
    throw Error("Data context is undefined");
  }
  return dataContext;
};

export default useLoRContext;