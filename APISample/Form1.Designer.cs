namespace SMARTQuery
{
    partial class Form1
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.FeatureColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContentColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.IDColumn_S = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FeatureColumn_S = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContentColumn_S = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RAWColumn_S = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.radioButton_USB = new System.Windows.Forms.RadioButton();
            this.volumeComboBox = new System.Windows.Forms.ComboBox();
            this.radioButton_ATA = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton_NVMe = new System.Windows.Forms.RadioButton();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(12, 109);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(646, 402);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage1.Location = new System.Drawing.Point(4, 32);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage1.Size = new System.Drawing.Size(638, 366);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Drive Info";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FeatureColumn,
            this.ContentColumn});
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(632, 360);
            this.dataGridView1.TabIndex = 0;
            // 
            // FeatureColumn
            // 
            this.FeatureColumn.HeaderText = "Feature";
            this.FeatureColumn.MinimumWidth = 6;
            this.FeatureColumn.Name = "FeatureColumn";
            // 
            // ContentColumn
            // 
            this.ContentColumn.HeaderText = "Content";
            this.ContentColumn.MinimumWidth = 6;
            this.ContentColumn.Name = "ContentColumn";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView2);
            this.tabPage2.Location = new System.Drawing.Point(4, 32);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage2.Size = new System.Drawing.Size(638, 366);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "SMART Info";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IDColumn_S,
            this.FeatureColumn_S,
            this.ContentColumn_S,
            this.RAWColumn_S});
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.Location = new System.Drawing.Point(3, 3);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.RowHeadersWidth = 51;
            this.dataGridView2.RowTemplate.Height = 24;
            this.dataGridView2.Size = new System.Drawing.Size(632, 360);
            this.dataGridView2.TabIndex = 0;
            // 
            // IDColumn_S
            // 
            this.IDColumn_S.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.IDColumn_S.FillWeight = 25F;
            this.IDColumn_S.Frozen = true;
            this.IDColumn_S.HeaderText = "ID";
            this.IDColumn_S.MinimumWidth = 6;
            this.IDColumn_S.Name = "IDColumn_S";
            this.IDColumn_S.Width = 48;
            // 
            // FeatureColumn_S
            // 
            this.FeatureColumn_S.FillWeight = 180F;
            this.FeatureColumn_S.HeaderText = "Feature";
            this.FeatureColumn_S.MinimumWidth = 6;
            this.FeatureColumn_S.Name = "FeatureColumn_S";
            // 
            // ContentColumn_S
            // 
            this.ContentColumn_S.FillWeight = 60F;
            this.ContentColumn_S.HeaderText = "Content";
            this.ContentColumn_S.MinimumWidth = 6;
            this.ContentColumn_S.Name = "ContentColumn_S";
            // 
            // RAWColumn_S
            // 
            this.RAWColumn_S.FillWeight = 60F;
            this.RAWColumn_S.HeaderText = "Raw";
            this.RAWColumn_S.MinimumWidth = 6;
            this.RAWColumn_S.Name = "RAWColumn_S";
            this.RAWColumn_S.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // radioButton_USB
            // 
            this.radioButton_USB.AutoSize = true;
            this.radioButton_USB.Location = new System.Drawing.Point(75, 25);
            this.radioButton_USB.Name = "radioButton_USB";
            this.radioButton_USB.Size = new System.Drawing.Size(61, 24);
            this.radioButton_USB.TabIndex = 5;
            this.radioButton_USB.Text = "USB";
            this.radioButton_USB.UseVisualStyleBackColor = true;
            this.radioButton_USB.CheckedChanged += new System.EventHandler(this.radioButton_USB_CheckedChanged);
            // 
            // volumeComboBox
            // 
            this.volumeComboBox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.volumeComboBox.FormattingEnabled = true;
            this.volumeComboBox.Location = new System.Drawing.Point(9, 51);
            this.volumeComboBox.Name = "volumeComboBox";
            this.volumeComboBox.Size = new System.Drawing.Size(515, 23);
            this.volumeComboBox.TabIndex = 0;
            this.volumeComboBox.SelectedIndexChanged += new System.EventHandler(this.volumeComboBox_SelectedIndexChanged);
            // 
            // radioButton_ATA
            // 
            this.radioButton_ATA.AutoSize = true;
            this.radioButton_ATA.Checked = true;
            this.radioButton_ATA.Location = new System.Drawing.Point(9, 25);
            this.radioButton_ATA.Name = "radioButton_ATA";
            this.radioButton_ATA.Size = new System.Drawing.Size(69, 24);
            this.radioButton_ATA.TabIndex = 4;
            this.radioButton_ATA.TabStop = true;
            this.radioButton_ATA.Text = "SATA";
            this.radioButton_ATA.UseVisualStyleBackColor = true;
            this.radioButton_ATA.CheckedChanged += new System.EventHandler(this.radioButton_ATA_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton_NVMe);
            this.groupBox1.Controls.Add(this.radioButton_USB);
            this.groupBox1.Controls.Add(this.volumeComboBox);
            this.groupBox1.Controls.Add(this.radioButton_ATA);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(530, 88);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select The Drive Interface";
            // 
            // radioButton_NVMe
            // 
            this.radioButton_NVMe.AutoSize = true;
            this.radioButton_NVMe.Location = new System.Drawing.Point(135, 25);
            this.radioButton_NVMe.Name = "radioButton_NVMe";
            this.radioButton_NVMe.Size = new System.Drawing.Size(71, 24);
            this.radioButton_NVMe.TabIndex = 8;
            this.radioButton_NVMe.TabStop = true;
            this.radioButton_NVMe.Text = "NVMe";
            this.radioButton_NVMe.UseVisualStyleBackColor = true;
            this.radioButton_NVMe.CheckedChanged += new System.EventHandler(this.radioButton_NVMe_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 523);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "S.M.A.R.T Information Query";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn FeatureColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContentColumn;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewTextBoxColumn IDColumn_S;
        private System.Windows.Forms.DataGridViewTextBoxColumn FeatureColumn_S;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContentColumn_S;
        private System.Windows.Forms.RadioButton radioButton_USB;
        private System.Windows.Forms.ComboBox volumeComboBox;
        private System.Windows.Forms.RadioButton radioButton_ATA;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton_NVMe;
        private System.Windows.Forms.DataGridViewTextBoxColumn RAWColumn_S;
    }
}

