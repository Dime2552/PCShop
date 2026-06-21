using MediatR;

namespace PCShop.Application.Categories.Queries.GetCategoryFilters
{
    public record GetCategoryFiltersQuery(int CategoryId) : IRequest<Dictionary<string, List<string>>>;
}
