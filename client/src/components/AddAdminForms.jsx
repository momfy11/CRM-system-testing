
export default function AddAdminForms()

{
    return <>
        <form>
            <label>Name:
                <input type="text"
                    id="name"
                    placeholder="Enter name" />
             
            </label>
        
            <label>Email:
                <input type="text"
                    id="mail"
                placeholder="Enter email"/>

            </label>
    
            <label>Password:
                <input type="text"
                    id="password"
                    placeholder="Enter password" />
            </label>
        
        
            <label>Company:
                <input type="text"
                    id="company"
                    placeholder="Enter company " />
            </label>
        </form>

    
    </>

    
}