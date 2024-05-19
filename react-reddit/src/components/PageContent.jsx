import { Box } from "@mui/material";

export default function PageContent({ children }) {
  return (
    <Box
      sx={{
        border: 0,
        display: "flex",
        flexDirection: "column",
        justifyContent: "center",
        alignItems: "center",
        margin: "0 auto",
        marginTop: "150px",
      }}
    >
      {children}
    </Box>
  );
}
