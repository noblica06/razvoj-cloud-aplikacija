import { useRouteError } from "react-router-dom";
import MainNavigation from "../components/MainNavigation";
import { Box, Typography } from "@mui/material";
const ErrorPage = () => {
  const error = useRouteError();
  let title = "An error occurred!";
  let message = "Something went wrong.";

  if (error.status === 500) {
    message = JSON.parse(error.data).message;
  }
  if (error.status === 404) {
    title = "Not found";
    message = "Could not find resource or page.";
  }

  return (
    <>
      <MainNavigation />
      <Box
        sx={{
          border: 0,
          display: "flex",
          flexDirection: "column",
          justifyContent: "center",
          alignItems: "start",
          gap: "16px",
          margin: "0 auto",
          marginTop: "150px",
        }}
      >
        <Typography variant="h1" component="div" sx={{ flexGrow: 1 }}>
          {title}
        </Typography>
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          {message}
        </Typography>
      </Box>
    </>
  );
};

export default ErrorPage;
