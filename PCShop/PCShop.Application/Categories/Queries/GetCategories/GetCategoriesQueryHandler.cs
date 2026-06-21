using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Categories.DTOs;
using PCShop.Application.Common.Interfaces;

namespace PCShop.Application.Categories.Queries.GetCategories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly IAppDbContext _context;

        public GetCategoriesQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Categories
                .Select(c => new CategoryDto(c.Id, c.Name, c.Description))
                .ToListAsync(cancellationToken);
        }
    }
}
