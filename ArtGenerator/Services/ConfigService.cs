using ArtGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace ArtGenerator.Services
{
    public class ConfigService : IConfigService
    {
        private static string _fileName = "config.ini";
        private static string _myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string _cfgPath = _myDocs + "\\ArtGenerator\\" + _fileName;

        private readonly HashSet<string> _availableVariable = new();
        private readonly List<Application> _applicationsForLanguages = new();
        private Variables _variables = new();

        public Variables GetConfig()
        {
            return _variables;
        }

        /// <summary>
        /// Checks if the config file exists and creates a new config file if it doesn't.
        /// </summary>
        public ConfigService()
        {
            LoadDefaultConfig();

            if (!File.Exists(_cfgPath))
            {
                Directory.CreateDirectory(_myDocs + "\\ArtGenerator");
                File.Create(Path.Combine(_myDocs + "\\ArtGenerator\\", _fileName)).Close();
                SaveConfig();
            }

            if (!File.Exists(_myDocs + "\\ArtGenerator\\GeneratedArt"))
            {
                Directory.CreateDirectory(_myDocs + "\\ArtGenerator\\GeneratedArt");
            }

            if (!File.Exists(_myDocs + "\\ArtGenerator\\OldGenerations"))
            {
                Directory.CreateDirectory(_myDocs + "\\ArtGenerator\\OldGenerations");
            }

            LoadConfig();
        }

        /// <summary>
        /// Loads the default config
        /// </summary>
        public void LoadDefaultConfig()
        {
            // This foreach loop goes through all the values in the Variables class and adds available
            // fields to the config, so it can automatically add or remove config values
            List<string> notAllowed = new() { "HasChanged", "AvailableLanguages", "PaintingBackgroundTypes" };
            foreach (PropertyInfo field in _variables.GetType().GetProperties())
            {
                if(field.GetSetMethod().IsPublic && !notAllowed.Contains(field.Name))
                {
                    _availableVariable.Add(field.Name);
                }
            }

            _variables.AllowRectangle = true;
            _variables.AllowTriangle = true;
            _variables.AllowCircle = true;
            _variables.AllowLine = true;
            _variables.AllowCurvedLine = true;
            _variables.AllowStar = false;
            _variables.AllowDot = true;
            _variables.AllowCutsInShapes = true;
            _variables.SaveAllPictures = false;
            _variables.LocationDetection = false;

            _variables.MaxShapeSize = 150;
            _variables.MinShapeSize = 40;
            _variables.MaxSkewing = 30;
            _variables.MinSkewing = -30;
            _variables.MaxBorderSize = 5;
            _variables.MinBorderSize = 0;
            _variables.StartCalculationsAtAmountOfParents = 10;
            _variables.AddedShapesPerParent = 60;

            _variables.ChanceToBeHollow = 0.05;
            _variables.ChanceToMutate = 0.10;

            _variables.PanelHeight = 760;
            _variables.PanelWidth = 480;

            _variables.CustomShape = "186.12,200.14,216.23,227.36,240.50,252.76,260.105,263.126,265.152,264.191,264.227,259.259,248.286,234.311,221.322,198.328,177.323,157.313,144.301,135.289,129.275,123.256,120.245,113.249,106.246,102.240,98.234,94.224,90.214,88.199,88.182,92.167,97.158,98.142,98.122,100.106,104.88,108.74,113.64,120.52,125.43,134.33,142.25,155.17,167.13,169.12,;;;158.148,168.144,183.141,192.144,197.148,194.161,187.168,175.170,166.167,158.160,;240.141,250.144,253.154,247.165,238.171,228.169,222.161,220.155,222.146,228.142,;;";
            _variables.BackgroundColor = "#ff454545";
            _variables.PaintingBackgroundColor = "#ffffffff";
            _variables.TitleColor = "#ffffffff";
            _variables.ChosenCulture = "Dutch";
            _variables.ChosenBackgroundType = "Dynamic";

            // The first language in this list will also be chosen at default if the chosen language wasn't found.
            _variables.AvailableLanguages = new() { "Dutch", "English" };
            _variables.PaintingBackgroundTypes = new() { "Dynamic", "Complementing", "Contrasting", "Custom" };

            _variables.HasChanged = false;
        }

        public Application LoadLanguage(Application window)
        {
            var resources = window.Resources.MergedDictionaries;
            var languageResource = resources.Where(l => l.Source != null && l.Source.ToString().StartsWith("Languages"));

            if (!_applicationsForLanguages.Contains(window)) _applicationsForLanguages.Add(window);
            if (languageResource.Count() != 0) resources.Remove(languageResource.First());
            
            resources.Add(GetLanguage());

            return window;
        }

        /// <summary>
        /// Gets the languages that are available.
        /// </summary>
        /// <returns>Returns a list with the available languages</returns>
        public List<string> GetLanguages()
        {
            return _variables.AvailableLanguages;
        }

        /// <summary>
        /// Gets the appropriate Language resource.
        /// </summary>
        /// <returns>Returns the chosen Resource Dictionary</returns>
        private ResourceDictionary GetLanguage()
        {
            string fileName;
            if(GetLanguages().Contains(_variables.ChosenCulture))
            {
                fileName = "Languages/" + _variables.ChosenCulture + ".xaml";
            }
            else
            {
                fileName = "Languages/" + GetLanguages().First() + ".xaml";
            }
            return new ResourceDictionary { Source = new Uri(fileName, UriKind.Relative) };
        }

        /// <summary>
        /// Changes the Languages of all registered resources.
        /// </summary>
        private void ChangeLanguages()
        {
            foreach(Application application in _applicationsForLanguages)
            {
                LoadLanguage(application);
            }
        }

        /// <summary>
        /// Adds values that were removed or weren't in the saved config file.
        /// </summary>
        /// <param name="forgotten">The forgotten values</param>
        /// <param name="needsToBeUpdated">The List that needs to be updated</param>
        /// <returns>an updated version of the List</returns>
        private List<string> AddForgottenValuesToConfig(List<string> forgotten, List<string> needsToBeUpdated)
        {
            foreach (string key in forgotten)
            {
                string value = null;
                string type = null;

                PropertyInfo property = _variables.GetType().GetProperty(key);

                switch (property.PropertyType.Name)
                {
                    case "bool":
                        value = (bool) property.GetValue(_variables) ? "1" : "0";
                        type = "b";
                        break;
                    case "string":
                        value = property.GetValue(_variables).ToString();
                        type = "s";
                        break;
                    case "int":
                        value = property.GetValue(_variables).ToString();
                        type = "i";
                        break;
                    case "double":
                        value = property.GetValue(_variables).ToString();
                        type = "d";
                        break;
                }
                needsToBeUpdated.Add(type + key + "=" + value);
            }
            return needsToBeUpdated;
        }

        /// <summary>
        /// Reads every line of the config and adds it in the correct dictionary.
        /// </summary>
        public void LoadConfig()
        {
            List<string> cfg = new(File.ReadAllLines(_cfgPath));
            List<string> defaultConfig = _availableVariable.ToList();

            foreach (string line in File.ReadAllLines(_cfgPath))
            {
                string[] split = line.Split("=");
                if(_availableVariable.Contains(split[0][1..]))
                {
                    PropertyInfo variable = _variables.GetType().GetProperty(split[0][1..]);
                    try
                    {
                        if (split[0].StartsWith("b"))
                        {
                            variable.SetValue(_variables, split[1] == "1");
                        }
                        else if (split[0].StartsWith("d"))
                        {
                            variable.SetValue(_variables, Double.Parse(split[1]));
                        }
                        else if (split[0].StartsWith("i"))
                        {
                            variable.SetValue(_variables, int.Parse(split[1]));
                        }
                        else if (split[0].StartsWith("s"))
                        {
                            variable.SetValue(_variables, split[1]);                         
                        }
                    }
                    catch (FormatException)
                    {
                        cfg[cfg.IndexOf(line)] = split[0] + "=" + variable.GetValue(split[0][1..]);
                    }
                    defaultConfig.Remove(split[0][1..]);
                }
                else
                {
                    cfg.Remove(line);
                }
            }

            // Will add values to the config if some were forgotten.
            if (defaultConfig.Count != 0)
            {
                cfg = AddForgottenValuesToConfig(defaultConfig, cfg);
            }

            File.WriteAllLines(_cfgPath, cfg);
            _variables.HasChanged = false;
        }

        /// <summary>
        /// Saves all the changed values to the config file.
        /// </summary>
        public void SaveConfig()
        {
            List<string> oldConfig = new();
            List<string> defaultConfig = _availableVariable.ToList();

            foreach (string line in defaultConfig)
            {
                string value = null;
                string type = null;

                PropertyInfo property = _variables.GetType().GetProperty(line);

                switch (property.PropertyType.Name.ToString())
                {
                    case "Boolean":
                        value = (bool)property.GetValue(_variables) ? "1" : "0";
                        type = "b";
                        break;
                    case "String":
                        value = property.GetValue(_variables).ToString();
                        type = "s";
                        break;
                    case "Int32":
                        value = property.GetValue(_variables).ToString();
                        type = "i";
                        break;
                    case "Double":
                        value = property.GetValue(_variables).ToString();
                        type = "d";
                        break;
                }

                if (type != null)
                {
                    oldConfig.Add(type + line + "=" + value);
                }
            }

            File.WriteAllLines(_cfgPath, oldConfig);
            _variables.HasChanged = false;
            ChangeLanguages();
        }
    }
}
