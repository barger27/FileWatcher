using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileWatcher.Config
{
    public static class FileWatcherCollectionExtensions
    {
        public static IServiceCollection AddConfig(this IServiceCollection services, 
                                                    IConfiguration config)
        {
            services.Configure<FileWatcherProperties>(config.GetSection("FileWatcher"));

            return services;
        }
    }
}
