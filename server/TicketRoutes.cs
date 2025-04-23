using Npgsql;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text;
using System.Data.Common;
using Microsoft.VisualBasic;
namespace server;

public class TicketRoutes
{
    public record Ticket(int id, int status, string description, int product_id, int ticket_category, string slug);

    public record TicketRatingDTO(string slug, float rating);

    public record GetTicketDTO(int id, int status, string customer_email, int product_id, int ticket_category, decimal? rating, string slug);


    public static async Task<Results<Ok<GetTicketDTO>, BadRequest<string>>> GetTicket(string slug, NpgsqlDataSource db)
    {
        using var cmd = db.CreateCommand(@"
    SELECT 
        t.id, t.status, t.description, t.product_id, t.ticket_category, t.rating, t.slug FROM tickets AS t 
        WHERE slug = $1");

        cmd.Parameters.AddWithValue(slug);
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var ticket = new GetTicketDTO(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetString(2),
                        reader.GetInt32(3),
                        reader.GetInt32(4),
                        await reader.IsDBNullAsync(5) ? null : reader.GetDecimal(5),
                        reader.GetString(6)
                    );
            return TypedResults.Ok(ticket);
        }
        else return TypedResults.BadRequest("Hittade ingen ticket");
    }

    public static async Task<Results<Ok<List<Ticket>>, BadRequest<string>>> GetUnassignedTickets(NpgsqlDataSource db, HttpContext ctx)
    {
        List<Ticket> tickets = new List<Ticket>();

        try
        {
            if (ctx.Session.IsAvailable)
            {
                var id = ctx.Session.GetInt32("id");
                if (id == null)
                {
                    return TypedResults.BadRequest("Session not exisiting");
                }
                using var cmd = db.CreateCommand(@"
    SELECT 
        t.id,
        t.status,
        t.description,
        t.product_id,
        t.ticket_category,
        t.slug
    FROM 
        tickets AS t 
    JOIN 
        customer_agentsxticket_category AS catc 
        ON t.ticket_category = catc.ticket_category  
    WHERE 
        t.customer_agent IS NULL AND catc.customer_agent = $1");

                cmd.Parameters.AddWithValue(id);


                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var ticket = new Ticket(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetString(2),
                        reader.GetInt32(3),
                        reader.GetInt32(4),
                        reader.GetString(5)
                    );
                    tickets.Add(ticket);
                }
                return TypedResults.Ok(tickets);
            }
            else
            {
                return TypedResults.BadRequest($"Missing session");
            }
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Ett fel inträffade: {ex.Message}");
        }
    }


    public static async Task<Results<Ok<string>, BadRequest<string>>> AssignTicket(NpgsqlDataSource db, HttpContext ctx)
    {

        try
        {
            if (ctx.Session.IsAvailable && ctx.Session.GetInt32("role") is int roleInt && Enum.IsDefined(typeof(UserRole), roleInt) && (UserRole)roleInt == UserRole.Service_agent)
            {
                int id;
                var agent = ctx.Session.GetInt32("id");
                if (!agent.HasValue)
                {
                    return TypedResults.BadRequest("Session not available");
                }

                await using var conn = await db.OpenConnectionAsync();
                await using var transaction = await conn.BeginTransactionAsync();

                using var cmdRandomSelect =  new NpgsqlCommand(@"select id from tickets  join customer_agentsxticket_category as caXtc 
              on tickets.ticket_category = caXtc.ticket_category
              where caXtc.customer_agent=$1 and tickets.customer_agent is null 
              order by random() 
              limit 1;",conn,transaction);

                cmdRandomSelect.Parameters.AddWithValue(agent);
                var reader = await cmdRandomSelect.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    id = reader.GetInt32(0);
                }
                else
                {
                    transaction.Commit();
                    return TypedResults.BadRequest("Failed to select a ticket");
                }


                using var cmd =  new NpgsqlCommand("UPDATE tickets SET customer_agent = $2, status = 2 WHERE id = $1",conn,transaction);

                cmd.Parameters.AddWithValue(id);
                cmd.Parameters.AddWithValue(agent);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    await transaction.CommitAsync();
                    return TypedResults.Ok("Ticket assigned successfully.");
                }
                else
                {
                    await transaction.RollbackAsync();
                    return TypedResults.BadRequest("Ticket assignment failed. Ticket ID or customer agent might be invalid.");
                }
            }
            else
            {
                return TypedResults.BadRequest($"Missing Session");
            }
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Error: {ex.Message}");
        }
    }



    public static async Task<Results<Ok<List<Ticket>>, BadRequest<string>>> GetAssignedTickets(NpgsqlDataSource db, HttpContext ctx)
    {
        List<Ticket> tickets = new List<Ticket>();


        try
        {
            if (ctx.Session.IsAvailable)
            {
                var id = ctx.Session.GetInt32("id");
                if (id == null)
                {
                    return TypedResults.BadRequest("Session not exisiting");
                }
                using var cmd = db.CreateCommand(@"
                SELECT  t.id, t.status, t.description,t.product_id, t.ticket_category, t.slug
                FROM tickets t 
                WHERE t.customer_agent = $1 AND t.status != 3");

                cmd.Parameters.AddWithValue(id);


                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    tickets.Add(new Ticket(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetString(2),
                    reader.GetInt32(3),
                    reader.GetInt32(4),
                    reader.GetString(5)
                    ));
                }

                return TypedResults.Ok(tickets);
            }
            else
            {
                return TypedResults.BadRequest($"Missing session");
            }
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Ett fel inträffade: {ex.Message}");
        }
    }

    public static string GenerateSlugs(int length)
    {
        const string capitalLetters = "QWERTYUIOPASDFGHJKLZXCVBNM";
        const string smallLetters = "qwertyuiopasdfghjklzxcvbnm";
        const string digits = "0123456789";
        const string allChars = capitalLetters + smallLetters + digits;
        StringBuilder password = new StringBuilder();
        Random rnd = new Random();
        while (0 < length--)
        {
            password.Append(allChars[rnd.Next(allChars.Length)]);
        }
        return password.ToString();
    }



    public record NewTicket(int productId, int categoryId, string message, string email, string description);
    public static async Task<Results<Ok<string>, BadRequest<string>>> CreateTicket(NewTicket ticket, NpgsqlDataSource db)
    {
        try
        {

            await using var conn = await db.OpenConnectionAsync();
            await using var transaction = await conn.BeginTransactionAsync();

            int status = 1;
            int? ticketIdNullable;
            int ticketId;
            string slug = GenerateSlugs(16);

            var sql1 = "INSERT INTO tickets (status, description, customer_email, product_id, ticket_category, slug) VALUES ($1, $2, $3, $4, $5, $6) RETURNING id";
            using (var cmd1 = new NpgsqlCommand(sql1, conn, transaction))
            {
                cmd1.Parameters.AddWithValue(status);
                cmd1.Parameters.AddWithValue(ticket.description);
                cmd1.Parameters.AddWithValue(ticket.email);
                cmd1.Parameters.AddWithValue(ticket.productId);
                cmd1.Parameters.AddWithValue(ticket.categoryId);
                cmd1.Parameters.AddWithValue(slug);

                ticketIdNullable = (int?)await cmd1.ExecuteScalarAsync();
                if (!ticketIdNullable.HasValue)
                {
                    await transaction.DisposeAsync();
                    return TypedResults.BadRequest("Failed to create ticket");
                }
                else
                {
                    ticketId = ticketIdNullable.Value;
                }
            }

            var sql2 = "INSERT INTO messages(text, ticket, time, customer) VALUES ($1, $2, $3, $4)";

            using (var cmd2 = new NpgsqlCommand(sql2, conn, transaction))
            {
                cmd2.Parameters.AddWithValue(ticket.message);
                cmd2.Parameters.AddWithValue(ticketId);
                cmd2.Parameters.AddWithValue(DateTime.Now);
                cmd2.Parameters.AddWithValue(true);

                await cmd2.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();

            string url = "http://localhost:5173/customer/" + slug + "/chat";
            string subject = "Ticket on Produkt" + ticket.productId;
            string message = "Hej, Vi kommer svara på din ticket så fort vi kan! Här är en länk till chatten. " + url;
            MailService.SendMail(ticket.email, subject, message);
            return TypedResults.Ok(url);


        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"An error occurred: {ex.Message}");
        }
    }


    public record statusDTO(int status);
    public static async Task<Results<Ok<string>, BadRequest<string>>> ChangeStatus(string slug, statusDTO status, NpgsqlDataSource db)
    {
        try
        {   int? idNullable = await MessageRoutes.GetIdBySlug(db,slug); 
            if(!idNullable.HasValue){
                return TypedResults.BadRequest("failed to get ticket id from slug");
            }
            int id = idNullable.Value; 
            using var cmd = db.CreateCommand(
                @"UPDATE tickets SET status = $1 WHERE id = $2"
            );
            cmd.Parameters.AddWithValue(status.status < 3 ? 3 : 2);
            cmd.Parameters.AddWithValue(id);

            var done = await cmd.ExecuteNonQueryAsync();
            if (done > 0)
            {
                return TypedResults.Ok("Du ändrade status på ticketen");
            }
            else { return TypedResults.BadRequest($"Failed to update ticket status"); }
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"An error occurred: {ex.Message}");
        }
    }

    public static async Task<Results<Ok<int>, BadRequest<string>>> TicketRating(string slug, TicketRatingDTO ticketRating,
        NpgsqlDataSource db)
    {
        if (ticketRating.rating >= 1 && ticketRating.rating <= 5)
        {

            try
            {

                int? idNullable = await MessageRoutes.GetIdBySlug(db,slug); 
                if(!idNullable.HasValue){
                    return TypedResults.BadRequest("failed to get ticket id from slug");
                }
                int id = idNullable.Value; 
                using var cmd = db.CreateCommand("UPDATE tickets SET rating=$2 WHERE id=$1 AND status=3 AND rating is null");

                cmd.Parameters.AddWithValue(id);
                cmd.Parameters.AddWithValue(ticketRating.rating);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    return TypedResults.Ok(id);
                }
                else
                {
                    return TypedResults.BadRequest("Already rated!");
                }
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest("fan är det för fel " + ex.Message);
            }

        }
        else
        {
            return TypedResults.BadRequest("Invalid rating");
        }

    }
//    public record Ticket(int id, int status, string description, int product_id, int ticket_category, string slug);
    public static async Task<Results<Ok<List<Ticket>>, BadRequest<string>>> GetClosedTicketsByUserId(NpgsqlDataSource db, HttpContext ctx)
    {

        int? agentId = ctx.Session.GetInt32("id");
        List<Ticket> closedTicketlist = new();

        try
        {

            using var cmd = db.CreateCommand("SELECT id, status,description, product_id, ticket_category, slug FROM tickets WHERE status = 3 AND customer_agent = $1");
            if (agentId != null)
            {
                cmd.Parameters.AddWithValue(agentId);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    closedTicketlist.Add(new Ticket(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetString(2),
                        reader.GetInt32(3),
                        reader.GetInt32(4),
                        reader.GetString(5)
                    ));
                }
                return TypedResults.Ok(closedTicketlist);




            }
            return TypedResults.BadRequest("agentID kan ej vara null. Logga in som en customer_agent!!!");

        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }

    }

}



