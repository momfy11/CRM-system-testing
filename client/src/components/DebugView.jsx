import { NavLink } from "react-router";

export default function DebugPage()

{
    return<main>
        <NavLink to="/super-admin"><button >SuperAdmin</button> </NavLink>
        <NavLink to="/customer-service"><button>CustomerSupport</button></NavLink> 
        <NavLink to="/customer"><button>Customer</button></NavLink> 
        <NavLink to="/admin"><button>Admin</button></NavLink>
        <NavLink to="/customer/addTicket"><button>Create Ticket</button></NavLink>
        </main>
}