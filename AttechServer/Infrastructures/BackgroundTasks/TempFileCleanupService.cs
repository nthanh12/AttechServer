using AttechServer.Infrastructures.Abstractions;

namespace AttechServer.Infrastructures.BackgroundTasks
{
    public class TempFileCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TempFileCleanupService> _logger;

        public TempFileCleanupService(IServiceProvider serviceProvider, ILogger<TempFileCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var wysiwygFileProcessor = scope.ServiceProvider.GetRequiredService<IWysiwygFileProcessor>();
                        await wysiwygFileProcessor.CleanTempFilesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error cleaning temp files");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
