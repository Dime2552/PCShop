using MediatR;
using Microsoft.AspNetCore.Mvc;
using PCShop.Application.Categories.Queries.GetCategoryFilters;
using PCShop.Application.Common.Interfaces;
using PCShop.Application.Common.Models;
using PCShop.Application.Products.Commands.CreateProduct;
using PCShop.Application.Products.DTOs;
using PCShop.Application.Products.Queries.GetProducts;
using PCShop.WebApi.Contracts.Products;
using System.Text.Json;

namespace PCShop.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromForm] CreateProductRequest request)
        {
            var fileDtos = new List<FileUploadDto>();
            if (request.Images != null)
            {
                foreach (var file in request.Images)
                {
                    var stream = file.OpenReadStream();
                    fileDtos.Add(new FileUploadDto(stream, file.FileName, file.ContentType));
                }
            }

            var attributes = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(request.AttributesJson))
            {
                attributes = JsonSerializer.Deserialize<Dictionary<string, string>>(request.AttributesJson) ?? new();
            }

            var command = new CreateProductCommand(
                request.Name,
                request.CategoryId,
                request.Brand,
                request.Price,
                request.StockQuantity,
                request.Description,
                fileDtos,
                attributes
            );

            var productId = await _mediator.Send(command);

            return Ok(productId);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedList<ProductDto>>> GetProducts([FromQuery] GetProductsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("filters/{categoryId}")]
        public async Task<ActionResult<Dictionary<string, List<string>>>> GetFilters(int categoryId)
        {
            var result = await _mediator.Send(new GetCategoryFiltersQuery(categoryId));
            return Ok(result);
        }
    }
}
