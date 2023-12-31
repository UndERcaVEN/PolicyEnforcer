import { Box, Button, InputLabel, TextField, Typography } from '@mui/material';
import { Controller, SubmitHandler, useForm, useFormState } from 'react-hook-form';
import { ILoginCred } from '../../models/authCred';
import { Link, useNavigate } from 'react-router-dom';
import { login } from '../../services/auth-service/auth-service';
import { AlignHorizontalCenter } from '@mui/icons-material';

const LoginForm: React.FC = () => {
  const navigate = useNavigate();

  const { control, handleSubmit } = useForm<ILoginCred>({
    mode: 'onChange',
  });

  const { errors } = useFormState({
    control,
  });

  const onSubmit: SubmitHandler<ILoginCred> = async (data) => {
    await login(data.login, data.password);
    navigate('/admin');
  };

  return (
    <Box
      width="600px"
      sx={{
        bgcolor: '#121212',
        px: 4,
        py: 8,
        borderRadius: '30px',
        boxShadow: '10px 10px 10px black',
        alignItems: 'center'
      }}>
      <Typography variant="h3" textAlign="center" sx={{ color: 'white', mb: 2 }}>
        Добро пожаловать
      </Typography>

      <Typography variant="h5" textAlign="center" sx={{ color: 'gray', mb: 6 }}>
        Войдите в систему для продолжения
      </Typography>

      <form onSubmit={handleSubmit(onSubmit)}>
        <Controller
          name="login"
          control={control}
          rules={{ required: { value: true, message: 'Требуется' } }}
          render={({ field }) => (
            <>
              <InputLabel shrink sx={{ fontSize: '30px', color: 'white' }}>
                Логин
              </InputLabel>
              <TextField
                onChange={(e) => field.onChange(e)}
                value={field.value}
                error={!!errors.login?.message}
                helperText={errors.login?.message}
                variant="outlined"
                fullWidth
                focused
                InputProps={{ style: { borderRadius: '15px', color: 'white' } }}
              />
            </>
          )}
        />

        <Controller
          name="password"
          control={control}
          rules={{ required: { value: true, message: 'Требуется' } }}
          render={({ field }) => (
            <>
              <InputLabel shrink sx={{ fontSize: '30px', color: 'white', mt: 6 }}>
                Пароль
              </InputLabel>
              <TextField
                onChange={(e) => field.onChange(e)}
                value={field.value}
                error={!!errors.password?.message}
                helperText={errors.password?.message}
                type="password"
                variant="outlined"
                fullWidth
                focused
                InputProps={{ style: { borderRadius: '15px', color: 'white' } }}
              />
            </>
          )}
        />

        <Button
          type="submit"
          size="large"
          sx={{
            mt: 4,
            border: '1px solid',
            px: 10,
            py: 1,
            borderRadius: '15px',
          }}>
          Войти
        </Button>
      </form>
    </Box>
  );
};

export default LoginForm;
