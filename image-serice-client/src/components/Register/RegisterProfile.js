import classes from "./RegisterProfile.module.css";
import { useNavigate } from "react-router-dom";
import { useRef, useState } from "react";
import { apiUrl } from "../../api";

const RegisterProfile = () => {
  const navigate = useNavigate();

  const nameInputRef = useRef("");
  const surnameInputRef = useRef("");
  const emailInputRef = useRef("");
  const passwordInputRef = useRef("");
  const confirmPasswordInputRef = useRef("");
  const [isLoading, setIsLoading] = useState(false);
  const [entropy, setEntropy] = useState(0);
  const [entropyM, setEntropyM] = useState("");

  const signIn = () => {
    navigate("/auth");
  };

  const submitHandler = (event) => {
    event.preventDefault();
    setIsLoading(true);

    const enteredName = nameInputRef.current.value;
    const enteredSurname = surnameInputRef.current.value;
    const enteredEmail = emailInputRef.current.value;
    const enteredPassword = passwordInputRef.current.value;
    const enteredConfirmPassword = confirmPasswordInputRef.current.value;
    const url = `${apiUrl}/register`;

    fetch(url, {
      method: "POST",
      body: JSON.stringify({
        name: enteredName,
        surname: enteredSurname,
        email: enteredEmail,
        password: enteredPassword,
        confirmPassword: enteredConfirmPassword,
      }),
      headers: {
        "Content-Type": "application/json",
      },
    })
      .then((res) => {
        setIsLoading(false);
        if (res.ok) {
          navigate("/finish-registration");
          nameInputRef.current.value = "";
          surnameInputRef.current.value = "";
          passwordInputRef.current.value = "";
          confirmPasswordInputRef.current.value = "";
          emailInputRef.current.value = "";
        } else {
          return res.json().then((data) => {
            throw new Error(data.Message);
          });
        }
      })
      .catch((err) => {
        setIsLoading(false);
        console.log(err.message);
        alert(err.message);
      });
  };

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
        if (data.Entropy <= 50) {
          setEntropyM("Password is very weak! Not allowed to register!");
        } else if (data.Entropy > 50 && data.Entropy < 80) {
          setEntropyM("Medium secure! Allow to register");
        } else if (data.Entropy >= 80) {
          setEntropyM("Password is strong! Good Job");
        }
      })
      .catch((err) => {
        alert("Entropy Count Error");
      });
  };

  return (
    <section className={classes.auth}>
      <h1>Register</h1>
      <form onSubmit={submitHandler}>
        <div className={classes.control}>
          <label htmlFor="name">Your Name</label>
          <input type="text" required ref={nameInputRef} />
        </div>

        <div className={classes.control}>
          <label htmlFor="surname">Your Surname</label>
          <input type="text" required ref={surnameInputRef} />
        </div>

        <div className={classes.control}>
          <label htmlFor="email">Your Email</label>
          <input type="email" required ref={emailInputRef} />
        </div>
        <div className={classes.control}>
          <label htmlFor="password">Your Password</label>
          <input
            type="password"
            required
            ref={passwordInputRef}
            onBlur={handleBlur}
          />
          <p>
            {entropyM} <br></br> Entropy: {entropy}
          </p>
        </div>

        <div className={classes.control}>
          <label htmlFor="password">Confirm Password</label>
          <input type="password" required ref={confirmPasswordInputRef} />
        </div>

        <div className={classes.actions}>
          <button>SignUp</button>
          <button type="button" className={classes.toggle} onClick={signIn}>
            Have already account ? SingIn
          </button>
        </div>
      </form>

      {isLoading && <p>Loading...</p>}
    </section>
  );
};

export default RegisterProfile;
