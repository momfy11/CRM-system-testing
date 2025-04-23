import { BrowserRouter, NavLink, useFetcher, useNavigate, useLocation, useParams } from "react-router";
import "./adminViewStyle.css";
import { createContext, useEffect, useState, use } from "react";
import logo from './assets/logo.png';

export default function LoginPage() {

    let navigate = useNavigate();

    function login(event) {
        event.preventDefault();
        const form = event.target;
        let data = new FormData(form);
        data = Object.fromEntries(data);
        data = JSON.stringify(data);

        fetch(form.action, {
            headers: {
                "Content-Type": "application/json"
            },
            method: form.method,
            body: data,
        })
            .then(response => {
                if (response.ok) {
                    response.text().then(location => { navigate(location.slice(1, -1)) })

                }
            })


    }

    return <main className="login-main">
        <section className="main-page-content"></section>
        <section className="loginSection">
            <h2>Swine-Sync</h2>
            <p>Logga in med din grisiga Epost och Lösenord</p>
        <form name="login-form" onSubmit={login} action="/api/login" method="POST">
            <input type="email" name="email" placeholder="Email.."></input>
            <input type="password" name="password" placeholder="Password.."></input>
            <input type="submit" value="login!"></input>
            </form>
        </section>
    </main>
}