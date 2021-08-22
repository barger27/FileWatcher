using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace FileWatcher
{
    public class FileWatcherService
    {
        private FileWatcherProperties _properties;
        private IFileAction _action;
        private ILogger<FileWatcherService> _logger;

        private FileSystemWatcher _watcher;
        private int _consecutiveErrors = 0;


        public bool IsRunning { get; private set; } = false;


        public FileWatcherService(IOptions<FileWatcherProperties> properties, 
                                  IFileAction command,
                                  ILogger<FileWatcherService> logger) 
        {
            _properties = properties.Value;
            _action = command;
            _logger = logger;
        }


        public void StartWatching()
        {
            if (!IsRunning)
            {
                _logger.LogInformation("FileWatcher started...");
                _logger.LogInformation($"Watching Folder: {_properties.WatchedPath}");

                SetupWatcher();
            }
        }


        public void StopWatching()
        {
            if (_watcher != null && IsRunning)
            {
                IsRunning = false;
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
            }
        }


        private void SetupWatcher()
        {
            try
            {
                if (Directory.Exists(_properties.WatchedPath))
                {
                    _watcher = new FileSystemWatcher();

                    _watcher.Path = _properties.WatchedPath;

                    _watcher.Created += Watcher_Created;
                    _watcher.Changed += Watcher_Changed;
                    _watcher.Deleted += Watcher_Deleted;
                    _watcher.Error += Watcher_Error;

                    _watcher.EnableRaisingEvents = true;

                    IsRunning = true;
                    _consecutiveErrors = 0;
                }
            } 
            catch (Exception ex)
            {
                IsRunning = false;
                _logger.LogError($"Error setting up File Watcher: {ex.Message}");
                _consecutiveErrors++;

                if (_consecutiveErrors == 5)
                    throw ex;
            }
        }


        private void Watcher_Created(object sender, FileSystemEventArgs evt)
        {
            if (_action != null)
                _action.FileCreated(evt.FullPath);
        }


        private void Watcher_Changed(object sender, FileSystemEventArgs evt)
        {
            if (_action != null)
                _action.FileModified(evt.FullPath);
        }
        

        private void Watcher_Deleted(object sender, FileSystemEventArgs evt)
        {
            if (_action != null)
                _action.FileDeleted(evt.FullPath);
        }


        private void Watcher_Error(object sender, ErrorEventArgs evt)
        {
            _logger.LogError($"Error running the file watcher: {evt.ToString()}");
            _watcher.Dispose();
            IsRunning = false;
        }
    }
}
