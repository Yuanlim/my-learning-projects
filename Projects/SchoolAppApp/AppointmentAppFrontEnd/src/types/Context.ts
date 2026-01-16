import { Dispatch, SetStateAction } from "react";
import { RoleType } from "../redux/login";
import { LoR } from "./LoR";

export type LoRContextType = {
  LoR: LoR;
  setLoR: React.Dispatch<React.SetStateAction<LoR>>;
  role: RoleType;
  setRole: Dispatch<SetStateAction<RoleType>>;
  id: string;
  setId: Dispatch<SetStateAction<string>>;
  email: string;
  setEmail: Dispatch<SetStateAction<string>>;
  name: string;
  setName: Dispatch<SetStateAction<string>>;
  phoneNumber: string;
  setPhoneNumber: Dispatch<SetStateAction<string>>;
  schoolClass: string;
  setSchoolClass: Dispatch<SetStateAction<string>>;
  password: string;
  setPassword: Dispatch<SetStateAction<string>>;
  handleRoleButtonStyle: (role: RoleType) => React.CSSProperties | undefined;
  handleLoRButtonStyle: (callAs: LoR) => React.CSSProperties;
  handleLogin: () => void;
  handleRegister: () => void;
};
