import { Stack } from "@mui/material";
import LoginForm from "../components/Auth/LoginForm";

const LoginPage: React.FC = () => {
  return (
    <Stack
      component="div"
      alignItems="center"
      justifyContent="center"
      height="100vh"
    >
      <LoginForm />
    </Stack>
  );
};

export default LoginPage;
