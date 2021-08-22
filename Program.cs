using System;

using FileWatcher.Config;
using FileWatcher.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileWatcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception)
            {

            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "File Watcher";
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<FileWatcherBackgroundService>();
                    services.AddConfig(hostContext.Configuration);

                    services.AddSingleton<FileWatcherService>();
                    services.AddSingleton<IFileAction, UnzipFileAction>();
                    services.AddSingleton<TarGZ>();
                });
        }
    }
}
