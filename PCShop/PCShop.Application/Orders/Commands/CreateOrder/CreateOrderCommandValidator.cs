using FluentValidation;

namespace PCShop.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CartId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ShippingAddress).NotNull();
            RuleFor(x => x.ShippingAddress.Country).NotEmpty();
            RuleFor(x => x.ShippingAddress.City).NotEmpty();
            RuleFor(x => x.ShippingAddress.Street).NotEmpty();
            RuleFor(x => x.ShippingMethod).NotEmpty();
        }
    }
}
