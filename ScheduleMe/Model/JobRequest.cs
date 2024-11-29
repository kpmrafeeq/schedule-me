namespace ScheduleMe.Model
{
    public class JobRequest
    {
        public string? Name { get; set; }
        public string? CronExpression { get; set; }
        public string? Url { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
        public string? Body { get; set; }
    }
}
