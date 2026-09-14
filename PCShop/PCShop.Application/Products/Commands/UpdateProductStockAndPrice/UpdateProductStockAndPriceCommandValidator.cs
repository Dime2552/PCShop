using FluentValidation;

namespace PCShop.Application.Products.Commands.UpdateProductStockAndPrice
{
    public class UpdateProductStockAndPriceCommandValidator : AbstractValidator<UpdateProductStockAndPriceCommand>
    {
        public UpdateProductStockAndPriceCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Product ID is required.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");

            When(x => x.DiscountPrice.HasValue, () =>
            {
                RuleFor(x => x.DiscountPrice!.Value)
                    .GreaterThan(0).WithMessage("Discount price must be greater than 0.")
                    .LessThan(x => x.Price).WithMessage("Discount price must be less than regular price.");
            });

            When(x => x.DiscountStartDate.HasValue && x.DiscountEndDate.HasValue, () =>
            {
                RuleFor(x => x.DiscountEndDate!.Value)
                    .GreaterThanOrEqualTo(x => x.DiscountStartDate!.Value)
                    .WithMessage("Discount end date must be on or after start date.");
            });
        }
    }
}
