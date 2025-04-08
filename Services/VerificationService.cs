using System.Threading.Tasks;
using MongoDB.Driver;
using Fillow.Models;

public class VerificationService : IVerificationService
{
    private readonly IMongoCollection<VerificationData> _verificationCollection;

    public VerificationService(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("MongoDb"));
        var database = client.GetDatabase(config["DatabaseSettings:DatabaseName"]);
        _verificationCollection = database.GetCollection<VerificationData>("Verifications");
    }

    public async Task<VerificationData> GetVerificationDataByUserIdAsync(string userId)
    {
        return await _verificationCollection
            .Find(data => data.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task SaveVerificationDataAsync(VerificationData verificationData, string sessionUserId,  string? idPhotoPath = null, string ? personalIdPhotoPath = null, string? guidePhotoPath = null, string? businessLicensePath = null)
    {
        verificationData.UserId = sessionUserId;

        var existingData = await _verificationCollection
            .Find(data => data.UserId == sessionUserId)
            .FirstOrDefaultAsync();

        if (existingData != null)
        {
            var updateDefinition = Builders<VerificationData>.Update
                .Set(v => v.Email, verificationData.Email)
                .Set(v => v.PhoneNumber, verificationData.PhoneNumber)
                .Set(v => v.FullName, verificationData.FullName)
                .Set(v => v.BirthDate, verificationData.BirthDate)
                .Set(v => v.Address, verificationData.Address)
                .Set(v => v.CompanyName, verificationData.CompanyName)
                .Set(v => v.CompanyPhoneNumber, verificationData.CompanyPhoneNumber)
                .Set(v => v.CompanyAddress, verificationData.CompanyAddress)
                .Set(v => v.RepresentativeName, verificationData.RepresentativeName)
                .Set(v => v.RepresentativePosition, verificationData.RepresentativePosition)
                .Set(v => v.PassportId, verificationData.PassportId)
                .Set(v => v.IssueDate, verificationData.IssueDate);

            if (!string.IsNullOrEmpty(idPhotoPath))
            {
                updateDefinition = updateDefinition.Set(v => v.IdPhotoPath, idPhotoPath);
            }

            if (!string.IsNullOrEmpty(personalIdPhotoPath))
            {
                updateDefinition = updateDefinition.Set(v => v.personalIdPhotoPath, personalIdPhotoPath);
            }

            if (!string.IsNullOrEmpty(guidePhotoPath))
            {
                updateDefinition = updateDefinition.Set(v => v.GuidePhotoPath, guidePhotoPath);
            }

            if (!string.IsNullOrEmpty(businessLicensePath))
            {
                updateDefinition = updateDefinition.Set(v => v.BusinessLicensePath, businessLicensePath);
            }

            await _verificationCollection.UpdateOneAsync(
                data => data.UserId == sessionUserId,
                updateDefinition
            );
        }
        else
        {
            if (!string.IsNullOrEmpty(idPhotoPath))
            {
                verificationData.IdPhotoPath = idPhotoPath;
            }

            if (!string.IsNullOrEmpty(guidePhotoPath))
            {
                verificationData.GuidePhotoPath = guidePhotoPath;
            }

            if (!string.IsNullOrEmpty(businessLicensePath))
            {
                verificationData.BusinessLicensePath = businessLicensePath;
            }


            if (!string.IsNullOrEmpty(personalIdPhotoPath))
            {
                verificationData.personalIdPhotoPath = personalIdPhotoPath;
            }

            await _verificationCollection.InsertOneAsync(verificationData);
        }
    }
}
