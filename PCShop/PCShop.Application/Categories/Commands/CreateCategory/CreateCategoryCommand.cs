using MediatR;

namespace PCShop.Application.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(string Name, string? Description) : IRequest<int>;
}
