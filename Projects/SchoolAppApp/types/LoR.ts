import { RoleType } from "../redux/login"

export type LoR = "login" | "register"

export type loginPayload = {
  id:string,
  role:RoleType,
  password:string
}

export type registerPayload = {
  role: RoleType,
  id: string,
  password: string,
  email: string,
  name: string,
  phoneNumber: string,
  Class: string
}