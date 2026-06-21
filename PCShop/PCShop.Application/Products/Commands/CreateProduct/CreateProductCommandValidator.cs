using FluentValidation;

namespace PCShop.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.CategoryId).GreaterThan(0);
            RuleFor(x => x.Brand).NotEmpty();
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);

            RuleFor(x => x.Images)
                .NotEmpty().WithMessage("At least one image is required.")
                .Must(images => images.Count <= 5).WithMessage("Maximum 5 images allowed.");
        }
    }
}
