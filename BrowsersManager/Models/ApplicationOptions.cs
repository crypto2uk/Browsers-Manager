using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BrowsersManager.Models
{
    /// <summary>
    /// Manages application options such as form control values
    /// </summary>
    public class ApplicationOptions
    {
        // Form control values to be persisted
        public string TaskText { get; set; } = string.Empty;
        public string UrlText { get; set; } = string.Empty;
        public int ColumnCount { get; set; } = 2;
        public int RowCount { get; set; } = 2;
        public bool IsTopMost { get; set; } = false;
        
        /// <summary>
        /// Bottom reserved space in pixels for window arrangement
        /// </summary>
        public int BottomReserved { get; set; } = 0;
        
        /// <summary>
        /// Right reserved space in pixels for window arrangement
        /// </summary>
        public int RightReserved { get; set; } = 0;
        
        /// <summary>
        /// Whether to use topmost window setting during window arrangement
        /// </summary>
        public bool UseTopmostDuringArrangement { get; set; } = false;
        
        /// <summary>
        /// Stores the name of the currently open browser group
        /// </summary>
        public string CurrentBrowserGroup { get; set; } = string.Empty;
        
        /// <summary>
        /// Stores the list of selected profile names
        /// </summary>
        public List<string> SelectedProfileNames { get; set; } = new List<string>();

        // File path for storing options
        private static readonly string OptionsFilePath = Path.Combine(
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            "options.json");

        /// <summary>
        /// Loads application options from the options.json file
        /// </summary>
        /// <returns>ApplicationOptions object with loaded values or default values if file not found</returns>
        public static ApplicationOptions Load()
        {
            try
            {
                if (File.Exists(OptionsFilePath))
                {
                    string jsonContent = File.ReadAllText(OptionsFilePath);
                    var options = JsonConvert.DeserializeObject<ApplicationOptions>(jsonContent);
                    return options ?? new ApplicationOptions();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading application options: {ex.Message}");
            }
            
            // Return default options if file doesn't exist or there was an error
            return new ApplicationOptions();
        }

        /// <summary>
        /// Saves application options to the options.json file
        /// </summary>
        public void Save()
        {
            try
            {
                string jsonContent = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(OptionsFilePath, jsonContent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving application options: {ex.Message}");
            }
        }
    }
} 