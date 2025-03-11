namespace BrowsersManager.Views
{
    partial class BrowserGroupView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserGroupView));
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxBrowserType = new System.Windows.Forms.ComboBox();
            this.textBoxUserDataDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonProfilePath = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxRange = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxTotal = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonTemplateOpen = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxTemplate = new System.Windows.Forms.TextBox();
            this.textBoxOtherParameters = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxLanguage = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxUserAgent = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dataGridViewItems = new System.Windows.Forms.DataGridView();
            this.buttonCreateItems = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonSelectSource = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxCopySource = new System.Windows.Forms.TextBox();
            this.buttonGenerateAvatar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewItems)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Tomato;
            this.label2.Location = new System.Drawing.Point(22, 162);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "用户数据文件夹（必填）：";
            // 
            // comboBoxBrowserType
            // 
            this.comboBoxBrowserType.FormattingEnabled = true;
            this.comboBoxBrowserType.Location = new System.Drawing.Point(174, 130);
            this.comboBoxBrowserType.Name = "comboBoxBrowserType";
            this.comboBoxBrowserType.Size = new System.Drawing.Size(385, 20);
            this.comboBoxBrowserType.TabIndex = 40;
            // 
            // textBoxUserDataDir
            // 
            this.textBoxUserDataDir.Location = new System.Drawing.Point(174, 159);
            this.textBoxUserDataDir.Name = "textBoxUserDataDir";
            this.textBoxUserDataDir.Size = new System.Drawing.Size(332, 21);
            this.textBoxUserDataDir.TabIndex = 50;
            this.textBoxUserDataDir.Text = "D:\\BrowserManager_Data";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Tomato;
            this.label1.Location = new System.Drawing.Point(70, 133);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "浏览器（必填）：";
            // 
            // buttonProfilePath
            // 
            this.buttonProfilePath.Location = new System.Drawing.Point(509, 159);
            this.buttonProfilePath.Name = "buttonProfilePath";
            this.buttonProfilePath.Size = new System.Drawing.Size(50, 22);
            this.buttonProfilePath.TabIndex = 55;
            this.buttonProfilePath.Text = "选择";
            this.buttonProfilePath.UseVisualStyleBackColor = true;
            this.buttonProfilePath.Click += new System.EventHandler(this.buttonProfilePath_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Tomato;
            this.label3.Location = new System.Drawing.Point(58, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "分组名称（必填）：";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(174, 17);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(385, 21);
            this.textBoxName.TabIndex = 10;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(174, 46);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(385, 21);
            this.textBoxDescription.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(130, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "说明：";
            // 
            // textBoxRange
            // 
            this.textBoxRange.Location = new System.Drawing.Point(174, 73);
            this.textBoxRange.Name = "textBoxRange";
            this.textBoxRange.Size = new System.Drawing.Size(385, 21);
            this.textBoxRange.TabIndex = 30;
            this.textBoxRange.Text = "1-50";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Tomato;
            this.label5.Location = new System.Drawing.Point(22, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(149, 12);
            this.label5.TabIndex = 15;
            this.label5.Text = "配置文件夹范围（必填）：";
            // 
            // textBoxTotal
            // 
            this.textBoxTotal.Location = new System.Drawing.Point(174, 102);
            this.textBoxTotal.Name = "textBoxTotal";
            this.textBoxTotal.ReadOnly = true;
            this.textBoxTotal.Size = new System.Drawing.Size(385, 21);
            this.textBoxTotal.TabIndex = 1000;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(70, 105);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 12);
            this.label6.TabIndex = 17;
            this.label6.Text = "配置文件夹总数：";
            // 
            // buttonTemplateOpen
            // 
            this.buttonTemplateOpen.Location = new System.Drawing.Point(509, 190);
            this.buttonTemplateOpen.Name = "buttonTemplateOpen";
            this.buttonTemplateOpen.Size = new System.Drawing.Size(50, 22);
            this.buttonTemplateOpen.TabIndex = 65;
            this.buttonTemplateOpen.Text = "打开";
            this.buttonTemplateOpen.UseVisualStyleBackColor = true;
            this.buttonTemplateOpen.Click += new System.EventHandler(this.buttonTemplateOpen_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(58, 193);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 12);
            this.label7.TabIndex = 20;
            this.label7.Text = "浏览器模板文件夹：";
            // 
            // textBoxTemplate
            // 
            this.textBoxTemplate.Location = new System.Drawing.Point(174, 190);
            this.textBoxTemplate.Name = "textBoxTemplate";
            this.textBoxTemplate.Size = new System.Drawing.Size(332, 21);
            this.textBoxTemplate.TabIndex = 60;
            this.textBoxTemplate.Text = "Template";
            // 
            // textBoxOtherParameters
            // 
            this.textBoxOtherParameters.Location = new System.Drawing.Point(174, 298);
            this.textBoxOtherParameters.Name = "textBoxOtherParameters";
            this.textBoxOtherParameters.Size = new System.Drawing.Size(385, 21);
            this.textBoxOtherParameters.TabIndex = 90;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(106, 309);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 26;
            this.label9.Text = "其它参数：";
            // 
            // textBoxLanguage
            // 
            this.textBoxLanguage.Location = new System.Drawing.Point(174, 271);
            this.textBoxLanguage.Name = "textBoxLanguage";
            this.textBoxLanguage.Size = new System.Drawing.Size(385, 21);
            this.textBoxLanguage.TabIndex = 80;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(106, 274);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 24;
            this.label10.Text = "默认语言：";
            // 
            // textBoxUserAgent
            // 
            this.textBoxUserAgent.Location = new System.Drawing.Point(174, 244);
            this.textBoxUserAgent.Name = "textBoxUserAgent";
            this.textBoxUserAgent.Size = new System.Drawing.Size(385, 21);
            this.textBoxUserAgent.TabIndex = 70;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(70, 247);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(101, 12);
            this.label11.TabIndex = 22;
            this.label11.Text = "默认User-Agent：";
            // 
            // dataGridViewItems
            // 
            this.dataGridViewItems.AllowUserToAddRows = false;
            this.dataGridViewItems.AllowUserToDeleteRows = false;
            this.dataGridViewItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewItems.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridViewItems.Location = new System.Drawing.Point(0, 336);
            this.dataGridViewItems.Name = "dataGridViewItems";
            this.dataGridViewItems.RowTemplate.Height = 23;
            this.dataGridViewItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewItems.Size = new System.Drawing.Size(759, 350);
            this.dataGridViewItems.TabIndex = 200;
            // 
            // buttonCreateItems
            // 
            this.buttonCreateItems.Location = new System.Drawing.Point(614, 17);
            this.buttonCreateItems.Name = "buttonCreateItems";
            this.buttonCreateItems.Size = new System.Drawing.Size(108, 23);
            this.buttonCreateItems.TabIndex = 100;
            this.buttonCreateItems.Text = "生成多开账号";
            this.buttonCreateItems.UseVisualStyleBackColor = true;
            this.buttonCreateItems.Click += new System.EventHandler(this.buttonCreateItems_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(614, 49);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(108, 23);
            this.buttonSave.TabIndex = 110;
            this.buttonSave.Text = "保存分组信息";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonSelectSource
            // 
            this.buttonSelectSource.Location = new System.Drawing.Point(509, 217);
            this.buttonSelectSource.Name = "buttonSelectSource";
            this.buttonSelectSource.Size = new System.Drawing.Size(50, 22);
            this.buttonSelectSource.TabIndex = 1003;
            this.buttonSelectSource.Text = "选择";
            this.buttonSelectSource.UseVisualStyleBackColor = true;
            this.buttonSelectSource.Click += new System.EventHandler(this.buttonSelectSource_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(70, 220);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(101, 12);
            this.label8.TabIndex = 1001;
            this.label8.Text = "复制来源文件夹：";
            // 
            // textBoxCopySource
            // 
            this.textBoxCopySource.Location = new System.Drawing.Point(174, 217);
            this.textBoxCopySource.Name = "textBoxCopySource";
            this.textBoxCopySource.Size = new System.Drawing.Size(332, 21);
            this.textBoxCopySource.TabIndex = 65;
            // 
            // buttonGenerateAvatar
            // 
            this.buttonGenerateAvatar.Location = new System.Drawing.Point(614, 78);
            this.buttonGenerateAvatar.Name = "buttonGenerateAvatar";
            this.buttonGenerateAvatar.Size = new System.Drawing.Size(108, 23);
            this.buttonGenerateAvatar.TabIndex = 1004;
            this.buttonGenerateAvatar.Text = "生成账号头像";
            this.buttonGenerateAvatar.UseVisualStyleBackColor = true;
            this.buttonGenerateAvatar.Click += new System.EventHandler(this.buttonGenerateAvatar_Click);
            // 
            // BrowserGroupView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 686);
            this.Controls.Add(this.buttonGenerateAvatar);
            this.Controls.Add(this.buttonSelectSource);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxCopySource);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonCreateItems);
            this.Controls.Add(this.dataGridViewItems);
            this.Controls.Add(this.textBoxOtherParameters);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxLanguage);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.textBoxUserAgent);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.buttonTemplateOpen);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxTemplate);
            this.Controls.Add(this.textBoxTotal);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxRange);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonProfilePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxBrowserType);
            this.Controls.Add(this.textBoxUserDataDir);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BrowserGroupView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "浏览器分组多开设置及模板";
            this.Load += new System.EventHandler(this.ProfileGroupView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxBrowserType;
        private System.Windows.Forms.TextBox textBoxUserDataDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonProfilePath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxRange;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxTotal;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonTemplateOpen;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxTemplate;
        private System.Windows.Forms.TextBox textBoxOtherParameters;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxLanguage;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxUserAgent;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DataGridView dataGridViewItems;
        private System.Windows.Forms.Button buttonCreateItems;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonSelectSource;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxCopySource;
        private System.Windows.Forms.Button buttonGenerateAvatar;
    }
}