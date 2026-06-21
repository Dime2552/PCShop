using MediatR;
using PCShop.Application.Categories.DTOs;

namespace PCShop.Application.Categories.Queries.GetCategories
{
    public record GetCategoriesQuery : IRequest<List<CategoryDto>>;
}
