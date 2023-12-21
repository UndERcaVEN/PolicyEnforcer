import { IUser } from "./user";

export interface ILoginCred {
  login: string;
  password: string;
}

export interface IAuthResult {
  token: string;
  userID: string;
}
