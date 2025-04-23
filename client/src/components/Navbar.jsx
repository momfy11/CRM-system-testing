import "./../styles.css"
import logo from './../assets/logo.png';
import {data, NavLink, useFetcher, useNavigate} from "react-router";
import { useEffect } from "react";

export function NavigationBar({ back }) {
     let navigate = useNavigate();
    function Logout() {
        fetch("/api/login", {
            headers: {
                "Content-Type": "application/json"
            },
            method: "DELETE",
            body: JSON.stringify({}),
        }).then(response => {if (response.ok){navigate("/")} else {alert("Failed to sign out")}})
    }
   
    return <nav className="navbar">
        <img src={logo}></img>
        {back != "/" ? <NavLink to={back}>
            
            <button className="back-button">⬅️ Back</button>
        </NavLink> : <div></div>}
        <NavLink to={"/change-password"}>
            <button className="back-button">🔒 Change Password</button>
        </NavLink>
        <button className="back-button" onClick={Logout}> Sign Out</button>
        </nav>

}