using Fillow.Models;

public interface IVerificationService
{
    Task<VerificationData> GetVerificationDataByUserIdAsync(string userId);
    Task SaveVerificationDataAsync(VerificationData verificationData, string sessionUserId, string? idPhotoPath = null, string? personalIdPhotoPath = null, string? guidePhotoPath = null, string? businessLicensePath = null);
}