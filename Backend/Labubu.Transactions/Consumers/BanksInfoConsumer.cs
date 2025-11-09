using MassTransit;
using Microsoft.Extensions.Options;
using Shared.Contracts;
using Transactions.Configuration;

namespace Transactions.Consumers;

public class BanksInfoConsumer(IOptions<BanksConfig> banksOptions) : IConsumer<BankInfoRequest>
{
    public async Task Consume(ConsumeContext<BankInfoRequest> context)
    {
        await context.RespondAsync(banksOptions.Value.Banks);
    }
}