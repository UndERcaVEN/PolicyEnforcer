import { Box, Button, Stack } from '@mui/material';
import { IUser } from '../../models/user';
import { DataGrid, GridColDef, GridRowParams } from '@mui/x-data-grid';
import { useState, useEffect } from 'react';
import { getAllUsers, getBrowserHistory } from '../../services/admin-service/admin-service';
import { redirect, useNavigate, useParams } from 'react-router-dom';
import { IBrowserHistory } from '../../models/browserHistory';

const columns: GridColDef[] = [
  { field: 'url', headerName: 'Url', width: 500 },
  { field: 'dateVisited', headerName: 'Date', width: 300 },
  { field: 'browserName', headerName: 'Browser Name', width: 200 },
];

const BrowserHistory: React.FC = (ur) => {
  const params = useParams();

  const [history, setHistory] = useState<IBrowserHistory[]>([]);

  useEffect(() => {
    const getHistory = async () => {
      const response = await getBrowserHistory(params.userId);
      setHistory(response.data);
    };
    getHistory();
    console.log(history);
  }, []);

  return (
    <Stack width="100%" alignItems="center" justifyContent="center" height="100vh">
      <Box sx={{ height: 550, width: '100%' }}>
        <DataGrid
          getRowId={(row) => row.url}
          rows={history}
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
    </Stack>
  );
};

export default BrowserHistory;
