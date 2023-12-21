import { Outlet } from "react-router-dom";
import Navbar from "./Navbar";
import { Box } from "@mui/material";
import { FC } from "react";

const MainLayout: FC = () => {
  return (
    <Box sx={{ display: "flex", bgcolor: '#FAFAFA'}}>
      <Navbar />

      <Box
        component="main"
        sx={{
          ml: 12,
          mt: 8,
          height: '100vh', 
        }}
      >
        <Outlet />
      </Box>
    </Box>
  );
};

export default MainLayout;
