namespace BrowsersManager.Views
{
    partial class SelectGroupView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectGroupView));
            this.listViewGroup = new System.Windows.Forms.ListView();
            this.dataGridViewBrowserList = new System.Windows.Forms.DataGridView();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonOpenGroup = new System.Windows.Forms.Button();
            this.buttonOpenSelected = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonPrevious = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBrowserList)).BeginInit();
            this.SuspendLayout();
            // 
            // listViewGroup
            // 
            this.listViewGroup.HideSelection = false;
            this.listViewGroup.Location = new System.Drawing.Point(3, 113);
            this.listViewGroup.MultiSelect = false;
            this.listViewGroup.Name = "listViewGroup";
            this.listViewGroup.Size = new System.Drawing.Size(232, 575);
            this.listViewGroup.TabIndex = 60;
            this.listViewGroup.UseCompatibleStateImageBehavior = false;
            this.listViewGroup.View = System.Windows.Forms.View.Details;
            // 
            // dataGridViewBrowserList
            // 
            this.dataGridViewBrowserList.AllowUserToAddRows = false;
            this.dataGridViewBrowserList.AllowUserToDeleteRows = false;
            this.dataGridViewBrowserList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewBrowserList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBrowserList.Location = new System.Drawing.Point(242, 113);
            this.dataGridViewBrowserList.Name = "dataGridViewBrowserList";
            this.dataGridViewBrowserList.RowTemplate.Height = 23;
            this.dataGridViewBrowserList.Size = new System.Drawing.Size(768, 575);
            this.dataGridViewBrowserList.TabIndex = 80;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(3, 84);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(63, 23);
            this.buttonAdd.TabIndex = 30;
            this.buttonAdd.Text = "增加组";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Location = new System.Drawing.Point(72, 84);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(63, 23);
            this.buttonEdit.TabIndex = 40;
            this.buttonEdit.Text = "编辑组";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(141, 84);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(63, 23);
            this.buttonDelete.TabIndex = 50;
            this.buttonDelete.Text = "删除组";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonOpenGroup
            // 
            this.buttonOpenGroup.Location = new System.Drawing.Point(3, 21);
            this.buttonOpenGroup.Name = "buttonOpenGroup";
            this.buttonOpenGroup.Size = new System.Drawing.Size(132, 41);
            this.buttonOpenGroup.TabIndex = 10;
            this.buttonOpenGroup.Text = "打开整组的浏览器";
            this.buttonOpenGroup.UseVisualStyleBackColor = true;
            this.buttonOpenGroup.Click += new System.EventHandler(this.buttonOpenGroup_Click);
            // 
            // buttonOpenSelected
            // 
            this.buttonOpenSelected.Location = new System.Drawing.Point(141, 21);
            this.buttonOpenSelected.Name = "buttonOpenSelected";
            this.buttonOpenSelected.Size = new System.Drawing.Size(132, 41);
            this.buttonOpenSelected.TabIndex = 20;
            this.buttonOpenSelected.Text = "打开选中的浏览器";
            this.buttonOpenSelected.UseVisualStyleBackColor = true;
            this.buttonOpenSelected.Click += new System.EventHandler(this.buttonOpenSelected_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(242, 84);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(117, 23);
            this.buttonSave.TabIndex = 70;
            this.buttonSave.Text = "保存浏览器参数";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Location = new System.Drawing.Point(770, 84);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(117, 23);
            this.buttonNext.TabIndex = 81;
            this.buttonNext.Text = "选择下一批";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonPrevious
            // 
            this.buttonPrevious.Location = new System.Drawing.Point(893, 84);
            this.buttonPrevious.Name = "buttonPrevious";
            this.buttonPrevious.Size = new System.Drawing.Size(117, 23);
            this.buttonPrevious.TabIndex = 82;
            this.buttonPrevious.Text = "选择上一批";
            this.buttonPrevious.UseVisualStyleBackColor = true;
            this.buttonPrevious.Click += new System.EventHandler(this.buttonPrevious_Click);
            // 
            // SelectGroupView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1013, 694);
            this.Controls.Add(this.buttonPrevious);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonOpenSelected);
            this.Controls.Add(this.buttonOpenGroup);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.dataGridViewBrowserList);
            this.Controls.Add(this.listViewGroup);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectGroupView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "选择浏览器";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBrowserList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewGroup;
        private System.Windows.Forms.DataGridView dataGridViewBrowserList;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonOpenGroup;
        private System.Windows.Forms.Button buttonOpenSelected;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonPrevious;
    }
}