using MongoDB.Driver;
using ScheduleMe.Models;

namespace ScheduleMe.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<JobModel> Jobs => _database.GetCollection<JobModel>("Jobs");
    }
}

