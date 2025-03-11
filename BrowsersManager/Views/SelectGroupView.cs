using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrowsersManager.Models;

namespace BrowsersManager.Views
{
    public partial class SelectGroupView : Form
    {
        private Dictionary<string, BrowserGroup> _groups;
        
        // 返回值属性
        public string SelectedGroupName { get; private set; }
        public List<ProfileItem> SelectedProfiles { get; private set; } = new List<ProfileItem>();

        public SelectGroupView()
        {
            InitializeComponent();
            
            // 设置ListView显示详情视图，添加列
            listViewGroup.View = View.Details;
            listViewGroup.FullRowSelect = true;
            listViewGroup.Columns.Add("分组", 60);
            listViewGroup.Columns.Add("说明", 150);
            
            // 设置DataGridView列
            SetupDataGridColumns();
            
            // 绑定ListView选择变更事件
            listViewGroup.SelectedIndexChanged += ListViewGroup_SelectedIndexChanged;
            
            // 启用DataGridView的多选功能
            dataGridViewBrowserList.MultiSelect = true;
            dataGridViewBrowserList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            // 加载分组数据
            LoadGroups();
            
            // 注册窗体显示完成事件，用于检测模态状态
            this.Shown += SelectGroupView_Shown;
        }
        
        private void SelectGroupView_Shown(object sender, EventArgs e)
        {
            // 检查窗体是否以模态方式显示
            UpdateButtonsVisibility();
            
            // 异步选择保存的配置项，确保UI完全加载后执行
            BeginInvoke(new Action(SelectSavedProfiles));
        }
        
        private void UpdateButtonsVisibility()
        {
            // Modal属性为true表示窗体是以ShowDialog()模态方式打开的
            // 只有在模态时才显示确认按钮
            buttonOpenGroup.Visible = this.Modal;
            buttonOpenSelected.Visible = this.Modal;
        }
        
        private void SetupDataGridColumns()
        {
            // 清除现有列
            dataGridViewBrowserList.Columns.Clear();
            
            // 添加新列
            dataGridViewBrowserList.Columns.AddRange(new DataGridViewColumn[] 
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
        
        private void LoadGroups()
        {
            try
            {
                // 清除现有项目
                listViewGroup.Items.Clear();
                
                // 加载所有分组
                _groups = BrowserGroup.LoadAllGroups();
                
                // 将分组添加到ListView
                foreach (var group in _groups.Values)
                {
                    ListViewItem item = new ListViewItem(group.Name);
                    item.SubItems.Add(group.Description);
                    item.Tag = group; // 存储完整的组对象以便后续使用
                    listViewGroup.Items.Add(item);
                }

                // 加载完成后，选中当前保存的浏览器组
                SelectCurrentBrowserGroup();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载浏览器分组时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 选中与 ApplicationOptions.CurrentBrowserGroup 匹配的浏览器组
        /// </summary>
        private void SelectCurrentBrowserGroup()
        {
            try
            {
                // 加载应用程序选项
                var options = ApplicationOptions.Load();
                
                // 如果存在当前浏览器组且不为空
                if (!string.IsNullOrEmpty(options.CurrentBrowserGroup))
                {
                    // 查找匹配的项目
                    foreach (ListViewItem item in listViewGroup.Items)
                    {
                        if (item.Text == options.CurrentBrowserGroup)
                        {
                            // 选中项目
                            item.Selected = true;
                            item.EnsureVisible();
                            return;
                        }
                    }
                }
                
                // 如果没有找到匹配项或当前组为空，则默认选中第一项（如果有）
                if (listViewGroup.Items.Count > 0)
                {
                    listViewGroup.Items[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"选中当前浏览器组时出错: {ex.Message}");
                // 不显示消息框，避免干扰用户
            }
        }
        
        private void ListViewGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSelectedGroupItems();
        }
        
        private void LoadSelectedGroupItems()
        {
            // 清除现有项目
            dataGridViewBrowserList.Rows.Clear();
            
            // 获取选中的分组
            if (listViewGroup.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listViewGroup.SelectedItems[0];
                BrowserGroup selectedGroup = selectedItem.Tag as BrowserGroup;
                
                if (selectedGroup != null && selectedGroup.Items != null)
                {
                    // 添加分组中的浏览器明细到DataGridView
                    foreach (var item in selectedGroup.Items)
                    {
                        int rowIndex = dataGridViewBrowserList.Rows.Add(
                            item.ProfileName,
                            item.Language ?? selectedGroup.DefaultLanguage,
                            item.UserAgent ?? selectedGroup.DefaultUserAgent,
                            item.OtherParameters ?? selectedGroup.OtherParameters
                        );
                        
                        // 将ProfileItem存储在行的Tag属性中
                        dataGridViewBrowserList.Rows[rowIndex].Tag = item;
                    }
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // 打开ProfileGroupView窗体，传递空的组名参数
            var profileGroupView = new BrowserGroupView();
            profileGroupView.ShowDialog();
            
            // 刷新分组列表
            LoadGroups();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            // 检查是否有分组被选中
            if (listViewGroup.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择要编辑的分组", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            // 获取选中的分组名
            string groupName = listViewGroup.SelectedItems[0].Text;
            
            // 打开ProfileGroupView窗体，传递选中的组名
            var profileGroupView = new BrowserGroupView(groupName);
            profileGroupView.ShowDialog();
            
            // 刷新分组列表
            LoadGroups();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // 检查是否有分组被选中
            if (listViewGroup.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择要删除的分组", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            // 获取选中的分组名
            string groupName = listViewGroup.SelectedItems[0].Text;
            
            // 确认删除
            DialogResult result = MessageBox.Show(
                $"确定要删除分组 '{groupName}' 吗？此操作不可撤销。", 
                "确认删除", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning);
                
            if (result == DialogResult.Yes)
            {
                try
                {
                    // 删除分组
                    _groups.Remove(groupName);
                    BrowserGroup.SaveAllGroups(_groups);
                    
                    // 刷新分组列表
                    LoadGroups();
                    
                    MessageBox.Show($"分组 '{groupName}' 已成功删除", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"删除分组时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonOpenGroup_Click(object sender, EventArgs e)
        {
            // 检查是否有分组被选中
            if (listViewGroup.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择要打开的分组", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            try
            {
                // 设置返回值 - 组名
                SelectedGroupName = listViewGroup.SelectedItems[0].Text;
                
                // 设置返回值 - 空的配置文件列表
                SelectedProfiles.Clear();
                
                // 设置对话框结果并关闭
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开分组时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonOpenSelected_Click(object sender, EventArgs e)
        {
            // 检查是否有分组被选中
            if (listViewGroup.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择要打开的分组", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            // 检查是否有配置文件被选中
            if (dataGridViewBrowserList.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择要打开的浏览器配置文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            try
            {
                // 设置返回值 - 组名
                SelectedGroupName = listViewGroup.SelectedItems[0].Text;
                
                // 设置返回值 - 获取选中的配置文件
                SelectedProfiles.Clear();
                
                // 获取选中行中的ProfileItem对象
                foreach (DataGridViewRow row in dataGridViewBrowserList.SelectedRows)
                {
                    // 如果行中有Tag存储的ProfileItem，直接使用
                    if (row.Tag is ProfileItem profileItem)
                    {
                        SelectedProfiles.Add(profileItem);
                    }
                    else
                    {
                        // 否则，创建一个新的ProfileItem对象
                        profileItem = new ProfileItem
                        {
                            ProfileName = row.Cells["ProfileName"].Value?.ToString(),
                            Language = row.Cells["Language"].Value?.ToString(),
                            UserAgent = row.Cells["UserAgent"].Value?.ToString(),
                            OtherParameters = row.Cells["OtherParameters"].Value?.ToString()
                        };
                        
                        SelectedProfiles.Add(profileItem);
                    }
                }
                
                // 设置对话框结果并关闭
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开选中的浏览器配置文件时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // 检查是否有分组被选中
            if (listViewGroup.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择要保存的分组", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            try
            {
                // 获取选中的分组
                ListViewItem selectedItem = listViewGroup.SelectedItems[0];
                string groupName = selectedItem.Text;
                BrowserGroup selectedGroup = selectedItem.Tag as BrowserGroup;
                
                if (selectedGroup == null)
                {
                    MessageBox.Show("无法获取选中的分组信息", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                // 创建新的配置项列表
                List<ProfileItem> updatedItems = new List<ProfileItem>();
                
                // 从DataGridView中读取所有行数据
                foreach (DataGridViewRow row in dataGridViewBrowserList.Rows)
                {
                    // 跳过新行（如果有）
                    if (row.IsNewRow)
                        continue;
                    
                    // 从行中创建ProfileItem对象
                    ProfileItem item = new ProfileItem
                    {
                        ProfileName = row.Cells["ProfileName"].Value?.ToString(),
                        Language = row.Cells["Language"].Value?.ToString(),
                        UserAgent = row.Cells["UserAgent"].Value?.ToString(),
                        OtherParameters = row.Cells["OtherParameters"].Value?.ToString()
                    };
                    
                    // 添加到更新列表
                    updatedItems.Add(item);
                }
                
                // 更新选中分组的配置项
                selectedGroup.Items = updatedItems.ToArray();
                
                // 更新分组字典并保存
                _groups[groupName] = selectedGroup;
                BrowserGroup.SaveAllGroups(_groups);
                
                MessageBox.Show($"分组 '{groupName}' 的配置项已成功保存", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存分组配置时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            try
            {
                // 获取ApplicationOptions中的行列设置
                var options = Models.ApplicationOptions.Load();
                int batchSize = options.ColumnCount * options.RowCount;
                if (batchSize <= 0) batchSize = 1; // 确保批次大小至少为1
                
                // 检查是否有数据
                if (dataGridViewBrowserList.Rows.Count == 0)
                    return;
                
                // 找出当前选中的最大索引
                int currentMaxIndex = -1; // 初始化为最小值
                foreach (DataGridViewRow row in dataGridViewBrowserList.SelectedRows)
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
                if (newStartIndex >= dataGridViewBrowserList.Rows.Count)
                {
                    MessageBox.Show("已经到达列表底部。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                // 清除当前选择
                dataGridViewBrowserList.ClearSelection();
                
                // 选择新的一批
                int endIndex = Math.Min(newStartIndex + batchSize - 1, dataGridViewBrowserList.Rows.Count - 1);
                for (int i = newStartIndex; i <= endIndex; i++)
                {
                    dataGridViewBrowserList.Rows[i].Selected = true;
                }
                
                // 滚动到可见位置
                if (newStartIndex < dataGridViewBrowserList.Rows.Count)
                {
                    dataGridViewBrowserList.FirstDisplayedScrollingRowIndex = newStartIndex;
                }
                
                // 更新选中的配置项列表
                UpdateSelectedProfiles();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"选择下一批配置项时出错: {ex.Message}");
                MessageBox.Show($"选择下一批配置项时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                // 获取ApplicationOptions中的行列设置
                var options = Models.ApplicationOptions.Load();
                int batchSize = options.ColumnCount * options.RowCount;
                if (batchSize <= 0) batchSize = 1; // 确保批次大小至少为1
                
                // 检查是否有数据
                if (dataGridViewBrowserList.Rows.Count == 0)
                    return;
                
                // 找出当前选中的最小索引
                int currentMinIndex = dataGridViewBrowserList.Rows.Count; // 初始化为最大值
                foreach (DataGridViewRow row in dataGridViewBrowserList.SelectedRows)
                {
                    if (row.Index < currentMinIndex)
                        currentMinIndex = row.Index;
                }
                
                // 如果没有选中行，则默认为最后一行
                if (currentMinIndex == dataGridViewBrowserList.Rows.Count)
                    currentMinIndex = dataGridViewBrowserList.Rows.Count - 1;
                
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
                dataGridViewBrowserList.ClearSelection();
                
                // 选择新的一批
                int endIndex = Math.Min(newStartIndex + batchSize - 1, dataGridViewBrowserList.Rows.Count - 1);
                for (int i = newStartIndex; i <= endIndex; i++)
                {
                    dataGridViewBrowserList.Rows[i].Selected = true;
                }
                
                // 滚动到可见位置
                if (newStartIndex >= 0 && newStartIndex < dataGridViewBrowserList.Rows.Count)
                {
                    dataGridViewBrowserList.FirstDisplayedScrollingRowIndex = newStartIndex;
                }
                
                // 更新选中的配置项列表
                UpdateSelectedProfiles();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"选择上一批配置项时出错: {ex.Message}");
                MessageBox.Show($"选择上一批配置项时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // 辅助方法：更新选中的配置项列表
        private void UpdateSelectedProfiles()
        {
            SelectedProfiles.Clear();
            
            foreach (DataGridViewRow row in dataGridViewBrowserList.SelectedRows)
            {
                if (row.Tag is ProfileItem item)
                {
                    SelectedProfiles.Add(item);
                }
            }
        }

        private void SelectSavedProfiles()
        {
            try
            {
                // 加载应用程序选项
                var options = ApplicationOptions.Load();
                
                // 如果有已保存的选中配置项名称，选择匹配的配置项
                if (options.SelectedProfileNames != null && options.SelectedProfileNames.Count > 0)
                {
                    // 先清除当前选择
                    dataGridViewBrowserList.ClearSelection();
                    
                    // 选择匹配的配置项
                    foreach (DataGridViewRow row in dataGridViewBrowserList.Rows)
                    {
                        if (row.Tag is ProfileItem profileItem && 
                            options.SelectedProfileNames.Contains(profileItem.ProfileName))
                        {
                            row.Selected = true;
                        }
                    }
                    
                    // 如果有选中项，确保第一个选中项可见
                    if (dataGridViewBrowserList.SelectedRows.Count > 0)
                    {
                        int firstSelectedIndex = dataGridViewBrowserList.SelectedRows
                            .Cast<DataGridViewRow>()
                            .Min(r => r.Index);
                        
                        dataGridViewBrowserList.FirstDisplayedScrollingRowIndex = firstSelectedIndex;
                    }
                    
                    // 更新选中的配置项列表
                    UpdateSelectedProfiles();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"选择保存的配置项时出错: {ex.Message}");
                // 不显示消息框，避免干扰用户
            }
        }
    }
}
