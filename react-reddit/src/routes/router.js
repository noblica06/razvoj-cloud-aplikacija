import { createBrowserRouter } from "react-router-dom";

//pages
import RootLayout from ".././pages/Root";
import HomePage from ".././pages/Home";
import LoginPage from "../pages/Login";
import SignupPage from "../pages/Signup";
import ProfileRootLayout from "../pages/ProfileRoot";
import ProfileDetailsPage from "../pages/ProfileDetails";
import EditProfilePage from "../pages/EditProfile";
import ChangePasswordPage from "../pages/ChangePassword";
import PostsRootLayout from "../pages/PostsRoot";
import PostDetailsPage from "../pages/PostDetails";
import NewPostPage from "../pages/NewPost";
import EditPostPage from "../pages/EditPost";
import ErrorPage from "../pages/Error";
//loaders

//actions

const router = createBrowserRouter([
  {
    path: "/",
    element: <RootLayout />,
    children: [
      { index: true, element: <HomePage /> },
      { path: "login", element: <LoginPage />, action: () => {} },
      { path: "signup", element: <SignupPage />, action: () => {} },
      {
        path: "profile",
        element: <ProfileRootLayout />,
        children: [
          {
            path: ":userId",
            element: <ProfileDetailsPage />,
          },
          {
            path: ":userId/edit",
            element: <EditProfilePage />,
            action: () => {},
          },
          {
            path: ":userId/change-password",
            element: <ChangePasswordPage />,
            action: () => {},
          },
        ],
      },
      {
        path: "posts",
        element: <PostsRootLayout />,
        children: [
          { path: ":postId", element: <PostDetailsPage /> },
          { path: "new", element: <NewPostPage />, action: () => {} },
          { path: ":postId/edit", element: <EditPostPage />, action: () => {} },
        ],
      },
    ],
    errorElement: <ErrorPage />,
  },
]);

export default router;
