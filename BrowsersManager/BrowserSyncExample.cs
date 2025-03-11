using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BrowserManager.Services;

namespace BrowserManager.Examples
{
    /// <summary>
    /// 浏览器同步示例类，展示如何使用BrowserSyncManager
    /// </summary>
    public class BrowserSyncExample
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private BrowserSyncManager _syncManager;
        private List<IntPtr> _chromeWindows = new List<IntPtr>();
        private IntPtr _masterWindow = IntPtr.Zero;

        /// <summary>
        /// 初始化浏览器同步示例
        /// </summary>
        public BrowserSyncExample()
        {
            _syncManager = new BrowserSyncManager();
        }

        /// <summary>
        /// 获取当前打开的所有Chrome窗口
        /// </summary>
        /// <returns>Chrome窗口句柄列表</returns>
        public List<IntPtr> FindChromeWindows()
        {
            _chromeWindows.Clear();
            
            EnumWindows((hwnd, lParam) =>
            {
                if (IsWindowVisible(hwnd))
                {
                    System.Text.StringBuilder className = new System.Text.StringBuilder(256);
                    GetClassName(hwnd, className, className.Capacity);
                    
                    if (className.ToString() == "Chrome_WidgetWin_1")
                    {
                        System.Text.StringBuilder title = new System.Text.StringBuilder(256);
                        GetWindowText(hwnd, title, title.Capacity);
                        
                        // 确保是Chrome窗口而不是其他使用Chrome引擎的应用
                        if (title.Length > 0 && !title.ToString().Contains("应用") 
                            && !title.ToString().Contains("Microsoft Edge"))
                        {
                            _chromeWindows.Add(hwnd);
                        }
                    }
                }
                return true;
            }, IntPtr.Zero);
            
            return _chromeWindows;
        }

        /// <summary>
        /// 设置主控窗口
        /// </summary>
        /// <param name="windowHandle">窗口句柄</param>
        public void SetMasterWindow(IntPtr windowHandle)
        {
            if (windowHandle != IntPtr.Zero)
            {
                _masterWindow = windowHandle;
            }
        }

        /// <summary>
        /// 开始同步操作
        /// </summary>
        /// <returns>是否成功开启同步</returns>
        public bool StartSync()
        {
            try
            {
                if (_masterWindow == IntPtr.Zero)
                {
                    MessageBox.Show("请先设置主控窗口", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                
                List<IntPtr> syncWindows = new List<IntPtr>();
                
                // 添加除主控窗口外的所有Chrome窗口
                foreach (IntPtr hwnd in _chromeWindows)
                {
                    if (hwnd != _masterWindow)
                    {
                        syncWindows.Add(hwnd);
                    }
                }
                
                if (syncWindows.Count == 0)
                {
                    MessageBox.Show("没有可同步的浏览器窗口", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                
                // 启动同步
                _syncManager.StartSync(_masterWindow, syncWindows);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动同步失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 停止同步操作
        /// </summary>
        public void StopSync()
        {
            _syncManager.StopSync();
        }

        /// <summary>
        /// 获取同步状态
        /// </summary>
        public bool IsSyncing => _syncManager.IsSyncing;

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _syncManager.Dispose();
        }
    }
} 