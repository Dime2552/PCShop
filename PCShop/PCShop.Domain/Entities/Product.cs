using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace PCShop.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; } // Navigation

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Brand { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }

        public int StockQuantity { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<ProductAttributeItem> Attributes { get; set; } = new();
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public bool IsDeleted { get; set; } = false;
    }
}
