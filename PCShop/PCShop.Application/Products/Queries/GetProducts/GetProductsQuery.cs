using MediatR;
using PCShop.Application.Common.Models;
using PCShop.Application.Products.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PCShop.Application.Products.Queries.GetProducts
{
    public record GetProductsQuery(
        int CategoryId,
        int PageNumber = 1,
        int PageSize = 10,
        string? SortBy = null, // "price_asc", "price_desc"
        Dictionary<string, string>? Filters = null // Ex. { "Memory": "12GB", "Socket": "AM5" }
    ) : IRequest<PaginatedList<ProductDto>>;
}
