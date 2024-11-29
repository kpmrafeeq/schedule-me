using Quartz;
using System.Text.Json;
using System.Text;

namespace ScheduleMe.Job
{

    public class HttpJob : IJob
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpJob> _logger;

        public HttpJob(IHttpClientFactory httpClientFactory, ILogger<HttpJob> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Starting job");

            var url = context.MergedJobDataMap.GetString("Url");
            var headersJson = context.MergedJobDataMap.GetString("Headers");
            var body = context.MergedJobDataMap.GetString("Body");

            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("Url is required");
            }

            var headers = !string.IsNullOrEmpty(headersJson)
                ? JsonSerializer.Deserialize<Dictionary<string, string>>(headersJson)
                : new Dictionary<string, string>();

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(body ?? string.Empty, Encoding.UTF8, "application/json")
            };

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(header.Value);
                    }
                    else
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
            }

            var response = await client.SendAsync(request);
            _logger.LogInformation("API response: {StatusCode}, {Content}", response.StatusCode, await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
        }
    }
}

