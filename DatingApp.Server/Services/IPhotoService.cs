namespace DatingApp.Server.Services;

public interface IPhotoService
{
    public Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
    Task<DeletionResult> DeletePhotoAsync(string publicId);
}
