
import { NavLink, useNavigate, useLocation, useParams } from "react-router";
import "./styles.css"
import tsol from './assets/tsol.png';
import { React, useEffect, useState } from "react";



export default function TechSolutions()
{
    const companyId = 2;
    
    

    return (
        <main className="form-main">
            <nav className="navbar-mobile"><img src={tsol} className="mobile-logo"/><p>Tech Solutions</p></nav>
            <div className="mobile-company-content-tech">
                <p>
                    Välkommen till Tech-Solutions
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






