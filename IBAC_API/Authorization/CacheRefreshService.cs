namespace API.Authorization
{
    public class CacheRefreshService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CacheRefreshService> _logger;
        private readonly TimeSpan _refreshInterval = TimeSpan.FromMinutes(5); // Set the interval as needed

        public CacheRefreshService(IServiceProvider serviceProvider, ILogger<CacheRefreshService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Cache refresh service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Running cache refresh...");

                try
                {
                    // Create a new scope to get scoped services
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var indexBuilder = scope.ServiceProvider.GetRequiredService<IndexBuilder>();

                        // Call GetIndex to force an update of the cache if necessary
                        indexBuilder.GetIndex();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while refreshing the cache.");
                }

                // Wait for the next scheduled refresh
                await Task.Delay(_refreshInterval, stoppingToken);
            }

            _logger.LogInformation("Cache refresh service is stopping.");
        }
    }
}
