using ArtGenerator.Models;
using System.Collections.Generic;
using System.Windows;

namespace ArtGenerator.Services
{
    public interface IConfigService
    {
        Variables GetConfig();
        public Application LoadLanguage(Application window);
        public List<string> GetLanguages();
        void LoadDefaultConfig();
        void LoadConfig();
        void SaveConfig();
    }
}
