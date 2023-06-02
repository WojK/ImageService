import { useEffect, useState } from "react";
import classes from "./MyImagesPage.module.css";
import { apiUrl } from "../api";

const PublicImagesPage = () => {
  const [content, setContent] = useState();
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const url = `${apiUrl}/get-publicimages`;
    setIsLoading(true);
    fetch(url, {
      method: "GET",
      headers: {
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
    })
      .then((res) => {
        return res.json();
      })
      .then((data) => {
        if (data.length > 0) {
          const ctn = data.map((elem) => (
            <li key={elem.id}>
              <p>
                Author: {elem.userName} {elem.userSurname}
              </p>
              <img className={classes.img} alt="" src={elem.content} />
            </li>
          ));

          setContent(ctn);
          setIsLoading(false);
        } else {
          setContent(<p>Empty List</p>);
          setIsLoading(false);
        }
      })
      .catch((err) => {
        alert(err.message);
      });
  }, []);

  return (
    <div className={classes.main}>
      <ul>
        {isLoading && <p>Loading...</p>}
        {content}
      </ul>
    </div>
  );
};

export default PublicImagesPage;
