using MediatR;
using Microsoft.AspNetCore.Mvc;
using PCShop.Application.Categories.Commands.CreateCategory;
using PCShop.Application.Categories.DTOs;
using PCShop.Application.Categories.Queries.GetCategories;

namespace PCShop.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            var result = await _mediator.Send(new GetCategoriesQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateCategoryCommand command)
        {
            var categoryId = await _mediator.Send(command);
            return Ok(categoryId);
        }
    }
}
