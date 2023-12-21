import { Box, Button, Stack } from '@mui/material';
import { IUser } from '../../models/user';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import { useState, useEffect } from 'react';
import { getHardware, requestBrowserHistory } from '../../services/admin-service/admin-service';
import { useParams } from 'react-router-dom';
import { IHardware } from '../../models/hardware';

const columns: GridColDef[] = [
  { field: 'instanceName', headerName: 'Name', width: 350 },
  { field: 'temperature', headerName: 'Temperature', width: 150 },
  { field: 'load', headerName: 'Load', width: 200 },
  { field: 'dateMeasured', headerName: 'Date', width: 200 },
];

const Hardware: React.FC = () => {
  const [hardware, setHardware] = useState<IHardware[]>([]);
  const params = useParams();

  console.log(params.userId);


  useEffect(() => {
    const getHardwareInfo = async () => {
      const response = await getHardware(params.userId);
      console.log(response);
      setHardware(response.data);
    };
    getHardwareInfo();
    console.log(hardware);
  }, []);

  return (
    <Stack alignItems="center" justifyContent="center" height="100vh" width="100%">
      <Box sx={{ height: 550, width: '100%' }}>
        <DataGrid
          getRowId={(row) => row.instanceName}
          rows={hardware}
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

export default Hardware;
