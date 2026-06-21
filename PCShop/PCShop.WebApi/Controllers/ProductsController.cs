using MediatR;
using Microsoft.AspNetCore.Mvc;
using PCShop.Application.Common.Interfaces;
using PCShop.Application.Products.Commands.CreateProduct;
using PCShop.WebApi.Contracts.Products;

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
            if (request.Images != null && request.Images.Any())
            {
                foreach (var file in request.Images)
                {
                    var stream = file.OpenReadStream();
                    fileDtos.Add(new FileUploadDto(stream, file.FileName, file.ContentType));
                }
            }

            var command = new CreateProductCommand(
                request.Name, request.CategoryId, request.Brand,
                request.Price, request.StockQuantity, request.Description, fileDtos);

            var productId = await _mediator.Send(command);
            return Ok(productId);
        }
    }
}
