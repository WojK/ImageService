import classes from "../components/Auth/AuthForm.module.css";
import { useRef, useState } from "react";
import { apiUrl } from "../api";

const ResetPasswordEmailPage = () => {
  const emailInputRef = useRef("");
  const [emailSent, setEmailSent] = useState(false);

  const submitHandler = (event) => {
    event.preventDefault();
    const emailValue = emailInputRef.current.value;

    const url =
      `${apiUrl}/forgot-password?email=` + encodeURIComponent(emailValue);

    fetch(url, {
      method: "POST",
    }).catch((err) => {
      alert(err.message);
    });

    emailInputRef.current.value = "";
    setEmailSent(true);
  };

  return (
    <section className={classes.auth}>
      <h1>Reseting your password</h1>
      <form onSubmit={submitHandler}>
        <div className={classes.control}>
          <label htmlFor="email">Your Email</label>
          <input type="email" required ref={emailInputRef} />
        </div>

        <div className={classes.actions}>
          <button>Send email to reset password</button>
        </div>
      </form>

      {emailSent && <h2>Email was sent</h2>}
    </section>
  );
};

export default ResetPasswordEmailPage;
