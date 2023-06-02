import { Navigate, Route, Routes } from "react-router-dom";
import Layout from "./components/Layout/Layout";
import ProfilePage from "./pages/ProfilePage";
import AuthPage from "./pages/AuthPage";
import HomePage from "./pages/HomePage";
import ResetPasswordEmailPage from "./pages/ResetPasswordEmailPage";
import ResetPasswordPage from "./pages/ResetPasswordPage";
import ToFinishRegistrationPage from "./pages/ToFinishRegistrationPage";
import CompleteRegistrationPage from "./pages/CompleteRegistrationPage";
import MyImagesPage from "./pages/MyImagesPage";
import AddImagePage from "./pages/AddImagePage";
import SelectedUsersPage from "./pages/SelectedUsersPage";
import GlobalStyle from "./globalStyles";
import RegisterPage from "./pages/RegisterPage";
import { useContext } from "react";
import AuthContext from "./store/auth-context";
import PublicImagesPage from "./pages/PublicImagesPage";

function App() {
  const authCtx = useContext(AuthContext);
  const isLoggedIn = authCtx.isLoggedIn;

  return (
    <>
      <GlobalStyle />
        <Layout>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/auth" element={<AuthPage />} />
            <Route
              path="/profile"
              element={
                isLoggedIn ? (
                  <ProfilePage />
                ) : (
                  <Navigate replace to={"/auth"} />
                )
              }
            />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/finish-registration" element={<ToFinishRegistrationPage />} />
            <Route path="/verification-token/:token" element={<CompleteRegistrationPage />} />
            <Route path="/reset-password/" element={<ResetPasswordEmailPage />} />
            <Route path="/change-password/:token" element={<ResetPasswordPage />} />
            <Route path="/add-image" element={<AddImagePage />} />
            <Route path="/public-images" element={<PublicImagesPage />} />
            <Route path="/my-images" element={<MyImagesPage />} />
            <Route path="/selected-users" element={<SelectedUsersPage />} />
          </Routes>
        </Layout>
    </>
  );
}

export default App;
