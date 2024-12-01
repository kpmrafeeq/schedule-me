using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ScheduleMe.Models
{
    public class JobModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public string JobType { get; set; }
        public string JobData { get; set; }
    }
}