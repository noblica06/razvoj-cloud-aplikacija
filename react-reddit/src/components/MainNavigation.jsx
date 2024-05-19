import { NavLink, useNavigate } from "react-router-dom";
import * as React from "react";

import {
  AppBar,
  Box,
  Toolbar,
  IconButton,
  MenuItem,
  Menu,
  Button,
} from "@mui/material";
import { AccountCircle } from "@mui/icons-material";
import HomeIcon from "@mui/icons-material/Home";

export default function MainNavigation() {
  const [auth, setAuth] = React.useState(true);
  const [anchorEl, setAnchorEl] = React.useState(null);

  const [isLoggedIn, setIsLoggedIn] = React.useState(false);
  const navigate = useNavigate();

  const handleChange = (event) => {
    setAuth(event.target.checked);
    if (auth) {
      setIsLoggedIn(true);
    }
    setIsLoggedIn(false);
    navigate("/login");
  };

  const handleMenu = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar position="static" enableColorOnDark sx={{ borderRadius: "30px" }}>
        <Toolbar sx={{ display: "flex", justifyContent: "space-between" }}>
          <IconButton size="large" component={NavLink} to="/" color="inherit">
            <HomeIcon />
          </IconButton>
          {auth && (
            <div>
              <IconButton
                size="large"
                aria-label="account of current user"
                aria-controls="menu-appbar"
                aria-haspopup="true"
                onClick={handleMenu}
                color="inherit"
              >
                <AccountCircle />
              </IconButton>
              <Menu
                id="menu-appbar"
                anchorEl={anchorEl}
                anchorOrigin={{
                  vertical: "top",
                  horizontal: "right",
                }}
                keepMounted
                transformOrigin={{
                  vertical: "top",
                  horizontal: "right",
                }}
                open={Boolean(anchorEl)}
                onClose={handleClose}
              >
                <MenuItem>
                  <Button
                    component={NavLink}
                    to="/profile/1"
                    onClick={handleClose}
                  >
                    <Box
                      sx={{
                        display: "flex",
                        flexDirection: "row",
                        justifyContent: "space-between",
                      }}
                    >
                      <p>Profile</p>
                    </Box>
                  </Button>
                </MenuItem>
                <MenuItem onClick={handleClose}>
                  <Button component={NavLink} to="/login" onClick={() => {}}>
                    Log out
                  </Button>
                </MenuItem>
              </Menu>
            </div>
          )}
        </Toolbar>
      </AppBar>
    </Box>
  );
}
