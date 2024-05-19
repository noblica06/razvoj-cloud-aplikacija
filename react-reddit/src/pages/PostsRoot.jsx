import { Outlet } from "react-router-dom";
export default function PostsRootLayout() {
  return (
    <>
      <main>
        <Outlet />
      </main>
    </>
  );
}
