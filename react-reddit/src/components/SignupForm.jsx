import { Typography, TextField, Container, Box, Button, Grid, Link, Checkbox, FormControlLabel} from "@mui/material";
import { colors } from "../themes/colors";
export default function SignupForm() {
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
      <Box component="form" noValidate sx={{ mt: 3, alignItems: 'center'}}>
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
            />
          </Grid>
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
    </Box>
  </Container>
  )
}
