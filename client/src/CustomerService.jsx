import "./styles.css"
import { useState, useEffect } from "react";
import { NavLink, useNavigate, useLocation, useParams } from "react-router";
import { NavigationBar } from "./components/Navbar";
import logo from './assets/logo.png';
export function CustomerServiceView() {
    //const [ticket, setTicket] = useState(null);
    const [unassignedTickets, setUnassignedTickets] = useState([]);
    const [assignedTickets, setAssignedTickets] = useState([]);
    const [closedTickets, setClosedTickets] = useState([]);
    const [showClosed, setShowClosed] = useState(false);

    useEffect(GetUnassignedTickets, []);
    useEffect(GetAssignedTickets, []);
    useEffect(GetClosedTickets, []);


    function GetUnassignedTickets() {
        fetch("/api/tickets/unassigned/")
            .then((response) => response.json())
            .then((data) => setUnassignedTickets(data))
    }

    function GetAssignedTickets() {
        fetch("/api/tickets/assigned/")
            .then((response) => response.json())
            .then((data) => setAssignedTickets(data))
    }
    function GetClosedTickets() {
        fetch("/api/tickets/closed/")
            .then((response) => response.json())
            .then((data) => setClosedTickets(data))
    }




    async function randomiser() {

        try {
            var respons = await fetch(`/api/tickets`, {
                headers: { "Content-Type": "application/json" },
                method: "PUT",
            })
            if (!respons.ok) {
                alert("unable to assign a new ticket")
            }


            GetUnassignedTickets();
            GetAssignedTickets();
        } catch (error) {
            console.error("Error assigning ticket:", error);
        }
    }


    return (
        <main className="form-main">
            <NavigationBar back={"/"} />
            <section className="upper-section">
                <div className="rollToGetATicket">
                    <button onClick={randomiser} className="middle-button">Roll to get a ticket</button>
                </div>
            </section>
            <section className="lower-section">
                <div className="tickets-left">
                        <h2>YOUR TICKETS:</h2>
                        {assignedTickets.length > 0 ? (
                            <ul className="ticket-list">
                                {assignedTickets.map(TicketCard)}
                            </ul>

                        ) : (
                            <ul className="ticket-list"><li className="ticket-list-item" key={"emptyassigned"}><div className="ticket-info"><p>Inga tickets</p></div></li></ul>
                        )}
                    <section className="closed-tickets-box">
                        <button onClick={() => setShowClosed(prevState => !prevState)} className="toggle-inactive-btn">
                            {showClosed ? "Hide closed tickets" : "Show closed tickets"}
                        </button>
                        {showClosed && (
                            <ul className="closed-list">
                                {closedTickets.map(ClosedTicketCard)}
                            </ul>
                        )}

                    </section>
                </div>
                <div className="tickets-right">
                    <h2>ALL TICKETS:</h2>
                    {unassignedTickets.length > 0 ? (
                        <ul className="ticket-list">
                            {unassignedTickets.map(UnassignedTicketCard)}
                        </ul>
                    ) : (
                        <ul className="ticket-list"><li className="ticket-list-item" key={"emptyunnasigned"}><div className="ticket-info"><p>Inga tickets</p></div></li></ul>
                    )}
                </div>
            </section>
        </main>
    );
    function TicketCard(ticket) {
        return <li key={ticket.id} className="ticket-list-item"><NavLink to={"/customer-service/" + ticket.slug + "/ticket-info"} >

            <h2>{ticket.description}</h2>
            <div className="ticket-info">
                <p>Ticket id: {ticket.id}</p>
            </div></NavLink>
        </li>
    }
    function UnassignedTicketCard(ticket) {
        return <li key={ticket.id} className="ticket-list-item"><a>

            <h2>{ticket.description}</h2>
            <div className="ticket-info">
                <p>Ticket id: {ticket.id}</p>
            </div></a>
        </li>
    }

    function ClosedTicketCard(ticket) {
        return <li key={ticket.id} className="closed-ticket-list-item"><NavLink to={"/customer-service/" + ticket.slug + "/ticket-info"}>
            <p>{ticket.description}</p>
        </NavLink>
        </li>
    }

}


export function TicketInfoView() {
    const { slug } = useParams()
    const [messageText, setMessageText] = useState("");
    const [messages, setMessages] = useState([])
    const [ticket, setTicket] = useState("")

    function GetTicketMessages() {
        fetch(`/api/messages/${slug}`).then(response => response.json()).then(data => { setMessages(data) })
    }
    useEffect(GetTicketMessages, [])

    useEffect(() => {
        const intervalId = setInterval(GetTicketMessages, 2000);
        return () => clearInterval(intervalId);
    }, []);



    function PostMessage(e) {
        e.preventDefault();
        const form = e.target;
        let formData = new FormData(form);
        let dataObject = Object.fromEntries(formData)
        dataObject.slug = slug;
        dataObject.customer = false;

        let dataJson = JSON.stringify(dataObject);
        console.log(dataJson);
        fetch("/api/messages/",
            {
                headers: { "Content-Type": "application/json" },
                method: "POST",
                body: dataJson
            }).then(response => {
                if (response.ok) {
                    GetTicketMessages();
                    setMessageText("")
                } else {
                    console.log(response)
                    alert("respons");
                }
            })

    }
    useEffect(GetTicket, [])

    function GetTicket() {
        fetch("/api/tickets/" + slug).then(response => response.json()).then(data => setTicket(data))
    }

    function ChangeStatus() {
        fetch("/api/tickets/status/" + slug,
            {
                headers: { "Content-Type": "application/json" },
                method: "PUT",
                body: JSON.stringify({ status: ticket.status }),
            }).then(response => {
                if (response.ok) {
                    GetTicket();
                } else {
                    console.log(response)
                    alert("respons");
                }
            })
    }

    return (
        <main className="chat-main">
            <NavigationBar back={"/customer-service"} />
            <section className="chat-header"><h1>Chat with Customer</h1></section>
            <section className="chat">
                <ul className="chat-ul"> {messages.map(MessageCard)}
                </ul>
            </section>
            <section className="chat-message-box">
                <form className="chat-message-form" onSubmit={PostMessage} method="POST" >
                    <textarea value={messageText} onChange={(e) => setMessageText(e.target.value)} name="text" type="textarea" className="text-area"></textarea>
                    <input className="small-button" type="submit" value="Send" disabled={!messageText || ticket.status > 2}></input>
                </form>
                <button className="small-button" onClick={ChangeStatus}>{ticket.status < 3 ? "Close Ticket" : "Open Ticket"}</button>
            </section>
        </main>
    );

    function MessageCard(message) {
        const messageSender = message.customer ? "chat-left-message" : "chat-right-message"
        const messageholder = message.customer ? "message-holder-left" : "message-holder-right"
        return <li key={message.id} className={messageSender}><p className={messageholder}>{message.text}</p><p>{message.time}</p></li>
    }
}
