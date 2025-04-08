using Fillow.Models.partneradmin;

namespace Fillow.Services
{
    public interface IWithdrawalService
    {
        List<WithdrawalRequestModel> GetWithdrawalRecords(string userId);
        bool ProcessWithdrawalRequest(WithdrawalRequestModel model);
    }
}