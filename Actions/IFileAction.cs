using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileWatcher
{
    public interface IFileAction
    {
        void FileCreated(string filepath);
        void FileModified(string filepath);
        void FileDeleted(string filepath);
    }
}
