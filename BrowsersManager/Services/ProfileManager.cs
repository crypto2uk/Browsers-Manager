using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using BrowsersManager.Models;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text;
using System.Management;
using System.Windows.Forms;

namespace BrowsersManager.Services
{
    /// <summary>
    /// Manages browser profiles and groups
    /// </summary>
    public class ProfileManager
    {
        private readonly string _configDirectory;
        private List<BrowserType> _browserTypes;
        private Dictionary<string, BrowserGroup> _browserGroups;

        // Windows API declarations
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref RECT pvParam, uint fWinIni);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // 窗口状态枚举
        private enum WindowShowState
        {
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
            // 其他状态省略
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // Instance variables for window handles and configuration IDs
        private Dictionary<IntPtr, string> _windowHandleToConfigId;
        private List<BrowserWindowInfo> _browserWindows;

        // 添加 WM_CLOSE 常量
        private const uint WM_CLOSE = 0x0010;

        // 设置窗口位置的常量
        private static readonly IntPtr HWND_TOP = new IntPtr(0);
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const uint SPI_GETWORKAREA = 0x0030;

        // 添加窗口状态常量
        private const int SW_RESTORE = 9;
        private const int SW_MINIMIZE = 6;  // 添加最小化常量

        // 在窗口位置常量部分添加以下常量
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const int SW_SHOWNORMAL = 1;

        public ProfileManager(string configDirectory)
        {
            _configDirectory = configDirectory;
            _windowHandleToConfigId = new Dictionary<IntPtr, string>();
            _browserWindows = new List<BrowserWindowInfo>();
            LoadConfigurations();
        }

        private void LoadConfigurations()
        {
            try
            {
                // 使用BrowserType类的静态方法加载浏览器类型
                _browserTypes = BrowserType.LoadAllBrowserTypes();
                
                // 使用BrowserGroup类的静态方法加载浏览器组
                _browserGroups = BrowserGroup.LoadAllGroups();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to load browser configurations", ex);
            }
        }

        /// <summary>
        /// Gets all browser types
        /// </summary>
        public IReadOnlyList<BrowserType> GetBrowserTypes()
        {
            return _browserTypes.AsReadOnly();
        }

        /// <summary>
        /// Gets all browser groups
        /// </summary>
        public IReadOnlyDictionary<string, BrowserGroup> GetBrowserGroups()
        {
            return _browserGroups;
        }

        /// <summary>
        /// Gets a specific browser group by name
        /// </summary>
        public BrowserGroup GetGroup(string groupName)
        {
            return _browserGroups.TryGetValue(groupName, out var group) ? group : null;
        }

        /// <summary>
        /// Creates a new browser group
        /// </summary>
        public void CreateGroup(string groupName, BrowserGroup group)
        {
            if (_browserGroups.ContainsKey(groupName))
            {
                throw new ArgumentException($"Group {groupName} already exists");
            }

            _browserGroups[groupName] = group;
            
            // 使用静态方法保存组
            BrowserGroup.SaveAllGroups(_browserGroups);
        }

        /// <summary>
        /// Updates an existing browser group
        /// </summary>
        public void UpdateGroup(string groupName, BrowserGroup group)
        {
            if (!_browserGroups.ContainsKey(groupName))
            {
                throw new ArgumentException($"Group {groupName} does not exist");
            }

            _browserGroups[groupName] = group;
            
            // 使用静态方法保存组
            BrowserGroup.SaveAllGroups(_browserGroups);
        }

        /// <summary>
        /// Deletes a browser group
        /// </summary>
        public void DeleteGroup(string groupName)
        {
            if (_browserGroups.Remove(groupName))
            {
                // 使用静态方法保存组
                BrowserGroup.SaveAllGroups(_browserGroups);
            }
        }

        public BrowserType GetBrowserType(string browserTypeName)
        {
            // 从缓存中获取浏览器类型配置
            return _browserTypes.FirstOrDefault(bt => bt.Name == browserTypeName);
        }

        /// <summary>
        /// Scans for all browser windows and updates the window handle to config ID mapping
        /// </summary>
        public void ScanBrowserWindows()
        {
            _windowHandleToConfigId.Clear();
            _browserWindows.Clear();

            // 创建调试输出列表
            List<string> debugMessages = new List<string>();
            debugMessages.Add("===== 浏览器进程命令行参数调试信息 =====");

            // Enumerate all windows and find browser windows
            EnumWindows(EnumWindowCallback, IntPtr.Zero);

            // Process found windows to extract configuration IDs
            foreach (var handle in _windowHandleToConfigId.Keys.ToList())
            {
                uint processId;
                GetWindowThreadProcessId(handle, out processId);

                try
                {
                    using (var process = Process.GetProcessById((int)processId))
                    {
                        // 获取窗口标题以便于调试
                        StringBuilder titleBuilder = new StringBuilder(256);
                        GetWindowText(handle, titleBuilder, titleBuilder.Capacity);
                        string windowTitle = titleBuilder.ToString();

                        string commandLine = GetProcessCommandLine(process);
                        string configId = ExtractConfigId(commandLine);
                        
                        // 添加调试信息
                        debugMessages.Add($"窗口标题: {windowTitle}");
                        debugMessages.Add($"进程ID: {processId}");
                        debugMessages.Add($"命令行: {commandLine}");
                        debugMessages.Add($"提取的配置ID: {configId}");
                        debugMessages.Add("----------------------------------------");
                        
                        if (!string.IsNullOrEmpty(configId))
                        {
                            _windowHandleToConfigId[handle] = configId;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 添加异常信息到调试输出
                    debugMessages.Add($"进程访问错误 (可能已退出): {ex.Message}");
                    debugMessages.Add("----------------------------------------");
                    
                    // Process might have exited, remove it from dictionary
                    _windowHandleToConfigId.Remove(handle);
                }
            }

            // 将调试信息写入文件
            try
            {
                File.WriteAllLines(Path.Combine(_configDirectory, "browser_debug.log"), debugMessages);
            }
            catch (Exception)
            {
                // 忽略日志写入错误
            }

            // Update browser windows list
            UpdateBrowserWindowsList();
        }

        /// <summary>
        /// Updates the browser windows list with current window information
        /// </summary>
        private void UpdateBrowserWindowsList()
        {
            _browserWindows.Clear();

            foreach (var kvp in _windowHandleToConfigId)
            {
                var handle = kvp.Key;
                var configId = kvp.Value;
                
                // 跳过没有配置ID的窗口
                if (string.IsNullOrEmpty(configId))
                    continue;

                // Get window title
                StringBuilder titleBuilder = new StringBuilder(256);
                GetWindowText(handle, titleBuilder, titleBuilder.Capacity);
                string title = titleBuilder.ToString();

                // Get window state
                WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                placement.length = Marshal.SizeOf(placement);
                GetWindowPlacement(handle, ref placement);
                
                string windowState = "未知";
                switch (placement.showCmd)
                {
                    case (int)WindowShowState.Normal:
                        windowState = "正常";
                        break;
                    case (int)WindowShowState.Minimized:
                        windowState = "最小化";
                        break;
                    case (int)WindowShowState.Maximized:
                        windowState = "最大化";
                        break;
                    default:
                        windowState = "其他";
                        break;
                }

                _browserWindows.Add(new BrowserWindowInfo
                {
                    ConfigId = configId,
                    WindowState = windowState,
                    WindowTitle = title,
                    WindowHandle = handle
                });
            }
        }

        /// <summary>
        /// Gets the command line of a process
        /// </summary>
        private string GetProcessCommandLine(Process process)
        {
            try
            {
                using (var searcher = new System.Management.ManagementObjectSearcher(
                    $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}"))
                {
                    using (var objects = searcher.Get())
                    {
                        foreach (var obj in objects)
                        {
                            return obj["CommandLine"]?.ToString() ?? "";
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Handle exception gracefully
            }
            return "";
        }

        /// <summary>
        /// Extracts configuration ID from the command line
        /// </summary>
        private string ExtractConfigId(string commandLine)
        {
            // 查找 --user-data-dir 参数，带引号的格式
            var match = Regex.Match(commandLine, @"--user-data-dir=""([^""]+)""");
            
            // 如果找到带引号的参数
            if (match.Success)
            {
                string userDataDir = match.Groups[1].Value;
                
                // 获取路径的最后一级目录名作为配置 ID
                return Path.GetFileName(userDataDir);
            }
            
            // 尝试不带引号的格式 (作为后备)
            match = Regex.Match(commandLine, @"--user-data-dir=([^\s""]+)");
            if (match.Success)
            {
                string userDataDir = match.Groups[1].Value;
                
                // 获取路径的最后一级目录名作为配置 ID
                return Path.GetFileName(userDataDir);
            }
            
            // 未找到参数，返回空字符串
            return "";
        }

        /// <summary>
        /// Callback for window enumeration
        /// </summary>
        private bool EnumWindowCallback(IntPtr hWnd, IntPtr lParam)
        {
            if (!IsWindowVisible(hWnd))
                return true;

            StringBuilder titleBuilder = new StringBuilder(256);
            GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity);
            string windowTitle = titleBuilder.ToString();

            try
            {
                uint processId = 0;
                GetWindowThreadProcessId(hWnd, out processId);
                
                if (processId > 0)
                {
                    // Get process by ID
                    Process process = Process.GetProcessById((int)processId);
                    if (process != null)
                    {
                        string processPath = process.MainModule?.FileName;
                        
                        // If process path is found, check if it matches any browser path
                        if (!string.IsNullOrEmpty(processPath))
                        {
                            foreach (var browserType in _browserTypes)
                            {
                                if (!string.IsNullOrEmpty(browserType.Path) &&
                                    string.Equals(processPath, browserType.Path, StringComparison.OrdinalIgnoreCase))
                                {
                                    _windowHandleToConfigId[hWnd] = ""; // Temporary placeholder for config ID
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error but continue enumeration
                Console.WriteLine($"Error checking process path for window {hWnd}: {ex.Message}");
            }

            return true;
        }

        /// <summary>
        /// Gets a sorted list of browser windows
        /// </summary>
        /// <returns>List of browser windows sorted by configuration ID (numerically)</returns>
        public List<BrowserWindowInfo> GetBrowserWindows()
        {
            // Scan windows first to ensure we have up-to-date information
            ScanBrowserWindows();
            
            // 过滤掉那些没有窗口状态和窗口标题的重复ConfigId窗口
            var filteredWindows = _browserWindows
                .GroupBy(win => win.ConfigId)
                .SelectMany<IGrouping<string, BrowserWindowInfo>, BrowserWindowInfo>(group => 
                {
                    // 如果这个ConfigId只有一个窗口，直接返回
                    if (group.Count() == 1)
                        return group.ToList();
                        
                    // 检查组内是否有带状态和标题的窗口
                    var validWindows = group.Where(win => 
                        !string.IsNullOrEmpty(win.WindowState) && 
                        !string.IsNullOrEmpty(win.WindowTitle)).ToList();
                        
                    // 如果有带状态和标题的窗口，只返回这些窗口
                    if (validWindows.Any())
                        return validWindows;
                        
                    // 如果所有窗口都没有状态或标题，返回所有窗口
                    return group.ToList();
                });
            
            // Return numerically sorted list
            return filteredWindows
                .OrderBy(window => 
                {
                    // 尝试将 ConfigId 解析为整数进行数字排序
                    if (int.TryParse(window.ConfigId, out int numericId))
                    {
                        return numericId; // 如果是纯数字，按数值排序
                    }
                    else
                    {
                        // 对于非数字的 ConfigId，为了保持排序顺序合理，
                        // 我们返回一个非常大的数，确保它们排在数字之后
                        return int.MaxValue;
                    }
                })
                .ThenBy(window => window.ConfigId) // 对于非数字ID或数字相同的情况，再按字符串排序
                .ToList();
        }

        /// <summary>
        /// Closes all browser windows
        /// </summary>
        /// <returns>Number of windows closed</returns>
        public int CloseAllBrowserWindows()
        {
            // Get all browser windows
            var browserWindows = GetBrowserWindows();
            int closedCount = 0;
            
            // Close each window by sending WM_CLOSE message
            foreach (var window in browserWindows)
            {
                if (window.WindowHandle != IntPtr.Zero)
                {
                    SendMessage(window.WindowHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    closedCount++;
                }
            }
            
            // Refresh the window list
            ScanBrowserWindows();
            
            return closedCount;
        }

        /// <summary>
        /// Arranges browser windows in a grid pattern
        /// </summary>
        /// <param name="windowHandles">List of window handles to arrange</param>
        /// <param name="columnsCount">Number of columns in the grid</param>
        /// <param name="rowsCount">Number of rows in the grid</param>
        /// <param name="bottomReserved">Reserved space at the bottom of screen in pixels (default: 0)</param>
        /// <param name="rightReserved">Reserved space at the right of screen in pixels (default: 0)</param>
        /// <param name="leftReserved">Reserved space at the left of screen in pixels (default: 0)</param>
        /// <param name="topReserved">Reserved space at the top of screen in pixels (default: 0)</param>
        /// <param name="hPadding">Horizontal padding between windows in pixels (default: -15)</param>
        /// <param name="vPadding">Vertical padding between windows in pixels (default: -8)</param>
        /// <returns>Number of windows successfully arranged</returns>
        public int ArrangeWindows(List<IntPtr> windowHandles, int columnsCount, int rowsCount, 
            int bottomReserved = 0, int rightReserved = 0, int leftReserved = -8, int topReserved = 0,
            int hPadding = -15, int vPadding = 0)
        {
            if (windowHandles == null || windowHandles.Count == 0)
                return 0;
            
            if (columnsCount <= 0 || rowsCount <= 0)
                return 0;
            
            int arrangedCount = 0;
            
            // Get the working area of the screen (excludes taskbar)
            RECT workArea = new RECT();
            SystemParametersInfo(SPI_GETWORKAREA, 0, ref workArea, 0);
            
            // Consider reserved spaces in calculations
            int screenWidth = workArea.Right - workArea.Left - rightReserved - leftReserved;
            int screenHeight = workArea.Bottom - workArea.Top - bottomReserved - topReserved;
            
            // Calculate window dimensions - reduce width slightly to decrease gaps
            int windowWidth = (screenWidth / columnsCount) - hPadding;
            int windowHeight = (screenHeight / rowsCount) - vPadding;
            
            // First pass - set all windows to normal to ensure they show in front
            foreach (IntPtr handle in windowHandles)
            {
                if (handle != IntPtr.Zero)
                {
                    // 将窗口设置为正常显示
                    ShowWindow(handle, SW_SHOWNORMAL);
                }
            }
            
            // Arrange windows from left to right, top to bottom
            for (int i = 0; i < windowHandles.Count; i++)
            {
                IntPtr handle = windowHandles[i];
                
                // Check if window handle is valid
                if (handle == IntPtr.Zero)
                    continue;
                
                // First restore window to normal state (from minimized or maximized)
                ShowWindow(handle, SW_RESTORE);
                
                // Calculate position based on index
                int row = i / columnsCount;
                int col = i % columnsCount;
                
                if (row >= rowsCount)  // Skip if exceeds row count
                    continue;
                    
                // Adjust position to account for padding and reserved spaces
                int x = workArea.Left + leftReserved + (col * (windowWidth + hPadding));
                int y = workArea.Top + topReserved + (row * (windowHeight + vPadding));
                
                // Set window position and size
                if (SetWindowPos(handle, HWND_TOP, x, y, windowWidth, windowHeight, SWP_SHOWWINDOW))
                {
                    arrangedCount++;
                }
            }
            
            return arrangedCount;
        }

        /// <summary>
        /// Minimizes all browser windows
        /// </summary>
        /// <returns>Number of windows minimized</returns>
        public int MinimizeAllBrowserWindows()
        {
            int minimizedCount = 0;
            
            // Skip full browser scan if we already have window handles
            // Only scan if the list is empty
            if (_windowHandleToConfigId.Count == 0)
            {
                EnumWindows(EnumWindowCallback, IntPtr.Zero);
                
                // Process found windows to identify browser windows
                foreach (var handle in _windowHandleToConfigId.Keys.ToList())
                {
                    uint processId;
                    GetWindowThreadProcessId(handle, out processId);
                    
                    try
                    {
                        using (var process = Process.GetProcessById((int)processId))
                        {
                            string commandLine = GetProcessCommandLine(process);
                            string configId = ExtractConfigId(commandLine);
                            
                            if (!string.IsNullOrEmpty(configId))
                            {
                                _windowHandleToConfigId[handle] = configId;
                            }
                        }
                    }
                    catch
                    {
                        // Process might have exited, remove it from dictionary
                        _windowHandleToConfigId.Remove(handle);
                    }
                }
            }
            
            // Use direct window handles instead of rebuilding the full BrowserWindows list
            var windowHandles = _windowHandleToConfigId.Keys.ToList();
            
            // Minimize windows in a single pass
            foreach (var handle in windowHandles)
            {
                if (handle != IntPtr.Zero)
                {
                    // Check if window is already minimized to avoid unnecessary API calls
                    WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                    placement.length = Marshal.SizeOf(placement);
                    
                    if (GetWindowPlacement(handle, ref placement) && 
                        placement.showCmd != (int)WindowShowState.Minimized)
                    {
                        if (ShowWindow(handle, SW_MINIMIZE))
                        {
                            minimizedCount++;
                        }
                    }
                }
            }
            
            // We don't need a full scan update since we're just minimizing
            // Just update the window state in our existing collection
            foreach (var browserWindow in _browserWindows)
            {
                browserWindow.WindowState = "最小化";
            }
            
            return minimizedCount;
        }

        /// <summary>
        /// Closes a specific browser window by its handle
        /// </summary>
        /// <param name="windowHandle">The window handle to close</param>
        /// <returns>True if the window was successfully closed, false otherwise</returns>
        public bool CloseBrowserWindow(IntPtr windowHandle)
        {
            // Validate the window handle
            if (windowHandle == IntPtr.Zero)
                return false;
            
            // Check if the window is valid and visible
            if (!IsWindowVisible(windowHandle))
                return false;
            
            // Send WM_CLOSE message to the window
            bool result = SendMessage(windowHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero;
            
            // Refresh the window list
            ScanBrowserWindows();
            
            return result;
        }

        /// <summary>
        /// Closes multiple browser windows by their handles
        /// </summary>
        /// <param name="windowHandles">List of window handles to close</param>
        /// <returns>Number of windows successfully closed</returns>
        public int CloseBrowserWindows(List<IntPtr> windowHandles)
        {
            if (windowHandles == null || windowHandles.Count == 0)
                return 0;
            
            int closedCount = 0;
            
            // Process each window handle in the list
            foreach (var handle in windowHandles)
            {
                // Skip invalid handles
                if (handle == IntPtr.Zero)
                    continue;
                
                // Skip windows that are not visible
                if (!IsWindowVisible(handle))
                    continue;
                
                // Send WM_CLOSE message to the window
                if (SendMessage(handle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero)
                {
                    closedCount++;
                }
            }
            
            // Refresh the window list if any windows were closed
            if (closedCount > 0)
            {
                ScanBrowserWindows();
            }
            
            return closedCount;
        }

    }

    /// <summary>
    /// Represents information about a browser window
    /// </summary>
    public class BrowserWindowInfo
    {
        public string ConfigId { get; set; }
        public string WindowState { get; set; }
        public string WindowTitle { get; set; }
        public IntPtr WindowHandle { get; set; }

        public override string ToString()
        {
            return $"{ConfigId}, {WindowState}, {WindowTitle}, {WindowHandle}";
        }
    }
} 