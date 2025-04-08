using Fillow.Models;
namespace Fillow.Services

{
    public interface IUserProfileService
    {
        Task SaveUserProfileAsync(UserProfile userProfile, string sessionUserId, string profilePicturePath = null, string videoIntroductionPath = null);


        Task<UserProfile> GetUserProfileByUserIdAsync(string userId);
	}
}
