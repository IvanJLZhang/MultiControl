namespace MultiControl
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
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startSelectedAndroidDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelMappingConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.phoneSDCardPathConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logOutputConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configuratToSysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPortIndexTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPortIndexTableToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.initializeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tERequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.checkBoxModel = new System.Windows.Forms.CheckBox();
            this.buttonUserControlLoad = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(26, 487);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 38);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.advanceToolStripMenuItem,
            this.tERequestToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 28);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(203, 25);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startTestToolStripMenuItem,
            this.startSelectedAndroidDeviceToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(63, 21);
            this.fileToolStripMenuItem.Text = "Control";
            // 
            // startTestToolStripMenuItem
            // 
            this.startTestToolStripMenuItem.Name = "startTestToolStripMenuItem";
            this.startTestToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.startTestToolStripMenuItem.Text = "Start Test";
            this.startTestToolStripMenuItem.Click += new System.EventHandler(this.startTestToolStripMenuItem_Click);
            // 
            // startSelectedAndroidDeviceToolStripMenuItem
            // 
            this.startSelectedAndroidDeviceToolStripMenuItem.Name = "startSelectedAndroidDeviceToolStripMenuItem";
            this.startSelectedAndroidDeviceToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.startSelectedAndroidDeviceToolStripMenuItem.Text = "Start Selected Android Device";
            this.startSelectedAndroidDeviceToolStripMenuItem.Click += new System.EventHandler(this.startSelectedAndroidDeviceToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.resetToolStripMenuItem.Text = "&Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // advanceToolStripMenuItem
            // 
            this.advanceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurationToolStripMenuItem,
            this.modelMappingConfigurationToolStripMenuItem,
            this.phoneSDCardPathConfigurationToolStripMenuItem,
            this.logOutputConfigurationToolStripMenuItem,
            this.configuratToSysToolStripMenuItem,
            this.viewPortIndexTableToolStripMenuItem});
            this.advanceToolStripMenuItem.Name = "advanceToolStripMenuItem";
            this.advanceToolStripMenuItem.Size = new System.Drawing.Size(77, 21);
            this.advanceToolStripMenuItem.Text = "Configure";
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(304, 22);
            this.configurationToolStripMenuItem.Text = "Single Model Configuration";
            this.configurationToolStripMenuItem.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
            // 
            // modelMappingConfigurationToolStripMenuItem
            // 
            this.modelMappingConfigurationToolStripMenuItem.Name = "modelMappingConfigurationToolStripMenuItem";
            this.modelMappingConfigurationToolStripMenuItem.Size = new System.Drawing.Size(304, 22);
            this.modelMappingConfigurationToolStripMenuItem.Text = "Multi Model  Configuration";
            this.modelMappingConfigurationToolStripMenuItem.Click += new System.EventHandler(this.modelMappingConfigurationToolStripMenuItem_Click);
            // 
            // phoneSDCardPathConfigurationToolStripMenuItem
            // 
            this.phoneSDCardPathConfigurationToolStripMenuItem.Name = "phoneSDCardPathConfigurationToolStripMenuItem";
            this.phoneSDCardPathConfigurationToolStripMenuItem.Size = new System.Drawing.Size(304, 22);
            this.phoneSDCardPathConfigurationToolStripMenuItem.Text = "Phone Internal Card Path Configuration";
            this.phoneSDCardPathConfigurationToolStripMenuItem.Click += new System.EventHandler(this.phoneSDCardPathConfigurationToolStripMenuItem_Click);
            // 
            // logOutputConfigurationToolStripMenuItem
            // 
            this.logOutputConfigurationToolStripMenuItem.Name = "logOutputConfigurationToolStripMenuItem";
            this.logOutputConfigurationToolStripMenuItem.Size = new System.Drawing.Size(304, 22);
            this.logOutputConfigurationToolStripMenuItem.Text = "Log Output Configuration";
            this.logOutputConfigurationToolStripMenuItem.Click += new System.EventHandler(this.logOutputConfigurationToolStripMenuItem_Click);
            // 
            // configuratToSysToolStripMenuItem
            // 
            this.configuratToSysToolStripMenuItem.Name = "configuratToSysToolStripMenuItem";
            this.configuratToSysToolStripMenuItem.Size = new System.Drawing.Size(304, 22);
            this.configuratToSysToolStripMenuItem.Text = "Configurate to sysinfo.cfg location";
            this.configuratToSysToolStripMenuItem.Click += new System.EventHandler(this.configuratToSysToolStripMenuItem_Click);
            // 
            // viewPortIndexTableToolStripMenuItem
            // 
            this.viewPortIndexTableToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewPortIndexTableToolStripMenuItem1,
            this.initializeToolStripMenuItem});
            this.viewPortIndexTableToolStripMenuItem.Name = "viewPortIndexTableToolStripMenuItem";
            this.viewPortIndexTableToolStripMenuItem.Size = new System.Drawing.Size(304, 22);
            this.viewPortIndexTableToolStripMenuItem.Text = "Port<-->Index Table";
            // 
            // viewPortIndexTableToolStripMenuItem1
            // 
            this.viewPortIndexTableToolStripMenuItem1.Name = "viewPortIndexTableToolStripMenuItem1";
            this.viewPortIndexTableToolStripMenuItem1.Size = new System.Drawing.Size(247, 22);
            this.viewPortIndexTableToolStripMenuItem1.Text = "&View Port<-->Index Table";
            this.viewPortIndexTableToolStripMenuItem1.Click += new System.EventHandler(this.viewPortIndexTableToolStripMenuItem1_Click);
            // 
            // initializeToolStripMenuItem
            // 
            this.initializeToolStripMenuItem.Name = "initializeToolStripMenuItem";
            this.initializeToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.initializeToolStripMenuItem.Text = "Initialize Port<-->Index Table";
            this.initializeToolStripMenuItem.Click += new System.EventHandler(this.initializeToolStripMenuItem_Click);
            // 
            // tERequestToolStripMenuItem
            // 
            this.tERequestToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem});
            this.tERequestToolStripMenuItem.Name = "tERequestToolStripMenuItem";
            this.tERequestToolStripMenuItem.Size = new System.Drawing.Size(85, 21);
            this.tERequestToolStripMenuItem.Text = "TE Request";
            this.tERequestToolStripMenuItem.Visible = false;
            // 
            // ToolStripMenuItem
            // 
            this.ToolStripMenuItem.Name = "ToolStripMenuItem";
            this.ToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.ToolStripMenuItem.Text = "Script Support Configuration";
            this.ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(55, 21);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoEllipsis = true;
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(12, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 22);
            this.label1.TabIndex = 11;
            this.label1.Text = "CFG >>>";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Visible = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 531);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(890, 20);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(875, 15);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "Build 20150929 Version 1.1.2";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxModel
            // 
            this.checkBoxModel.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxModel.Checked = true;
            this.checkBoxModel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxModel.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxModel.Location = new System.Drawing.Point(685, 11);
            this.checkBoxModel.Name = "checkBoxModel";
            this.checkBoxModel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.checkBoxModel.Size = new System.Drawing.Size(193, 22);
            this.checkBoxModel.TabIndex = 13;
            this.checkBoxModel.Text = "Multi DUT Model Support";
            this.checkBoxModel.UseVisualStyleBackColor = false;
            this.checkBoxModel.Visible = false;
            this.checkBoxModel.CheckedChanged += new System.EventHandler(this.checkBoxModel_CheckedChanged);
            // 
            // buttonUserControlLoad
            // 
            this.buttonUserControlLoad.Location = new System.Drawing.Point(180, 498);
            this.buttonUserControlLoad.Name = "buttonUserControlLoad";
            this.buttonUserControlLoad.Size = new System.Drawing.Size(203, 21);
            this.buttonUserControlLoad.TabIndex = 14;
            this.buttonUserControlLoad.Text = "User Control Load";
            this.buttonUserControlLoad.UseVisualStyleBackColor = true;
            this.buttonUserControlLoad.Visible = false;
            this.buttonUserControlLoad.Click += new System.EventHandler(this.buttonUserControlLoad_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Yellow;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(32, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 32);
            this.label2.TabIndex = 15;
            this.label2.Text = "INSTALL PQAA";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(142, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 32);
            this.label3.TabIndex = 16;
            this.label3.Text = "TESTING";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Red;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(365, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 32);
            this.label4.TabIndex = 17;
            this.label4.Text = "FAIL";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(475, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 32);
            this.label5.TabIndex = 18;
            this.label5.Text = "PASS";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(254, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 32);
            this.label6.TabIndex = 19;
            this.label6.Text = "OFFLINE";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 551);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonUserControlLoad);
            this.Controls.Add(this.checkBoxModel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wistron Multi Android Control Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseEnter += new System.EventHandler(this.Form1_MouseEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startSelectedAndroidDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelMappingConfigurationToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxModel;
        private System.Windows.Forms.Button buttonUserControlLoad;
        private System.Windows.Forms.ToolStripMenuItem logOutputConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tERequestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolStripMenuItem phoneSDCardPathConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configuratToSysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPortIndexTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPortIndexTableToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem initializeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}

