namespace CosmeticApi.Model
{
    public class CartDto
    {
        public int UserId { get; set; }
        public string TovarArticle { get; set; } = null!;
        public int? CartTovarCount { get; set; }
    }
}