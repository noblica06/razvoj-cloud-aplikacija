import { colors } from "./colors";
import { createTheme } from "@mui/material/styles";
import { buttonStyle, typographyStyle } from "../styles/component-styles";

const theme = createTheme({
  palette: {
    primary: {
      main: colors["lavender"],
    },
    secondary: {
      main: colors["lavender"],
    },
    background: {
      default: colors["space-cadet"],
      paper: colors["space-cadet"],
    },
  },
  components: {
    MuiButton: {
      styleOverrides: buttonStyle,
    },
    MuiTypography: {
      styleOverrides: typographyStyle,
    },
  },
});

export default theme;
