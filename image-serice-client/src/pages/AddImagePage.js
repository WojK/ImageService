import { useState, useRef } from "react";
import FormData from "form-data";
import axios from "axios";
import classes from "./AddImagePage.module.css";
import { apiUrl } from "../api";

const AddImagePage = () => {
  const [selectedImage, setSelectedImage] = useState(null);
  const [selectedImageState, setSelectedImageState] = useState(false);
  const [uploadedImage, setUploadedImage] = useState(false);
  const [idImage, setIdImage] = useState(0);
  const [choseSelectedUsers, setChoseSelectedUsers] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [users, setUsers] = useState();
  const [selectedUsersIds, setSelectedUsersIds] = useState([]);

  const imageOptionRef = useRef("Public");

  const addUserHandler = (event) => {
    const idUser = event.currentTarget.id;
    setSelectedUsersIds((arr) => [...arr, idUser]);
    document.getElementById(event.currentTarget.id).style.display = "none";
  };

  const addChoosenUsersHandler = () => {
    const url = `${apiUrl}/add-users?idImage=` + idImage;
    fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
      body: JSON.stringify(selectedUsersIds),
    }).catch((err) => {
      alert(err.message);
    });

    setSelectedUsersIds([]);
    setChoseSelectedUsers(false);
  };

  const uploadHandler = () => {
    const formData = new FormData();
    const imageOption = imageOptionRef.current.value;
    const url = `${apiUrl}/add-image?option=` + imageOption;

    formData.append("image", selectedImage, selectedImage.name);
    axios
      .post(url, formData, {
        headers: {
          accept: "application/json",
          "Content-Type": `multipart/form-data; boundary=${formData._boundary}`,
          Authorization: "Bearer " + localStorage.getItem("token"),
        },
      })
      .then((res) => {
        setIdImage(JSON.parse(res.data).id);
        setSelectedImage(null);
        setSelectedImageState(false);
        setUploadedImage(true);
      })
      .catch((err) => {
        alert("Our service does not accept your image");
      });

    if (imageOption === "SelectedUsers") {
      setChoseSelectedUsers(true);

      const url = `${apiUrl}/get-users`;
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
                  {elem.name} {elem.surname}
                </p>
                <p>{elem.email}</p>
                <button
                  className={classes.btn}
                  id={elem.id}
                  onClick={addUserHandler}
                >
                  Add user
                </button>
              </li>
            ));

            setUsers(ctn);
            setIsLoading(false);
          } else {
            setUsers(<p>Empty List</p>);
            setIsLoading(false);
          }
        })
        .catch((err) => {
          alert(err.message);
        });
    }
  };

  return (
    <div className={classes.main}>
      <h1>Upload Image</h1>
      {selectedImageState && (
        <div>
          <img
            alt="To upload"
            width={"250px"}
            src={URL.createObjectURL(selectedImage)}
          />
          <br />
          <button
            className={classes.btn}
            onClick={() => {
              setSelectedImage(null);
              setSelectedImageState(false);
            }}
          >
            Remove
          </button>
        </div>
      )}

      {!selectedImage && (
        <input
          className={classes.fileChoose}
          type="file"
          name="myImage"
          accept="image/png, image/jpg, image/jpeg"
          onChange={(event) => {
            setSelectedImage(event.target.files[0]);
            setSelectedImageState(true);
            setUploadedImage(false);
          }}
        />
      )}

      {selectedImage && (
        <select className={classes.customSelect} ref={imageOptionRef}>
          <option>Public</option>
          <option>Private</option>
          <option>SelectedUsers</option>
        </select>
      )}

      {selectedImage && (
        <button className={classes.btn} onClick={uploadHandler}>
          Upload
        </button>
      )}

      {uploadedImage && <h2 style={{ color: "black" }}>Uploaded Image!</h2>}

      {isLoading && <p>Loading...</p>}
      {choseSelectedUsers && (
        <div>
          <ul>{users}</ul>
          <button className={classes.btn} onClick={addChoosenUsersHandler}>
            Add Choosen Users
          </button>
        </div>
      )}
    </div>
  );
};

export default AddImagePage;
