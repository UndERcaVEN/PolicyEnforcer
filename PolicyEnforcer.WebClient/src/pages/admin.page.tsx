import { Stack } from '@mui/material';
import React, { useState } from 'react';
import UserList from '../components/Users/UserList';
import { IUser } from '../models/user';

const AdminPage: React.FC = () => {
  return (
    <Stack component="div" alignItems="center" justifyContent="center" height="100vh">
      <UserList />
      
    </Stack>
  );
}

export default AdminPage;