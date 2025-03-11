using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BrowsersManager.Services
{
    /// <summary>
    /// Provides functionality for logging operations and reading log files
    /// </summary>
    public class LogService
    {
        private readonly string _logDirectory;
        
        /// <summary>
        /// Initializes a new instance of the LogService class
        /// </summary>
        public LogService()
        {
            // Define the log directory as a subdirectory of the application
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _logDirectory = Path.Combine(baseDirectory, "logs");
            
            // Create the logs directory if it doesn't exist
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }
        
        /// <summary>
        /// Gets the path to the log file for the specified date
        /// </summary>
        /// <param name="date">The date for which to get the log file path</param>
        /// <returns>The full path to the log file</returns>
        public string GetLogFilePath(DateTime date)
        {
            string fileName = $"{date:yyyy-MM-dd}.log";
            return Path.Combine(_logDirectory, fileName);
        }
        
        /// <summary>
        /// Logs an operation asynchronously
        /// </summary>
        /// <param name="operationType">Type of the operation (e.g., "打开浏览器窗口")</param>
        /// <param name="affectedWindows">Affected window identifiers</param>
        /// <param name="taskName">Name of the task</param>
        /// <param name="taskUrl">URL associated with the task</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task LogOperationAsync(string operationType, string affectedWindows, string taskName, string taskUrl)
        {
            try
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {operationType} {affectedWindows} {taskName} {taskUrl}";
                string logFilePath = GetLogFilePath(DateTime.Now);
                
                // Append the log entry to the file
                using (StreamWriter writer = new StreamWriter(logFilePath, true, Encoding.UTF8))
                {
                    await writer.WriteLineAsync(logEntry);
                }
            }
            catch (Exception ex)
            {
                // Log the error to debug output - we don't want to disrupt the application flow
                // if logging fails
                System.Diagnostics.Debug.WriteLine($"Error writing to log: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Reads the log file for the specified date asynchronously
        /// </summary>
        /// <param name="date">The date for which to read logs</param>
        /// <returns>The content of the log file, or empty string if the file doesn't exist</returns>
        public async Task<string> ReadLogFileAsync(DateTime date)
        {
            string logFilePath = GetLogFilePath(date);
            
            if (!File.Exists(logFilePath))
            {
                return string.Empty;
            }
            
            try
            {
                using (StreamReader reader = new StreamReader(logFilePath, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading log file: {ex.Message}");
                return $"Error reading log file: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Gets the list of available log files
        /// </summary>
        /// <returns>List of dates for which logs are available</returns>
        public List<DateTime> GetAvailableLogDates()
        {
            List<DateTime> availableDates = new List<DateTime>();
            
            try
            {
                if (Directory.Exists(_logDirectory))
                {
                    string[] logFiles = Directory.GetFiles(_logDirectory, "*.log");
                    
                    foreach (string logFile in logFiles)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(logFile);
                        if (DateTime.TryParseExact(fileName, "yyyy-MM-dd", 
                            System.Globalization.CultureInfo.InvariantCulture, 
                            System.Globalization.DateTimeStyles.None, 
                            out DateTime logDate))
                        {
                            availableDates.Add(logDate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting available log dates: {ex.Message}");
            }
            
            // Sort dates in descending order (newest first)
            availableDates.Sort((a, b) => b.CompareTo(a));
            
            return availableDates;
        }
    }
} 