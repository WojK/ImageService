import { useEffect, useState } from "react";
import classes from "./MyImagesPage.module.css";
import { apiUrl } from "../api";

const MyImagesPage = () => {
  const [content, setContent] = useState();
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    setIsLoading(true);
    const url = `${apiUrl}/get-myprivateimages`;
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

export default MyImagesPage;
