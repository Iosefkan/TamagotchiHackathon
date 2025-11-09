namespace Labubu.Main.DAL;

public class LabubuClothes
{
    public Guid LabubuId { get; set; }
    public Guid ClothesId { get; set; }

    public Labubu Labubu { get; set; } = null!;
    public Clothes Clothes { get; set; } = null!;
}