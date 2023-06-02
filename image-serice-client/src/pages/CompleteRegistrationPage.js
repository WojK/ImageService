import CompleteRegistration from "../components/Register/CompleteRegistration";
import { useParams } from "react-router-dom";
import { useEffect } from "react";
import { apiUrl } from "../api";

const CompleteRegistrationPage = () => {
  const { token } = useParams();

  const url = `${apiUrl}/verify-account?token=` + token;
  console.log(url);

  useEffect(() => {
    fetch(url, {
      method: "POST",
    }).catch((err) => {
      alert(err.message);
    });
  }, [url]);

  return <CompleteRegistration />;
};

export default CompleteRegistrationPage;
