namespace PCShop.WebApi.Contracts.Products
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Description { get; set; }
        public string AttributesJson { get; set; } = "{}";
        public IFormFileCollection Images { get; set; } = null!;
    }
}
