using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using BrowsersManager.Models;
using Newtonsoft.Json.Linq;


namespace BrowsersManager.Services
{
    public class ProfileCreationService
    {
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        
        [DllImport("user32.dll")]
        private static extern bool GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        
        private const uint WM_CLOSE = 0x0010;
        
        /// <summary>
        /// Creates browser profiles according to the specified parameters
        /// </summary>
        public async Task<List<ProfileItem>> CreateProfilesAsync(string browserType, string profileRange, 
            string userDataDir, string templateProfile)
        {
            // 解析范围文本
            var profilePaths = ParseProfileRange(profileRange);
            var createdProfiles = new List<ProfileItem>();
            var newlyCreatedCount = 0;
            
            // 首先关闭所有现有的浏览器窗口，防止干扰
            CloseBrowserWindows(browserType);
            await Task.Delay(1000); // 等待窗口完全关闭
            
            foreach (var profilePath in profilePaths)
            {
                // 无论配置文件夹是否存在，都将其添加到结果列表中
                createdProfiles.Add(new ProfileItem { ProfileName = profilePath });
                
                // 合并路径作为实际的用户数据目录
                string combinedUserDataDir = Path.Combine(userDataDir, profilePath);
                
                // 检查新配置文件夹是否存在
                if (!Directory.Exists(combinedUserDataDir))
                {
                    newlyCreatedCount++;
                    
                    // 启动浏览器创建配置文件
                    string browserExePath = GetBrowserPath(browserType);
                    await LaunchBrowserAsync(browserExePath, combinedUserDataDir);
                    
                    // 等待足够时间让配置文件创建完成
                    await Task.Delay(1000);
                    
                    // 关闭所有浏览器窗口
                    await EnsureAllBrowserWindowsClosed(browserType);
                    
                    // 如果有指定模板，执行复制
                    if (!string.IsNullOrEmpty(templateProfile))
                    {
                        string templateDir = Path.Combine(userDataDir, templateProfile, "Default");
                        string targetDefaultDir = Path.Combine(combinedUserDataDir, "Default");
                        
                        if (Directory.Exists(templateDir))
                        {
                            await CopyProfileDirectoryWithRetryAsync(templateDir, targetDefaultDir);
                        }
                    }
                    
                    // 设置配置文件显示名称为profilePath
                    await SetProfileNameAsync(combinedUserDataDir, profilePath, userDataDir);
                }
            }
            
            // 可选：记录新创建的配置文件数量
            if (newlyCreatedCount > 0)
            {
                Console.WriteLine($"实际创建了 {newlyCreatedCount} 个新配置文件");
            }
            
            return createdProfiles;
        }
        
        /// <summary>
        /// Parses the profile range string and returns individual profile paths
        /// </summary>
        private List<string> ParseProfileRange(string rangeText)
        {
            var result = new List<string>();
            
            // 按"/"分隔多个配置
            var ranges = rangeText.Split('/');
            
            foreach (var range in ranges)
            {
                // 检查是否包含"-"表示范围
                if (range.Contains("-"))
                {
                    var parts = range.Split('-');
                    if (parts.Length == 2)
                    {
                        string startPart = parts[0].Trim();
                        string endPart = parts[1].Trim();
                        
                        // 提取数字部分
                        var startMatch = Regex.Match(startPart, @"(\D*)(\d+)$");
                        var endMatch = Regex.Match(endPart, @"(\D*)(\d+)$");
                        
                        if (startMatch.Success && endMatch.Success)
                        {
                            string prefix = startMatch.Groups[1].Value;
                            int startNum = int.Parse(startMatch.Groups[2].Value);
                            int endNum = int.Parse(endMatch.Groups[2].Value);
                            
                            // 生成范围内的所有配置文件名
                            for (int i = startNum; i <= endNum; i++)
                            {
                                result.Add($"{prefix}{i}");
                            }
                        }
                    }
                }
                else
                {
                    // 单个配置文件
                    result.Add(range.Trim());
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Launches the browser with specific profile path
        /// </summary>
        private async Task LaunchBrowserAsync(string browserPath, string combinedUserDataDir)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = browserPath,
                    Arguments = $"--user-data-dir=\"{combinedUserDataDir}\"",
                    UseShellExecute = true
                };
                
                using (var process = Process.Start(startInfo))
                {
                    // 异步等待，避免UI冻结
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                // 记录异常
                Console.WriteLine($"Error launching browser: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Gets all windows that belong to a specific browser by its executable path
        /// </summary>
        private List<IntPtr> GetBrowserWindowsByPath(string browserPath)
        {
            List<IntPtr> browserWindows = new List<IntPtr>();
            
            if (string.IsNullOrEmpty(browserPath) || !File.Exists(browserPath))
            {
                Console.WriteLine($"Invalid browser path: {browserPath}");
                return browserWindows;
            }
            
            try
            {
                // 获取所有匹配此路径的进程
                var processes = Process.GetProcesses()
                    .Where(p => 
                    {
                        try
                        {
                            return p.MainModule?.FileName != null && 
                                   string.Equals(p.MainModule.FileName, browserPath, StringComparison.OrdinalIgnoreCase);
                        }
                        catch
                        {
                            // 如果无法访问进程信息，忽略此进程
                            return false;
                        }
                    })
                    .ToList();
                
                // 对于每个进程，枚举其窗口
                foreach (var process in processes)
                {
                    EnumWindows((hWnd, lParam) =>
                    {
                        // 获取窗口的进程ID
                        uint windowProcessId;
                        GetWindowThreadProcessId(hWnd, out windowProcessId);
                        
                        // 如果窗口属于当前进程并且可见，则添加到列表
                        if (process.Id == windowProcessId && IsWindowVisible(hWnd))
                        {
                            browserWindows.Add(hWnd);
                        }
                        
                        return true;
                    }, IntPtr.Zero);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting browser windows: {ex.Message}");
            }
            
            return browserWindows;
        }
        
        /// <summary>
        /// Closes all browser windows that match the browser type
        /// </summary>
        private void CloseBrowserWindows(string browserType)
        {
            string browserPath = GetBrowserPath(browserType);
            List<IntPtr> windowsToClose = GetBrowserWindowsByPath(browserPath);
            
            // 发送关闭消息到所有匹配的窗口
            foreach (var hWnd in windowsToClose)
            {
                PostMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
            
            Console.WriteLine($"已发送关闭命令给 {windowsToClose.Count} 个浏览器窗口");
        }
        
        /// <summary>
        /// Ensures all browser windows are closed before proceeding
        /// </summary>
        private async Task EnsureAllBrowserWindowsClosed(string browserType)
        {
            int attempts = 0;
            int maxAttempts = 10;
            bool windowsExist = true;
            
            while (windowsExist && attempts < maxAttempts)
            {
                // 关闭当前的浏览器窗口
                CloseBrowserWindows(browserType);
                
                // 等待窗口关闭
                await Task.Delay(1000);
                
                // 检查是否还有匹配的窗口
                windowsExist = CheckForBrowserWindows(browserType);
                
                attempts++;
            }
            
            // 如果仍然有窗口，可能需要强制结束进程
            if (windowsExist)
            {
                Console.WriteLine("警告：部分浏览器窗口未能正常关闭");
                // 可以考虑添加强制结束进程的代码
            }
        }
        
        /// <summary>
        /// Checks if any browser windows matching the browser type exist
        /// </summary>
        private bool CheckForBrowserWindows(string browserType)
        {
            string browserPath = GetBrowserPath(browserType);
            return GetBrowserWindowsByPath(browserPath).Count > 0;
        }
        
        /// <summary>
        /// Copies files from template profile to new profile directory with retry logic
        /// </summary>
        private async Task CopyProfileDirectoryWithRetryAsync(string sourceDir, string targetDir)
        {
            // 确保目标目录存在
            Directory.CreateDirectory(targetDir);
            
            // 复制所有文件
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDir, fileName);
                
                await CopyFileWithRetryAsync(file, destFile);
            }
            
            // 递归复制所有子目录
            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(directory);
                string destDir = Path.Combine(targetDir, dirName);
                await CopyProfileDirectoryWithRetryAsync(directory, destDir);
            }
        }
        
        /// <summary>
        /// Copies a single file with retry logic to handle locked files
        /// </summary>
        private async Task CopyFileWithRetryAsync(string sourceFile, string destFile)
        {
            int retryCount = 0;
            int maxRetries = 5;
            int retryDelayMs = 500;
            
            while (retryCount < maxRetries)
            {
                try
                {
                    File.Copy(sourceFile, destFile, true);
                    return; // 复制成功，退出
                }
                catch (IOException ex) when (IsFileLockException(ex))
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        Console.WriteLine($"无法复制文件 {Path.GetFileName(sourceFile)} 经过 {maxRetries} 次尝试: {ex.Message}");
                        break;
                    }
                    
                    // 等待一段时间后重试
                    await Task.Delay(retryDelayMs * retryCount);
                }
                catch (Exception ex)
                {
                    // 其他非锁定异常，记录但不重试
                    Console.WriteLine($"复制文件 {Path.GetFileName(sourceFile)} 时出错: {ex.Message}");
                    break;
                }
            }
        }
        
        /// <summary>
        /// Determines if an exception is related to a file being locked
        /// </summary>
        private bool IsFileLockException(Exception ex)
        {
            return ex.Message.Contains("being used by another process") || 
                   ex.Message.Contains("访问被拒绝") || 
                   ex.Message.Contains("无法访问") ||
                   ex.Message.Contains("Access is denied");
        }
        
        /// <summary>
        /// Gets the browser executable path based on browser type
        /// </summary>
        private string GetBrowserPath(string browserType)
        {
            // 从BrowserType类获取浏览器路径
            var browser = BrowserType.FindByName(browserType);
            if (browser != null && !string.IsNullOrEmpty(browser.Path) && File.Exists(browser.Path))
            {
                return browser.Path;
            }
            
            // 如果找不到匹配的浏览器类型或路径不存在，使用默认值
            string defaultPath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            Console.WriteLine($"Browser path not found for {browserType}, using default: {defaultPath}");
            return defaultPath;
        }
        
        /// <summary>
        /// Updates the profile name in the Preferences file
        /// </summary>
        private async Task SetProfileNameAsync(string profileDir, string profileName, string userDataDir)
        {
            try
            {
                // 修复路径：Preferences 文件路径
                string prefsPath = Path.Combine(profileDir, "Default", "Preferences");
                // 修复路径：Local State 文件路径（在配置文件目录下）
                string localStatePath = Path.Combine(profileDir, "Local State");
                
                // 打印日志帮助调试
                Console.WriteLine($"尝试访问配置文件: {prefsPath}");
                Console.WriteLine($"尝试访问状态文件: {localStatePath}");
                
                if (!File.Exists(prefsPath))
                {
                    Console.WriteLine($"警告: Preferences 文件不存在");
                }
                
                if (!File.Exists(localStatePath))
                {
                    Console.WriteLine($"警告: Local State 文件不存在");
                    // 尝试使用 userDataDir 中的 Local State
                    localStatePath = Path.Combine(userDataDir, "Local State");
                    Console.WriteLine($"尝试备用状态文件: {localStatePath}");
                }
                
                if (!File.Exists(prefsPath) || !File.Exists(localStatePath))
                {
                    Console.WriteLine($"无法设置配置文件名称：配置文件不完整");
                    return;
                }
                
                // 等待文件被释放
                await Task.Delay(1000);
                
                // 更新Preferences文件
                await UpdateJsonFileAsync(prefsPath, "profile.name", profileName);
                
                // 更新Local State文件中的配置信息
                // 注意：在Local State中，需要使用配置文件的相对路径名作为键
                string profileFolderName = Path.GetFileName(profileDir);
                await UpdateLocalStateAsync(localStatePath, profileFolderName, profileName);
                
                Console.WriteLine($"成功设置配置文件 {profileName} 的显示名称");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"设置配置文件名称时出错: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Updates a JSON value at the specified path
        /// </summary>
        private async Task UpdateJsonFileAsync(string filePath, string jsonPath, object newValue)
        {
            // 使用多次重试机制处理文件锁定情况
            int retryCount = 0;
            int maxRetries = 5;
            
            while (retryCount < maxRetries)
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    JObject obj = JObject.Parse(json);
                    
                    // 按路径定位节点
                    string[] pathParts = jsonPath.Split('.');
                    JToken current = obj;
                    
                    // 导航到倒数第二层
                    for (int i = 0; i < pathParts.Length - 1; i++)
                    {
                        if (current[pathParts[i]] == null)
                        {
                            current[pathParts[i]] = new JObject();
                        }
                        current = current[pathParts[i]];
                    }
                    
                    // 设置最终属性值
                    string lastPart = pathParts[pathParts.Length - 1];
                    current[lastPart] = JToken.FromObject(newValue);
                    
                    // 写回文件
                    File.WriteAllText(filePath, obj.ToString());
                    break;
                }
                catch (IOException ex) when (IsFileLockException(ex))
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        throw;
                    }
                    await Task.Delay(500 * retryCount);
                }
            }
        }
        
        /// <summary>
        /// Updates the profile name in the Local State file
        /// </summary>
        private async Task UpdateLocalStateAsync(string filePath, string profileId, string newName)
        {
            // 使用多次重试机制处理文件锁定情况
            int retryCount = 0;
            int maxRetries = 5;
            
            while (retryCount < maxRetries)
            {
                try
                {
                    Console.WriteLine($"正在更新 Local State 文件: {filePath}");
                    Console.WriteLine($"配置文件ID: {profileId}, 新名称: {newName}");
                    
                    string json = File.ReadAllText(filePath);
                    JObject root = JObject.Parse(json);
                    
                    // 确保路径存在
                    if (root["profile"] == null)
                    {
                        root["profile"] = new JObject();
                    }
                    
                    if (root["profile"]["info_cache"] == null)
                    {
                        root["profile"]["info_cache"] = new JObject();
                    }
                    
                    JObject profiles = (JObject)root["profile"]["info_cache"];
                    
                    // 列出所有现有的配置文件，帮助调试
                    Console.WriteLine("Local State 文件中现有的配置文件:");
                    foreach (var prop in profiles.Properties())
                    {
                        Console.WriteLine($"  - {prop.Name}");
                    }
                    
                    // 检查配置文件是否存在
                    // 注意：配置文件ID可能是路径名"1"，也可能是"Default"
                    // 如果没有找到，尝试使用"Default"
                    if (profiles[profileId] == null)
                    {
                        Console.WriteLine($"配置文件ID '{profileId}' 在 Local State 中不存在");
                        
                        // 如果配置文件是新创建的，它可能在 Local State 中以 "Default" 标识
                        if (profiles["Default"] != null)
                        {
                            Console.WriteLine("尝试更新 'Default' 配置文件");
                            profileId = "Default";
                        }
                        else
                        {
                            Console.WriteLine("创建新的配置文件条目");
                            profiles[profileId] = new JObject();
                        }
                    }
                    
                    // 设置名称
                    profiles[profileId]["name"] = newName;
                    profiles[profileId]["is_using_default_name"] = false;
                    
                    // 如果 profiles_order 存在，确保此配置文件在其中
                    JArray profilesOrder = root["profile"]["profiles_order"] as JArray;
                    if (profilesOrder != null)
                    {
                        bool found = false;
                        foreach (var item in profilesOrder)
                        {
                            if (item.ToString() == profileId)
                            {
                                found = true;
                                break;
                            }
                        }
                        
                        if (!found)
                        {
                            profilesOrder.Add(profileId);
                            Console.WriteLine($"将配置文件 '{profileId}' 添加到配置文件顺序中");
                        }
                    }
                    
                    // 写回文件
                    File.WriteAllText(filePath, root.ToString());
                    Console.WriteLine("成功更新 Local State 文件");
                    break;
                }
                catch (IOException ex) when (IsFileLockException(ex))
                {
                    retryCount++;
                    Console.WriteLine($"文件锁定，第 {retryCount} 次重试");
                    if (retryCount >= maxRetries)
                    {
                        Console.WriteLine("达到最大重试次数，抛出异常");
                        throw;
                    }
                    await Task.Delay(500 * retryCount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"更新 Local State 文件时发生异常: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Copies browser profile folders from source to destination folders
        /// </summary>
        /// <param name="sourcePathWithRange">Source path with range (e.g. C:\Users\name\AppData\Local\Google\Chrome\User Data\Profile 1-Profile 20)</param>
        /// <param name="destinationFolders">List of destination folder paths</param>
        /// <returns>Number of successfully copied profiles</returns>
        public async Task<int> CopyProfileFoldersAsync(string sourcePathWithRange, List<string> destinationFolders)
        {
            if (string.IsNullOrEmpty(sourcePathWithRange) || destinationFolders == null || destinationFolders.Count == 0)
            {
                Console.WriteLine("源路径或目标文件夹参数无效");
                return 0;
            }

            try
            {
                // 解析源文件夹路径和范围
                List<string> sourceFolders = ParseSourcePathWithRange(sourcePathWithRange);
                Console.WriteLine($"已解析 {sourceFolders.Count} 个源文件夹");

                // 确定需要复制的文件夹数量（取源和目标的最小值）
                int copyCount = Math.Min(sourceFolders.Count, destinationFolders.Count);
                Console.WriteLine($"将复制 {copyCount} 个配置文件夹");

                int successCount = 0;

                // 一一对应复制文件夹
                for (int i = 0; i < copyCount; i++)
                {
                    string sourceFolder = sourceFolders[i];
                    string destinationFolder = destinationFolders[i];

                    // 确保源文件夹存在
                    if (!Directory.Exists(sourceFolder))
                    {
                        Console.WriteLine($"源文件夹不存在: {sourceFolder}");
                        continue;
                    }

                    Console.WriteLine($"正在复制: {sourceFolder} -> {destinationFolder}");

                    try
                    {
                        // 确保目标目录存在
                        Directory.CreateDirectory(destinationFolder);

                        // 执行文件夹复制
                        await CopyProfileDirectoryWithRetryAsync(sourceFolder, destinationFolder);
                        successCount++;
                        
                        Console.WriteLine($"成功复制配置文件夹: {Path.GetFileName(sourceFolder)}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"复制文件夹时出错: {sourceFolder} -> {destinationFolder}");
                        Console.WriteLine($"异常: {ex.Message}");
                    }
                }

                return successCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"复制配置文件夹时发生异常: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Parses the source path with range into a list of individual folder paths
        /// </summary>
        /// <param name="sourcePathWithRange">Source path with range (e.g. C:\Users\name\AppData\Local\Google\Chrome\User Data\Profile 1-Profile 20)</param>
        /// <returns>List of individual source folder paths</returns>
        private List<string> ParseSourcePathWithRange(string sourcePathWithRange)
        {
            var result = new List<string>();

            try
            {
                // 查找最后一个路径分隔符位置
                int lastSeparatorIndex = Math.Max(
                    sourcePathWithRange.LastIndexOf('\\'),
                    sourcePathWithRange.LastIndexOf('/'));

                if (lastSeparatorIndex <= 0)
                {
                    // 无效的路径格式
                    Console.WriteLine($"无效的路径格式: {sourcePathWithRange}");
                    return result;
                }

                // 分割基础路径和配置文件范围部分
                string basePath = sourcePathWithRange.Substring(0, lastSeparatorIndex);
                string rangePart = sourcePathWithRange.Substring(lastSeparatorIndex + 1);

                Console.WriteLine($"基础路径: {basePath}");
                Console.WriteLine($"范围部分: {rangePart}");

                // 使用现有的配置文件范围解析方法
                List<string> profilePaths = ParseProfileRange(rangePart);

                // 组合完整路径
                foreach (var profilePath in profilePaths)
                {
                    result.Add(Path.Combine(basePath, profilePath));
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解析源路径范围时出错: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Generates browser avatars for the specified profiles
        /// </summary>
        /// <param name="profileConfigs">List of profiles with configID and configPath</param>
        /// <returns>Number of successfully processed profiles</returns>
        public async Task<int> GenerateBrowserAvatarsAsync(List<Dictionary<string, string>> profileConfigs)
        {
            if (profileConfigs == null || profileConfigs.Count == 0)
            {
                Console.WriteLine("配置列表为空或无效");
                return 0;
            }

            int successCount = 0;
            ImageGenerator imageGenerator = new ImageGenerator();

            foreach (var config in profileConfigs)
            {
                try
                {
                    // Validate required keys exist
                    if (!config.ContainsKey("configID") || !config.ContainsKey("configPath"))
                    {
                        Console.WriteLine("配置参数缺少必要的键: configID 或 configPath");
                        continue;
                    }

                    string configID = config["configID"];
                    string configPath = config["configPath"];

                    // Ensure the config path exists
                    if (!Directory.Exists(configPath))
                    {
                        Console.WriteLine($"配置路径不存在: {configPath}");
                        continue;
                    }

                    Console.WriteLine($"正在处理配置ID: {configID}, 路径: {configPath}");

                    // Create Avatars directory if it doesn't exist
                    string avatarsDir = Path.Combine(configPath, "Avatars");
                    Directory.CreateDirectory(avatarsDir);

                    // Define the avatar image path
                    string avatarImagePath = Path.Combine(avatarsDir, "avatar_origami_cat.png");

                    // Generate the image
                    Console.WriteLine($"生成头像图像: {avatarImagePath}");
                    bool imageGenerated = imageGenerator.GenerateImage(configID, avatarImagePath);
                    if (!imageGenerated)
                    {
                        Console.WriteLine($"为配置ID {configID} 生成头像失败");
                        continue;
                    }

                    // // Update the Preferences file
                    // string prefsPath = Path.Combine(configPath, "Default", "Preferences");
                    // if (!File.Exists(prefsPath))
                    // {
                    //     Console.WriteLine($"Preferences 文件不存在: {prefsPath}");
                    //     continue;
                    // }

                    // // Set the avatar index in the Preferences file
                    // Console.WriteLine($"更新配置文件 {prefsPath} 中的头像索引为 27");
                    // await UpdateJsonFileAsync(prefsPath, "profile.avatar_index", 27);

                    successCount++;
                    Console.WriteLine($"成功为配置ID {configID} 生成并设置头像");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"处理配置时出错: {ex.Message}");
                }
            }

            return successCount;
        }
    }
} 