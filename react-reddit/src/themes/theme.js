import { colors } from "./colors";
import { createTheme } from "@mui/material/styles";
import { buttonStyle, textFieldStyle, typographyStyle } from "../styles/component-styles";

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
    MuiTextField: {
      styleOverrides: textFieldStyle,
    }
  },
});

export default theme;
