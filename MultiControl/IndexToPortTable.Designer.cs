namespace MultiControl
{
    partial class IndexToPortTable
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
            this.dgv_index2port = new System.Windows.Forms.DataGridView();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.usb_port_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_index2port)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_index2port
            // 
            this.dgv_index2port.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_index2port.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Index,
            this.usb_port_no});
            this.dgv_index2port.Location = new System.Drawing.Point(12, 12);
            this.dgv_index2port.Name = "dgv_index2port";
            this.dgv_index2port.ReadOnly = true;
            this.dgv_index2port.RowTemplate.Height = 23;
            this.dgv_index2port.Size = new System.Drawing.Size(630, 421);
            this.dgv_index2port.TabIndex = 0;
            // 
            // Index
            // 
            this.Index.HeaderText = "Index";
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            // 
            // usb_port_no
            // 
            this.usb_port_no.HeaderText = "USB Port No.";
            this.usb_port_no.Name = "usb_port_no";
            this.usb_port_no.ReadOnly = true;
            // 
            // IndexToPortTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 445);
            this.Controls.Add(this.dgv_index2port);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IndexToPortTable";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "IndexToPortTable";
            ((System.ComponentModel.ISupportInitialize)(this.dgv_index2port)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv_index2port;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn usb_port_no;
    }
}