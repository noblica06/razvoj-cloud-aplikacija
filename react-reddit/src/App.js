import { RouterProvider } from "react-router-dom";
import { ThemeProvider } from "@mui/material/styles";
import CssBaseline from "@mui/material/CssBaseline";
import DocumentTitle from "react-document-title";

import theme from "./themes/theme";
import router from "./routes/router";
function App() {
  return (
    <DocumentTitle title={"Reddit"}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <div
          style={{
            display: "flex",
            flexDirection: "column",
            margin: "20px",
            background: theme.palette.background.default,
          }}
        >
          <RouterProvider router={router}></RouterProvider>
        </div>
      </ThemeProvider>
    </DocumentTitle>
  );
}

export default App;
