using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Transactions.DAL;

[Index(nameof(Name), IsUnique = true)]
public class User
{
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;
}