using HomePageApp.Core.Interfaces.ScratchPad;

namespace HomePageApp.Infrastructure.DemoInfrastructure.Services
{
    public class DemoScratchPadService : IScratchPadStorage
    {
        private string scratchPadContent = string.Empty;

        public DemoScratchPadService()
        {
            scratchPadContent = "Automatic saving free-text area for simple note keeping.";
        }

        public Task<string> Load()
        {
            return Task.FromResult(scratchPadContent);
        }

        public Task SaveAsync(string content)
        {
            scratchPadContent = content;
            return Task.CompletedTask;
        }
    }
}
