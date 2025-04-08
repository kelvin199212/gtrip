using Fillow.Services;
using Microsoft.AspNetCore.Mvc;
using Fillow.Models;

namespace Fillow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : Controller
    {
        private readonly IVerificationService _verificationService;
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService, IVerificationService VerificationService)
        {
            _userProfileService = userProfileService;
            _verificationService = VerificationService;
        }
        [HttpPost("EditProfile")]
        public async Task<IActionResult> EditProfile([FromForm] UserProfile userProfile, IFormFile? ProfilePicture, IFormFile? VideoIntroduction)
        {
            try
            {
                string userId = HttpContext.Session.GetString("UserId");

                // Initialize paths to null
                string? relativeProfilePicturePath = null;
                string? relativeVideoPath = null;

                // Check if ProfilePicture is provided
                if (ProfilePicture != null)
                {
                    var profilePictureDirectory = Path.Combine("wwwroot/uploads", userId, "images");
                    if (!Directory.Exists(profilePictureDirectory))
                    {
                        Directory.CreateDirectory(profilePictureDirectory);
                    }
                    var profilePictureFileName = Path.GetFileName(ProfilePicture.FileName);
                    var profilePictureFilePath = Path.Combine(profilePictureDirectory, profilePictureFileName);
                    relativeProfilePicturePath = Path.Combine("uploads", userId, "images", profilePictureFileName);

                    // Save the profile picture
                    using (var stream = new FileStream(profilePictureFilePath, FileMode.Create))
                    {
                        await ProfilePicture.CopyToAsync(stream);
                    }
                }

                // Check if VideoIntroduction is provided
                if (VideoIntroduction != null)
                {
                    var videoIntroductionDirectory = Path.Combine("wwwroot/uploads", userId, "videos");
                    if (!Directory.Exists(videoIntroductionDirectory))
                    {
                        Directory.CreateDirectory(videoIntroductionDirectory);
                    }
                    var videoIntroductionFileName = Path.GetFileName(VideoIntroduction.FileName);
                    var videoIntroductionFilePath = Path.Combine(videoIntroductionDirectory, videoIntroductionFileName);
                    relativeVideoPath = Path.Combine("uploads", userId, "videos", videoIntroductionFileName);

                    // Save the video introduction
                    using (var stream = new FileStream(videoIntroductionFilePath, FileMode.Create))
                    {
                        await VideoIntroduction.CopyToAsync(stream);
                    }
                }

                // Pass the file paths to your SaveUserProfileAsync method
                await _userProfileService.SaveUserProfileAsync(userProfile, userId, relativeProfilePicturePath, relativeVideoPath);

                return Ok(new { message = "Edit successful!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("GetProfileData")]
        public async Task<IActionResult> GetProfileData()
        {
            try
            {
                string userId = HttpContext.Session.GetString("UserId");
                var userProfile = await _userProfileService.GetUserProfileByUserIdAsync(userId);

                if (userProfile != null)
                {
                    return Ok(userProfile);
                }

                return NotFound("Verification data not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        [HttpGet("GetVerificationData")]
        public async Task<IActionResult> GetVerificationData()
        {
            try
            {
                string userId = HttpContext.Session.GetString("UserId");

                var verificationData = await _verificationService.GetVerificationDataByUserIdAsync(userId);

                if (verificationData != null)
                {
                    return Ok(verificationData);
                }

                return NotFound("Verification data not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        [HttpPost("SubmitVerification")]
        public async Task<IActionResult> SubmitVerification([FromForm] VerificationData verificationData, IFormFile? IdPhoto, IFormFile? personalIdPhoto, IFormFile? GuidePhoto, IFormFile? BusinessLicense)
        {
            try
            {
                string userId = HttpContext.Session.GetString("UserId");

                string? idPhotoPath = null;
                string? guidePhotoPath = null;
                string? personalIdPhotoPath = null;
                string? businessLicensePath = null;

                if (IdPhoto != null)
                {
                    var idPhotoDirectory = Path.Combine("wwwroot/uploads", userId, "idPhotos");
                    if (!Directory.Exists(idPhotoDirectory))
                    {
                        Directory.CreateDirectory(idPhotoDirectory);
                    }
                    var idPhotoFileName = Path.GetFileName(IdPhoto.FileName);
                    var idPhotoFilePath = Path.Combine(idPhotoDirectory, idPhotoFileName);
                    idPhotoPath = Path.Combine("uploads", userId, "idPhotos", idPhotoFileName);
                    using (var stream = new FileStream(idPhotoFilePath, FileMode.Create))
                    {
                        await IdPhoto.CopyToAsync(stream);
                    }
                }

                if (personalIdPhoto != null)
                {
                    var guidePhotoDirectory = Path.Combine("wwwroot/uploads", userId, "personalIdPhoto");
                    if (!Directory.Exists(guidePhotoDirectory))
                    {
                        Directory.CreateDirectory(guidePhotoDirectory);
                    }
                    var personalIdPhotoFileName = Path.GetFileName(personalIdPhoto.FileName);
                    var personalIdPhotoFilePath = Path.Combine(guidePhotoDirectory, personalIdPhotoFileName);
                    personalIdPhotoPath = Path.Combine("uploads", userId, "personalIdPhoto", personalIdPhotoFileName);
                    using (var stream = new FileStream(personalIdPhotoFilePath, FileMode.Create))
                    {
                        await personalIdPhoto.CopyToAsync(stream);
                    }
                }

                if (GuidePhoto != null)
                {
                    var guidePhotoDirectory = Path.Combine("wwwroot/uploads", userId, "guidePhotos");
                    if (!Directory.Exists(guidePhotoDirectory))
                    {
                        Directory.CreateDirectory(guidePhotoDirectory);
                    }
                    var guidePhotoFileName = Path.GetFileName(GuidePhoto.FileName);
                    var guidePhotoFilePath = Path.Combine(guidePhotoDirectory, guidePhotoFileName);
                    guidePhotoPath = Path.Combine("uploads", userId, "guidePhotos", guidePhotoFileName);
                    using (var stream = new FileStream(guidePhotoFilePath, FileMode.Create))
                    {
                        await GuidePhoto.CopyToAsync(stream);
                    }
                }

                if (BusinessLicense != null)
                {
                    var businessLicenseDirectory = Path.Combine("wwwroot/uploads", userId, "businessLicenses");
                    if (!Directory.Exists(businessLicenseDirectory))
                    {
                        Directory.CreateDirectory(businessLicenseDirectory);
                    }
                    var businessLicenseFileName = Path.GetFileName(BusinessLicense.FileName);
                    var businessLicenseFilePath = Path.Combine(businessLicenseDirectory, businessLicenseFileName);
                    businessLicensePath = Path.Combine("uploads", userId, "businessLicenses", businessLicenseFileName);
                    using (var stream = new FileStream(businessLicenseFilePath, FileMode.Create))
                    {
                        await BusinessLicense.CopyToAsync(stream);
                    }
                }

                await _verificationService.SaveVerificationDataAsync(verificationData, userId, idPhotoPath, personalIdPhotoPath, guidePhotoPath, businessLicensePath);

                return Ok(new { message = "Verification submitted successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }
        }
    }
