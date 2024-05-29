import axiosInstance from "./axios"
import { createContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import * as React from 'react';
import { redirect } from "react-router-dom";

const UserContext = createContext({});

export const UserProvider = ({ children }) => {
    //const navigate = useNavigate();
    const [user, setUser] = useState(null);
    const [isReady, setIsReady] = useState(false);

    useEffect(() => {
        const user = localStorage.getItem("user");
        if (user) {
          setUser(JSON.parse(user));
          //axios.defaults.headers.common["Authorization"] = "Bearer " + token;
        }
        setIsReady(true);
      }, []);

    const loginApi = async (email, password) => {
        const formData = new FormData();
        formData.append("email", email);
        formData.append("password", password);
        const data = await axios.post("auth/login", formData,
        {
          headers: {
              'Content-Type': 'application/json',
          }})
          .then((response) => {
            console.log(response.data);
            const userObj = {       
                email: response.data.email,
            };
            setUser(userObj);
            //navigate("/");
            redirect("/");
          })
          .catch((error) => {
            console.log("Wrong creditentials")
            console.log(error)});
    }

    const registerApi = async (firstName, lastName, address, city, country, password, email, phoneNumber) => {
      console.log("Pre axiosaaaa!!!!!!")
      //FormData formData = new FormData();
      //formData.append("email", email);
      //await axios.post("auth/register", 
      //{email, password, firstName, lastName, city, address, country, phoneNumber})
      axios.post('auth/register', 
      {password, firstName, lastName, address, city, country, phoneNumber, email, firstName},{
        headers: {
            'Content-Type': 'application/json',
        }})
      .then((res) =>{
        console.log(res.data);
        
        /*const userObj = {       
            email: res.data.email,
        }; */
        console.log("POSLE axiosaaaa!!!!!!")
        //setUser(userObj);
        //localStorage.setItem("user", JSON.stringify(userObj));
      })
      .catch((error) => console.log(error));
      
      //navigate("/");
      redirect("/");
  }

  const logout = () => {
    //localStorage.removeItem("token");
    localStorage.removeItem("user");
    setUser(null);
    //setToken("");
    //navigate("/");
  };

    return (
        <UserContext.Provider
          value={{ loginApi, user, registerApi, logout }}
        >
          {isReady ? children : null}
        </UserContext.Provider>
      );
};

export const useAuth = () => React.useContext(UserContext);