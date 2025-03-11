using System;
using System.Windows.Forms;
using System.IO;
using BrowsersManager.Services;
using BrowsersManager.Models;
using BrowsersManager.Views;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Diagnostics;

#pragma warning disable CS4014 // 因为我们确实不想等待这些异步更新UI的操作完成

namespace BrowsersManager
{
    public partial class MainForm : Form
    {
        private readonly ProfileManager _profileManager;
        private readonly BrowserProcessService _browserProcessService;
        private readonly LogService _logService;
        private System.Windows.Forms.Timer _refreshTimer;
        private CancellationTokenSource _refreshCts;
        private bool _isRefreshing = false;
        private ApplicationOptions _options;
        
        // 保存最近选择的分组和浏览器列表的实例变量
        private string _lastSelectedGroupName;
        private List<ProfileItem> _lastSelectedProfiles = new List<ProfileItem>();
        

        public MainForm()
        {
            InitializeComponent();
            
            // Initialize ProfileManager with the executable directory path
            string configPath = Path.GetDirectoryName(Application.ExecutablePath);
            _profileManager = new ProfileManager(configPath);
            
            _browserProcessService = new BrowserProcessService();
            
            // Initialize LogService
            _logService = new LogService();
            
            InitializeControls();
            
            // 注册窗体关闭事件
            this.FormClosing += MainForm_FormClosing;
            
            // 注册窗体加载事件
            this.Load += MainForm_Load;

        }

        private void InitializeControls()
        {
            // 配置数据网格 - 用于显示已打开的浏览器窗口
            dataGridViewBrowsers.AllowUserToAddRows = false;
            dataGridViewBrowsers.AllowUserToDeleteRows = false;
            dataGridViewBrowsers.ReadOnly = true;
            dataGridViewBrowsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewBrowsers.MultiSelect = true;
            
            // 添加这一行来防止自动生成列
            dataGridViewBrowsers.AutoGenerateColumns = false;

            // 确保行头可见，并设置合适的宽度
            dataGridViewBrowsers.RowHeadersVisible = true;
            dataGridViewBrowsers.RowHeadersWidth = 50;

            // 添加双击事件处理
            dataGridViewBrowsers.CellDoubleClick += DataGridViewBrowsers_CellDoubleClick;
            
            // 添加行绘制事件处理
            dataGridViewBrowsers.RowPostPaint += DataGridViewBrowsers_RowPostPaint;

            // 更新列以匹配 BrowserWindowInfo 属性
            dataGridViewBrowsers.Columns.Clear();
            dataGridViewBrowsers.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn 
                { 
                    Name = "ConfigId",
                    HeaderText = "浏览器ID",
                    DataPropertyName = "ConfigId",
                    FillWeight = 50
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "WindowState",
                    HeaderText = "窗口状态",
                    DataPropertyName = "WindowState",
                    FillWeight = 50
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "WindowTitle",
                    HeaderText = "窗口标题",
                    DataPropertyName = "WindowTitle",
                    FillWeight = 200
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "WindowHandle",
                    HeaderText = "窗口句柄",
                    DataPropertyName = "WindowHandle",
                    FillWeight = 100
                }
            });

            // 初始化定时器
            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 5000; // 改为5秒
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        private async void btnOpenBrowsers_Click(object sender, EventArgs e)
        {
            try
            {
                // 打开选择分组的窗体，获取用户选择的组和浏览器
                using (var selectGroupView = new SelectGroupView())
                {
                    if (selectGroupView.ShowDialog() == DialogResult.OK)
                    {
                        // 获取用户选择的组名
                        string groupName = selectGroupView.SelectedGroupName;
                        
                        // 获取用户选择的配置文件列表
                        List<ProfileItem> selectedProfiles = selectGroupView.SelectedProfiles;
                        
                        // 保存到实例变量以便复用
                        _lastSelectedGroupName = groupName;
                        _lastSelectedProfiles = new List<ProfileItem>(selectedProfiles);
                        
                        // 保存当前分组名称到应用程序选项
                        _options.CurrentBrowserGroup = groupName;
                        // 保存选择的配置文件名列表
                        _options.SelectedProfileNames = selectedProfiles.Select(p => p.ProfileName).ToList();
                        _options.Save();
                        
                        // 提取输入的URL（如果有输入框的话）
                        string url = null;
                        if (textBoxUrl != null && !string.IsNullOrWhiteSpace(textBoxUrl.Text))
                        {
                            url = textBoxUrl.Text;
                        }
                        
                        // 获取任务名称
                        string taskName = textBoxTask != null ? textBoxTask.Text : string.Empty;
                        
                        // 启动浏览器组
                        btnOpenBrowsers.Enabled = false;
                        btnOpenBrowsers.Text = "启动中...";
                        
                        // 使用修改后的 LaunchBrowserGroupAsync 方法启动浏览器
                        int count;
                        if (selectedProfiles.Count > 0)
                        {
                            // 将 ProfileItem 列表转换为 profileName 字符串数组
                            string[] profileNames = selectedProfiles.Select(p => p.ProfileName).ToArray();
                            
                            // 启动用户选择的特定配置文件
                            count = await _browserProcessService.LaunchBrowserGroupAsync(groupName, url, profileNames);
                            
                            await _logService.LogOperationAsync("打开选定浏览器窗口", 
                                $"{groupName}，选中{selectedProfiles.Count}个窗口", 
                                taskName, 
                                url ?? string.Empty);
                        }
                        else
                        {
                            // 启动整个组
                            count = await _browserProcessService.LaunchBrowserGroupAsync(groupName, url);
                            await _logService.LogOperationAsync("打开浏览器窗口", 
                                $"{groupName}，共{count}个窗口", 
                                taskName, 
                                url ?? string.Empty);
                        }
                        
                        btnOpenBrowsers.Text = $"已启动 {count} 个浏览器";
                        await Task.Delay(2000); // 短暂显示结果
                        btnOpenBrowsers.Text = "打开浏览器";
                        btnOpenBrowsers.Enabled = true;
                        
                        // 刷新窗口列表
                        _ = RefreshOpenBrowsersListAsync();
                        
                        // 刷新日志显示
                        LoadTodayLogAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动浏览器失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnOpenBrowsers.Text = "打开浏览器/链接";
                btnOpenBrowsers.Enabled = true;
            }
        }

        private void menuItemBrowserGroup_Click(object sender, EventArgs e)
        {
            try
            {
                // 创建SelectGroupView窗体用于编辑分组
                var selectGroupView = new SelectGroupView();
                
                // 使用非模态方式打开窗口
                selectGroupView.Show(this);
                
                // 注册窗体关闭事件，确保资源被释放
                selectGroupView.FormClosed += (s, args) => 
                {
                    // 窗体关闭时记录日志
                    _logService.LogOperationAsync("编辑浏览器分组", "关闭分组编辑窗口", "", "");
                    
                    // 释放资源
                    selectGroupView.Dispose();
                };
                
                // 记录打开操作
                _logService.LogOperationAsync("编辑浏览器分组", "打开分组编辑窗口", "", "");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开分组编辑窗口失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // 异步刷新方法
        private async void RefreshTimer_Tick(object sender, EventArgs e)
        {
            // 如果已经在刷新中，跳过本次刷新
            if (_isRefreshing)
                return;
            
            await RefreshOpenBrowsersListAsync();
        }

        // 修改刷新方法，修复返回值语法错误
        private async Task RefreshOpenBrowsersListAsync()
        {
            // 如果已经在刷新中，直接返回完成的任务
            if (_isRefreshing)
            {
                await Task.CompletedTask;
                return;
            }
            
            var tcs = new TaskCompletionSource<bool>();
            
            _isRefreshing = true;
            
            try
            {
                // 取消之前的刷新操作
                if (_refreshCts != null)
                {
                    _refreshCts.Cancel();
                    _refreshCts.Dispose();
                }
                
                _refreshCts = new CancellationTokenSource();
                CancellationToken token = _refreshCts.Token;
                
                // 在后台线程获取和处理窗口列表
                var refreshTask = Task.Run(async () => 
                {
                    try
                    {
                        var browserWindows = await Task.Run(() => _profileManager.GetBrowserWindows(), token);
                        
                        // 如果已取消，直接返回
                        if (token.IsCancellationRequested)
                        {
                            tcs.SetResult(false);
                            return;
                        }
                            
                        // 重新启用变更检测 - 如果没有变化则不更新UI
                        bool needsUpdate = await Task.Run(() => HasWindowListChanged(browserWindows), token);
                        
                        if (!needsUpdate || token.IsCancellationRequested)
                        {
                            tcs.SetResult(false);
                            return;
                        }
                            
                        // 准备数据源
                        var dataSource = await Task.Run(() => 
                            browserWindows.Select(w => new {
                                ConfigId = w.ConfigId,
                                WindowState = w.WindowState,
                                WindowTitle = w.WindowTitle,
                                WindowHandle = w.WindowHandle.ToString()
                            }).ToList(), token);
                        
                        if (token.IsCancellationRequested)
                        {
                            tcs.SetResult(false);
                            return;
                        }
                            
                        // 转换为object列表
                        var objectDataSource = dataSource.Cast<object>().ToList();
                        
                        // 在UI线程上更新界面
                        await UpdateUIAsync(objectDataSource, token);
                        
                        // 标记刷新已完成
                        tcs.SetResult(true);
                    }
                    catch (OperationCanceledException)
                    {
                        // 操作被取消，忽略异常
                        tcs.SetResult(false);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"刷新窗口列表错误: {ex.Message}");
                        tcs.SetException(ex);
                    }
                    finally
                    {
                        _isRefreshing = false;
                    }
                });
                
                // 等待任务完成
                await tcs.Task;
            }
            catch (Exception ex)
            {
                _isRefreshing = false;
                System.Diagnostics.Debug.WriteLine($"刷新窗口列表启动错误: {ex.Message}");
                throw; // 重新抛出异常而不是返回任务
            }
        }

        // 改进的UI更新方法，正确处理选中行
        private async Task UpdateUIAsync(List<object> dataSource, CancellationToken token)
        {
            // 如果窗体已关闭，直接返回
            if (this.IsDisposed || !this.IsHandleCreated)
                return;
            
            // 确保在UI线程上执行
            if (this.InvokeRequired)
            {
                await Task.Factory.FromAsync(
                    this.BeginInvoke(new Action<List<object>, CancellationToken>((ds, t) => UpdateUIAsync(ds, t)), 
                    dataSource, token),
                    ar => this.EndInvoke(ar));
                return;
            }
            
            // 检查是否已取消
            if (token.IsCancellationRequested)
                return;
            
            try
            {
                // 暂停布局以减少闪烁
                dataGridViewBrowsers.SuspendLayout();
                
                try
                {
                    // 保存当前选中的行索引和句柄
                    List<int> selectedIndices = new List<int>();
                    Dictionary<string, int> handleToIndex = new Dictionary<string, int>();
                    
                    // 记录当前选中行的索引
                    foreach (DataGridViewRow row in dataGridViewBrowsers.SelectedRows)
                    {
                        selectedIndices.Add(row.Index);
                        
                        // 同时记录窗口句柄到索引的映射
                        string handle = row.Cells["WindowHandle"].Value?.ToString();
                        if (!string.IsNullOrEmpty(handle) && !handleToIndex.ContainsKey(handle))
                        {
                            handleToIndex.Add(handle, row.Index);
                        }
                    }
                    
                    // 创建BindingSource - 这实现了IBindingList
                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = dataSource;
                    
                    // 更新数据源
                    dataGridViewBrowsers.DataSource = bindingSource;
                    
                    // 先清除所有选择
                    dataGridViewBrowsers.ClearSelection();
                    
                    // 恢复选中状态 - 优先尝试保持相同的索引位置
                    if (selectedIndices.Count > 0)
                    {
                        foreach (int index in selectedIndices)
                        {
                            // 确保索引在有效范围内
                            if (index < dataGridViewBrowsers.Rows.Count)
                            {
                                dataGridViewBrowsers.Rows[index].Selected = true;
                            }
                        }
                    }
                    
                    // 如果没有成功选中任何行（例如行数变化），尝试按窗口句柄匹配
                    if (dataGridViewBrowsers.SelectedRows.Count == 0 && handleToIndex.Count > 0)
                    {
                        for (int i = 0; i < dataGridViewBrowsers.Rows.Count; i++)
                        {
                            if (token.IsCancellationRequested)
                                return;
                                
                            string handle = dataGridViewBrowsers.Rows[i].Cells["WindowHandle"].Value?.ToString();
                            if (handleToIndex.ContainsKey(handle))
                            {
                                dataGridViewBrowsers.Rows[i].Selected = true;
                            }
                        }
                    }
                }
                finally
                {
                    // 恢复布局
                    dataGridViewBrowsers.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"更新UI错误: {ex.Message}");
            }
        }

        // 改进的窗口变化检测方法
        private bool HasWindowListChanged(List<BrowserWindowInfo> newWindows)
        {
            try
            {
                // 快速检查行数是否匹配
                if (dataGridViewBrowsers.Rows.Count != newWindows.Count)
                    return true;
                    
                // 获取当前显示的窗口句柄列表
                var currentHandles = new List<string>();
                for (int i = 0; i < dataGridViewBrowsers.Rows.Count; i++)
                {
                    var handle = dataGridViewBrowsers.Rows[i].Cells["WindowHandle"].Value?.ToString();
                    if (handle != null)
                        currentHandles.Add(handle);
                }
                
                // 获取新窗口的句柄列表
                var newHandles = newWindows.Select(w => w.WindowHandle.ToString()).ToList();
                
                // 比较窗口句柄列表（顺序敏感）
                if (!currentHandles.SequenceEqual(newHandles))
                    return true;
                    
                // 如果句柄一致，检查其他属性
                for (int i = 0; i < dataGridViewBrowsers.Rows.Count; i++)
                {
                    var row = dataGridViewBrowsers.Rows[i];
                    var newWindow = newWindows[i];
                    
                    // 检查关键属性是否变化
                    if (row.Cells["ConfigId"].Value?.ToString() != newWindow.ConfigId ||
                        row.Cells["WindowState"].Value?.ToString() != newWindow.WindowState ||
                        row.Cells["WindowTitle"].Value?.ToString() != newWindow.WindowTitle)
                    {
                        return true;
                    }
                }
                
                // 没有检测到变化
                return false;
            }
            catch (Exception ex)
            {
                // 如果比较过程中出错，返回true以确保更新UI
                System.Diagnostics.Debug.WriteLine($"窗口变化检测错误: {ex.Message}");
                return true;
            }
        }

        // 窗体关闭事件处理
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // 确保在关闭前保存最新选项
                if (_options != null)
                {
                    // 确保获取最新的控件值
                    if (textBoxTask != null) _options.TaskText = textBoxTask.Text;
                    if (textBoxUrl != null) _options.UrlText = textBoxUrl.Text;
                    if (numericUpDownColumn != null) _options.ColumnCount = (int)numericUpDownColumn.Value;
                    if (numericUpDownRow != null) _options.RowCount = (int)numericUpDownRow.Value;
                    
                    // 保存到文件
                    _options.Save();
                }
                
                // 如果是非强制关闭，可以阻止关闭并进行异步清理后再关闭
                if (e.CloseReason == CloseReason.UserClosing || 
                    e.CloseReason == CloseReason.ApplicationExitCall)
                {
                    if (_isRefreshing)
                    {
                        // 如果还在刷新，则延迟关闭
                        e.Cancel = true;
                        CleanupAndClose();
                        return;
                    }
                }
                
                // 立即清理资源
                CleanupResources();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"窗体关闭时出错: {ex.Message}");
                // 不会阻止关闭，继续关闭窗体
            }

        }

        private async void CleanupAndClose()
        {
            try
            {
                // 停止并清理定时器
                if (_refreshTimer != null)
                {
                    _refreshTimer.Stop();
                    _refreshTimer.Tick -= RefreshTimer_Tick;
                }
                
                // 取消任何进行中的刷新操作
                if (_refreshCts != null)
                {
                    _refreshCts.Cancel();
                }
                
                // 等待当前刷新操作完成
                while (_isRefreshing)
                {
                    await Task.Delay(100);
                }
                
                // 清理资源
                CleanupResources();
                
                // 安全地在UI线程上关闭窗体
                if (!IsDisposed && IsHandleCreated)
                {
                    this.Invoke(new Action(() => this.Close()));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"异步清理过程出错: {ex.Message}");
                // 强制关闭
                if (!IsDisposed && IsHandleCreated)
                {
                    this.Invoke(new Action(() => this.Close()));
                }
            }
        }

        private void CleanupResources()
        {
            // 确保所有资源都被释放
            if (_refreshTimer != null)
            {
                _refreshTimer.Stop();
                _refreshTimer.Tick -= RefreshTimer_Tick;
                _refreshTimer.Dispose();
                _refreshTimer = null;
            }
            
            // 取消任何进行中的刷新操作
            if (_refreshCts != null)
            {
                try 
                { 
                    _refreshCts.Cancel();
                    _refreshCts.Dispose();
                }
                catch (ObjectDisposedException) 
                { 
                    // 忽略已释放对象的异常
                }
                _refreshCts = null;
            }
            
            // 将所有数据绑定断开连接
            if (dataGridViewBrowsers != null && !dataGridViewBrowsers.IsDisposed)
            {
                dataGridViewBrowsers.DataSource = null;
                dataGridViewBrowsers.CellDoubleClick -= DataGridViewBrowsers_CellDoubleClick;
            }
            
            // 释放其他资源
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }

        // 添加该方法在程序退出时调用
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            
            // 确保所有本机资源和RCW被完全释放
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void btnArrange_Click(object sender, EventArgs e)
        {
            try
            {

                // 收集所有选中行的索引和窗口句柄
                List<(int Index, IntPtr Handle)> selectedRows = new List<(int, IntPtr)>();
                foreach (DataGridViewRow row in dataGridViewBrowsers.SelectedRows)
                {
                    if (row.Cells["WindowHandle"].Value != null)
                    {
                        string handleStr = row.Cells["WindowHandle"].Value.ToString();
                        if (long.TryParse(handleStr, out long handleValue))
                        {
                            selectedRows.Add((row.Index, new IntPtr(handleValue)));
                        }
                    }
                }

                // 按行索引排序（从上到下）
                selectedRows.Sort((a, b) => a.Index.CompareTo(b.Index));
                
                // 提取排序后的句柄列表
                List<IntPtr> selectedHandles = selectedRows.Select(item => item.Handle).ToList();

                // 检查是否有选中的窗口
                if (selectedHandles.Count == 0)
                {
                    MessageBox.Show("请先选择要排列的浏览器窗口。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // 首先最小化所有浏览器窗口
                _profileManager.MinimizeAllBrowserWindows();

                // 使用数字控件设置的行数和列数
                int columns = (int)numericUpDownColumn.Value;
                int rows = (int)numericUpDownRow.Value;
                
                // 确保至少有1行1列
                if (columns < 1) columns = 1;
                if (rows < 1) rows = 1;
                
                // 生成窗口ID列表字符串，用于日志记录
                string windowIds = GetWindowIdsString(selectedRows.Select(r => r.Index + 1).ToList());
                
                int bottomReserved = _options.BottomReserved;
                int rightReserved = _options.RightReserved;
                
                // 如果控件值与选项不同，则优先使用控件值
                if (!string.IsNullOrEmpty(maskedTextBoxBottom.Text) && int.TryParse(maskedTextBoxBottom.Text, out int bottom))
                {
                    bottomReserved = bottom;
                    // 更新选项值
                    _options.BottomReserved = bottom;
                }
                
                // 获取右方预留空白值
                if (!string.IsNullOrEmpty(maskedTextBoxRight.Text) && int.TryParse(maskedTextBoxRight.Text, out int right))
                {
                    rightReserved = right;
                    // 更新选项值
                    _options.RightReserved = right;
                }
                
                // 保存更新的选项
                _options.Save();
                
                // 调用排列方法，只排列选中的窗口
                int arrangedCount = _profileManager.ArrangeWindows(selectedHandles, columns, rows, bottomReserved, rightReserved);
                
                // 记录排列操作
                string taskName = textBoxTask != null ? textBoxTask.Text : string.Empty;
                string url = textBoxUrl != null ? textBoxUrl.Text : string.Empty;
                _logService.LogOperationAsync("排列窗口", 
                    windowIds, 
                    taskName, 
                    url);
                
                // 刷新窗口列表
                _ = RefreshOpenBrowsersListAsync();
                
                // 刷新日志显示
                LoadTodayLogAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"排列窗口失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 将窗口ID列表转换为紧凑的字符串表示，例如 "1-3,5,7-9"
        /// </summary>
        private string GetWindowIdsString(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return string.Empty;
            
            // 对ID进行排序
            ids.Sort();
            
            var result = new StringBuilder();
            int start = ids[0];
            int prev = start;
            
            for (int i = 1; i < ids.Count; i++)
            {
                int current = ids[i];
                
                // 如果不连续
                if (current > prev + 1)
                {
                    // 添加之前的范围
                    if (start == prev)
                        result.Append(start).Append(',');
                    else
                        result.Append(start).Append('-').Append(prev).Append(',');
                    
                    // 开始新范围
                    start = current;
                }
                
                prev = current;
            }
            
            // 添加最后一个范围
            if (start == prev)
                result.Append(start);
            else
                result.Append(start).Append('-').Append(prev);
            
            return result.ToString();
        }

        // 添加双击事件处理方法
        private void DataGridViewBrowsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // 忽略列头的双击
            if (e.RowIndex < 0) return;

            try
            {
                // 获取窗口句柄
                if (dataGridViewBrowsers.Rows[e.RowIndex].Cells["WindowHandle"].Value != null)
                {
                    string handleStr = dataGridViewBrowsers.Rows[e.RowIndex].Cells["WindowHandle"].Value.ToString();
                    
                    // 修改这里：先将字符串转换为长整型，然后创建 IntPtr
                    if (long.TryParse(handleStr, out long handleValue))
                    {
                        IntPtr windowHandle = new IntPtr(handleValue);
                        // 将窗口最大化并显示在前端
                        MaximizeAndBringToFront(windowHandle);
                        
                        // 刷新窗口状态
                        _ = RefreshOpenBrowsersListAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法激活浏览器窗口: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 添加 Windows API 声明
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindow(IntPtr hWnd);

        // 添加窗口操作方法
        private void MaximizeAndBringToFront(IntPtr windowHandle)
        {
            // 检查窗口是否有效
            if (IsWindow(windowHandle))
            {
                // SW_MAXIMIZE = 3
                ShowWindow(windowHandle, 3);
                SetForegroundWindow(windowHandle);
            }
            else
            {
                MessageBox.Show("窗口不再存在。请刷新窗口列表。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void buttonCloseAll_Click(object sender, EventArgs e)
        {
            // 调用ProfileManager关闭所有浏览器窗口
            int closedCount = _profileManager.CloseAllBrowserWindows();
            
            // 记录关闭操作
            string taskName = textBoxTask != null ? textBoxTask.Text : string.Empty;
            string url = textBoxUrl != null ? textBoxUrl.Text : string.Empty;
            _logService.LogOperationAsync("关闭所有窗口", 
                $"共{closedCount}个窗口", 
                taskName, 
                url);
            
            // 刷新窗口列表显示
            _ = RefreshOpenBrowsersListAsync();
            
            // 刷新日志显示
            LoadTodayLogAsync();
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            // 调用ProfileManager的最小化所有窗口方法
            int minimizedCount = _profileManager.MinimizeAllBrowserWindows();
            
            // 显示操作结果 (可选)
        }

        // 修改 MainForm_Load 方法，加载保存的选项
        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // 加载保存的应用选项
                _options = ApplicationOptions.Load();
                
                // 应用保存的值到控件
                if (textBoxTask != null) textBoxTask.Text = _options.TaskText;
                if (textBoxUrl != null) textBoxUrl.Text = _options.UrlText;
                if (numericUpDownColumn != null) numericUpDownColumn.Value = _options.ColumnCount;
                if (numericUpDownRow != null) numericUpDownRow.Value = _options.RowCount;
                
                // 应用预留空间值
                if (maskedTextBoxBottom != null) maskedTextBoxBottom.Text = _options.BottomReserved.ToString();
                if (maskedTextBoxRight != null) maskedTextBoxRight.Text = _options.RightReserved.ToString();
                
                // 恢复上次使用的分组信息
                if (!string.IsNullOrEmpty(_options.CurrentBrowserGroup))
                {
                    _lastSelectedGroupName = _options.CurrentBrowserGroup;
                    
                    // 恢复上次选择的配置文件列表
                    if (_options.SelectedProfileNames.Count > 0 && _profileManager != null)
                    {
                        // 尝试从当前分组中获取完整的 ProfileItem 信息
                        var group = _profileManager.GetGroup(_options.CurrentBrowserGroup);
                        if (group != null && group.Items != null)
                        {
                            // 从保存的配置文件名重新构建 ProfileItem 列表
                            _lastSelectedProfiles = new List<ProfileItem>();
                            foreach (string profileName in _options.SelectedProfileNames)
                            {
                                var profile = Array.Find(group.Items, p => p.ProfileName == profileName);
                                if (profile != null)
                                {
                                    _lastSelectedProfiles.Add(profile);
                                }
                            }
                        }
                    }
                }
                
                // 应用窗体置顶状态
                this.TopMost = _options.IsTopMost;
                
                // 更新菜单项的选中状态
                if (ToolStripMenuItemTopmost != null)
                {
                    ToolStripMenuItemTopmost.Checked = this.TopMost;
                }
                
                // 设置日期选择器为当天
                if (dateTimePickerLog != null)
                {
                    dateTimePickerLog.Value = DateTime.Today;
                    // 注册日期选择事件
                    dateTimePickerLog.ValueChanged += DateTimePickerLog_ValueChanged;
                }
                
                // 窗体加载完成后立即刷新浏览器列表（并等待刷新完成）
                await RefreshOpenBrowsersListAsync();
                
                // 异步加载当天日志
                LoadTodayLogAsync();
                
                // 计算要选择的窗口数量
                int selectCount = (int)numericUpDownColumn.Value * (int)numericUpDownRow.Value;
                if (selectCount <= 0) selectCount = 1; // 确保至少选择一个
                
                // 确保不超过可用窗口数量
                selectCount = Math.Min(selectCount, dataGridViewBrowsers.Rows.Count);
                
                // 如果有窗口可选择，则选择前N个窗口
                if (selectCount > 0 && dataGridViewBrowsers.Rows.Count > 0)
                {
                    // 先清除任何已有选择
                    dataGridViewBrowsers.ClearSelection();
                    
                    // 选择前N个窗口
                    for (int i = 0; i < selectCount; i++)
                    {
                        dataGridViewBrowsers.Rows[i].Selected = true;
                    }
                    
                    // 确保第一行可见
                    if (dataGridViewBrowsers.Rows.Count > 0)
                    {
                        dataGridViewBrowsers.FirstDisplayedScrollingRowIndex = 0;
                    }
                }
                
                // 添加控件值变化事件处理
                RegisterControlChangeEvents();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"窗体加载时选择窗口出错: {ex.Message}");
            }
        }

        // 添加新方法：注册控件值变化事件
        private void RegisterControlChangeEvents()
        {
            if (textBoxTask != null)
                textBoxTask.TextChanged += Control_ValueChanged;
            
            if (textBoxUrl != null)
                textBoxUrl.TextChanged += Control_ValueChanged;
            
            if (numericUpDownColumn != null)
                numericUpDownColumn.ValueChanged += Control_ValueChanged;
            
            if (numericUpDownRow != null)
                numericUpDownRow.ValueChanged += Control_ValueChanged;
                
            // 注册预留空间控件的事件
            if (maskedTextBoxBottom != null)
                maskedTextBoxBottom.TextChanged += Control_ValueChanged;
                
            if (maskedTextBoxRight != null)
                maskedTextBoxRight.TextChanged += Control_ValueChanged;
        }

        // 添加新方法：控件值变化事件处理函数
        private void Control_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 更新选项对象的值
                if (textBoxTask != null) _options.TaskText = textBoxTask.Text;
                if (textBoxUrl != null) _options.UrlText = textBoxUrl.Text;
                if (numericUpDownColumn != null) _options.ColumnCount = (int)numericUpDownColumn.Value;
                if (numericUpDownRow != null) _options.RowCount = (int)numericUpDownRow.Value;
                
                // 保存预留空间值
                if (maskedTextBoxBottom != null && !string.IsNullOrEmpty(maskedTextBoxBottom.Text) && 
                    int.TryParse(maskedTextBoxBottom.Text, out int bottomValue))
                {
                    _options.BottomReserved = bottomValue;
                }
                
                if (maskedTextBoxRight != null && !string.IsNullOrEmpty(maskedTextBoxRight.Text) && 
                    int.TryParse(maskedTextBoxRight.Text, out int rightValue))
                {
                    _options.RightReserved = rightValue;
                }
                
                // 保存到文件
                _options.Save();
            }
            catch (Exception ex)
            {
                // 记录错误但不显示给用户，避免干扰用户体验
                System.Diagnostics.Debug.WriteLine($"保存选项时出错: {ex.Message}");
            }
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                // 计算每批窗口数量
                int batchSize = (int)numericUpDownColumn.Value * (int)numericUpDownRow.Value;
                if (batchSize <= 0) batchSize = 1; // 确保批次大小至少为1
                
                // 检查是否有数据
                if (dataGridViewBrowsers.Rows.Count == 0)
                    return;
                
                // 找出当前选中的最小索引
                int currentMinIndex = dataGridViewBrowsers.Rows.Count; // 初始化为最大值
                foreach (DataGridViewRow row in dataGridViewBrowsers.SelectedRows)
                {
                    if (row.Index < currentMinIndex)
                        currentMinIndex = row.Index;
                }
                
                // 如果没有选中行，则默认为最后一行
                if (currentMinIndex == dataGridViewBrowsers.Rows.Count)
                    currentMinIndex = dataGridViewBrowsers.Rows.Count - 1;
                
                // 计算上一批的起始索引
                int newStartIndex = currentMinIndex - batchSize;
                
                // 检查是否已经到达顶部
                if (newStartIndex < 0)
                {
                    // 如果已经在顶部但还有选择，则选择第一批
                    if (currentMinIndex > 0)
                    {
                        newStartIndex = 0;
                    }
                    else
                    {
                        MessageBox.Show("已经到达列表顶部。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                
                // 清除当前选择
                dataGridViewBrowsers.ClearSelection();
                
                // 选择新的一批
                int endIndex = Math.Min(newStartIndex + batchSize - 1, dataGridViewBrowsers.Rows.Count - 1);
                for (int i = newStartIndex; i <= endIndex; i++)
                {
                    dataGridViewBrowsers.Rows[i].Selected = true;
                }
                
                // 滚动到可见位置
                if (newStartIndex >= 0 && newStartIndex < dataGridViewBrowsers.Rows.Count)
                {
                    dataGridViewBrowsers.FirstDisplayedScrollingRowIndex = newStartIndex;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"选择上一批窗口时出错: {ex.Message}");
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            try
            {
                // 计算每批窗口数量
                int batchSize = (int)numericUpDownColumn.Value * (int)numericUpDownRow.Value;
                if (batchSize <= 0) batchSize = 1; // 确保批次大小至少为1
                
                // 检查是否有数据
                if (dataGridViewBrowsers.Rows.Count == 0)
                    return;
                
                // 找出当前选中的最大索引
                int currentMaxIndex = -1; // 初始化为最小值
                foreach (DataGridViewRow row in dataGridViewBrowsers.SelectedRows)
                {
                    if (row.Index > currentMaxIndex)
                        currentMaxIndex = row.Index;
                }
                
                // 如果没有选中行，则默认为第一行的前一行
                if (currentMaxIndex == -1)
                    currentMaxIndex = -1;
                
                // 计算下一批的起始索引
                int newStartIndex = currentMaxIndex + 1;
                
                // 检查是否已经到达底部
                if (newStartIndex >= dataGridViewBrowsers.Rows.Count)
                {
                    MessageBox.Show("已经到达列表底部。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                // 清除当前选择
                dataGridViewBrowsers.ClearSelection();
                
                // 选择新的一批
                int endIndex = Math.Min(newStartIndex + batchSize - 1, dataGridViewBrowsers.Rows.Count - 1);
                for (int i = newStartIndex; i <= endIndex; i++)
                {
                    dataGridViewBrowsers.Rows[i].Selected = true;
                }
                
                // 滚动到可见位置
                if (newStartIndex < dataGridViewBrowsers.Rows.Count)
                {
                    dataGridViewBrowsers.FirstDisplayedScrollingRowIndex = newStartIndex;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"选择下一批窗口时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 在行头绘制行号，并根据窗口状态设置背景色
        /// </summary>
        private void DataGridViewBrowsers_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv != null && e.RowIndex >= 0 && e.RowIndex < dgv.Rows.Count)
            {
                var row = dgv.Rows[e.RowIndex];
                string windowState = row.Cells["WindowState"].Value as string;

                Color backColor = Color.White; // 默认背景色
                if (windowState == "正常")
                {
                    backColor = Color.LightGreen; // 正常状态为绿色
                }

                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    // 绘制整个行头的背景
                    e.Graphics.FillRectangle(brush, new Rectangle(0, e.RowBounds.Top, dgv.RowHeadersWidth, e.RowBounds.Height));
                }

                // 绘制行号
                if (dgv.RowHeadersVisible)
                {
                    using (SolidBrush b = new SolidBrush(dgv.RowHeadersDefaultCellStyle.ForeColor))
                    {
                        // 创建大小为12的字体
                        using (Font rowNumberFont = new Font(dgv.RowHeadersDefaultCellStyle.Font.FontFamily, 12))
                        {
                            e.Graphics.DrawString((e.RowIndex + 1).ToString(),
                                rowNumberFont, // 使用新字体
                                b,
                                e.RowBounds.Left + 15, // 水平位置，可调整
                                e.RowBounds.Top + ((e.RowBounds.Height - rowNumberFont.Height) / 2)); // 垂直居中
                        }
                    }
                }
            }
        }

        private void buttonBookmark_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a context menu to show bookmark options
                ContextMenuStrip bookmarkMenu = new ContextMenuStrip();
                
                // Add "Add Bookmark" option
                ToolStripMenuItem addBookmarkItem = new ToolStripMenuItem("加入书签");
                addBookmarkItem.Click += (s, args) => 
                {
                    string bookmarkName = textBoxTask.Text;
                    string bookmarkUrl = textBoxUrl.Text;
                    
                    if (string.IsNullOrWhiteSpace(bookmarkName) || string.IsNullOrWhiteSpace(bookmarkUrl))
                    {
                        MessageBox.Show("请先输入任务描述和URL！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    
                    try
                    {
                        // Add or update the bookmark
                        var updatedBookmarks = BrowsersManager.Models.Bookmark.AddOrUpdateBookmark(bookmarkName, bookmarkUrl);
                        
                        // Show success message
                        MessageBox.Show($"已成功{(updatedBookmarks.Any(b => b.Name == bookmarkName && b.AddedTime > DateTime.Now.AddMinutes(-1)) ? "添加" : "更新")}书签：{bookmarkName}", 
                                        "书签保存成功", 
                                        MessageBoxButtons.OK, 
                                        MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"保存书签时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                
                bookmarkMenu.Items.Add(addBookmarkItem);
                
                // Add "Delete Current Bookmark" option
                ToolStripMenuItem deleteBookmarkItem = new ToolStripMenuItem("删除当前书签");
                deleteBookmarkItem.Click += (s, args) => 
                {
                    string bookmarkName = textBoxTask.Text;
                    
                    if (string.IsNullOrWhiteSpace(bookmarkName))
                    {
                        MessageBox.Show("请先输入要删除的书签名称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    
                    // Check if bookmark exists
                    var bookmark = BrowsersManager.Models.Bookmark.FindByName(bookmarkName);
                    if (bookmark == null)
                    {
                        MessageBox.Show($"未找到名为 \"{bookmarkName}\" 的书签。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    
                    // Confirm deletion with user
                    DialogResult result = MessageBox.Show(
                        $"确定要删除书签 \"{bookmarkName}\" 吗？", 
                        "确认删除", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            // Delete the bookmark
                            bool deleted = BrowsersManager.Models.Bookmark.DeleteBookmark(bookmarkName);
                            
                            if (deleted)
                            {
                                MessageBox.Show($"已成功删除书签：{bookmarkName}", 
                                                "删除成功", 
                                                MessageBoxButtons.OK, 
                                                MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show($"删除书签失败：未找到 {bookmarkName}", 
                                                "删除失败", 
                                                MessageBoxButtons.OK, 
                                                MessageBoxIcon.Warning);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"删除书签时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                };
                
                bookmarkMenu.Items.Add(deleteBookmarkItem);
                
                // Add separator
                bookmarkMenu.Items.Add(new ToolStripSeparator());
                
                // Load recent bookmarks
                var recentBookmarks = BrowsersManager.Models.Bookmark.GetRecentBookmarks(30);
                
                if (recentBookmarks.Count == 0)
                {
                    ToolStripMenuItem emptyItem = new ToolStripMenuItem("(没有书签)") { Enabled = false };
                    bookmarkMenu.Items.Add(emptyItem);
                }
                else
                {
                    // Add recent bookmarks to the menu
                    foreach (var bookmark in recentBookmarks)
                    {
                        ToolStripMenuItem bookmarkItem = new ToolStripMenuItem(bookmark.Name);
                        
                        // Store the bookmark reference for later use
                        bookmarkItem.Tag = bookmark;
                        
                        bookmarkItem.Click += (s, args) => 
                        {
                            if (s is ToolStripMenuItem menuItem && menuItem.Tag is BrowsersManager.Models.Bookmark selectedBookmark)
                            {
                                // Update the text boxes with the selected bookmark information
                                textBoxTask.Text = selectedBookmark.Name;
                                textBoxUrl.Text = selectedBookmark.Url;
                                
                                // Update the UI options
                                if (_options != null)
                                {
                                    _options.TaskText = selectedBookmark.Name;
                                    _options.UrlText = selectedBookmark.Url;
                                    _options.Save();
                                }
                            }
                        };
                        
                        bookmarkMenu.Items.Add(bookmarkItem);
                    }
                }
                
                // Show the popup menu next to the button
                bookmarkMenu.Show(buttonBookmark, new System.Drawing.Point(0, buttonBookmark.Height));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"显示书签菜单时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            LoadTodayLogAsync();
        }

        // 添加日期选择器事件处理方法
        private async void DateTimePickerLog_ValueChanged(object sender, EventArgs e)
        {
            await LoadLogForDateAsync(dateTimePickerLog.Value);
        }
        
        // 加载当天日志的异步方法
        private async void LoadTodayLogAsync()
        {
            await LoadLogForDateAsync(DateTime.Today);
            
            // 如果日期选择器不是今天，也更新为今天
            if (dateTimePickerLog != null && dateTimePickerLog.Value.Date != DateTime.Today)
            {
                dateTimePickerLog.ValueChanged -= DateTimePickerLog_ValueChanged; // 暂时移除事件处理，避免触发不必要的加载
                dateTimePickerLog.Value = DateTime.Today;
                dateTimePickerLog.ValueChanged += DateTimePickerLog_ValueChanged; // 重新添加事件处理
            }
        }
        
        // 根据指定日期加载日志的异步方法
        private async Task LoadLogForDateAsync(DateTime date)
        {
            try
            {
                if (textBoxLog != null)
                {
                    // 设置提示信息
                    textBoxLog.Text = $"正在加载 {date:yyyy-MM-dd} 的日志...";
                    
                    // 异步加载日志
                    string logContent = await _logService.ReadLogFileAsync(date);
                    
                    // 更新UI
                    if (string.IsNullOrEmpty(logContent))
                    {
                        textBoxLog.Text = $"{date:yyyy-MM-dd} 没有日志记录。";
                    }
                    else
                    {
                        textBoxLog.Text = logContent;
                        // 滚动到文本框底部，显示最新日志
                        textBoxLog.SelectionStart = textBoxLog.Text.Length;
                        textBoxLog.ScrollToCaret();
                    }
                }
            }
            catch (Exception ex)
            {
                if (textBoxLog != null)
                {
                    textBoxLog.Text = $"加载日志时出错: {ex.Message}";
                }
                System.Diagnostics.Debug.WriteLine($"加载日志时出错: {ex.Message}");
            }
        }

        private async void buttonOpenTaskUrl_Click(object sender, EventArgs e)
        {
            try
            {
                // 检查是否已经选择了分组
                if (string.IsNullOrEmpty(_lastSelectedGroupName))
                {
                    MessageBox.Show("请先选择浏览器分组和配置文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnOpenBrowsers_Click(sender, e); // 打开选择窗体
                    return;
                }
                
                // 提取输入的URL（如果有输入框的话）
                string url = null;
                if (textBoxUrl != null && !string.IsNullOrWhiteSpace(textBoxUrl.Text))
                {
                    url = textBoxUrl.Text;
                }
                else
                {
                    MessageBox.Show("请先输入要打开的URL。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (textBoxUrl != null) textBoxUrl.Focus();
                    return;
                }
                
                // 获取任务名称
                string taskName = textBoxTask != null ? textBoxTask.Text : string.Empty;
                
                // 确保当前分组信息保存到应用程序选项
                _options.CurrentBrowserGroup = _lastSelectedGroupName;
                // 保存选择的配置文件名列表
                if (_lastSelectedProfiles.Count > 0)
                {
                    _options.SelectedProfileNames = _lastSelectedProfiles.Select(p => p.ProfileName).ToList();
                }
                _options.Save();
                
                // 启动浏览器组
                buttonOpenTaskUrl.Enabled = false;
                buttonOpenTaskUrl.Text = "启动中...";
                
                // 使用保存的分组和配置文件信息启动浏览器
                int count;
                if (_lastSelectedProfiles.Count > 0)
                {
                    // 将 ProfileItem 列表转换为 profileName 字符串数组
                    string[] profileNames = _lastSelectedProfiles.Select(p => p.ProfileName).ToArray();
                    
                    // 启动用户选择的特定配置文件
                    count = await _browserProcessService.LaunchBrowserGroupAsync(_lastSelectedGroupName, url, profileNames);
                    
                    await _logService.LogOperationAsync("打开选定浏览器窗口", 
                        $"{_lastSelectedGroupName}，选中{_lastSelectedProfiles.Count}个窗口", 
                        taskName, 
                        url);
                }
                else
                {
                    // 启动整个组
                    count = await _browserProcessService.LaunchBrowserGroupAsync(_lastSelectedGroupName, url);
                    await _logService.LogOperationAsync("打开浏览器窗口", 
                        $"{_lastSelectedGroupName}，共{count}个窗口", 
                        taskName, 
                        url);
                }
                
                buttonOpenTaskUrl.Text = $"已启动 {count} 个浏览器";
                await Task.Delay(2000); // 短暂显示结果
                buttonOpenTaskUrl.Text = "打开任务URL";
                buttonOpenTaskUrl.Enabled = true;
                
                // 刷新窗口列表
                _ = RefreshOpenBrowsersListAsync();
                
                // 刷新日志显示
                LoadTodayLogAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动浏览器失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonOpenTaskUrl.Text = "打开任务URL";
                buttonOpenTaskUrl.Enabled = true;
            }
        }

        private void ToolStripMenuItemTopmost_Click(object sender, EventArgs e)
        {
            try
            {
                // Toggle the TopMost property of the form
                this.TopMost = !this.TopMost;
                
                // Update the menu item's checked state
                if (sender is ToolStripMenuItem menuItem)
                {
                    menuItem.Checked = this.TopMost;
                }
                
                // Save the setting to options
                if (_options != null)
                {
                    _options.IsTopMost = this.TopMost;
                    _options.Save();
                }
                
                // Log the state change
                string actionText = this.TopMost ? "启用窗口置顶" : "取消窗口置顶";
                System.Diagnostics.Debug.WriteLine(actionText);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"切换置顶状态时出错: {ex.Message}");
            }
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuItemBrowserType_Click(object sender, EventArgs e)
        {
            using (var browserTypeView = new BrowserTypeView())
            {
                browserTypeView.ShowDialog();
            }
        }

        private void buttonCloseSelected_Click(object sender, EventArgs e)
        {
            try
            {
                // Collect all selected row indices and window handles
                List<(int Index, IntPtr Handle)> selectedRows = new List<(int, IntPtr)>();
                foreach (DataGridViewRow row in dataGridViewBrowsers.SelectedRows)
                {
                    if (row.Cells["WindowHandle"].Value != null)
                    {
                        string handleStr = row.Cells["WindowHandle"].Value.ToString();
                        if (long.TryParse(handleStr, out long handleValue))
                        {
                            selectedRows.Add((row.Index, new IntPtr(handleValue)));
                        }
                    }
                }

                // Sort by row index (from top to bottom)
                selectedRows.Sort((a, b) => a.Index.CompareTo(b.Index));
                
                // Extract the sorted handle list
                List<IntPtr> selectedHandles = selectedRows.Select(item => item.Handle).ToList();

                // Check if any windows are selected
                if (selectedHandles.Count == 0)
                {
                    MessageBox.Show("请先选择要关闭的浏览器窗口。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                // Generate a window ID list string for logging
                string windowIds = GetWindowIdsString(selectedRows.Select(r => r.Index + 1).ToList());
                
                // Call the CloseBrowserWindows method to close selected windows
                int closedCount = _profileManager.CloseBrowserWindows(selectedHandles);
                
                // Log the operation
                string taskName = textBoxTask != null ? textBoxTask.Text : string.Empty;
                string url = textBoxUrl != null ? textBoxUrl.Text : string.Empty;
                _logService.LogOperationAsync("关闭选中窗口", 
                    $"已关闭窗口: {windowIds}, 共{closedCount}个", 
                    taskName, 
                    url);
                
                // Refresh the browser window list
                _ = RefreshOpenBrowsersListAsync();
                
                // Refresh the log display
                LoadTodayLogAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"关闭窗口失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}

