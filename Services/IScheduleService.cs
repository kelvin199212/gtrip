using Fillow.Models.partneradmin;
using MongoDB.Bson;

namespace Fillow.Services
{
    public interface IScheduleService
    {
        Task<Schedule> CreateScheduleAsync(Schedule schedule);
        Task DeleteScheduleAsync(string id);
        Task<List<Schedule>> GetSchedulesAsync();
        Task<Schedule> UpdateScheduleAsync(string id, Schedule updatedSchedule);
    }
}