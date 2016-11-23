namespace MultiControl
{
    partial class FormSDCard
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
            this.labelDescription = new System.Windows.Forms.Label();
            this.buttonApply = new System.Windows.Forms.Button();
            this.dataGridViewSDCard = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSDCard)).BeginInit();
            this.SuspendLayout();
            // 
            // labelDescription
            // 
            this.labelDescription.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescription.ForeColor = System.Drawing.Color.Red;
            this.labelDescription.Location = new System.Drawing.Point(12, 9);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(430, 61);
            this.labelDescription.TabIndex = 2;
            // 
            // buttonApply
            // 
            this.buttonApply.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonApply.Location = new System.Drawing.Point(12, 291);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(299, 37);
            this.buttonApply.TabIndex = 3;
            this.buttonApply.Text = "Apply Current SDCard Path Config Table";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // dataGridViewSDCard
            // 
            this.dataGridViewSDCard.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSDCard.Location = new System.Drawing.Point(12, 80);
            this.dataGridViewSDCard.Name = "dataGridViewSDCard";
            this.dataGridViewSDCard.Size = new System.Drawing.Size(459, 203);
            this.dataGridViewSDCard.TabIndex = 4;
            // 
            // FormSDCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 340);
            this.Controls.Add(this.dataGridViewSDCard);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.labelDescription);
            this.Name = "FormSDCard";
            this.Text = "SD Card Path Config";
            this.Load += new System.EventHandler(this.FormSDCard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSDCard)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.DataGridView dataGridViewSDCard;
    }
}