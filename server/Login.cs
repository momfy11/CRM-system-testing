using Npgsql;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using server;
using System.Collections;
using Org.BouncyCastle.Asn1;

namespace server;

public static class LoginRoutes
{
    public record Credentials(string Email, string Password);
    public record LoginDTO(int Id, string Name, UserRole Role, int Company, string Password);

    public static async Task<Results<Ok<string>, BadRequest, BadRequest<string>>> LoginByRole(Credentials credentials, NpgsqlDataSource db, HttpContext ctx, PasswordHasher<string> hasher)
    {

        var cmd = db.CreateCommand("select id, name, role, company,password from users where email = $1 and active=true");
        cmd.Parameters.AddWithValue(credentials.Email);


        using var reader = await cmd.ExecuteReaderAsync();
        LoginDTO accountDetails;
        if (await reader.ReadAsync())
        {
            accountDetails = new(reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetFieldValue<UserRole>(2),
                                reader.GetInt32(3),
                                reader.GetString(4));

        }
        else
        {
            return TypedResults.BadRequest("Invalid credentials");
        }


        // hashed password= accountDetails.Password 
        // from frontend crendentials.Password 

        var verificationResult = hasher.VerifyHashedPassword("", accountDetails.Password, credentials.Password);

        //PasswordVerificationResult 0,1,2 failed ,success ,succesRehashNeeded
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return TypedResults.BadRequest("Authentication failed");

        }
        else if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            string newHash = hasher.HashPassword("", credentials.Password);
            using var cmdNewHash = db.CreateCommand("Update users set password=$1 where id=$2 and email=$3");
            cmdNewHash.Parameters.AddWithValue(newHash);
            cmdNewHash.Parameters.AddWithValue(accountDetails.Id);
            cmdNewHash.Parameters.AddWithValue(credentials.Email);
        }


        //Succes & successRehash 
        ctx.Session.SetInt32("id", accountDetails.Id);
        ctx.Session.SetString("name", accountDetails.Name);
        ctx.Session.SetInt32("role", (int)accountDetails.Role);
        ctx.Session.SetInt32("company", accountDetails.Company);

        string location = "";

        switch (accountDetails.Role)
        {
            case UserRole.Admin:
                {
                    location = "/admin";
                    break;
                }
            case UserRole.super_admin:
                {
                    location = "/super-admin";
                    break;
                }
            case UserRole.Service_agent:
                {
                    location = "/customer-service";
                    break;
                }

        }
        return TypedResults.Ok(location);

    }

    public static Results<Ok<string>, BadRequest, BadRequest<string>> LogOut(HttpContext ctx)
    {
        ctx.Session.Clear();
        if (ctx.Session.GetInt32("id").HasValue)
        {
            return TypedResults.BadRequest("Du får inte logga ut");
        }
        else
        {
            return TypedResults.Ok("Du har loggat ut");
        }
    }


}

