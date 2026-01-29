import { useContext } from "react";
import DataContext from "../context/DataContext";

export const useDataContext = () => {
  const dataContext = useContext(DataContext);
  if (!dataContext) {
    throw Error("Data context is undefined");
  }
  return dataContext;
};

export default useDataContext;
