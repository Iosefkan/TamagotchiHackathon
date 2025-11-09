using Shared.Enums;

namespace Transactions.Services.Banking.Models;

public class AccountModel
{
    public string AccountId { get; set; } = null!;
    public string Status { get; set; } = null!;
    public CurrencyCode Currency { get; set; }
    public string AccountType { get; set; } = null!;
    public string Nickname { get; set; } = null!;
    public AccountInfoModel[] Account { get; set; } = null!;
}

public class AccountInfoModel
{
    public string SchemeName { get; set; } = null!;
    public string Identification { get; set; } = null!;
    public string Name { get; set; } = null!;
}