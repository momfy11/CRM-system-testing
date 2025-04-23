import { NavLink, useNavigate, useLocation, useParams } from "react-router";
import "./styles.css"
import { useEffect, useState } from "react";
import { NavigationBar } from "./components/Navbar";



export function SuperAdminView() {
    return <main className="option-main">
        <NavigationBar back={"/"} />
        <div className="big-button-container">
            <NavLink to="/companies"><button className="big-button">Companies</button></NavLink>
            <NavLink to="/admins"><button className="big-button">Admins</button></NavLink>
        </div>
    </main>;
}


export function SuperAdminCompanyView() {
    const [companies, setCompanies] = useState([]);
    const [InactiveCompanies, setInactiveCompanies] = useState([]);
    const [showInactive, setShowInactive] = useState(false);

    function GetCompanies() {
        fetch("/api/companies/?active=true").then(response =>
            response.json())
            .then(data => setCompanies(data));
    }
    function GetInactiveCompanies() {
        fetch("/api/companies/?active=false").then(response =>
            response.json())
            .then(data => setInactiveCompanies(data));
    }



    function BlockCompanyById(id, active) {
        fetch(`/api/companies/block/${id}/${active}`, {
            headers: { "Content-Type": "application/json" },
            method: "PUT",
            body: JSON.stringify(id, active),
        })
            .then(response => {
                if (response.ok) { console.log("Det Funkade Igen"), GetCompanies(), GetInactiveCompanies() }
            })
    }

    useEffect(GetCompanies, []);
    useEffect(GetInactiveCompanies,[]);

    return <main className="role-specific-main">
        <NavigationBar back={"/super-admin"}/>
        <section className="header-section"><h1>All Companies</h1></section>
        <div className="list-content-box">
        <ul className="list">
            {companies.map(CompanyCard)}
        </ul>
            <section className="statusBox">
                    <button onClick={() => setShowInactive(prevState => !prevState)} className="toggle-inactive-btn">
                        {showInactive ? "Hide Inactive Companies" : "Show Inactive Companies"}
                    </button>
                    {showInactive && (
                        <ul className="inactive-list">
                            {InactiveCompanies.map(InactiveCompanyCard)}
                        </ul>
                    )}
                </section>
        </div>
        <section className="content-box">
            <NavLink to="/companies/add"><button className="middle-button">Add Company</button></NavLink></section>
    </main>;

    function CompanyCard(company) {
        return <li className="list-item" key={company.id}>
            <div className="card-info">
                <p><strong>Name:</strong><br />{company.name}</p><br />
                <p><strong>Email:</strong><br />{company.email}</p><br />
                <p><strong>Domain:</strong><br />{company.domain}</p><br />
            </div>
            <div className="card-buttons">
                <NavLink to={"/companies/" + company.id + "/edit"}><button>Edit</button></NavLink>
                <button className="small-button" onClick={() => BlockCompanyById(company.id, company.active)}>block</button>
            </div>
        </li>
    }

    function InactiveCompanyCard(company) {
        return <li className="inactive-list-item" key={company.id}>
            <p>{company.name}</p><button onClick={() => BlockCompanyById(company.id, company.active)}>Activate</button>
        </li>
    }
}
export function SuperAdminAddCompanyView() {

    function postCompany(e) {
        e.preventDefault();
        const form = e.target;
        let formData = new FormData(form);
        let dataObject = Object.fromEntries(formData);
        let dataJson = JSON.stringify(dataObject);

        fetch(form.action, {
            headers: { "Content-Type": "application/json" },
            method: form.method,
            body: dataJson
        }).then(response => {
            if (response.ok) {
                alert(`Du lade till ${dataObject.name} i databasen 🎉`);
            } else {
                alert("Något gick fel ❌");
            }
        });
    }

    return (
        <main className="form-main">
            <NavigationBar back={"/companies"}/>
            <form className="data-form" onSubmit={postCompany} action="/api/companies?active=true" method="POST">
                <div className="form-box">
                    <label>
                        Name:
                        <input
                            name="name"
                            type="text"
                            required
                        />
                    </label>

                    <label>
                        Email:
                        <input
                            name="email"
                            type="email"
                            required
                        />
                    </label>

                    <label>
                        Phone:
                        <input
                            name="phone"
                            type="tel"
                            required
                        />
                    </label>
                    <label>
                        Description:
                        <input
                            name="description"
                            type="text"
                            required
                        />
                    </label>
                    <label>
                        Domain:
                        <input
                            name="domain"
                            type="url"
                            required
                        />
                    </label>
                </div>
                <button className="middle-button" type="submit">Save</button>
            </form>
            <section className="content-box">
            </section>
        </main>
    );
}
export function SuperAdminEditCompanyView() {
    const [company, setCompany] = useState(null);
    const { id } = useParams();

    useEffect(() => {
        fetch("/api/companies/" + id).then(response => response.json()).then(data => { setCompany(data) })
    },[])

    function updateCompany(e) {
        e.preventDefault();
        const form = e.target;

        let formData = new FormData(form);
        let dataObject = Object.fromEntries(formData);
        let dataJson = JSON.stringify(dataObject);
        fetch(form.action, {
            headers: { "Content-Type": "application/json" },
            method: "PUT",
            body: dataJson
        }).then(response => {
            if (response.ok) {
                alert(`Du updaterade ${dataObject.name}'s information 🎉`);
            } else {
                alert("Något gick fel ❌");
            }
        })
    }

    return (
        <main>
            <NavigationBar back={"/companies"}/>
            <form className="data-form" onSubmit={updateCompany} action={`/api/companies/${id}`} method="PUT">
                <div className="form-box">
                    <label>
                        Name:
                        <input
                            name="name"
                            defaultValue={company?.name}
                            type="text"
                            required
                        />
                    </label>

                    <label>
                        Email:
                        <input
                            name="email"
                            defaultValue={company?.email}
                            type="email"
                            required
                        />
                    </label>

                    <label>
                        Phone:
                        <input
                            name="phone"
                            defaultValue={company?.phone}
                            type="tel"
                            required
                        />
                    </label>

                    <label>
                        Description:
                        <input
                            name="description"
                            defaultValue={company?.description}
                            type="text"
                            required
                        />
                    </label>
                    <label>
                        Domain:
                        <input
                            name="domain"
                            defaultValue={company?.domain}
                            type="url"
                            required
                        />
                    </label>
                </div>
                <input type="submit" value="Save" className="middle-button" />
            </form>
        </main>
    );
}


export function SuperAdminAdminView() {
    const [admins, setAdmins] = useState([]);
    const [InactiveAdmins, setInactiveAdmins] = useState([]);
    const [showInactive, setShowInactive] = useState(false);

    function GetAdmins() {
        fetch("/api/roles/users/admin/?active=true").then(response =>
            response.json())
            .then(data => setAdmins(data));
    }
    function GetInactiveAdmins() {
        fetch("/api/roles/users/admin/?active=false").then(response =>
            response.json())
            .then(data => setInactiveAdmins(data));
    }



    function BlockAdminById(id, active) {
        fetch(`/api/users/block/${id}/${active}`, {
            headers: { "Content-Type": "application/json" },
            method: "PUT",
            body: JSON.stringify(id, active),
        })
            .then(response => {
                if (response.ok) { console.log("Det Funkade Igen"), GetAdmins(), GetInactiveAdmins() }
            })
    }

    useEffect(GetAdmins, []);
    useEffect(GetInactiveAdmins, []);

    return <main className="role-specific-main">
        <NavigationBar back={"/super-admin"}/>
        <section className="header-section"><h1>All Admins</h1></section>
        <div className="list-content-box">
        <ul className="list">
            {admins.map(AdminCard)}
        </ul>
        <section className="statusBox">
                    <button onClick={() => setShowInactive(prevState => !prevState)} className="toggle-inactive-btn">
                        {showInactive ? "Hide Inactive Admins" : "Show Inactive Admins"}
                    </button>
                    {showInactive && (
                        <ul className="inactive-list">
                            {InactiveAdmins.map(InactiveAdminCard)}
                        </ul>
                    )}
            </section>
        </div>
        <section className="content-box">
            <NavLink to="/admins/add"><button className="middle-button">Add Admin</button></NavLink>
        </section>
    </main>;


    function AdminCard(admin) {

        return <li className="list-item" key={admin.id}>
            <div className="card-info">
                <p><strong>Name:</strong><br />{admin.name}</p><br />
                <p><strong>Email:</strong><br />{admin.email}</p><br />
                <p><strong>Company:</strong><br />{admin.company}</p><br />
            </div>
            <div className="card-buttons">
                <NavLink to={"/users/" + admin.id + "/edit"}><button>Edit</button></NavLink>
                <button className="small-button" onClick={() => BlockAdminById(admin.id, admin.active)}>block</button>
            </div></li>
    }
    function InactiveAdminCard(admin) {
        return <li className="inactive-list-item" key={admin.id}>
            <p>{admin.email}</p><button onClick={() => BlockAdminById(admin.id, admin.active)}>Activate</button>
        </li>
    }
}
export function SuperAdminAddAdminView() {
    const [companies, setCompanies] = useState([]);

    useEffect(() => {
        fetch("/api/companies?active=true")
            .then(response => response.json())
            .then(data => setCompanies(data))
            .catch(error => console.error("Error fetching companies:", error));
    }, []);

    function postUser(e) {
        e.preventDefault();
        const form = e.target;
        let formData = new FormData(form);
        let dataObject = Object.fromEntries(formData);
        dataObject.role = "admin";

        let dataJson = JSON.stringify(dataObject);

        fetch(form.action, {
            headers: { "Content-Type": "application/json" },
            method: form.method,
            body: dataJson
        }).then(response => {
            if (response.ok) {
                alert(`Du lade till ${dataObject.name} i databasen 🎉`);
            } else {
                alert("Något gick fel ❌");
            }
        });
    }

    return (
        <main>
            <NavigationBar back={"/admins"}/>
            <form className="data-form" onSubmit={postUser} action="/api/users" method="POST">
                <div className="form-box">
                    <label>
                        Name:
                        <input
                            name="name"
                            type="text"
                            required
                        />
                    </label>

                    <label>
                        Email:
                        <input
                            name="email"
                            type="email"
                            required
                        />
                    </label>
                    <label>
                        Company:
                        <select
                            name="company"
                            required
                        >
                            <option value="" disabled hidden>Välj ett företag</option>
                            {companies.map(company => (
                                <option key={company.id} value={company.id}>{company.name}</option>
                            ))}
                        </select>
                    </label>
                </div>
                <input type="submit" className="middle-button" value="Save"></input>
            </form>
        </main>
    );
}


export function SuperAdminEditAdminView() {
    const { id } = useParams()
    const [admin, setAdmin] = useState(null);
    const [disabled, setDisabled] = useState(false);
  
    function ResetPassword(e)
    {
     e.preventDefault();
    setDisabled(true);
    setTimeout(() => {
        setDisabled(false);
    }, 2000)
    
        fetch("/api/users/password/" + id, {
            headers: { "Content-Type": "application/json" },
            method: "PUT",
            body: JSON.stringify({})
        })
            .then(response => {
                if (response.ok) {
                    alert("Password has been reset")
                } else {
                    alert("An error occured when reseting the password.")
                }
            }
            )
    }


    useEffect(() => {
        fetch("/api/users/" + id)
            .then(response => response.json())
            .then(data => { setAdmin(data) })
            .catch(error => console.error("Error fetching user:", error));

    },[])


    function updateUser(e) {
        e.preventDefault();
        const form = e.target;

        let formData = new FormData(form);
        let dataObject = Object.fromEntries(formData);
        dataObject.role = 3;
        let dataJson = JSON.stringify(dataObject);
        fetch(form.action, {
            headers: { "Content-Type": "application/json" },
            method: "PUT",
            body: dataJson
        }).then(response => {
            if (response.ok) {
                alert(`Du updaterade ${dataObject.name}'s information 🎉`);
            } else {
                alert("Något gick fel ❌");
            }
        })
    }

    return (
        <main>
            <NavigationBar back={"/admins"}/>
            <form className="data-form" onSubmit={updateUser} action={`/api/users/${id}`} method="PUT">
                <div className="form-box">
                    <label>
                        Name:
                        <input
                            name="name"
                            defaultValue={admin?.name}
                            type="text"
                            required
                        />
                    </label>

                    <label>
                        Email:
                        <input
                            name="email"
                            defaultValue={admin?.email}
                            type="email"
                            required
                        />
                    </label>
                </div>
                <input type="submit" value="Save" className="middle-button"></input>  <button disabled={disabled} className="middle-button reset-button" onClick={ResetPassword} >Reset Password</button>
            </form>
        </main>
    );
}
