import { createTheme } from "@mui/material";

export const theme = createTheme({
  palette: {
    primary: {
      main: "#fff",
    },
    secondary: {
      main: "#4F4891",
    },
    error: {
      main: "#CB0113",
    },
  },
  typography: {
    fontFamily: [
      "'Nunito'",
      'sans-serif'
    ].join(','),
  },
});
