
import { NavLink, useNavigate, useLocation, useParams } from "react-router";
import "./styles.css";
import ecologo from './assets/eco-logo.png';
import { React, useEffect, useState } from "react";



export default function EcoEnterprises()
{
    const companyId = 3;



    return (
        <main className="form-main">
                    <nav className="navbar-mobile"><img src={ecologo} className="mobile-logo"/><p>Echo Enterprices</p></nav>
                    <div className="mobile-company-content-eco">
                        <p>
                            Välkommen till Echo Enterprices
                        </p>
                    </div>
                    <p>Har du några frågor eller gnäll om våra produkter?</p>
                    <section className="mobile-crm">
                        <iframe
                            src = {`http://localhost:5173/customer/addTicket?companyId=${companyId}`}
                            className="ticket-iframe"
                            title="Customer Ticket View"
                            frameBorder="0"
                            width="100%"
                            height="100%"
                        />
                            
                        
                    </section>
                    </main>
    );
}






