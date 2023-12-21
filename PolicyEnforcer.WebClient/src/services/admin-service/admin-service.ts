import axios from 'axios';
import { IUser } from '../../models/user';
import authHeader from '../auth-service/auth-header';
import { IBrowserHistory } from '../../models/browserHistory';
import { IHardware } from '../../models/hardware';

const API_URL = 'https://26.85.180.83:6969/api/Admin/';

export const getAllUsers = async () => {
  return await axios.get<IUser[]>(API_URL + 'getusers', { headers: authHeader() });
};

export const getBrowserHistory = async (userId? : string) => {
  return await axios.get<IBrowserHistory[]>(API_URL + 'getbrowserhistory/' + userId, {
    headers: authHeader(),
  });
};

export const getHardware = async (userId? : string) => {
  return await axios.get<IHardware[]>(API_URL + 'gethardwarereadings/' +  userId , {
    headers: authHeader(),
  });
};

export const requestBrowserHistory = async () => {
  return await axios.get(API_URL + 'requestbrowserhistory', {
    headers: authHeader(),
  });
};

export const requestHardwareReadings = async () => {
  return await axios.get(API_URL + 'requesthardwarereadings', {
    headers: authHeader(),
  });
};

