import { ManageAccounts, Logout, Login } from "@mui/icons-material";
import {
  Stack,
  Typography,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Drawer,
  Box,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getCurrentUserId, getUserById, logout } from "../../services/auth-service/auth-service";

import { IUser } from "../../models/user";

const listItems = [
  {
    listIcon: <ManageAccounts htmlColor="rgba(255, 255, 255, 0.56)" />,
    listText: "Админ",
    path: "/admin",
  },
];

const drawerWidth = 250;

const Navbar: React.FC = () => {
  const userId = getCurrentUserId();
  const [user, setUser] = useState<IUser>();
  useEffect(() => {
    const getById = async () => {
      const response = await getUserById(userId);
      setUser(response.data);
    };
    getById()
  }, []);

  console.log(user)

  const navigate = useNavigate();

  const onClickLogOut = () => {
    logout()
    navigate("/login");
  };

  return (
    <Drawer
      sx={{
        width: drawerWidth,
      }}
      variant="permanent"
      anchor="left"
    >
      <Stack
        direction="column"
        sx={{
          width: drawerWidth,
          background:
            "linear-gradient(180deg, rgba(255, 255, 255, 0.05) 0%, rgba(255, 255, 255, 0.05) 100%), #121212",
          height: "100vh",
          color: "white",
        }}
      >
        <Stack direction="row" sx={{ mt: 4 }}>
          <Typography variant="subtitle1" fontWeight="bold" marginLeft={"12px"}>
              Policy Enforcer
          </Typography>
        </Stack>

        <List sx={{ mt: 4 }}>
          {listItems.map((item) => (
            <ListItem
              key={item.listText}
              disablePadding
              sx={{ "&:hover": { bgcolor: "primary.main" } }}
            >
              <ListItemButton onClick={() => navigate(item.path)}>
                <ListItemIcon>{item.listIcon}</ListItemIcon>
                <ListItemText primary={item.listText} />
              </ListItemButton>
            </ListItem>
          ))}
        </List>

        <Box sx={{ marginTop: "auto", mb: 2 }}>
          {user ? (
            <>
              <ListItem sx={{ mb: 2 }}>
                <Typography
                  sx={{ ml: 3}}
                  fontWeight={"bold"}
                >
                  Hi, {user.login}
                </Typography>
              </ListItem>
              <ListItem
                disablePadding
                sx={{ "&:hover": { bgcolor: "primary.main" } }}
              >
                <ListItemButton onClick={onClickLogOut}>
                  <ListItemIcon>
                    <Logout htmlColor="rgba(255, 255, 255, 0.56)" />
                  </ListItemIcon>
                  <ListItemText primary="Выйти" />
                </ListItemButton>
              </ListItem>
            </>
          ) : (
            <ListItem disablePadding>
              <ListItemButton onClick={() => navigate("/login")}>
                <ListItemIcon>
                  <Login htmlColor="rgba(255, 255, 255, 0.56)" />
                </ListItemIcon>
                <ListItemText primary="Войти" />
              </ListItemButton>
            </ListItem>
          )}
          
        </Box>
      </Stack>
    </Drawer>
  );
};

export default Navbar;
