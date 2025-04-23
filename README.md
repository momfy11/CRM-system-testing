# SwineSync - Testning av CRM-system

Detta projekt är en del av kursen Testning. Systemet är ett CRM-system byggt i tidigare kursmoment, där målet nu är att säkerställa funktionalitet och kvalitet via automatiserad testning.

## 📁 Projektstruktur

```
SwineSync/
├── server/                     # Backend (.NET)
├── client/                     # Frontend (React)
├── SwineSync.Tests/            # Enhetstester (xUnit)
├── SwineSync.GuiTests/         # GUI-tester (Playwright)
├── api-test/                   # API-tester (Postman-kollektion)
├── .github/workflows/ci.yml    # GitHub Actions CI/CD
└── README.md
```

## 🧪 Testning

Testningen är uppdelad i tre nivåer:

| Nivå      | Verktyg            | Vad testas                                 |
|-----------|--------------------|---------------------------------------------|
| Unit      | xUnit              | Inloggning, lösenord, kontrollstrukturer     |
| API       | Postman + Newman   | Endpoints för företag, användare, tickets    |
| GUI       | Playwright         | Fullständiga användarflöden i gränssnittet   |

## 🧱 Testkommandon

- **Enhetstester**: `dotnet test SwineSync.Tests`
- **GUI-tester**: `dotnet test SwineSync.GuiTests`
- **API-tester**: `newman run ./api-test/SwineSyncAPITests.postman_collection.json`

## 🚀 CI/CD

Via GitHub Actions:
- Bygger hela lösningen
- Kör enhetstester, API-tester och GUI-tester
- Inget deploymentsteg i denna version

Se `.github/workflows/ci.yml`

## 🔐 Säkerhetsbrister

- 🔓 Inga sessionskontroller på `Company`-endpoints
- 🔓 Tickets kan skapas via API utan inloggning (ska kräva customer-agent)
- 🔓 Customers ska endast kunna *stänga* tickets – men kan även öppna
- 🔓 GUI och API saknar rollback/reset för testdata

## ✅ Klarlagda tester

| Nivå  | Typ                  | Exempel |
|-------|-----------------------|---------|
| Unit  | Lösenord, inloggning | PasswordHasher, Rollhantering |
| API   | Tickets, Användare   | Skapa, blockera, redigera     |
| GUI   | Login, flöden        | SuperAdmin/Admin/Agent flows  |

## 🧾 Rapporter

| Dokument         | Länk                         |
|------------------|------------------------------|

- ✅ [**Förstudierapport**](https://thorengruppen-my.sharepoint.com/:w:/g/personal/srecko_radivojevic_student_nbi-handelsakademin_se/ESrxXiEg50tAvhujmoHRcx8Btczqr2vBAeF3L1JhqFuQbg?e=vjcO42)
 
- ✅  [**Slutrapport**](https://thorengruppen-my.sharepoint.com/:w:/g/personal/srecko_radivojevic_student_nbi-handelsakademin_se/EcZKJ2IFA8BEsMZi2ziEv8cBJvM1Zr2lkTZuwvqtAoYrmg?e=o4M8FD) 
- ✅ [**Testfall & loggar (Excel)**](https://thorengruppen-my.sharepoint.com/:x:/g/personal/srecko_radivojevic_student_nbi-handelsakademin_se/EYdu7uRyQYlPqnNhndwGKHYBRMMh1vl094p8MLcQOWTgyA?e=CqIK3w)

## 📎 Tips

- GUI-tester använder `Guid.NewGuid()` för unika e-postadresser och namn
- Customer-vy kräver mobil-vy för att visas korrekt
- Sessions hanteras automatiskt i riktiga flöden – vissa API saknar detta

---

# SwineSync - CRM-system Live

## Instructions

### Set-up:

1. **Download Project**

   - Build PostgreSQL database with queries in `/server/databasefiles/swine_sync.txt`
   - Add mock data from the queries in `/server/databasefiles/mockdata.txt`

2. **Navigate to the server folder**

   ```sh
   cd /server/
   ```

3. **Update Database Password**

   - Open `Program.cs`
   - Change the database password to your own or use an environment variable.

4. **Build and Run the Server**

   ```sh
   dotnet build
   dotnet run
   ```

5. **Hash Passwords**

   - Open HTTPie or Postman
   - Make a POST request to:
     ```
     http://localhost:5000/api/mockhash
     ```

6. **Navigate to the Client Folder**

   ```sh
   cd /client
   ```

7. **Install Dependencies**

   ```sh
   npm install
   ```

8. **Run the Client**

   ```sh
   npm run dev
   ```

9. **Access the Application**
   - Click the local link: [http://localhost:5173/](http://localhost:5173/)
   - Use the following credentials:
     - **Super-admin:** `super_gris@mail.com` / `kung`
     - **Admin:** `grune@grymt.se` / `hejhej`
     - **Customer Agent:** `tryne@hotmail.com` / `asd123`

### Creating a Ticket:

1. Navigate to [http://localhost:5173/tech-solutions](http://localhost:5173/tech-solutions)
2. Create a ticket for the Company Tech-Solution.
3. Adjust window to mobile view size.
4. After submitting a ticket with your email, you will receive an email with a link to chat with the company's customer agent.

---

## API Documentation

### API Root Path:

- `http://localhost:5000/api`

---

### **Companies**

#### **GET**

- **Route:** `/companies`

  - Retrieves data for all active or soft-deleted companies using Swine Sync CRM.
  - **Query Parameters:** `active` (bool) – Indicates whether the company is active.
  - **Response:**
    - `200 OK`: List of companies.
    - `400 BadRequest`: Error message.

- **Route:** `/companies/{id}`
  - Retrieves data for a specific company by ID.
  - **Path Parameter:** `id` (int) – Company ID.
  - **Response:** - `200 OK`: `list<company>
record company` (int id, string name, string email, string phone, string description, string domain, bool active) - `400 BadRequest`: Error message.

#### **PUT**

- **Route:** `/companies/{id}`

  - Updates a company's details.
  - **Path Parameter:** `id` (int) – Company ID.
  - **Body Parameters:**
    - `name` (string)
    - `email` (string)
    - `phone` (string)
    - `description` (string)
    - `domain` (string)
  - **Response:**
    - `200 OK`: Success message.
    - `400 BadRequest`: Error message.

- **Route:** `/companies/block/{id}/{active}`
  - Changes the active status of a company (block/unblock).
  - **Path Parameters:**
    - `id` (int) – Company ID.
    - `active` (bool) – New status.
  - **Response:**
    - `200 OK`: Success message.
    - `400 BadRequest`: Error message.

#### **POST**

- **Route:** `/companies`
  - Adds a company to the Swine Sync database.
  - **Body Parameters:**
    - `name` (string)
    - `email` (string)
    - `phone` (string)
    - `description` (string)
    - `domain` (string)
  - **Response:**
    - `200 OK`: Success message.
    - `400 BadRequest`: Error message.

---

### **Users**

#### **GET**

- **Route:** `/users/company/{role}`

  - Retrieves all users by role in a specific company.
  - **Path Parameter:** `role` (string) – User role.
  - **Query Parameters:** `active` (bool) – Active status.
  - **Response:** JSON object containing all user details,
    - `GetUsersFromCompany` (TypedResults.Ok(users)),
      BadRequest(string),
      BadRequest(string),
      BadRequest(Error message);

- **Route:** `/users/{id}`
  - Retrieves a specific user by ID.
  - **Path Parameter:** `id` (int) – User ID.
  - **Response:** JSON object containing the users details,
    - `GetUser`(TypedResults.Ok(user))
      BadRequest(string);
      BadRequest(Error message);

#### **PUT**

- **Route:** `/users/{id}`
  - Updates user details.
  - **Path Parameter:** `id` (int) – User ID.
  - **Response:**
    - `EditAdmin` TypedResults.Ok("User updaterades"),
      TypedResults.NotFound(string),
      TypedResults.BadRequest(string),
      TypedResults.BadRequest(error message)

---

### **Categories**

#### **GET**

- **Route:** `/categories/{id}`

  - Retrieves all categories for the company of the user.

  - **Path Parameter:**
    - `id` (type:int32) – User ID.
  - **Response:**

    - `List<Category>`

- **Route:** `/categories/company/`
  - Retrieves categories for the logged-in admin’s company.
  - **Query Parameter:**
    - `active` (type:bool)
  - **Response:**
    - `List<Category>`

#### **PUT**

- **Route:** `/api/categories/status/`
  - Toggles the active status of a category.
  - **Query Parameters:**
    - `id` (type:int32)
    - `active` (type:bool)
  - **Response:**
    - `StatusCode, string`

#### **POST**

- **Route:** `/api/categories/`
  - Adds a new category.
  - **Body Parameter:**
    - `name` (string)
  - **Response:**
    - `StatusCode, string`

---

### **Products**

#### **GET**

- **Route:** `/products/company/`

  - Retrieves active products for a company.

  - **Query Parameter:**
    - `active` (bool)
  - **Response:**

    - `GetProducts` Task(Results(Ok(List(Product)), BadRequest(string)))

- **Route:** `/products/{ProductId}`
  - Retrieves a specific product by ID.
  - **Path Parameter:**
    - `ProductId` (int)
  - **Response:**
    - `GetProduct` Task(Results(Ok(Product), BadRequest(string)))

#### **POST**

- **Route:** `/products`
  - Adds a new product.
  - **Body Parameter:**
    - `PostProductDTO` (string, string, int, string, int)
  - **Response:**
    - `AddProduct` Task(IResult)

#### **PUT**

- **Route:** `/products`

  - Updates product data.
  - **Query Parameter:**
    - `PutProductDTO` (string, string, int, string, int, int)
  - **Response:**
    - `EditProduct` Task(IResult)

- **Route:** `/api/products/block/{id}/{active}`
  - Updates a product’s active status.
  - **Path Parameter:**
    - `id` (int) `active` (bool)
  - **Response:** - `BlockProductById` Task(IResult)

---

### **Tickets**

#### **GET**

- **Route:** `/tickets/{slug}`

  - Retrieves data for a specific ticket.
  - **Path Parameter:**
    - `slug` (string)
  - **Response:**
    - OK `GetTicket` (int id, int status, string customer_email, int product_id, int ticket_category, decimal? rating, string slug)
    - Error :BadRequest (string)

#### **PUT**

- **Route:** `/tickets`
  - Assigns a random ticket to a customer agent.
  - **Response:**
    - OK (string)
    - Error :BadRequest (string)

#### **POST**

- **Route:** `/tickets`

  - Creates a ticket based on customer input.
  - **Body Parameters:**
    - `NewTicket`(int productId, int categoryId, string message, string email, string description);
  - **Response:**
    - OK (string)
    - Error:BadRequest (string)

---

### **Password Hashing**

#### **POST**

- **Route:** `/password/mockhash/`
  - Hashes passwords from mock data.
  - **Response:**
    - Success or error message.
