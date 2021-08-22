using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileWatcher
{
    public class FileWatcherBackgroundService : BackgroundService
    {
        private readonly ILogger<FileWatcherBackgroundService> _logger;
        private readonly FileWatcherService _fileWatcherService;

        
        public FileWatcherBackgroundService(FileWatcherService service,
                                            ILogger<FileWatcherBackgroundService> logger)
        {
            _fileWatcherService = service;
            _logger = logger;
        }


        public override Task StopAsync(CancellationToken cancellationToken)
        {
            var task = base.StopAsync(cancellationToken);
            
            _fileWatcherService.StopWatching();

            return task;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    if (!_fileWatcherService.IsRunning)
                    {
                        _fileWatcherService.StartWatching();
                    }

                    await Task.Delay(60 * 1000);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Exiting Service...");
                    if (_fileWatcherService != null)
                        _fileWatcherService.StopWatching();
                }
            }
        }
    }
}
