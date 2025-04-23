import { useEffect, useState } from "react";
import { NavLink } from "react-router";
import logo from '../assets/logo.png';
import "../styles.css"
export default function CustomerTicketView() {

    
    const [productPick, setProductPick] = useState("");
    const [categoryPick, setCategoryPick] = useState("");
    const [message, setMessage] = useState("");   
    const [products, setProducts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [email, setEmail] = useState("");
    const [description, setDescription] = useState("");
   


    const urlParams = new URLSearchParams(window.location.search);
    const companyId = urlParams.get('companyId');
   
    useEffect(() => {
        if (companyId) {
            fetch(`/api/products/customer-ticket?companyId=${companyId}`)
                .then((response) => response.json())
                .then((data) => setProducts(data))
                .catch((error) => console.error("Error fetching products:", error));
        }
    }, []);
    
      
    useEffect(() => {
        
        fetch(`/api/tickets/categories?companyId=${companyId}`)
            .then((response) => response.json())
            .then((data) => setCategories(data))
            .catch((error) => console.error("Error fetching categories:", error));
    }, []);

       
    function handleOnSubmit(e) {
        e.preventDefault();
        const formData = {
            productId: productPick,
            categoryId: categoryPick,
            message: message,
            email: email,
            description: description
        };

        fetch("/api/tickets", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(formData),
            }).then(response=>{
                if (response.ok) {
                    alert(`Du lade till en ticket`);
                } else {
                    alert("Något gick fel ");
                }
            })
          
    };

    return (
        <>
            <main className="ticket-main">
                <form className="ticket-form" onSubmit={handleOnSubmit}>
                    <div className="mobile-ticket-field">
                        <select
                            name="product"
                            disabled={!setEmail}
                            value={productPick}
                            onChange={(e) => setProductPick(e.target.value)}
                            className="ticket-product-select">
                            <option value="">Select Product</option>
                            {products.map((product) => (
                                <option key={product.id} value={product.id} className="ticket-product-option">
                                    {product.name}
                                </option>
                            ))}
                        </select>
                    </div>
                    <div className="mobile-ticket-field">
                        <select
                            name="category"
                            disabled={!productPick}
                            value={categoryPick}
                            onChange={(e) => setCategoryPick(Number(e.target.value))}
                            className="ticket-category-select">
                            <option value="">Select Category</option>
                            {categories.map((category) => (
                                <option key={category.id} value={category.id} className="ticket-category-option">
                                    {category.category_name}
                                </option>
                            ))}
                        </select>
                    </div>
                    <div className="mobile-ticket-field">
                        <input
                            disabled={!categoryPick}
                            type="email"
                            name="email"
                            value={email}
                            onChange={e => setEmail(e.target.value)}
                            placeholder="Enter your email.."
                            className="ticket-input"
                            
                    />
                    </div>
                    <div className="mobile-ticket-field">
                        <input
                            disabled={email.trim() === ""}
                            type="text"
                            name="description"
                            value={description}
                            onChange={e=> setDescription(e.target.value)}
                            placeholder="Enter title.."
                            className="ticket-input"
                            
                        />
                    </div>
                    <div className="mobile-ticket-field">
                            <textarea
                                disabled={description.trim() === ""}
                                name="postTicket"
                                rows={10}
                                cols={25}
                            value={message}
                            placeholder="Enter message.."
                                onChange={(e) => setMessage(e.target.value)}
                                className="ticket-message-textarea"
                                required
                            />
                    </div>

                        <input disabled={!message} type="submit" className="ticket-submit-button" value='Create Ticket'/>
                    </form>
            </main>
        </>

    );
}
