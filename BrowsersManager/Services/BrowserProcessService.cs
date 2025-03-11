using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using BrowsersManager.Models;
using System.IO;

namespace BrowsersManager.Services
{
    public class BrowserProcessService
    {
        /// <summary>
        /// Launches a browser instance with specified profile and URL
        /// </summary>
        /// <param name="browser">Browser configuration including profile and startup parameters</param>
        /// <param name="profilePath">Path to the browser profile</param>
        /// <param name="url">URL to open in the browser</param>
        /// <param name="lang">Language setting for the browser</param>
        /// <param name="userAgent">User agent string for the browser</param>
        /// <param name="otherParameters">Additional command line parameters</param>
        /// <param name="proxyServer">Proxy server settings</param>
        /// <param name="userDataDir">Directory for user data</param>
        /// <returns>Process object of launched browser</returns>
        public async Task<Process> LaunchBrowserAsync(
            BrowserType browser, 
            string profilePath, 
            string url = null,
            string lang = null,
            string userAgent = null,
            string otherParameters = null,
            string proxyServer = null,
            string userDataDir = null)
        {
            try
            {
                // 创建合并后的用户数据目录路径
                string combinedUserDataDir;
                if (!string.IsNullOrEmpty(userDataDir))
                {
                    combinedUserDataDir = Path.Combine(userDataDir, profilePath);
                }
                else
                {
                    // 如果未提供用户数据目录，则使用默认路径
                    combinedUserDataDir = profilePath;
                }
                
                // 初始化参数字符串，仅包含 user-data-dir
                var arguments = $"--user-data-dir=\"{combinedUserDataDir}\"";
                
                // 添加可选参数
                if (!string.IsNullOrEmpty(lang))
                {
                    arguments += $" --lang={lang}";
                }
                
                if (!string.IsNullOrEmpty(userAgent))
                {
                    arguments += $" --user-agent=\"{userAgent}\"";
                }
                
                if (!string.IsNullOrEmpty(proxyServer))
                {
                    arguments += $" --proxy-server=\"{proxyServer}\"";
                }
                
                if (!string.IsNullOrEmpty(otherParameters))
                {
                    arguments += $" {otherParameters}";
                }
                
                if (!string.IsNullOrEmpty(url))
                {
                    arguments += $" \"{url}\"";
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = browser.Path,
                    Arguments = arguments,
                    UseShellExecute = false
                };

                await Task.Yield();
                var process = Process.Start(startInfo);
                Console.WriteLine($"Browser launched with profile: {profilePath}");
                return process;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to launch browser: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Launches multiple browser instances based on group configuration
        /// </summary>
        /// <param name="group">Browser group configuration</param>
        /// <param name="url">URL to open in the browsers</param>
        /// <param name="selectedProfiles">Optional list of profile names to launch. If null, all profiles in the group will be launched</param>
        /// <returns>Number of successfully launched browsers</returns>
        public async Task<int> LaunchBrowserGroupAsync(BrowserGroup group, string url = null, string[] selectedProfiles = null)
        {
            int successCount = 0;
            
            // Get browser type object based on group's browserType name
            BrowserType browserType = BrowserType.FindByName(group.BrowserType);
            if (browserType == null)
            {
                throw new ArgumentException($"Browser type '{group.BrowserType}' not found");
            }
            
            foreach (var profileItem in group.Items)
            {
                // Skip if selectedProfiles is provided and current profile is not in the list
                if (selectedProfiles != null && !selectedProfiles.Contains(profileItem.ProfileName))
                {
                    continue;
                }
                
                try
                {
                    await LaunchBrowserAsync(
                        browserType, 
                        profileItem.ProfileName, 
                        url,
                        string.IsNullOrWhiteSpace(profileItem.Language) ? group.DefaultLanguage : profileItem.Language,
                        string.IsNullOrWhiteSpace(profileItem.UserAgent) ? group.DefaultUserAgent : profileItem.UserAgent,
                        string.IsNullOrWhiteSpace(profileItem.OtherParameters) ? group.OtherParameters : profileItem.OtherParameters,
                        null, // Proxy server is not supported yet
                        group.UserDataDir); // Use group's user data directory
                        
                    successCount++;
                    
                    // Add delay between launches to prevent system overload
                    await Task.Delay(200);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to launch browser in group {group.Description}: {ex.Message}");
                }
            }

            return successCount;
        }
        
        /// <summary>
        /// Launches multiple browser instances based on group name
        /// </summary>
        /// <param name="groupName">Name of the browser group</param>
        /// <param name="url">URL to open in the browsers</param>
        /// <param name="selectedProfiles">Optional list of profile names to launch. If null, all profiles in the group will be launched</param>
        /// <returns>Number of successfully launched browsers</returns>
        public async Task<int> LaunchBrowserGroupAsync(string groupName, string url = null, string[] selectedProfiles = null)
        {
            BrowserGroup group = BrowserGroup.LoadGroup(groupName);
            if (group == null)
            {
                throw new ArgumentException($"Browser group '{groupName}' not found");
            }
            
            return await LaunchBrowserGroupAsync(group, url, selectedProfiles);
        }
    }
} 