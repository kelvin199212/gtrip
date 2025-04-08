using Fillow.Models.partneradmin;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Fillow.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IMongoCollection<Schedule> _scheduleCollection;

        public ScheduleService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            var database = client.GetDatabase(config["DatabaseSettings:DatabaseName"]);
            _scheduleCollection = database.GetCollection<Schedule>("Schedules");
        }

        public async Task<List<Schedule>> GetSchedulesAsync()
        {
            return await _scheduleCollection.Find(schedule => true).ToListAsync();
        }

        public async Task<Schedule> CreateScheduleAsync(Schedule schedule)
        {
            await _scheduleCollection.InsertOneAsync(schedule);
            return schedule;
        }

        public async Task<Schedule> UpdateScheduleAsync(string id, Schedule updatedSchedule)
        {
            var result = await _scheduleCollection.ReplaceOneAsync(schedule => schedule.Id == id, updatedSchedule);
            if (result.MatchedCount == 0)
            {
                throw new KeyNotFoundException("Schedule not found");
            }
            return updatedSchedule;
        }

        public async Task DeleteScheduleAsync(string id)
        {
            var result = await _scheduleCollection.DeleteOneAsync(schedule => schedule.Id == id);
            if (result.DeletedCount == 0)
            {
                throw new KeyNotFoundException("Schedule not found");
            }
        }
    }
}
