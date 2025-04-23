using Npgsql;
using server;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var dataSourceBuilder = new NpgsqlDataSourceBuilder("Host=217.76.56.135;Database=swine_sync;Username=postgres;Password=_FrozenPresidentSmacks!;Port=5438"); // add correct username and password for DB connection
dataSourceBuilder.MapEnum<UserRole>();
var db = dataSourceBuilder.Build();


builder.Services.AddSingleton<NpgsqlDataSource>(db);
builder.Services.AddSingleton<PasswordHasher<string>>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => { options.Cookie.IsEssential = true; });



var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.UseSession();


app.MapGet("/api/companies", CompanyRoutes.GetCompanies);
app.MapGet("/api/companies/{id}", CompanyRoutes.GetCompany);
app.MapPut("/api/companies/{id}", CompanyRoutes.EditCompany);
app.MapPut("/api/companies/block/{id}/{active}", CompanyRoutes.BlockCompany);
app.MapPost("/api/companies", CompanyRoutes.AddCompany);

app.MapGet("/api/roles/users/{role}", UserRoutes.GetUsers);

app.MapGet("/api/users/company/{role}", UserRoutes.GetUsersFromCompany);
app.MapGet("/api/users/{id}", UserRoutes.GetUser);
app.MapPut("/api/users/{id}", UserRoutes.EditAdmin);
app.MapPut("/api/users/block/{id}/{active}", UserRoutes.BlockUser); // url beskriver en plats inte en uppdatering i sig.  vid uppdatering ska ligga i bodyn. 
app.MapPut("/api/users/password/", UserRoutes.ChangePassword);
app.MapPut("/api/users/agent/{id}", UserRoutes.EditAgent);
app.MapPut("/api/users/password/{id}", UserRoutes.ResetPassword);
app.MapPost("/api/users", UserRoutes.AddAdmin);
app.MapPost("/api/users/agent", UserRoutes.AddAgent);

app.MapPost("/api/login", LoginRoutes.LoginByRole);
app.MapDelete("/api/login", LoginRoutes.LogOut);

app.MapGet("/api/categories/{id}", CategoryRoutes.GetCategoriesByUserId);
app.MapGet("/api/categories/company/", CategoryRoutes.GetCategoriesByCompany);
app.MapPut("/api/categories/status/", CategoryRoutes.ChangeStatus);
app.MapPost("/api/categories", CategoryRoutes.AddCategory);

app.MapGet("/api/products/company/", ProductRoutes.GetProducts);
app.MapGet("/api/products/{ProductId}", ProductRoutes.GetProduct);
app.MapGet("/api/products/customer-ticket/", ProductRoutes.GetProductsForTicket);
app.MapPut("/api/products", ProductRoutes.EditProduct);
app.MapPut("/api/products/block/{id}/{active}", ProductRoutes.BlockProductById);
app.MapPost("/api/products", ProductRoutes.AddProduct);

app.MapGet("/api/tickets/{slug}", TicketRoutes.GetTicket);
app.MapGet("/api/tickets/unassigned", TicketRoutes.GetUnassignedTickets);
app.MapGet("/api/tickets/assigned", TicketRoutes.GetAssignedTickets);
app.MapGet("/api/tickets/categories", CompanyRoutes.GetCategories);
app.MapGet("/api/tickets/closed", TicketRoutes.GetClosedTicketsByUserId);
app.MapPut("/api/tickets", TicketRoutes.AssignTicket);
app.MapPut("/api/tickets/status/{slug}", TicketRoutes.ChangeStatus);
app.MapPut("/api/tickets/rating/{slug}", TicketRoutes.TicketRating);
app.MapPost("/api/tickets", TicketRoutes.CreateTicket);

app.MapGet("/api/messages/{slug}", MessageRoutes.GetTicketMessages);
app.MapPost("/api/messages/", MessageRoutes.AddMessage);

//use after installation of mockdata. 
app.MapPost("/api/password/mockhash/", MockHasher.HashMockPasswords);
//app.MapPost("api/password/mockreset/", MockHasher.ResetMockPasswords); deprecated after more mockdata was created. 


app.Run();
