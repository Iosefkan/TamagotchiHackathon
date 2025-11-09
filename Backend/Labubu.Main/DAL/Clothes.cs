using Labubu.Main.DAL.Enums;

namespace Labubu.Main.DAL;

public class Clothes
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ClothesType Type { get; set; }
    public string Url { get; set; } = null!;
}