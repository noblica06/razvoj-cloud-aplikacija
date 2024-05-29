import { Typography, TextField, Container, Box, Button, Grid, Link, Checkbox, FormControlLabel} from "@mui/material";
import { colors } from "../themes/colors";
import { styled } from '@mui/material/styles';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import { useForm } from "react-hook-form";
import { useAuth } from "../services/user-service";
import { useNavigate } from "react-router-dom";
import { useEffect } from "react";

const VisuallyHiddenInput = styled('input')({
  clip: 'rect(0 0 0 0)',
  clipPath: 'inset(50%)',
  height: 1,
  overflow: 'hidden',
  position: 'absolute',
  bottom: 0,
  left: 0,
  whiteSpace: 'nowrap',
  width: 1,
});

export default function SignupForm() {
  const { register, handleSubmit } = useForm({
    defaultValues: {
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      address: '',
      city: '',
      country: '',
      phoneNumber: '',
    }
  });

  const {registerApi, user} = useAuth();

  const navigate = useNavigate();

  useEffect(() => {
    if(user !== null){
      navigate("/");
    }
  })

  const handleRegister = (form) => {
    registerApi(form.firstName, form.lastName, form.address, form.city, form.country, form.password, form.email, form.phoneNumber);
  }
  return (
    <Container component="main" maxWidth="xs">
    <Box
      sx={{
        // marginTop: 8,
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
      }}
    >
      <Typography component="h1" variant="h5">
        Sign up
      </Typography>
      <form onSubmit={handleSubmit(handleRegister)}>
        <Box noValidate sx={{ mt: 3, alignItems: 'center'}}>
          <Grid container spacing={2}>
            <Grid item xs={12} sm={6}>
              <TextField
              sx={{"& .MuiOutlinedInput-root": {color: colors["lavender"]}, "& .MuiOutlinedInput-notchedOutline": {color: colors["lavender"], borderColor: colors["lavender"], borderWidth: "2px"}, "& .MuiInputLabel-outlined": {color: colors["lavender"]}}}
                autoComplete="given-name"
                name="firstName"
                required
                fullWidth
                id="firstName"
                label="First Name"
                autoFocus
                {...register("firstName")}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
              sx={{"& .MuiOutlinedInput-root": {color: colors["lavender"]}, "& .MuiOutlinedInput-notchedOutline": {color: colors["lavender"], borderColor: colors["lavender"], borderWidth: "2px"}, "& .MuiInputLabel-outlined": {color: colors["lavender"]}}}
                required
                fullWidth
                id="lastName"
                label="Last Name"
                name="lastName"
                autoComplete="family-name"
                {...register("lastName")}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
              sx={{"& .MuiOutlinedInput-root": {color: colors["lavender"]}, "& .MuiOutlinedInput-notchedOutline": {color: colors["lavender"], borderColor: colors["lavender"], borderWidth: "2px"}, "& .MuiInputLabel-outlined": {color: colors["lavender"]}}}
                required
                fullWidth
                id="email"
                label="Email Address"
                name="email"
                autoComplete="email"
                type="Email"
                {...register("email")}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
              sx={{"& .MuiOutlinedInput-root": {color: colors["lavender"]}, "& .MuiOutlinedInput-notchedOutline": {color: colors["lavender"], borderColor: colors["lavender"], borderWidth: "2px"}, "& .MuiInputLabel-outlined": {color: colors["lavender"]}}}
                required
                fullWidth
                name="password"
                label="Password"
                type="password"
                id="password"
                autoComplete="new-password"
                {...register("password")}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                required
                sx={{"& .MuiOutlinedInput-root": {color: colors["lavender"]}, "& .MuiOutlinedInput-notchedOutline": {color: colors["lavender"], borderColor: colors["lavender"], borderWidth: "2px"}, "& .MuiInputLabel-outlined": {color: colors["lavender"]}}}
                fullWidth
                id="adress"
                label="Address"
                name="address"
                autoComplete="address"
                {...register("address")}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
              sx={{"& .MuiOutlinedInput-root": {color: colors["lavender"]}, "& .MuiOutlinedInput-notchedOutline": {color: colors["lavender"], borderColor: colors["lavender"], borderWidth: "2px"}, "& .MuiInputLabel-outlined": {color: colors["lavender"]}}}
                required
                fullWidth
                id="city"
                label="City"
                name="city"
                autoComplete="city"
                {...register("city")}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                required
                sx={{"& .MuiOutlinedInput-root": {color: colors["lavender"]}, "& .MuiOutlinedInput-notchedOutline": {color: colors["lavender"], borderColor: colors["lavender"], borderWidth: "2px"}, "& .MuiInputLabel-outlined": {color: colors["lavender"]}}}
                fullWidth
                id="country"
                label="Country"
                name="country"
                autoComplete="country"
                {...register("country")}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                required
                sx={{"& .MuiOutlinedInput-root": {color: colors["lavender"]}, "& .MuiOutlinedInput-notchedOutline": {color: colors["lavender"], borderColor: colors["lavender"], borderWidth: "2px"}, "& .MuiInputLabel-outlined": {color: colors["lavender"]}}}
                fullWidth
                id="phoneNumber"
                label="Phone Number"
                name="phoneNumber"
                autoComplete="phoneNumber"
                {...register("phoneNumber")}
              />
            </Grid>
          </Grid>
          <Grid item xs={12}>
            <Button
              sx={{mt: 3}}
              component="label"
              role={undefined}
              variant="contained"
              tabIndex={-1}
              startIcon={<CloudUploadIcon />}
            >
            Upload file
        <VisuallyHiddenInput type="file" />
      </Button>
        </Grid>
          <Grid container flexDirection="row" alignItems='center' gap={8}>
            <Grid item>
              <Button
                type="submit"
              
                variant="contained"
                sx={{ mt: 3, mb: 2 , fullWidth: true}}
              >
                Sign Up
              </Button>
            </Grid>
            <Grid item>
              <Link href="#" variant="body2">
                Already have an account? Sign in
              </Link>
            </Grid>
          </Grid>
        </Box>
      </form>
    </Box>
  </Container>
  )
}
