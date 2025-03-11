using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using BrowsersManager.Models;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace BrowsersManager.Views
{
    public partial class BrowserGroupView : Form
    {
        private BrowserGroup _currentGroup;
        private string _groupName;

        public BrowserGroupView(string groupName = null)
        {
            InitializeComponent();
            
            // 确保DataGridView的列始终被创建，即使没有数据
            SetupDataGridColumns();
            
            LoadBrowserTypes();
            
            if (!string.IsNullOrEmpty(groupName))
            {
                _groupName = groupName;
                LoadGroupInfo(groupName);
            }
        }

        private void LoadBrowserTypes()
        {
            try
            {
                // 使用BrowserType类的静态方法加载浏览器类型
                var browserTypes = BrowserType.LoadAllBrowserTypes();
                
                comboBoxBrowserType.Items.Clear();
                foreach (var browser in browserTypes)
                {
                    comboBoxBrowserType.Items.Add(browser.Name);
                }

                if (comboBoxBrowserType.Items.Count > 0)
                {
                    comboBoxBrowserType.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载浏览器类型时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGroupInfo(string groupName)
        {
            try
            {
                // 使用BrowserGroup类的静态方法加载分组
                _currentGroup = BrowserGroup.LoadGroup(groupName);
                
                if (_currentGroup != null)
                {
                    // 填充UI控件
                    textBoxName.Text = _currentGroup.Name;
                    textBoxTotal.Text = _currentGroup.Total.ToString();
                    SelectBrowserType(_currentGroup.BrowserType);
                    textBoxDescription.Text = _currentGroup.Description;
                    textBoxTemplate.Text = _currentGroup.ProfileTemplate;
                    textBoxUserDataDir.Text = _currentGroup.UserDataDir;
                    textBoxRange.Text = _currentGroup.ProfileRange;
                    textBoxCopySource.Text = _currentGroup.CopySourceRange;
                    textBoxUserAgent.Text = _currentGroup.DefaultUserAgent;
                    textBoxLanguage.Text = _currentGroup.DefaultLanguage;
                    textBoxOtherParameters.Text = _currentGroup.OtherParameters;
                    
                    // 填充数据网格视图中的项目
                    PopulateItemsDataGrid(_currentGroup.Items);
                }
                else
                {
                    MessageBox.Show($"未找到名为 '{groupName}' 的浏览器分组", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载浏览器分组信息时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectBrowserType(string browserType)
        {
            for (int i = 0; i < comboBoxBrowserType.Items.Count; i++)
            {
                if (comboBoxBrowserType.Items[i].ToString() == browserType)
                {
                    comboBoxBrowserType.SelectedIndex = i;
                    break;
                }
            }
        }

        private void PopulateItemsDataGrid(ProfileItem[] items)
        {
            // 清除现有项目
            dataGridViewItems.Rows.Clear();
            
            // 检查列是否存在，如果不存在才设置列
            if (dataGridViewItems.Columns.Count == 0)
            {
                SetupDataGridColumns();
            }
            
            // 添加项目
            foreach (var item in items)
            {
                dataGridViewItems.Rows.Add(
                    item.ProfileName,
                    item.Language ?? _currentGroup.DefaultLanguage, // 若为空则使用默认值
                    item.UserAgent ?? _currentGroup.DefaultUserAgent, // 若为空则使用默认值
                    item.OtherParameters ?? _currentGroup.OtherParameters // 若为空则使用默认值
                );
            }
        }

        private void SetupDataGridColumns()
        {
            // 清除现有列
            dataGridViewItems.Columns.Clear();
            
            // 添加新列
            dataGridViewItems.Columns.AddRange(new DataGridViewColumn[] 
            {
                new DataGridViewTextBoxColumn 
                { 
                    Name = "ProfileName",
                    HeaderText = "浏览器 ID",
                    DataPropertyName = "ProfileName",
                    ReadOnly = true,
                    FillWeight = 50
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "Language",
                    HeaderText = "语言",
                    DataPropertyName = "Language",
                    FillWeight = 50
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "UserAgent",
                    HeaderText = "User Agent",
                    DataPropertyName = "UserAgent",
                    FillWeight = 100
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "OtherParameters",
                    HeaderText = "其他参数",
                    DataPropertyName = "OtherParameters",
                    FillWeight = 200
                }
            });
        }
        
        private void buttonProfilePath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select Profile Directory";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxUserDataDir.Text = folderDialog.SelectedPath;
                }
            }
        }

        // 新增方法：创建批量配置文件项
        private async void buttonCreateItems_Click(object sender, EventArgs e)
        {
            try
            {
                // 验证必要的输入
                if (string.IsNullOrEmpty(textBoxRange.Text))
                {
                    MessageBox.Show("请输入配置文件范围", "验证错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (string.IsNullOrEmpty(textBoxUserDataDir.Text))
                {
                    MessageBox.Show("请选择用户数据目录", "验证错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // 目录不存在时自动创建
                if (!Directory.Exists(textBoxUserDataDir.Text))
                {
                    try
                    {
                        Directory.CreateDirectory(textBoxUserDataDir.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"创建目录失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    // 检查目录是否非空
                    if (Directory.GetFiles(textBoxUserDataDir.Text).Length > 0 || 
                        Directory.GetDirectories(textBoxUserDataDir.Text).Length > 0)
                    {
                        DialogResult result = MessageBox.Show(
                            "所选目录已存在数据，继续使用可能会覆盖现有数据。是否继续？", 
                            "警告", 
                            MessageBoxButtons.YesNo, 
                            MessageBoxIcon.Warning);
                            
                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }
                }
                
                string browserType = comboBoxBrowserType.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(browserType))
                {
                    MessageBox.Show("请选择浏览器类型", "验证错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // 创建进度对话框或状态
                buttonCreateItems.Enabled = false;
                buttonCreateItems.Text = "处理中...";
                Application.DoEvents();
                
                // 清空现有的配置文件列表
                dataGridViewItems.Rows.Clear();
                
                // 创建并调用配置文件创建服务
                var creationService = new BrowsersManager.Services.ProfileCreationService();
                var createdProfiles = await creationService.CreateProfilesAsync(
                    browserType,
                    textBoxRange.Text,
                    textBoxUserDataDir.Text,
                    textBoxTemplate.Text
                );
                
                // 更新UI
                if (createdProfiles.Count > 0)
                {
                    // 将新创建的配置文件添加到数据网格
                    foreach (var profile in createdProfiles)
                    {
                        dataGridViewItems.Rows.Add(
                            profile.ProfileName,
                            null, // 保持语言字段为空
                            null, // 保持User Agent字段为空
                            null  // 保持其他参数字段为空
                        );
                    }
                    
                    // 更新总数
                    int total = dataGridViewItems.Rows.Count - 1; // 减去"新行"
                    textBoxTotal.Text = total.ToString();                    
                    
                    // 自动保存分组
                    buttonSave_Click(sender, e);
                    
                    MessageBox.Show($"成功创建 {createdProfiles.Count} 个配置文件", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("没有创建新的配置文件，可能指定的配置文件已存在", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建配置文件时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 恢复按钮状态
                buttonCreateItems.Enabled = true;
                buttonCreateItems.Text = "创建配置文件";
            }
        }

        private bool TryParseProfileRange(string rangeText, out int startIndex, out int endIndex, out string prefix)
        {
            startIndex = 0;
            endIndex = 0;
            prefix = "";
            
            try
            {
                // 分割范围字符串
                string[] parts = rangeText.Split('-');
                if (parts.Length != 2)
                    return false;
                
                // 解析起始部分
                string startPart = parts[0].Trim();
                // 找到最后一个数字
                int startNumberIndex = startPart.LastIndexOfAny("0123456789".ToCharArray());
                if (startNumberIndex < 0)
                    return false;
                
                // 找到数字的起始位置
                int startNumberStartIndex = startPart.LastIndexOfAny("0123456789".ToCharArray(), startNumberIndex - 1);
                while (startNumberStartIndex >= 0 && char.IsDigit(startPart[startNumberStartIndex]))
                {
                    startNumberStartIndex--;
                }
                startNumberStartIndex++;
                
                // 提取前缀和数字
                prefix = startPart.Substring(0, startNumberStartIndex);
                startIndex = int.Parse(startPart.Substring(startNumberStartIndex));
                
                // 解析结束部分
                string endPart = parts[1].Trim();
                // 找到最后一个数字
                int endNumberIndex = endPart.LastIndexOfAny("0123456789".ToCharArray());
                if (endNumberIndex < 0)
                    return false;
                
                // 找到数字的起始位置
                int endNumberStartIndex = endPart.LastIndexOfAny("0123456789".ToCharArray(), endNumberIndex - 1);
                while (endNumberStartIndex >= 0 && char.IsDigit(endPart[endNumberStartIndex]))
                {
                    endNumberStartIndex--;
                }
                endNumberStartIndex++;
                
                // 提取数字
                endIndex = int.Parse(endPart.Substring(endNumberStartIndex));
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 保存分组按钮点击事件
        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 从UI控件更新当前分组对象
                if (_currentGroup == null)
                {
                    _currentGroup = new BrowserGroup();
                }
                
                _currentGroup.Name = textBoxName.Text;
                _currentGroup.BrowserType = comboBoxBrowserType.SelectedItem?.ToString();
                _currentGroup.Description = textBoxDescription.Text;
                _currentGroup.ProfileTemplate = textBoxTemplate.Text;
                _currentGroup.UserDataDir = textBoxUserDataDir.Text;
                _currentGroup.ProfileRange = textBoxRange.Text;
                _currentGroup.CopySourceRange = textBoxCopySource.Text;
                _currentGroup.DefaultLanguage = textBoxLanguage.Text;
                _currentGroup.DefaultUserAgent = textBoxUserAgent.Text;
                _currentGroup.OtherParameters = textBoxOtherParameters.Text;
                
                // 从DataGridView获取当前项目
                List<ProfileItem> profileItems = new List<ProfileItem>();
                foreach (DataGridViewRow row in dataGridViewItems.Rows)
                {
                    // 跳过未提交的新行
                    if (row.IsNewRow) continue;
                    
                    ProfileItem item = new ProfileItem
                    {
                        ProfileName = row.Cells["ProfileName"].Value?.ToString(),
                        Language = row.Cells["Language"].Value?.ToString(),
                        UserAgent = row.Cells["UserAgent"].Value?.ToString(),
                        OtherParameters = row.Cells["OtherParameters"].Value?.ToString()
                    };
                    
                    if (!string.IsNullOrEmpty(item.ProfileName))
                    {
                        profileItems.Add(item);
                    }
                }
                
                _currentGroup.Items = profileItems.ToArray();
                _currentGroup.Total = profileItems.Count;
                textBoxTotal.Text = _currentGroup.Total.ToString();
                
                // 保存分组
                BrowserGroup.SaveGroup(_currentGroup);
                
                MessageBox.Show($"分组 '{_currentGroup.Name}' 已成功保存", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // 如果这是一个新分组，设置_groupName
                if (string.IsNullOrEmpty(_groupName))
                {
                    _groupName = _currentGroup.Name;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存分组时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProfileGroupView_Load(object sender, EventArgs e)
        {
            
        }

        private void buttonTemplateOpen_Click(object sender, EventArgs e)
        {
            try
            {
                // 验证必要的输入
                if (string.IsNullOrEmpty(textBoxTemplate.Text))
                {
                    MessageBox.Show("请输入配置文件模板名称", "验证错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                                
                string browserType = comboBoxBrowserType.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(browserType))
                {
                    MessageBox.Show("请选择浏览器类型", "验证错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // 获取浏览器路径
                BrowserType browser = BrowserType.FindByName(browserType);
                if (browser == null || string.IsNullOrEmpty(browser.Path) || !File.Exists(browser.Path))
                {
                    MessageBox.Show($"无法找到浏览器 {browserType} 的可执行文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                // 构建启动参数 - 处理路径组合逻辑
                string userDataDir = textBoxUserDataDir.Text;
                string profileDir = textBoxTemplate.Text;
                string combinedUserDataDir;
                
                // 检查profileDir是否已经是绝对路径
                if (Path.IsPathRooted(profileDir))
                {
                    // 如果profileDir已经是绝对路径，则直接使用它
                    combinedUserDataDir = profileDir;
                }
                else
                {
                    // 如果profileDir不是绝对路径，则与userDataDir组合
                    combinedUserDataDir = Path.Combine(userDataDir, profileDir);
                }
                
                // 创建进程启动信息 - 仅使用 --user-data-dir 参数
                var startInfo = new ProcessStartInfo
                {
                    FileName = browser.Path,
                    Arguments = $"--user-data-dir=\"{combinedUserDataDir}\"",
                    UseShellExecute = true
                };
                
                // 启动浏览器
                Process.Start(startInfo);
                
                // 提示用户
                this.Text = $"已打开模板配置 - {profileDir}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开浏览器模板配置时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSelectSource_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "选择要复制的配置文件夹";
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxCopySource.Text = folderDialog.SelectedPath;
                }
            }

        }


        private void buttonGenerateAvatar_Click(object sender, EventArgs e)
        {
            // 检查是否有数据
            if (dataGridViewItems.Rows.Count == 0)
            {
                MessageBox.Show("没有可用的配置项数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 获取浏览器类型
            string browserType = comboBoxBrowserType.Text;
            if (string.IsNullOrEmpty(browserType))
            {
                MessageBox.Show("请先选择浏览器类型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 创建配置列表
                var profileConfigs = new List<Dictionary<string, string>>();
                
                // 遍历所有行
                foreach (DataGridViewRow row in dataGridViewItems.Rows)
                {
                    // 跳过已删除或空行
                    if (row.IsNewRow) continue;
                    
                    string profileName = row.Cells["ProfileName"].Value?.ToString();
                    if (string.IsNullOrEmpty(profileName)) continue;
                    
                    // 获取configID - 如果超过3个字符，则取最后3个字符
                    string configID = profileName;
                    if (configID.Length > 3)
                    {
                        configID = configID.Substring(configID.Length - 3);
                    }
                    
                    // 构建配置路径 - textBoxUserDataDir.Text\ProfileName
                    string configPath = Path.Combine(textBoxUserDataDir.Text, profileName);
                    
                    // 添加到配置列表
                    profileConfigs.Add(new Dictionary<string, string>
                    {
                        { "configID", configID },
                        { "configPath", configPath }
                    });
                }
                
                if (profileConfigs.Count == 0)
                {
                    MessageBox.Show("没有有效的配置数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                // 禁用按钮防止重复操作
                buttonGenerateAvatar.Enabled = false;
                buttonGenerateAvatar.Text = "正在生成...";
                Application.DoEvents();
                
                // 异步调用服务
                Task.Run(async () => 
                {
                    try
                    {
                        var service = new BrowsersManager.Services.ProfileCreationService();
                        int successCount = await service.GenerateBrowserAvatarsAsync(profileConfigs);
                        
                        // 返回主线程显示结果
                        this.Invoke((MethodInvoker)delegate
                        {
                            // 恢复按钮状态
                            buttonGenerateAvatar.Enabled = true;
                            buttonGenerateAvatar.Text = "生成头像";
                            
                            MessageBox.Show(
                                $"操作完成:\n总计: {profileConfigs.Count}\n成功: {successCount}\n失败: {profileConfigs.Count - successCount}\n\n后续手工操作：点击浏览器右上角用户头像，\n选择 自定义个人资料 菜单项，\n在弹出窗口中选择左起第 2 个头像即可。", 
                                "头像生成结果", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Information);
                        });
                    }
                    catch (Exception ex)
                    {
                        // 返回主线程显示错误
                        this.Invoke((MethodInvoker)delegate
                        {
                            // 恢复按钮状态
                            buttonGenerateAvatar.Enabled = true;
                            buttonGenerateAvatar.Text = "生成头像";
                            
                            MessageBox.Show($"生成头像时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

