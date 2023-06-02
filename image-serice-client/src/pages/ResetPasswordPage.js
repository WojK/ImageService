import classes from "../components/Auth/AuthForm.module.css";
import { useRef, useState } from "react";
import { useParams } from "react-router-dom";
import { apiUrl } from "../api";

const ResetPasswordPage = () => {
  const passwordInputRef = useRef("");
  const passwordConfirmInputRef = useRef("");
  const [changedPassword, setChangedPassword] = useState(false);
  const { token } = useParams();

  const submitHandler = (event) => {
    event.preventDefault();
    const enteredPassword = passwordInputRef.current.value;
    const enteredConfirmPassword = passwordConfirmInputRef.current.value;
    const url = `${apiUrl}/reset-password`;

    fetch(url, {
      method: "POST",
      body: JSON.stringify({
        token: token,
        password: enteredPassword,
        confirmPassword: enteredConfirmPassword,
      }),
      headers: {
        "Content-Type": "application/json",
      },
    }).catch((err) => {
      alert(err.message);
    });

    passwordInputRef.current.value = "";
    passwordConfirmInputRef.current.value = "";
    setChangedPassword(true);
  };

  return (
    <section className={classes.auth}>
      <h1>Change your password</h1>
      <form onSubmit={submitHandler}>
        <div className={classes.control}>
          <label htmlFor="password">Your New Password</label>
          <input type="password" required ref={passwordInputRef} />
        </div>
        <div className={classes.control}>
          <label htmlFor="password">Confirm Your New Password</label>
          <input type="password" required ref={passwordConfirmInputRef} />
        </div>

        <div className={classes.actions}>
          <button>Reset Password</button>
        </div>
      </form>
      {changedPassword && <h2>Your password has been changed!</h2>}
    </section>
  );
};

export default ResetPasswordPage;
