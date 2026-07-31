using HomePageApp.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomePageApp.Infrastructure.FileSystem
{
    public class ScratchPadStorage : IScratchPadStorage
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IUserAccountService _userAccountService;
        private string? _path;

        public ScratchPadStorage(IWebHostEnvironment environment, IConfiguration configuration, IUserAccountService userAccountService)
        {
            _environment = environment;
            _configuration = configuration;
            _userAccountService = userAccountService;
        }

        public async Task SaveAsync(string content)
        {
            var path = await GetScratchPadPathAsync();

            await File.WriteAllTextAsync(path, content);
        }

        public async Task<string> Load()
        {
            var path = await GetScratchPadPathAsync();

            if (!File.Exists(path))
                await File.WriteAllTextAsync(path, string.Empty);

            return await File.ReadAllTextAsync(path);
        }

        private async Task<string> GetScratchPadPathAsync()
        {
            if (_path == null)
            {
                var userId = await _userAccountService.GetCurrentUserProfileIdAsync();

                var path = Path.Combine(
                    _environment.ContentRootPath,
                    "wwwroot",
                    "uploads",
                    userId.ToString(),
                    _configuration["StorageSettings:ScratchPadPath"] ?? "");

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                _path = path;

                return path;
            }
            else
                return _path;            
        }
    }
}
