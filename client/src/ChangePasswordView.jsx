import { BrowserRouter, NavLink, useFetcher, useNavigate, useLocation, useParams } from "react-router";
import { createContext, useEffect, useState, use } from "react";
import logo from './assets/logo.png';

export function ChangePasswordView() {


    function updatePassword(e) {
        e.preventDefault();

        
        const form = e.target;

        let formData = new FormData(form);
        let dataObject = Object.fromEntries(formData);
        if(dataObject.password!==dataObject.repeatPassword){
            alert("Passwords do not match"); 
        }else{
            let dataJson = JSON.stringify(dataObject);
            fetch(form.action, {
                headers: { "Content-Type": "application/json" },
                method: "PUT",
                body: dataJson
            }).then(response => {
                if (response.ok) {
                    alert(`Du updaterade lösenordet `);
                } else {
                    alert("Var god ange korrekt nuvarande lösenord");
                }
            })
        }
        }

    return (
        <main>
            <nav className="navbar"><img src={logo}></img> <NavLink to="/"><button className="back-button">⬅️ Sign Out</button></NavLink></nav>
            <form className="data-form" onSubmit={updatePassword} action={`/api/users/password/`} method="PUT">
                <div className="form-box">
                        <input
                            name="oldPassword"
                            placeholder="current password"
                            type="password"
                            required
                        />
                       
                        <input
                            name="password"
                            placeholder="new password"
                            type="password"
                            required
                        />
                    <input
                            name="repeatPassword"
                            type="password"
                            placeholder="repeate new password"
                            required
                        />
                </div>
                <input type="submit" value="Accept" className="middle-button"></input>
            </form>
        </main>
    );
}