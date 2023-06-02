import { useNavigate } from "react-router-dom";
import { useRef, useContext } from "react";
import classes from "./AuthForm.module.css";
import AuthContext from "../../store/auth-context";
import { apiUrl } from "../../api";

const AuthForm = () => {
  const navigate = useNavigate();
  const emailInputRef = useRef("");
  const passwordInputRef = useRef("");
  const authCtx = useContext(AuthContext);

  const createAccount = () => {
    navigate("/register");
  };

  const resetPassword = () => {
    navigate("/reset-password");
  };

  const submitHandler = (event) => {
    event.preventDefault();

    const enteredEmail = emailInputRef.current.value;
    const enteredPassword = passwordInputRef.current.value;
    const url = `${apiUrl}/login`;

    fetch(url, {
      method: "POST",
      body: JSON.stringify({
        email: enteredEmail,
        password: enteredPassword,
      }),
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
      },
    })
      .then((res) => {
        if (res.ok) {
          return res.json();
        } else {
          return res.json().then((data) => {
            console.log(data);
            throw new Error(data.Message);
          });
        }
      })
      .then((data) => {
        authCtx.login(data.Token);

        navigate("/");
      })
      .catch((err) => {
        alert(err.message);
      });
    emailInputRef.current.value = "";
    passwordInputRef.current.value = "";
  };

  return (
    <section className={classes.auth}>
      <h1>Login</h1>
      <form onSubmit={submitHandler}>
        <div className={classes.control}>
          <label htmlFor="email">Your Email</label>
          <input type="email" required ref={emailInputRef} />
        </div>
        <div className={classes.control}>
          <label htmlFor="password">Your Password</label>
          <input type="password" required ref={passwordInputRef} />
        </div>

        <div className={classes.actions}>
          <button>Login</button>
          <button
            type="button"
            className={classes.toggle}
            onClick={createAccount}
          >
            Create new account
          </button>
          <button
            type="button"
            className={classes.toggle}
            onClick={resetPassword}
          >
            Forgot password ? Click here
          </button>
        </div>
      </form>
    </section>
  );
};

export default AuthForm;
