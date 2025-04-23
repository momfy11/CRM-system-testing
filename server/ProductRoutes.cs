using Npgsql;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data.Common;
namespace server;


public class ProductRoutes()
{




    public record Product(int id, string Name, string Description, int Price, string Category, int Company, bool active);

    public static async Task<Results<Ok<List<Product>>, BadRequest<string>>> GetProducts(bool active, HttpContext ctx, NpgsqlDataSource db)
    {
        List<Product> products = new();
        var company_nullable = ctx.Session.GetInt32("company");

        if (!company_nullable.HasValue)
        {
            return TypedResults.BadRequest("Session error accessing Session viables");
        }
        int company = company_nullable.Value;
        try
        {
            using var cmd = db.CreateCommand("SELECT id,product_name,product_description,price,product_category,company,active FROM products WHERE company = $1 AND active = $2  ORDER BY id ASC");
            cmd.Parameters.AddWithValue(company);
            cmd.Parameters.AddWithValue(active);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new Product(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetInt32(3),
                    reader.GetString(4),
                    reader.GetInt32(5),
                    reader.GetBoolean(6)
                ));
            }

            return TypedResults.Ok(products);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Ett fel uppstod: {ex.Message}");
        }
    }


    public record ProductTicketInfo(int id, string Name);
    public static async Task<Results<Ok<List<ProductTicketInfo>>, BadRequest<string>>> GetProductsForTicket(NpgsqlDataSource db, int companyId)
    {
        List<ProductTicketInfo> products = new();

        try
        {
            using var cmd = db.CreateCommand("SELECT id,product_name FROM products WHERE company = $1  ORDER BY id ASC");
            cmd.Parameters.AddWithValue(companyId);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new ProductTicketInfo(
                    reader.GetInt32(0),
                    reader.GetString(1)
                ));
            }

            return TypedResults.Ok(products);
        }
        catch (Exception ex)
        {

            return TypedResults.BadRequest($"Ett fel uppstod: {ex.Message}");
        }
    }

    public static async Task<Results<Ok<string>, BadRequest<string>>> BlockProductById(int id, bool active, NpgsqlDataSource db)
    {
        try
        {

            using var cmd = db.CreateCommand("UPDATE products SET active = $1 WHERE id = $2");
            cmd.Parameters.AddWithValue(!active);
            cmd.Parameters.AddWithValue(id);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
            {
                return TypedResults.Ok("Du har blockat eller unblockat en produkt");
            }
            else
            {
                return TypedResults.BadRequest("Det funkade inte att blocka..");
            }
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Det blev fel. {ex.Message}");
        }
    }


    public static async Task<Results<Ok<Product>, BadRequest<string>>> GetProduct(int ProductId, NpgsqlDataSource db)
    {
        try
        {
            using var cmd = db.CreateCommand("SELECT id,product_name,product_description,price,product_category,company,active  FROM products WHERE id=$1");
            cmd.Parameters.AddWithValue(ProductId);


            using var reader = await cmd.ExecuteReaderAsync();

            // Deklarera admin innan if-satsen
            Product? product = null;

            if (await reader.ReadAsync())
            {
                product = new Product(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetInt32(3),
                    reader.GetString(4),
                    reader.GetInt32(5),
                    reader.GetBoolean(6)

                );

                return TypedResults.Ok(product);
            }

            return TypedResults.BadRequest("Ingen produkt med det givna namnet kunde hittas");
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Ett fel uppstod: {ex.Message}");
        }
    }






    public record PostProductDTO(string Name, string Description, int Price, string Category, int Company);

    public static async Task<IResult> AddProduct(PostProductDTO product, HttpContext ctx, NpgsqlDataSource db)
    {
        try
        {
            var companyId_nullable = ctx.Session.GetInt32("company");
            if (!companyId_nullable.HasValue)
            {
                return TypedResults.BadRequest("Error when accessing Session variables");
            }
            int companyId = companyId_nullable.Value;

            using var cmd = db.CreateCommand(
                "INSERT INTO products (product_name, product_description, price, product_category, company) VALUES ($1, $2, $3, $4, $5) RETURNING id");

            cmd.Parameters.AddWithValue(product.Name);
            cmd.Parameters.AddWithValue(product.Description);
            cmd.Parameters.AddWithValue(product.Price);
            cmd.Parameters.AddWithValue(product.Category);
            cmd.Parameters.AddWithValue(companyId);

            var result = await cmd.ExecuteScalarAsync();

            if (result != null)
            {
                return TypedResults.Ok("Det funkade! Du la till en product!");
            }
            else
            {
                return TypedResults.BadRequest("Ajsing bajsing, det funkade ej att lägga till produkten");
            }
        }
        catch (PostgresException ex) when (ex.SqlState == "23505") // Hanterar unikhetsfel
        {
            return TypedResults.BadRequest("produkten är redan registrerad!");
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Ett fel inträffade: {ex.Message}");
        }
    }



    public record PutProductDTO(string Name, string Description, int Price, string Category, int Company, int id);
    public static async Task<IResult> EditProduct(PutProductDTO product, NpgsqlDataSource db)
    {
        try
        {
            using var cmd = db.CreateCommand(
                "UPDATE products SET product_name=$1, product_description=$2, price=$3, product_category=$4 WHERE company=$5 AND id=$6");


            cmd.Parameters.AddWithValue(product.Name);
            cmd.Parameters.AddWithValue(product.Description);
            cmd.Parameters.AddWithValue(product.Price);
            cmd.Parameters.AddWithValue(product.Category);
            cmd.Parameters.AddWithValue(product.Company);
            cmd.Parameters.AddWithValue(product.id);
            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return TypedResults.NotFound("Product kunde inte hittas");
            }

            return TypedResults.Ok("Produkt har uppdateras");
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"ett fel har inträffat: {ex.Message}");
        }
    }

}
