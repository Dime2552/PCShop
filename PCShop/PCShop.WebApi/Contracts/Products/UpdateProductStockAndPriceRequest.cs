namespace PCShop.WebApi.Contracts.Products
{
    public class UpdateProductStockAndPriceRequest
    {
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        public int StockQuantity { get; set; }
    }
}
