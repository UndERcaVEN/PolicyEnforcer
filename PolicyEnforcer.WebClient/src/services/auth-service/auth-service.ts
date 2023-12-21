import axios from "axios";
import { IAuthResult } from "../../models/authCred";
import { IUser } from "../../models/user";
import authHeader from "./auth-header";

const API_URL = 'https://26.85.180.83:6969/api/Users/';

export const login = async (login: string, password: string) => {
  const response = await axios.post(API_URL + "login", {
    login,
    password,
  });
  if (response.data.token) {
    localStorage.setItem("user", JSON.stringify(response.data));
  }
  return response.data;
};

export const logout = () => {
  localStorage.removeItem("user");
};

export const getUserById = async (userId?: string) => {
  return await axios.get<IUser>(API_URL + userId, { headers: authHeader() });
};

export const getCurrentUserId = () => {
  const userStr = localStorage.getItem("user");
  if (userStr) {
    const authResult: IAuthResult = JSON.parse(userStr);
    return authResult.userID;
  }
  return undefined;
};


export const getToken = () => {
  const userStr = localStorage.getItem("user");
  if (userStr) {
    const authResult: IAuthResult = JSON.parse(userStr);
    return authResult.token;
  }
  return "";
};
