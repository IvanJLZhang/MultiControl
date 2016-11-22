namespace MultiControl
{
    partial class PortToIndexForm
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
            this.cb_Index = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tb_PortNo = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chb_isEnabled = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cb_Index
            // 
            this.cb_Index.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Index.FormattingEnabled = true;
            this.cb_Index.Location = new System.Drawing.Point(408, 36);
            this.cb_Index.Name = "cb_Index";
            this.cb_Index.Size = new System.Drawing.Size(121, 20);
            this.cb_Index.TabIndex = 2;
            this.toolTip1.SetToolTip(this.cb_Index, "Index List");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(367, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "==>";
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(24, 168);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 23);
            this.btn_OK.TabIndex = 4;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(454, 168);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 5;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tb_PortNo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btn_Cancel);
            this.groupBox1.Controls.Add(this.btn_OK);
            this.groupBox1.Controls.Add(this.cb_Index);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(535, 206);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pick an Index to Represent USB Port No.";
            // 
            // tb_PortNo
            // 
            this.tb_PortNo.Location = new System.Drawing.Point(24, 36);
            this.tb_PortNo.Name = "tb_PortNo";
            this.tb_PortNo.ReadOnly = true;
            this.tb_PortNo.Size = new System.Drawing.Size(337, 21);
            this.tb_PortNo.TabIndex = 9;
            this.toolTip1.SetToolTip(this.tb_PortNo, "USB Port Number");
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chb_isEnabled);
            this.groupBox2.Location = new System.Drawing.Point(24, 79);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(505, 54);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            // 
            // chb_isEnabled
            // 
            this.chb_isEnabled.AutoSize = true;
            this.chb_isEnabled.Checked = true;
            this.chb_isEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb_isEnabled.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chb_isEnabled.ForeColor = System.Drawing.Color.Red;
            this.chb_isEnabled.Location = new System.Drawing.Point(24, 20);
            this.chb_isEnabled.Name = "chb_isEnabled";
            this.chb_isEnabled.Size = new System.Drawing.Size(336, 16);
            this.chb_isEnabled.TabIndex = 7;
            this.chb_isEnabled.Text = "IsEnabled to arrange Index to represent USB Port No.";
            this.chb_isEnabled.UseVisualStyleBackColor = true;
            this.chb_isEnabled.CheckedChanged += new System.EventHandler(this.chb_isEnabled_CheckedChanged);
            // 
            // PortToIndexForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 234);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PortToIndexForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "USB Port To Index Arrangement";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox cb_Index;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chb_isEnabled;
        private System.Windows.Forms.TextBox tb_PortNo;
    }
}