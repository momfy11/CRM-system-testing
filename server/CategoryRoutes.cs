using Npgsql;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data.Common;
using System.ComponentModel;
namespace server;

public class CategoryRoutes
{

    public static async Task<Results<Ok<List<int>>, BadRequest<string>>> GetCategoriesByUserId(NpgsqlDataSource db, int id)
    {
        List<int> usercategories = new();

        try
        {

            using var cmd = db.CreateCommand("SELECT ticket_category from customer_agentsxticket_category where customer_agent = $1 ");
            cmd.Parameters.AddWithValue(id);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                usercategories.Add(reader.GetInt32(0));
            }
            return TypedResults.Ok(usercategories);

        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest("nej de gick inte"+ex.Message);
        }

    }

    public record Category(int id, string category_name, bool active);

    public static async Task<Results<Ok<List<Category>>, BadRequest<string>>> GetCategoriesByCompany(NpgsqlDataSource db, HttpContext ctx, bool active)
    {

        List<Category> categorylist = new();

        if (ctx.Session.IsAvailable && ctx.Session.GetInt32("role") is int roleInt && Enum.IsDefined(typeof(UserRole), roleInt) && (UserRole)roleInt == UserRole.Admin)
        {
            try
            {

                using var cmd = db.CreateCommand("SELECT id, category_name,active FROM ticket_categories WHERE company = $1 AND active=$2");

                int? companyId = ctx.Session.GetInt32("company");

                if (!companyId.HasValue)
                {
                    return TypedResults.BadRequest("Session does not exist or you are not an admin");
                }

                cmd.Parameters.AddWithValue(companyId.Value);
                cmd.Parameters.AddWithValue(active);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    categorylist.Add(new Category(reader.GetInt32(0),
                                                  reader.GetString(1),
                                                  reader.GetBoolean(2)));
                }
                return TypedResults.Ok(categorylist);




            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }
        return TypedResults.BadRequest("No session available");


    }

    public record CategoryDTO(string CategoryName);
    public static async Task<Results<Ok<string>, BadRequest<string>>> AddCategory(NpgsqlDataSource db, HttpContext ctx, CategoryDTO categoryNameDTO)
    {
        if (ctx.Session.IsAvailable && ctx.Session.GetInt32("role") is int roleInt && Enum.IsDefined(typeof(UserRole), roleInt) && (UserRole)roleInt == UserRole.Admin)
        {
            try
            {

                using var cmd = db.CreateCommand("INSERT INTO ticket_categories (category_name, company) VALUES ($1, $2)");

                int? companyId = ctx.Session.GetInt32("company");

                if (!companyId.HasValue)
                {
                    return TypedResults.BadRequest("Session does not exist or you are not an admin");
                }

                cmd.Parameters.AddWithValue(categoryNameDTO.CategoryName);
                cmd.Parameters.AddWithValue(companyId.Value);


                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    return TypedResults.Ok("Allt gick bra");
                }
                else
                {
                    return TypedResults.BadRequest("Inget gick bra");
                }
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }
        return TypedResults.BadRequest("No session available");
    }

    public record StatusDTO(int id, bool active);
    public static async Task<Results<Ok<string>, BadRequest<string>>> ChangeStatus(NpgsqlDataSource db, HttpContext ctx, StatusDTO status)
    {
        if (ctx.Session.IsAvailable && ctx.Session.GetInt32("role") is int roleInt && Enum.IsDefined(typeof(UserRole), roleInt) && (UserRole)roleInt == UserRole.Admin)
        {
            try
            {

                using var cmd = db.CreateCommand("UPDATE ticket_categories SET active = $1 WHERE id = $2");

                cmd.Parameters.AddWithValue(status.active ? false : true);
                cmd.Parameters.AddWithValue(status.id);


                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    return TypedResults.Ok("Allt gick bra");
                }
                else
                {
                    return TypedResults.BadRequest("Inget gick bra");
                }
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }
        return TypedResults.BadRequest("No session available");
    }
}



