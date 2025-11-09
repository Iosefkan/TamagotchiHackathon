using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts;
using Transactions.DAL;

namespace Transactions.Consumers;

public class AddUserConsumer(TransactionsDataContext db) : IConsumer<AddUserRequest>
{
    public async Task Consume(ConsumeContext<AddUserRequest> context)
    {
        var user = new User()
        {
            Name = context.Message.Username
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }
}