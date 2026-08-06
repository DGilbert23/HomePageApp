using System;
using System.Collections.Generic;
using System.Text;

namespace HomePageApp.Core.Interfaces.ScratchPad
{
    public interface IScratchPadStorage
    {
        Task SaveAsync(string content);
        Task<string> Load();
    }
}
