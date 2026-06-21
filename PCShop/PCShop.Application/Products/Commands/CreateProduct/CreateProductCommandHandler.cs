using MediatR;
using PCShop.Application.Common.Interfaces;
using PCShop.Domain.Entities;

namespace PCShop.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IAppDbContext _context;
        private readonly IImageService _imageService;

        public CreateProductCommandHandler(IAppDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Load image to AWS S3
            var imageUrls = await _imageService.UploadImagesAsync(request.Images, cancellationToken);

            var attributesList = request.Attributes
                .Select(kvp => new ProductAttributeItem { Key = kvp.Key, Value = kvp.Value })
                .ToList();

            // Create entity
            var product = new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = request.CategoryId,
                Name = request.Name,
                Brand = request.Brand,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                Description = request.Description,
                ImageUrls = imageUrls,
                Attributes = attributesList
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }
}
