using System;
using System.IO;
using System.Threading;

using FileWatcher.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileWatcher
{
    public class UnzipFileAction : IFileAction
    {
        private ILogger<UnzipFileAction> _logger;
        private bool _processingFile = false;
        private string _outputPath;
        private TarGZ _tarGZ;


        public UnzipFileAction(TarGZ tarGZ,
                               ILogger<UnzipFileAction> logger,
                               IOptions<FileWatcherProperties> properties)
        {
            _logger = logger;
            _outputPath = properties.Value.OutputPath;
            _tarGZ = tarGZ;
        }



        public void FileCreated(string filepath)
        {
            _logger.LogInformation($"File Created: {filepath}");
        }

        public void FileDeleted(string filepath)
        {
            _logger.LogInformation($"File Deleted: {filepath}");
        }

        public void FileModified(string filepath)
        {
            _logger.LogInformation($"File Modified: {filepath}");

            if (!_processingFile && filepath.EndsWith("reports.tar.gz"))
            {
                _processingFile = true;

                UnzipArchive(filepath);
            }
        }


        private void UnzipArchive(string filepath)
        {
            if (string.IsNullOrEmpty(_outputPath))
                return;

            DateTime lastWriteTime;
            DateTime writeTime;

            do
            {
                lastWriteTime = File.GetLastWriteTime(filepath);
                Thread.Sleep(5 * 1000);
                writeTime = File.GetLastWriteTime(filepath);

                _logger.LogDebug($"{lastWriteTime} : {writeTime}");
            } while (lastWriteTime != writeTime);

            try
            {
                _tarGZ.ExtractTarGz(filepath, _outputPath);
                File.Delete(filepath);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error extracting {filepath}. Message: {ex.Message}");
            }

            _processingFile = false;
        }
    }
}
