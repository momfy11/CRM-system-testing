import React, {useState} from "react";
import { NavLink, useNavigate, useLocation, useParams, data } from "react-router";
import logo from '../assets/logo.png';


export default function CreateCustomerView () 
{
 
   
    
    const handleOnSubmit = async (e) => {
        e.preventDefault();
        const form = e.target;
        let formData = new FormData(form);
        let dataObject = Object.fromEntries(formData);
        dataObject.role=1; 
        if (dataObject.password !== dataObject.rePassword) {
            alert(`password does not match `)
            return;
        }
        let dataJson=JSON.stringify(dataObject);
        const response = await fetch("/api/users", {
            method: "POST",
            headers: { "Content-Type": "application/json" },                body: dataJson
            });
            
        if (!response.ok){
            throw new Error("User not created");
        }
        if (response.ok) {
            alert(`Du lade till ${dataObject.name} `);
        } else {
            alert("Något gick fel ");
        }         

    };    
    
    return (<main>
        <nav className="navbar"><img src={logo}></img></nav>
        <form className="data-form" onSubmit={handleOnSubmit}>
            <div className="form-box">
                        <label>
                                Name:
                            <input
                                type="text"
                                name="name"
                                placeholder="Enter name"
                                className="ticket-input"
                                required
                            />
                        </label>

                <label>
                    Email:
                            <input
                                type="email"
                                name="email"
                                placeholder="Enter email"
                                className="ticket-input"
                                required
                            />
                        </label>

                <label>
                    Password:
                            <input
                                type="password"
                                name="password"
                                placeholder="Enter password"
                                className="ticket-input"
                                required
                            />
                        </label>

                <label>
                    Repeat Password:
                            <input
                                type="password"
                                name="rePassword"
                                placeholder="Repeat password"
                                className="ticket-input"
                                required
                            />
                        </label>
                        </div>
                            <input type="submit" className="middle-button" value="Create Account">
                            </input>
                        
                    </form>
            </main>
    ); 

}
