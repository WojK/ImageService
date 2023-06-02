import UserProfile from "../components/Profile/UserProfile";
import classes from "../components/Auth/AuthForm.module.css";
import { useRef, useState } from "react";
import { apiUrl } from "../api";

const ProfilePage = () => {
  const passwordInputRef = useRef("");
  const passwordConfirmInputRef = useRef("");
  const [changedPasswordProfile, setChangedPasswordProfile] = useState(false);
  const [entropy, setEntropy] = useState(0);

  const handleBlur = () => {
    const enteredPassword = passwordInputRef.current.value;

    if (enteredPassword.length === 0) {
      return;
    }

    fetch(`${apiUrl}/count-entropy`, {
      method: "POST",
      body: JSON.stringify({ Password: enteredPassword }),
      headers: {
        "Content-Type": "application/json",
      },
    })
      .then((res) => {
        return res.json();
      })
      .then((data) => {
        setEntropy(data.Entropy);
      })
      .catch((err) => {
        alert("Entropy Count Error");
      });
  };

  const changePasswordHandler = (event) => {
    event.preventDefault();
    const password = passwordInputRef.current.value;
    const passwordConf = passwordConfirmInputRef.current.value;
    const url = `${apiUrl}/change-password`;

    fetch(url, {
      method: "POST",
      body: JSON.stringify({
        password: password,
        passwordConfirm: passwordConf,
      }),
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
    })
      .then((res) => {
        if (res.ok) {
          setChangedPasswordProfile(true);
        } else {
          return res.json().then((data) => {
            throw new Error(data.Message);
          });
        }
      })
      .catch((err) => {
        alert(err.message);
      });

    passwordInputRef.current.value = "";
    passwordConfirmInputRef.current.value = "";
  };

  return (
    <>
      <UserProfile />;
      <section className={classes.auth}>
        <h1>Change your password</h1>
        <form onSubmit={changePasswordHandler}>
          <div className={classes.control}>
            <label htmlFor="password">Your New Password</label>
            <input
              type="password"
              required
              ref={passwordInputRef}
              onBlur={handleBlur}
            />
          </div>
          <div className={classes.control}>
            <label htmlFor="password">Confirm Your New Password</label>
            <input type="password" required ref={passwordConfirmInputRef} />
            <p style={{ color: "white" }}>Entropy: {entropy} </p>
          </div>

          <div className={classes.actions}>
            <button>Change Password!</button>
          </div>
        </form>
        {changedPasswordProfile && <h2>Your password has been changed!</h2>}
      </section>
    </>
  );
};

export default ProfilePage;
