import { Link, useNavigate } from "react-router-dom";
import classes from "./MainNavigation.module.css";
import { useContext } from "react";
import AuthContext from "../../store/auth-context";

const MainNavigation = () => {
  const authCtx = useContext(AuthContext);
  const navigate = useNavigate();

  const isLoggedIn = authCtx.isLoggedIn;

  const logoutHandler = () => {
    authCtx.logout();
    navigate("/");
  };

  return (
    <header className={classes.header}>
      <Link to="/">
        <div className={classes.logo}>Image Service</div>
      </Link>
      <nav>
        <ul>
          {!isLoggedIn && (
            <li>
              <Link to="/auth">Login</Link>
            </li>
          )}

          {!isLoggedIn && (
            <li>
              <Link to="/register">SignUp</Link>
            </li>
          )}

          {isLoggedIn && (
            <li>
              <Link to="/my-images">My Images</Link>
            </li>
          )}

          {isLoggedIn && (
            <li>
              <Link to="/public-images">Public Images</Link>
            </li>
          )}

          {isLoggedIn && (
            <li>
              <Link to="/selected-users">Images For Selected Users</Link>
            </li>
          )}

          {isLoggedIn && (
            <li>
              <Link to="/add-image">Add Image</Link>
            </li>
          )}

          {isLoggedIn && (
            <li>
              <Link to="/profile">Profile</Link>
            </li>
          )}
          {isLoggedIn && (
            <li>
              <button onClick={logoutHandler}>Logout</button>
            </li>
          )}
        </ul>
      </nav>
    </header>
  );
};

export default MainNavigation;
