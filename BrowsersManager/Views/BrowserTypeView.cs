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

namespace BrowsersManager.Views
{
    public partial class BrowserTypeView : Form
    {
        private List<BrowserType> _browserTypes;

        public BrowserTypeView()
        {
            InitializeComponent();
            InitializeDataGridView();
            LoadBrowsersFromFile();
        }

        private void InitializeDataGridView()
        {
            dataGridViewBrowsersList.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Name", 
                HeaderText = "名称", 
                FillWeight = 50 
            });
            dataGridViewBrowsersList.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Path", 
                HeaderText = "路径",
                FillWeight = 250 
            });

            dataGridViewBrowsersList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadBrowsersFromFile()
        {
            try
            {
                _browserTypes = BrowserType.LoadAllBrowserTypes();
                RefreshDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载浏览器类型数据时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _browserTypes = new List<BrowserType>();
            }
        }

        private void RefreshDataGridView()
        {
            dataGridViewBrowsersList.Rows.Clear();
            foreach (var browser in _browserTypes)
            {
                dataGridViewBrowsersList.Rows.Add(browser.Name, browser.Path);
            }
        }

        private void SaveBrowsersToFile()
        {
            try
            {
                BrowserType.SaveAllBrowserTypes(_browserTypes);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存浏览器类型数据时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _browserTypes = new List<BrowserType>();
            foreach (DataGridViewRow row in dataGridViewBrowsersList.Rows)
            {
                if (row.IsNewRow) continue;
                
                var browserType = new BrowserType
                {
                    Name = row.Cells["Name"].Value?.ToString() ?? "",
                    Path = row.Cells["Path"].Value?.ToString() ?? ""
                };
                _browserTypes.Add(browserType);
            }
            SaveBrowsersToFile();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            dataGridViewBrowsersList.Rows.Add("", "");
            int lastRowIndex = dataGridViewBrowsersList.Rows.Count - 1;
            if (lastRowIndex >= 0)
            {
                dataGridViewBrowsersList.CurrentCell = dataGridViewBrowsersList.Rows[lastRowIndex].Cells[0];
                dataGridViewBrowsersList.BeginEdit(true);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewBrowsersList.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewBrowsersList.SelectedRows[0];
                if (selectedRow.Cells["Name"].Value != null)
                {
                    string name = selectedRow.Cells["Name"].Value.ToString();
                    _browserTypes.RemoveAll(b => b.Name == name);
                    RefreshDataGridView();
                    SaveBrowsersToFile();
                }
                else
                {
                    MessageBox.Show("选中行的名称值为空。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (dataGridViewBrowsersList.CurrentRow != null)
            {
                var currentRow = dataGridViewBrowsersList.CurrentRow;
                if (currentRow.Cells["Name"].Value != null)
                {
                    string name = currentRow.Cells["Name"].Value.ToString();
                    _browserTypes.RemoveAll(b => b.Name == name);
                    RefreshDataGridView();
                    SaveBrowsersToFile();
                }
                else
                {
                    MessageBox.Show("当前行的名称值为空。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("请选择要删除的浏览器信息。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BrowserInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnSave_Click(sender, e);
        }
    }
}
