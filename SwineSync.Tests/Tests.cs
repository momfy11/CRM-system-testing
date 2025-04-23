using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Xunit;
using server;
using Npgsql;
using System.Threading.Tasks;

namespace SwineSync.Tests;

public class Tests
{
    [Fact]
    public async Task LoginReturnAdminCorrect()
    {
        var db = new NpgsqlDataSourceBuilder("Host=217.76.56.135;Database=swine_sync;Username=postgres;Password=_FrozenPresidentSmacks!;Port=5438")
            .EnableUnmappedTypes()
            .Build();

        var credentials = new LoginRoutes.Credentials("grune@grymt.se", "hejhej");

        var ctx = new DefaultHttpContext();
        ctx.Session = new TestSession();
        var result = await LoginRoutes.LoginByRole(credentials, db, ctx, new PasswordHasher<string>());

        var results = Assert.IsType<Results<Ok<string>, BadRequest, BadRequest<string>>>(result);

        switch (results.Result)
        {
            case Ok<string> ok:
                Assert.Equal("/admin", ok.Value);
                break;
            case BadRequest:
                Assert.Fail("Got BadRequest instead of Ok.");
                break;
            case BadRequest<string> badRequest:
                Assert.Fail($"Got BadRequest<string> with message: {badRequest.Value}");
                break;
            default:
                Assert.Fail("Unexpected result type");
                break;
        }
    }

    [Fact]
    public async Task LoginReturnSuperAdminCorrect()
    {
        var db = new NpgsqlDataSourceBuilder("Host=217.76.56.135;Database=swine_sync;Username=postgres;Password=_FrozenPresidentSmacks!;Port=5438")
            .EnableUnmappedTypes()
            .Build();

        var credentials = new LoginRoutes.Credentials("super_gris@mail.com", "kung");

        var ctx = new DefaultHttpContext();
        ctx.Session = new TestSession();
        var result = await LoginRoutes.LoginByRole(credentials, db, ctx, new PasswordHasher<string>());

        var results = Assert.IsType<Results<Ok<string>, BadRequest, BadRequest<string>>>(result);

        switch (results.Result)
        {
            case Ok<string> ok:
                Assert.Equal("/super-admin", ok.Value);
                break;
            case BadRequest:
                Assert.Fail("Got BadRequest instead of Ok.");
                break;
            case BadRequest<string> badRequest:
                Assert.Fail($"Got BadRequest<string> with message: {badRequest.Value}");
                break;
            default:
                Assert.Fail("Unexpected result type");
                break;
        }
    }

    [Fact]
    public async Task LoginReturnCustomerAgentCorrect()
    {
        var db = new NpgsqlDataSourceBuilder("Host=217.76.56.135;Database=swine_sync;Username=postgres;Password=_FrozenPresidentSmacks!;Port=5438")
            .EnableUnmappedTypes()
            .Build();

        var credentials = new LoginRoutes.Credentials("svine.kloven@example.com", "password123");

        var ctx = new DefaultHttpContext();
        ctx.Session = new TestSession();
        var result = await LoginRoutes.LoginByRole(credentials, db, ctx, new PasswordHasher<string>());

        var results = Assert.IsType<Results<Ok<string>, BadRequest, BadRequest<string>>>(result);

        switch (results.Result)
        {
            case Ok<string> ok:
                Assert.Equal("/customer-service", ok.Value);
                break;
            case BadRequest:
                Assert.Fail("Got BadRequest instead of Ok.");
                break;
            case BadRequest<string> badRequest:
                Assert.Fail($"Got BadRequest<string> with message: {badRequest.Value}");
                break;
            default:
                Assert.Fail("Unexpected result type");
                break;
        }
    }

    [Fact]
    public async Task LoginReturnUnauthorizedWhenPasswordWrong()
    {
        var db = new NpgsqlDataSourceBuilder("Host=217.76.56.135;Database=swine_sync;Username=postgres;Password=_FrozenPresidentSmacks!;Port=5438")
            .EnableUnmappedTypes()
            .Build();

        var credentials = new LoginRoutes.Credentials("tryne@gris.se", "felparol");

        var ctx = new DefaultHttpContext();
        ctx.Session = new TestSession();
        var result = await LoginRoutes.LoginByRole(credentials, db, ctx, new PasswordHasher<string>());

        var results = Assert.IsType<Results<Ok<string>, BadRequest, BadRequest<string>>>(result);

        if (results.Result is BadRequest<string> badRequest)
        {
            Assert.Equal("Invalid credentials", badRequest.Value);
        }
        else
        {
            Assert.Fail("Expected BadRequest<string>, but got something else");
        }
    }
    [Fact]
    public async Task CreateTicketReturnOk()
    {
        var db = new NpgsqlDataSourceBuilder("Host=217.76.56.135;Database=swine_sync;Username=postgres;Password=_FrozenPresidentSmacks!;Port=5438")
            .EnableUnmappedTypes()
            .Build();

        var suffix = Guid.NewGuid().ToString().Substring(0, 6);
        var ticket = new TicketRoutes.NewTicket(
            productId: 1,
            categoryId: 2,
            message: $"Den skriver ut i rosa! {suffix}",
            email: $"test_{suffix}@kund.se",
            description: "Min skrivare fungerar inte"
        );

        var result = await TicketRoutes.CreateTicket(ticket, db);

        var results = Assert.IsType<Results<Ok<string>, BadRequest<string>>>(result);
        var ok = Assert.IsType<Ok<string>>(results.Result);
        Assert.Contains("http://localhost:5173/customer/", ok.Value);
    }


    [Fact]
    public async Task AddMessageReturnOkWhenValidSlugAndText()
    {
        var db = new NpgsqlDataSourceBuilder("Host=217.76.56.135;Database=swine_sync;Username=postgres;Password=_FrozenPresidentSmacks!;Port=5438")
            .EnableUnmappedTypes()
            .Build();

        var message = new MessageRoutes.MessageDTO2(
            text: "It tastes poo.",
            slug: "aa", // 
            customer: false // false = kundtjänst, true = kund
        );

        var result = await MessageRoutes.AddMessage(message, db);

        var results = Assert.IsType<Results<Ok<string>, BadRequest<string>>>(result);
        var ok = Assert.IsType<Ok<string>>(results.Result);
        Assert.Equal("La till meddelandet", ok.Value);
    }

    [Fact]
    public async Task GetMessagesWhenValidSlug()
    {
        var db = new NpgsqlDataSourceBuilder("Host=217.76.56.135;Database=swine_sync;Username=postgres;Password=_FrozenPresidentSmacks!;Port=5438")
            .EnableUnmappedTypes()
            .Build();

        var result = await MessageRoutes.GetTicketMessages("aa", db);

        var results = Assert.IsType<Results<Ok<List<MessageRoutes.MessageDTO>>, BadRequest<string>>>(result);
        var ok = Assert.IsType<Ok<List<MessageRoutes.MessageDTO>>>(results.Result);

        Assert.NotEmpty(ok.Value);
    }

    [Fact]
    public async Task AddProductReturnOkWhenValidInput()
    {
        var db = new NpgsqlDataSourceBuilder("Host=217.76.56.135;Database=swine_sync;Username=postgres;Password=_FrozenPresidentSmacks!;Port=5438")
            .EnableUnmappedTypes()
            .Build();

        var suffix = Guid.NewGuid().ToString().Substring(0, 6);
        var ctx = new DefaultHttpContext();
        ctx.Session = new TestSession();
        ctx.Session.SetInt32("company", 3);

        var newProduct = new ProductRoutes.PostProductDTO(
            Name: $"TestÖl_{suffix}",
            Description: $"Inte för den klene {suffix}",
            Price: 99,
            Category: $"TestKategori_{suffix}",
            Company: -1
        );

        var result = await ProductRoutes.AddProduct(newProduct, ctx, db);

        if (result is BadRequest<string> bad)
        {
            Assert.Fail($"BadRequest returned: {bad.Value}");
        }

        var ok = Assert.IsType<Ok<string>>(result);
        Assert.Equal("Det funkade! Du la till en product!", ok.Value);
    }

    [Fact]
    public void PasswordHasherShouldVerifyCorrectPassword()
    {
        var hasher = new PasswordHasher<string>();
        var plain = "Test123!";
        var hash = hasher.HashPassword("", plain);

        var result = hasher.VerifyHashedPassword("", hash, plain);

        Assert.Equal(PasswordVerificationResult.Success, result);
    }

    [Fact]
    public void PasswordHasherFailVerificationOnWrongPassword()
    {
        var hasher = new PasswordHasher<string>();
        var hash = hasher.HashPassword("", "CorrectPassword");

        var result = hasher.VerifyHashedPassword("", hash, "WrongPassword");

        Assert.Equal(PasswordVerificationResult.Failed, result);
    }



}
