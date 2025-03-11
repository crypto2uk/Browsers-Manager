namespace BrowsersManager
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageBrowsers = new System.Windows.Forms.TabPage();
            this.buttonCloseSelected = new System.Windows.Forms.Button();
            this.buttonOpenTaskUrl = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonPrevious = new System.Windows.Forms.Button();
            this.buttonMinimize = new System.Windows.Forms.Button();
            this.buttonCloseAll = new System.Windows.Forms.Button();
            this.dataGridViewBrowsers = new System.Windows.Forms.DataGridView();
            this.btnArrange = new System.Windows.Forms.Button();
            this.btnOpenBrowsers = new System.Windows.Forms.Button();
            this.tabPageLog = new System.Windows.Forms.TabPage();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.buttonReload = new System.Windows.Forms.Button();
            this.dateTimePickerLog = new System.Windows.Forms.DateTimePicker();
            this.textBoxTask = new System.Windows.Forms.TextBox();
            this.textBoxUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.menuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemTopmost = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemBrowserType = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemBrowserGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownColumn = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownRow = new System.Windows.Forms.NumericUpDown();
            this.buttonBookmark = new System.Windows.Forms.Button();
            this.maskedTextBoxBottom = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBoxRight = new System.Windows.Forms.MaskedTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPageBrowsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBrowsers)).BeginInit();
            this.tabPageLog.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownColumn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRow)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageBrowsers);
            this.tabControl1.Controls.Add(this.tabPageLog);
            this.tabControl1.Location = new System.Drawing.Point(2, 91);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(795, 490);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageBrowsers
            // 
            this.tabPageBrowsers.Controls.Add(this.buttonCloseSelected);
            this.tabPageBrowsers.Controls.Add(this.buttonOpenTaskUrl);
            this.tabPageBrowsers.Controls.Add(this.buttonNext);
            this.tabPageBrowsers.Controls.Add(this.buttonPrevious);
            this.tabPageBrowsers.Controls.Add(this.buttonMinimize);
            this.tabPageBrowsers.Controls.Add(this.buttonCloseAll);
            this.tabPageBrowsers.Controls.Add(this.dataGridViewBrowsers);
            this.tabPageBrowsers.Controls.Add(this.btnArrange);
            this.tabPageBrowsers.Controls.Add(this.btnOpenBrowsers);
            this.tabPageBrowsers.Location = new System.Drawing.Point(4, 22);
            this.tabPageBrowsers.Name = "tabPageBrowsers";
            this.tabPageBrowsers.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBrowsers.Size = new System.Drawing.Size(787, 464);
            this.tabPageBrowsers.TabIndex = 0;
            this.tabPageBrowsers.Text = "已打开的浏览器";
            this.tabPageBrowsers.UseVisualStyleBackColor = true;
            // 
            // buttonCloseSelected
            // 
            this.buttonCloseSelected.Location = new System.Drawing.Point(290, 15);
            this.buttonCloseSelected.Name = "buttonCloseSelected";
            this.buttonCloseSelected.Size = new System.Drawing.Size(90, 23);
            this.buttonCloseSelected.TabIndex = 85;
            this.buttonCloseSelected.Text = "关闭选中窗口";
            this.buttonCloseSelected.UseVisualStyleBackColor = true;
            this.buttonCloseSelected.Click += new System.EventHandler(this.buttonCloseSelected_Click);
            // 
            // buttonOpenTaskUrl
            // 
            this.buttonOpenTaskUrl.Location = new System.Drawing.Point(98, 15);
            this.buttonOpenTaskUrl.Name = "buttonOpenTaskUrl";
            this.buttonOpenTaskUrl.Size = new System.Drawing.Size(90, 23);
            this.buttonOpenTaskUrl.TabIndex = 70;
            this.buttonOpenTaskUrl.Text = "打开任务链接";
            this.buttonOpenTaskUrl.UseVisualStyleBackColor = true;
            this.buttonOpenTaskUrl.Click += new System.EventHandler(this.buttonOpenTaskUrl_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Location = new System.Drawing.Point(710, 15);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(74, 23);
            this.buttonNext.TabIndex = 120;
            this.buttonNext.Text = "选择下一批";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonPrevious
            // 
            this.buttonPrevious.Location = new System.Drawing.Point(630, 15);
            this.buttonPrevious.Name = "buttonPrevious";
            this.buttonPrevious.Size = new System.Drawing.Size(74, 23);
            this.buttonPrevious.TabIndex = 110;
            this.buttonPrevious.Text = "选择上一批";
            this.buttonPrevious.UseVisualStyleBackColor = true;
            this.buttonPrevious.Click += new System.EventHandler(this.buttonPrevious_Click);
            // 
            // buttonMinimize
            // 
            this.buttonMinimize.Location = new System.Drawing.Point(482, 15);
            this.buttonMinimize.Name = "buttonMinimize";
            this.buttonMinimize.Size = new System.Drawing.Size(98, 23);
            this.buttonMinimize.TabIndex = 100;
            this.buttonMinimize.Text = "最小化所有窗口";
            this.buttonMinimize.UseVisualStyleBackColor = true;
            this.buttonMinimize.Click += new System.EventHandler(this.buttonMinimize_Click);
            // 
            // buttonCloseAll
            // 
            this.buttonCloseAll.Location = new System.Drawing.Point(386, 15);
            this.buttonCloseAll.Name = "buttonCloseAll";
            this.buttonCloseAll.Size = new System.Drawing.Size(90, 23);
            this.buttonCloseAll.TabIndex = 90;
            this.buttonCloseAll.Text = "关闭所有窗口";
            this.buttonCloseAll.UseVisualStyleBackColor = true;
            this.buttonCloseAll.Click += new System.EventHandler(this.buttonCloseAll_Click);
            // 
            // dataGridViewBrowsers
            // 
            this.dataGridViewBrowsers.AllowUserToAddRows = false;
            this.dataGridViewBrowsers.AllowUserToDeleteRows = false;
            this.dataGridViewBrowsers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewBrowsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewBrowsers.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridViewBrowsers.Location = new System.Drawing.Point(3, 44);
            this.dataGridViewBrowsers.Name = "dataGridViewBrowsers";
            this.dataGridViewBrowsers.ReadOnly = true;
            this.dataGridViewBrowsers.RowTemplate.Height = 23;
            this.dataGridViewBrowsers.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridViewBrowsers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewBrowsers.Size = new System.Drawing.Size(781, 417);
            this.dataGridViewBrowsers.TabIndex = 0;
            // 
            // btnArrange
            // 
            this.btnArrange.Location = new System.Drawing.Point(194, 15);
            this.btnArrange.Name = "btnArrange";
            this.btnArrange.Size = new System.Drawing.Size(90, 23);
            this.btnArrange.TabIndex = 80;
            this.btnArrange.Text = "排列选中窗口";
            this.btnArrange.UseVisualStyleBackColor = true;
            this.btnArrange.Click += new System.EventHandler(this.btnArrange_Click);
            // 
            // btnOpenBrowsers
            // 
            this.btnOpenBrowsers.Location = new System.Drawing.Point(2, 15);
            this.btnOpenBrowsers.Name = "btnOpenBrowsers";
            this.btnOpenBrowsers.Size = new System.Drawing.Size(90, 23);
            this.btnOpenBrowsers.TabIndex = 60;
            this.btnOpenBrowsers.Text = "打开浏览器";
            this.btnOpenBrowsers.UseVisualStyleBackColor = true;
            this.btnOpenBrowsers.Click += new System.EventHandler(this.btnOpenBrowsers_Click);
            // 
            // tabPageLog
            // 
            this.tabPageLog.Controls.Add(this.textBoxLog);
            this.tabPageLog.Controls.Add(this.buttonReload);
            this.tabPageLog.Controls.Add(this.dateTimePickerLog);
            this.tabPageLog.Location = new System.Drawing.Point(4, 22);
            this.tabPageLog.Name = "tabPageLog";
            this.tabPageLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLog.Size = new System.Drawing.Size(787, 464);
            this.tabPageLog.TabIndex = 1;
            this.tabPageLog.Text = "操作日志";
            this.tabPageLog.UseVisualStyleBackColor = true;
            // 
            // textBoxLog
            // 
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBoxLog.Location = new System.Drawing.Point(3, 37);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLog.Size = new System.Drawing.Size(781, 424);
            this.textBoxLog.TabIndex = 2;
            // 
            // buttonReload
            // 
            this.buttonReload.Location = new System.Drawing.Point(145, 10);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(75, 23);
            this.buttonReload.TabIndex = 1;
            this.buttonReload.Text = "重新加载";
            this.buttonReload.UseVisualStyleBackColor = true;
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            // 
            // dateTimePickerLog
            // 
            this.dateTimePickerLog.Location = new System.Drawing.Point(3, 11);
            this.dateTimePickerLog.Name = "dateTimePickerLog";
            this.dateTimePickerLog.Size = new System.Drawing.Size(135, 21);
            this.dateTimePickerLog.TabIndex = 0;
            // 
            // textBoxTask
            // 
            this.textBoxTask.Location = new System.Drawing.Point(76, 32);
            this.textBoxTask.Name = "textBoxTask";
            this.textBoxTask.Size = new System.Drawing.Size(172, 21);
            this.textBoxTask.TabIndex = 10;
            this.textBoxTask.Text = "时空每日签到，免gas";
            // 
            // textBoxUrl
            // 
            this.textBoxUrl.Location = new System.Drawing.Point(76, 59);
            this.textBoxUrl.Name = "textBoxUrl";
            this.textBoxUrl.Size = new System.Drawing.Size(229, 21);
            this.textBoxUrl.TabIndex = 20;
            this.textBoxUrl.Text = "https://app.galxe.com/quest/SpaceandTimeDB/GCH8gtpnmi";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "任务名称：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "任务链接：";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFile,
            this.menuItemSettings});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(800, 25);
            this.menuStripMain.TabIndex = 10;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // menuItemFile
            // 
            this.menuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemTopmost,
            this.menuItemExit});
            this.menuItemFile.Name = "menuItemFile";
            this.menuItemFile.Size = new System.Drawing.Size(44, 21);
            this.menuItemFile.Text = "文件";
            // 
            // ToolStripMenuItemTopmost
            // 
            this.ToolStripMenuItemTopmost.Name = "ToolStripMenuItemTopmost";
            this.ToolStripMenuItemTopmost.Size = new System.Drawing.Size(124, 22);
            this.ToolStripMenuItemTopmost.Text = "置顶显示";
            this.ToolStripMenuItemTopmost.Click += new System.EventHandler(this.ToolStripMenuItemTopmost_Click);
            // 
            // menuItemExit
            // 
            this.menuItemExit.Name = "menuItemExit";
            this.menuItemExit.Size = new System.Drawing.Size(124, 22);
            this.menuItemExit.Text = "退出";
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // menuItemSettings
            // 
            this.menuItemSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemBrowserType,
            this.menuItemBrowserGroup});
            this.menuItemSettings.Name = "menuItemSettings";
            this.menuItemSettings.Size = new System.Drawing.Size(44, 21);
            this.menuItemSettings.Text = "设置";
            // 
            // menuItemBrowserType
            // 
            this.menuItemBrowserType.Name = "menuItemBrowserType";
            this.menuItemBrowserType.Size = new System.Drawing.Size(136, 22);
            this.menuItemBrowserType.Text = "浏览器类型";
            this.menuItemBrowserType.Click += new System.EventHandler(this.menuItemBrowserType_Click);
            // 
            // menuItemBrowserGroup
            // 
            this.menuItemBrowserGroup.Name = "menuItemBrowserGroup";
            this.menuItemBrowserGroup.Size = new System.Drawing.Size(136, 22);
            this.menuItemBrowserGroup.Text = "浏览器分组";
            this.menuItemBrowserGroup.Click += new System.EventHandler(this.menuItemBrowserGroup_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(535, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "每列个数：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(537, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "每行个数：";
            // 
            // numericUpDownColumn
            // 
            this.numericUpDownColumn.Location = new System.Drawing.Point(600, 34);
            this.numericUpDownColumn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownColumn.Name = "numericUpDownColumn";
            this.numericUpDownColumn.Size = new System.Drawing.Size(65, 21);
            this.numericUpDownColumn.TabIndex = 40;
            this.numericUpDownColumn.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // numericUpDownRow
            // 
            this.numericUpDownRow.Location = new System.Drawing.Point(600, 59);
            this.numericUpDownRow.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRow.Name = "numericUpDownRow";
            this.numericUpDownRow.Size = new System.Drawing.Size(65, 21);
            this.numericUpDownRow.TabIndex = 50;
            this.numericUpDownRow.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // buttonBookmark
            // 
            this.buttonBookmark.Location = new System.Drawing.Point(254, 31);
            this.buttonBookmark.Name = "buttonBookmark";
            this.buttonBookmark.Size = new System.Drawing.Size(52, 23);
            this.buttonBookmark.TabIndex = 30;
            this.buttonBookmark.Text = "书签";
            this.buttonBookmark.UseVisualStyleBackColor = true;
            this.buttonBookmark.Click += new System.EventHandler(this.buttonBookmark_Click);
            // 
            // maskedTextBoxBottom
            // 
            this.maskedTextBoxBottom.Location = new System.Drawing.Point(730, 59);
            this.maskedTextBoxBottom.Mask = "999";
            this.maskedTextBoxBottom.Name = "maskedTextBoxBottom";
            this.maskedTextBoxBottom.Size = new System.Drawing.Size(42, 21);
            this.maskedTextBoxBottom.TabIndex = 103;
            this.maskedTextBoxBottom.Text = "0";
            this.maskedTextBoxBottom.ValidatingType = typeof(int);
            // 
            // maskedTextBoxRight
            // 
            this.maskedTextBoxRight.Location = new System.Drawing.Point(730, 34);
            this.maskedTextBoxRight.Mask = "999";
            this.maskedTextBoxRight.Name = "maskedTextBoxRight";
            this.maskedTextBoxRight.Size = new System.Drawing.Size(42, 21);
            this.maskedTextBoxRight.TabIndex = 104;
            this.maskedTextBoxRight.Text = "0";
            this.maskedTextBoxRight.ValidatingType = typeof(int);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(774, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 105;
            this.label5.Text = "px";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(774, 38);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 106;
            this.label6.Text = "px";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(671, 38);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 107;
            this.label7.Text = "留空：右";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(707, 62);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 108;
            this.label8.Text = "下";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 585);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.maskedTextBoxRight);
            this.Controls.Add(this.maskedTextBoxBottom);
            this.Controls.Add(this.buttonBookmark);
            this.Controls.Add(this.numericUpDownRow);
            this.Controls.Add(this.numericUpDownColumn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.menuStripMain);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxUrl);
            this.Controls.Add(this.textBoxTask);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "浏览器多开管理 - 关注 x.com 账号 @crypto2uk";
            this.tabControl1.ResumeLayout(false);
            this.tabPageBrowsers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBrowsers)).EndInit();
            this.tabPageLog.ResumeLayout(false);
            this.tabPageLog.PerformLayout();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownColumn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageBrowsers;
        private System.Windows.Forms.TabPage tabPageLog;
        private System.Windows.Forms.Button btnOpenBrowsers;
        private System.Windows.Forms.Button btnArrange;
        private System.Windows.Forms.TextBox textBoxTask;
        private System.Windows.Forms.TextBox textBoxUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridViewBrowsers;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem menuItemFile;
        private System.Windows.Forms.ToolStripMenuItem menuItemExit;
        private System.Windows.Forms.ToolStripMenuItem menuItemSettings;
        private System.Windows.Forms.ToolStripMenuItem menuItemBrowserType;
        private System.Windows.Forms.ToolStripMenuItem menuItemBrowserGroup;
        private System.Windows.Forms.Button buttonCloseAll;
        private System.Windows.Forms.Button buttonMinimize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDownColumn;
        private System.Windows.Forms.NumericUpDown numericUpDownRow;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonPrevious;
        private System.Windows.Forms.Button buttonBookmark;
        private System.Windows.Forms.DateTimePicker dateTimePickerLog;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Button buttonReload;
        private System.Windows.Forms.Button buttonOpenTaskUrl;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemTopmost;
        private System.Windows.Forms.Button buttonCloseSelected;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxBottom;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxRight;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}

