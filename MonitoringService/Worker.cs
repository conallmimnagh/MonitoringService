

namespace MonitoringService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClientFactory httpClientFactory;
        private List<string> Urls = new List<string> { "https://mimsmuscleonlinecoaching.blob.core.windows.net/%24web/index.html" };

        public Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory)
        {
            this._logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PollUrls();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occured while polling url's");
                }
                finally
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        private async Task PollUrls()
        {
            var tasks = new List<Task>();
            foreach( var url in Urls)
            {
                tasks.Add(PollUrl(url));  
            }

            await Task.WhenAll(tasks);
        }

        private async Task PollUrl (string url)
        {
            try
            {
                var client = httpClientFactory.CreateClient();  
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("{Url} is online", url);
                }
                else
                {
                    _logger.LogWarning("{Url} is offline", url);
                }

            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex,"[Url} is offline", url);
            }
        }
    }
}