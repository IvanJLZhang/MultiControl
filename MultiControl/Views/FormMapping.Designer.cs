namespace MultiControl
{
    partial class FormMapping
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
            this.components = new System.ComponentModel.Container();
            this.dataGridViewPath = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editTestItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMoniPowerCFGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSysInfoCFGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editWIFICFGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editAudioLoopbackCFGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMultiTestGPSCFGtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.multiTestOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wIFIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPath)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewPath
            // 
            this.dataGridViewPath.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPath.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridViewPath.Location = new System.Drawing.Point(12, 65);
            this.dataGridViewPath.Name = "dataGridViewPath";
            this.dataGridViewPath.Size = new System.Drawing.Size(651, 212);
            this.dataGridViewPath.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editTestItemsToolStripMenuItem,
            this.editMoniPowerCFGToolStripMenuItem,
            this.editSysInfoCFGToolStripMenuItem,
            this.editWIFICFGToolStripMenuItem,
            this.editAudioLoopbackCFGToolStripMenuItem,
            this.editMultiTestGPSCFGtoolStripMenuItem,
            this.toolStripMenuItem2,
            this.multiTestOptionsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.helpToolStripMenuItem,
            this.toolStripMenuItem3});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(226, 214);
            // 
            // editTestItemsToolStripMenuItem
            // 
            this.editTestItemsToolStripMenuItem.Name = "editTestItemsToolStripMenuItem";
            this.editTestItemsToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.editTestItemsToolStripMenuItem.Text = "Edit Test Items CFG";
            this.editTestItemsToolStripMenuItem.Click += new System.EventHandler(this.editTestItemsToolStripMenuItem_Click);
            // 
            // editMoniPowerCFGToolStripMenuItem
            // 
            this.editMoniPowerCFGToolStripMenuItem.Name = "editMoniPowerCFGToolStripMenuItem";
            this.editMoniPowerCFGToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.editMoniPowerCFGToolStripMenuItem.Text = "Edit Moni Power CFG";
            this.editMoniPowerCFGToolStripMenuItem.Click += new System.EventHandler(this.editMoniPowerCFGToolStripMenuItem_Click);
            // 
            // editSysInfoCFGToolStripMenuItem
            // 
            this.editSysInfoCFGToolStripMenuItem.Name = "editSysInfoCFGToolStripMenuItem";
            this.editSysInfoCFGToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.editSysInfoCFGToolStripMenuItem.Text = "Edit SysInfo CFG";
            this.editSysInfoCFGToolStripMenuItem.Click += new System.EventHandler(this.editSysInfoCFGToolStripMenuItem_Click);
            // 
            // editWIFICFGToolStripMenuItem
            // 
            this.editWIFICFGToolStripMenuItem.Name = "editWIFICFGToolStripMenuItem";
            this.editWIFICFGToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.editWIFICFGToolStripMenuItem.Text = "Edit WIFI CFG";
            this.editWIFICFGToolStripMenuItem.Click += new System.EventHandler(this.editWIFICFGToolStripMenuItem_Click);
            // 
            // editAudioLoopbackCFGToolStripMenuItem
            // 
            this.editAudioLoopbackCFGToolStripMenuItem.Name = "editAudioLoopbackCFGToolStripMenuItem";
            this.editAudioLoopbackCFGToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.editAudioLoopbackCFGToolStripMenuItem.Text = "Edit Audio Loopback CFG";
            this.editAudioLoopbackCFGToolStripMenuItem.Click += new System.EventHandler(this.editAudioLoopbackCFGToolStripMenuItem_Click);
            // 
            // editMultiTestGPSCFGtoolStripMenuItem
            // 
            this.editMultiTestGPSCFGtoolStripMenuItem.Name = "editMultiTestGPSCFGtoolStripMenuItem";
            this.editMultiTestGPSCFGtoolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.editMultiTestGPSCFGtoolStripMenuItem.Text = "Edit MultiTest GPS CFG";
            this.editMultiTestGPSCFGtoolStripMenuItem.Click += new System.EventHandler(this.editMultiTestGPSCFGtoolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(222, 6);
            // 
            // multiTestOptionsToolStripMenuItem
            // 
            this.multiTestOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wIFIToolStripMenuItem,
            this.gPSToolStripMenuItem});
            this.multiTestOptionsToolStripMenuItem.Name = "multiTestOptionsToolStripMenuItem";
            this.multiTestOptionsToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.multiTestOptionsToolStripMenuItem.Text = "Multi Test Options";
            // 
            // wIFIToolStripMenuItem
            // 
            this.wIFIToolStripMenuItem.Name = "wIFIToolStripMenuItem";
            this.wIFIToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.wIFIToolStripMenuItem.Text = "WIFI";
            this.wIFIToolStripMenuItem.Click += new System.EventHandler(this.wIFIToolStripMenuItem_Click);
            // 
            // gPSToolStripMenuItem
            // 
            this.gPSToolStripMenuItem.Name = "gPSToolStripMenuItem";
            this.gPSToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.gPSToolStripMenuItem.Text = "GPS";
            this.gPSToolStripMenuItem.Click += new System.EventHandler(this.gPSToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(222, 6);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(225, 22);
            this.toolStripMenuItem3.Text = "?";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(567, 54);
            this.label1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(12, 282);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(260, 34);
            this.button1.TabIndex = 2;
            this.button1.Text = "Apply Current Mapping Config Table";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 328);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewPath);
            this.Name = "FormMapping";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Model Path Mapping Table";
            this.Load += new System.EventHandler(this.FormMapping_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPath)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editTestItemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem editMoniPowerCFGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editSysInfoCFGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editWIFICFGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editAudioLoopbackCFGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMultiTestGPSCFGtoolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem multiTestOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wIFIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gPSToolStripMenuItem;
    }
}