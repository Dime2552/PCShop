using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Common.Interfaces;

namespace PCShop.Application.Categories.Queries.GetCategoryFilters
{
    public class GetCategoryFiltersQueryHandler : IRequestHandler<GetCategoryFiltersQuery, Dictionary<string, List<string>>>
    {
        private readonly IAppDbContext _context;

        public GetCategoryFiltersQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, List<string>>> Handle(GetCategoryFiltersQuery request, CancellationToken cancellationToken)
        {
            var attributesLists = await _context.Products
                .AsNoTracking()
                .Where(p => p.CategoryId == request.CategoryId)
                .Select(p => p.Attributes)
                .ToListAsync(cancellationToken);

            var filters = new Dictionary<string, HashSet<string>>();

            foreach (var attributes in attributesLists)
            {
                foreach (var attr in attributes)
                {
                    if (!filters.ContainsKey(attr.Key))
                    {
                        filters[attr.Key] = new HashSet<string>();
                    }
                    filters[attr.Key].Add(attr.Value);
                }
            }

            return filters.ToDictionary(k => k.Key, v => v.Value.ToList());
        }
    }
}
