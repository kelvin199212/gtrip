using Fillow.Models;
using Fillow.Services;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

public class UserProfileService : IUserProfileService
{
    private readonly IMongoCollection<UserProfile> _userProfiles;

    public UserProfileService(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("MongoDb"));
        var database = client.GetDatabase(config["DatabaseSettings:DatabaseName"]);
        _userProfiles = database.GetCollection<UserProfile>("UserProfiles");
    }

    public async Task<List<UserProfile>> GetUsersAsync() =>
        await _userProfiles.Find(_ => true).ToListAsync();

    public async Task<UserProfile> GetUserByIdAsync(string id)
    {
    

        return await _userProfiles.Find(user => user.Id == id).FirstOrDefaultAsync();
    }
    public async Task<UserProfile> GetUserProfileByUserIdAsync(string userId)
    {
        return await _userProfiles
            .Find(profile => profile.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task SaveUserProfileAsync(UserProfile userProfile, string sessionUserId, string? profilePicturePath = null, string? videoIntroductionPath = null)
    {
        try
        {
            userProfile.UserId = sessionUserId;

            var existingProfile = await _userProfiles
                .Find(profile => profile.UserId == sessionUserId)
                .FirstOrDefaultAsync();

            if (existingProfile != null)
            {
                var updateDefinition = Builders<UserProfile>.Update
                    .Set(p => p.UserName, userProfile.UserName)
                    .Set(p => p.Email, userProfile.Email)
                    .Set(p => p.PhoneNumber, userProfile.PhoneNumber)
                    .Set(p => p.Gender, userProfile.Gender)
                    .Set(p => p.BirthDate, userProfile.BirthDate)
                    .Set(p => p.About, userProfile.About)
                    .Set(p => p.LocalAirport, userProfile.LocalAirport)
                    .Set(p => p.Address, userProfile.Address)
                    .Set(p => p.City, userProfile.City)
                    .Set(p => p.State, userProfile.State)
                    .Set(p => p.PostalCode, userProfile.PostalCode)
                    .Set(p => p.Country, userProfile.Country);

                if (!string.IsNullOrEmpty(profilePicturePath))
                {
                    updateDefinition = updateDefinition.Set(p => p.ProfilePicturePath, profilePicturePath);
                }

                if (!string.IsNullOrEmpty(videoIntroductionPath))
                {
                    updateDefinition = updateDefinition.Set(p => p.VideoIntroductionPath, videoIntroductionPath);
                }

                await _userProfiles.UpdateOneAsync(
                    profile => profile.UserId == sessionUserId,
                    updateDefinition
                );
            }
            else
            {
                if (!string.IsNullOrEmpty(profilePicturePath))
                {
                    userProfile.ProfilePicturePath = profilePicturePath;
                }

                if (!string.IsNullOrEmpty(videoIntroductionPath))
                {
                    userProfile.VideoIntroductionPath = videoIntroductionPath;
                }

                await _userProfiles.InsertOneAsync(userProfile);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public async Task DeleteUserAsync(string id)
    {
   

        await _userProfiles.DeleteOneAsync(user => user.Id == id);
    }
}
