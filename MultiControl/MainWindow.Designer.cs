namespace MultiControl
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.ms_CommandBar = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_operator = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeOperatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startSelectedDevicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewGlobalLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultConfigPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiModelConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.initializeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portIndexTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.initializePortIndexTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPortIndexTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sDCardPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.initializeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.configurateToSysinfocfgLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pQAAAPKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewPQAAApkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.ms_CommandBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // ms_CommandBar
            // 
            this.ms_CommandBar.Dock = System.Windows.Forms.DockStyle.None;
            this.ms_CommandBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.configurationToolStripMenuItem,
            this.pQAAAPKToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.ms_CommandBar.Location = new System.Drawing.Point(0, 0);
            this.ms_CommandBar.Name = "ms_CommandBar";
            this.ms_CommandBar.Size = new System.Drawing.Size(201, 25);
            this.ms_CommandBar.TabIndex = 0;
            this.ms_CommandBar.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsm_operator,
            this.startToolStripMenuItem,
            this.startSelectedDevicesToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.viewGlobalLogToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // tsm_operator
            // 
            this.tsm_operator.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logoutToolStripMenuItem,
            this.changeOperatorToolStripMenuItem});
            this.tsm_operator.Name = "tsm_operator";
            this.tsm_operator.Size = new System.Drawing.Size(209, 22);
            this.tsm_operator.Text = "&User";
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.logoutToolStripMenuItem.Text = "&Logout";
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.logoutToolStripMenuItem_Click);
            // 
            // changeOperatorToolStripMenuItem
            // 
            this.changeOperatorToolStripMenuItem.Name = "changeOperatorToolStripMenuItem";
            this.changeOperatorToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.changeOperatorToolStripMenuItem.Text = "&Change Operator";
            this.changeOperatorToolStripMenuItem.Click += new System.EventHandler(this.switchUserToolStripMenuItem_Click);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.startToolStripMenuItem.Text = "&Start All";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // startSelectedDevicesToolStripMenuItem
            // 
            this.startSelectedDevicesToolStripMenuItem.Name = "startSelectedDevicesToolStripMenuItem";
            this.startSelectedDevicesToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.startSelectedDevicesToolStripMenuItem.Text = "Start Selected Device/s";
            this.startSelectedDevicesToolStripMenuItem.Click += new System.EventHandler(this.startSelectedDevicesToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.resetToolStripMenuItem.Text = "&Reset All";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // viewGlobalLogToolStripMenuItem
            // 
            this.viewGlobalLogToolStripMenuItem.Name = "viewGlobalLogToolStripMenuItem";
            this.viewGlobalLogToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.viewGlobalLogToolStripMenuItem.Text = "&View Global Log";
            this.viewGlobalLogToolStripMenuItem.Click += new System.EventHandler(this.viewGlobalLogToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultConfigPathToolStripMenuItem,
            this.multiModelConfigurationToolStripMenuItem,
            this.portIndexTableToolStripMenuItem,
            this.sDCardPathToolStripMenuItem,
            this.configurateToSysinfocfgLocationToolStripMenuItem});
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(99, 21);
            this.configurationToolStripMenuItem.Text = "&Configuration";
            // 
            // defaultConfigPathToolStripMenuItem
            // 
            this.defaultConfigPathToolStripMenuItem.Name = "defaultConfigPathToolStripMenuItem";
            this.defaultConfigPathToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.defaultConfigPathToolStripMenuItem.Text = "Default config Path";
            this.defaultConfigPathToolStripMenuItem.Click += new System.EventHandler(this.defaultConfigPathToolStripMenuItem_Click);
            // 
            // multiModelConfigurationToolStripMenuItem
            // 
            this.multiModelConfigurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.initializeToolStripMenuItem,
            this.editToolStripMenuItem});
            this.multiModelConfigurationToolStripMenuItem.Name = "multiModelConfigurationToolStripMenuItem";
            this.multiModelConfigurationToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.multiModelConfigurationToolStripMenuItem.Text = "Specified config Path";
            // 
            // initializeToolStripMenuItem
            // 
            this.initializeToolStripMenuItem.Name = "initializeToolStripMenuItem";
            this.initializeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.initializeToolStripMenuItem.Text = "Initialize";
            this.initializeToolStripMenuItem.Click += new System.EventHandler(this.initializeToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.multiModelConfigurationToolStripMenuItem_Click);
            // 
            // portIndexTableToolStripMenuItem
            // 
            this.portIndexTableToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.initializePortIndexTableToolStripMenuItem,
            this.viewPortIndexTableToolStripMenuItem});
            this.portIndexTableToolStripMenuItem.Name = "portIndexTableToolStripMenuItem";
            this.portIndexTableToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.portIndexTableToolStripMenuItem.Text = "Port<-->Index Table";
            // 
            // initializePortIndexTableToolStripMenuItem
            // 
            this.initializePortIndexTableToolStripMenuItem.Name = "initializePortIndexTableToolStripMenuItem";
            this.initializePortIndexTableToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.initializePortIndexTableToolStripMenuItem.Text = "&Initialize";
            this.initializePortIndexTableToolStripMenuItem.Click += new System.EventHandler(this.initializePortIndexTableToolStripMenuItem_Click);
            // 
            // viewPortIndexTableToolStripMenuItem
            // 
            this.viewPortIndexTableToolStripMenuItem.Name = "viewPortIndexTableToolStripMenuItem";
            this.viewPortIndexTableToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.viewPortIndexTableToolStripMenuItem.Text = "&View";
            this.viewPortIndexTableToolStripMenuItem.Click += new System.EventHandler(this.viewPortIndexTableToolStripMenuItem_Click);
            // 
            // sDCardPathToolStripMenuItem
            // 
            this.sDCardPathToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.initializeToolStripMenuItem1,
            this.editToolStripMenuItem1});
            this.sDCardPathToolStripMenuItem.Name = "sDCardPathToolStripMenuItem";
            this.sDCardPathToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.sDCardPathToolStripMenuItem.Text = "SD Card Path";
            // 
            // initializeToolStripMenuItem1
            // 
            this.initializeToolStripMenuItem1.Name = "initializeToolStripMenuItem1";
            this.initializeToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
            this.initializeToolStripMenuItem1.Text = "&Initialize";
            // 
            // editToolStripMenuItem1
            // 
            this.editToolStripMenuItem1.Name = "editToolStripMenuItem1";
            this.editToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
            this.editToolStripMenuItem1.Text = "&Edit";
            this.editToolStripMenuItem1.Click += new System.EventHandler(this.editToolStripMenuItem1_Click);
            // 
            // configurateToSysinfocfgLocationToolStripMenuItem
            // 
            this.configurateToSysinfocfgLocationToolStripMenuItem.Name = "configurateToSysinfocfgLocationToolStripMenuItem";
            this.configurateToSysinfocfgLocationToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.configurateToSysinfocfgLocationToolStripMenuItem.Text = "Specify sysinfo.cfg file";
            this.configurateToSysinfocfgLocationToolStripMenuItem.Click += new System.EventHandler(this.configurateToSysinfocfgLocationToolStripMenuItem_Click);
            // 
            // pQAAAPKToolStripMenuItem
            // 
            this.pQAAAPKToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewVersionToolStripMenuItem,
            this.defaultToolStripMenuItem});
            this.pQAAAPKToolStripMenuItem.Name = "pQAAAPKToolStripMenuItem";
            this.pQAAAPKToolStripMenuItem.Size = new System.Drawing.Size(79, 21);
            this.pQAAAPKToolStripMenuItem.Text = "PQAA apk";
            this.pQAAAPKToolStripMenuItem.Visible = false;
            // 
            // addNewVersionToolStripMenuItem
            // 
            this.addNewVersionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewPQAAApkToolStripMenuItem,
            this.removeAllToolStripMenuItem});
            this.addNewVersionToolStripMenuItem.Name = "addNewVersionToolStripMenuItem";
            this.addNewVersionToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.addNewVersionToolStripMenuItem.Text = "&Action";
            // 
            // addNewPQAAApkToolStripMenuItem
            // 
            this.addNewPQAAApkToolStripMenuItem.Name = "addNewPQAAApkToolStripMenuItem";
            this.addNewPQAAApkToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.addNewPQAAApkToolStripMenuItem.Text = "&Add new PQAA apk";
            // 
            // removeAllToolStripMenuItem
            // 
            this.removeAllToolStripMenuItem.Name = "removeAllToolStripMenuItem";
            this.removeAllToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.removeAllToolStripMenuItem.Text = "&Remove All but default";
            // 
            // defaultToolStripMenuItem
            // 
            this.defaultToolStripMenuItem.Checked = true;
            this.defaultToolStripMenuItem.CheckOnClick = true;
            this.defaultToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
            this.defaultToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.defaultToolStripMenuItem.Text = "&Default";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(55, 21);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            this.aboutToolStripMenuItem.MouseEnter += new System.EventHandler(this.aboutToolStripMenuItem_MouseEnter);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 523);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(886, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // pageSetupDialog1
            // 
            this.pageSetupDialog1.Document = this.printDocument1;
            // 
            // printDialog1
            // 
            this.printDialog1.Document = this.printDocument1;
            this.printDialog1.UseEXDialog = true;
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Document = this.printDocument1;
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 545);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ms_CommandBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(20, 50);
            this.MainMenuStrip = this.ms_CommandBar;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MultiControl Test Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.ms_CommandBar.ResumeLayout(false);
            this.ms_CommandBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip ms_CommandBar;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewGlobalLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multiModelConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem initializeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem portIndexTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPortIndexTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem initializePortIndexTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sDCardPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem initializeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startSelectedDevicesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pQAAAPKToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewVersionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewPQAAApkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurateToSysinfocfgLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultConfigPathToolStripMenuItem;
        private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
        private System.Windows.Forms.ToolStripMenuItem tsm_operator;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeOperatorToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}