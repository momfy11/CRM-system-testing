using Npgsql;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;


public class MockHasher
{


    public static async Task<IResult> HashMockPasswords(NpgsqlDataSource db, PasswordHasher<string> hasher)
    {

     
        List<string> passwordList = new();


        try
        {
            using var cmd = db.CreateCommand("SELECT password from users ORDER BY id");
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                passwordList.Add(new string(reader.GetString(0)));
            }
            if (passwordList.Count > 0)
            {
                Console.WriteLine(passwordList.Count + " amount of password loaded");
            }
            else
            {
                Console.WriteLine("No passwords in list");
            }
            if (passwordList[0] == "password123")
            {

                using var conn = await db.OpenConnectionAsync();
                using var transaction = await conn.BeginTransactionAsync();


                for (int i = 0; i < passwordList.Count; i++)
                {

                    using var cmd2 = db.CreateCommand("UPDATE users SET password = $1 WHERE id = $2");
                    cmd2.Parameters.AddWithValue(hasher.HashPassword("", passwordList[i]));
                    cmd2.Parameters.AddWithValue(i + 1);

                    int? rowsAffected = await cmd2.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine("No rows affected");
                        return TypedResults.BadRequest("Njiet!");
                    }
                }
                await transaction.CommitAsync();
                return Results.Ok();
            }
            else
            {
                return Results.BadRequest("Lösenorden är redan hashade!");
            }
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }



// deprecated.
    public static async Task<IResult> ResetMockPasswords(NpgsqlDataSource db)
    {
        List<string> originalPasswords =new(){
        "password123", 
        "password123", 
        "password123", 
        "password123",
        "auwodh",
        "aiwdj",  
        "hejhej",
        "password123", 
        "nalle",
         "styre", 
        "nuff",  
        "asd123", 
        "aswe", 
        "best", 
        "kung",
        "123"
        };
           using var conn = await db.OpenConnectionAsync();
                using var transaction = await conn.BeginTransactionAsync();


        for (int i = 0; i < originalPasswords.Count; i++)
        {

            using var cmd2 = db.CreateCommand("UPDATE users SET password = $1 WHERE id = $2");
            cmd2.Parameters.AddWithValue(originalPasswords[i]);
            cmd2.Parameters.AddWithValue(i + 1);

            int? rowsAffected = await cmd2.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("No rows affected");
                return TypedResults.BadRequest("Njiet!");
            }
        }
        await transaction.CommitAsync();
        return Results.Ok(originalPasswords.Count);

    }



}


