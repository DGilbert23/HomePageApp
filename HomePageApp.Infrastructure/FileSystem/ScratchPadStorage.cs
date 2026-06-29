using HomePageApp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomePageApp.Infrastructure.FileSystem
{
    public class ScratchPadStorage : IScratchPadStorage
    {
        private readonly string _path;

        public ScratchPadStorage(string path)
        {
            _path = path;
        }

        public Task SaveAsync(string content)
        {
            return File.WriteAllTextAsync(_path, content);
        }

        public Task<string> Load()
        {
            if(File.Exists(_path))
                return File.ReadAllTextAsync(_path);
            else
            {
                File.Create(_path).Close();
                return File.ReadAllTextAsync(_path);
            }    
        }
    }
}
