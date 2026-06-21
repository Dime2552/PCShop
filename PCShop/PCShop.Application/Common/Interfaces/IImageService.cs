namespace PCShop.Application.Common.Interfaces
{
    public record FileUploadDto(Stream Content, string FileName, string ContentType);

    public interface IImageService
    {
        Task<List<string>> UploadImagesAsync(IEnumerable<FileUploadDto> files, CancellationToken cancellationToken);
    }
}
