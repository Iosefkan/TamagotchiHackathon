using Labubu.Main.DAL;
using Labubu.Main.Data;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts;

namespace Labubu.Main.Consumers;

public class AddUserConsumer(MainContext db, ILogger<AddUserConsumer> logger) : IConsumer<AddUserRequest>
{
    public async Task Consume(ConsumeContext<AddUserRequest> context)
    {
        try
        {
            var existingUser = await db.Users.FindAsync(context.Message.UserId);
            if (existingUser != null)
            {
                logger.LogWarning("User {UserId} already exists in Main service", context.Message.UserId);
                return;
            }

            var user = new User
            {
                Id = context.Message.UserId,
                Name = context.Message.Username,
                Email = context.Message.Email
            };

            var labubu = new DAL.Labubu
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Health = 100,
                Weight = 1,
                Status = 1,
                Energy = 100,
                IsMessageRead = true
            };

            user.Labubu = labubu;
            
            db.Users.Add(user);
            db.Labubus.Add(labubu);
            await db.SaveChangesAsync();
            
            logger.LogInformation("User {UserId} ({Username}) and Labubu created in Main service", user.Id, user.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user {UserId} in Main service", context.Message.UserId);
            throw;
        }
    }
}
