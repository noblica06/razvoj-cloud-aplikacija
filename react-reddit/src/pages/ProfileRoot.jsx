import { Outlet } from "react-router-dom";

const ProfileRootLayout = () => {
  return (
    <>
      <main>
        <Outlet />
      </main>
    </>
  );
};

export default ProfileRootLayout;
