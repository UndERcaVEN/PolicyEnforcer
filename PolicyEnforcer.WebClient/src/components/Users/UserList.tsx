import { Box, Button } from '@mui/material';
import { IUser } from '../../models/user';
import { DataGrid, GridColDef, GridRowParams } from '@mui/x-data-grid';
import { useState, useEffect } from 'react';
import { getAllUsers, requestBrowserHistory, requestHardwareReadings } from '../../services/admin-service/admin-service';
import { redirect, useNavigate } from 'react-router-dom';

const GameList: React.FC = () => {
  const navigate = useNavigate();

  const columns: GridColDef[] = [
    { field: 'userId', headerName: 'User ID', width: 350 },
    { field: 'login', headerName: 'Name', width: 350 },
    { field: 'accessLevel', headerName: 'Access', width: 70 },
    {
      field: 'Hardware',
      headerName: 'Hardware',
      width: 100,
      sortable: false,
      renderCell: (cellValues) => (
        <Button
          sx={{ color: 'blue' }}
          onClick={() => navigate('/hardware/' + cellValues.row.userId)}>
          Hardware
        </Button>
      ),
    },
    {
      field: 'Browser History',
      headerName: 'Browser History',
      width: 200,
      sortable: false,
      renderCell: (cellValues) => (
        <Button
          sx={{ color: 'blue' }}
          onClick={() => navigate('/browser-history/' + cellValues.row.userId)}>
          History
        </Button>
      ),
    },
  ];
  
  const [users, setUsers] = useState<IUser[]>([]);

  const requestHitory = () => {
    requestBrowserHistory();
  };

  const requestHardware = () => {
    requestHardwareReadings();
  };

  useEffect(() => {
    const getUsers = async () => {
      const response = await getAllUsers();
      setUsers(response.data);
    };
    getUsers();
  }, []);

  return (
    <Box>
    <Box sx={{ height: 550, width: '100%' }}>
      <DataGrid
        getRowId={(row) => row.userId}
        rows={users}
        columns={columns}
        initialState={{
          pagination: {
            paginationModel: {
              pageSize: 8,
            },
          },
        }}
        pageSizeOptions={[8]}
        disableRowSelectionOnClick
      />
    </Box>
      <Button variant="contained" onClick={() => requestHitory()}>History</Button>
      <Button variant="contained" onClick={() => requestHardware()}>Hardware</Button>
    </Box>
    
  );
};

export default GameList;

