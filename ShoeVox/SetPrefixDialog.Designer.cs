namespace ShoeVox
{
    partial class SetPrefixDialog
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
            this.prefixText = new System.Windows.Forms.TextBox();
            this.setPrefixOK = new System.Windows.Forms.Button();
            this.setPrefixCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // prefixText
            // 
            this.prefixText.Location = new System.Drawing.Point(38, 75);
            this.prefixText.Name = "prefixText";
            this.prefixText.Size = new System.Drawing.Size(197, 20);
            this.prefixText.TabIndex = 0;
            // 
            // setPrefixOK
            // 
            this.setPrefixOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.setPrefixOK.Location = new System.Drawing.Point(147, 121);
            this.setPrefixOK.Name = "setPrefixOK";
            this.setPrefixOK.Size = new System.Drawing.Size(75, 23);
            this.setPrefixOK.TabIndex = 1;
            this.setPrefixOK.Text = "OK";
            this.setPrefixOK.UseVisualStyleBackColor = true;
            // 
            // setPrefixCancel
            // 
            this.setPrefixCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.setPrefixCancel.Location = new System.Drawing.Point(52, 121);
            this.setPrefixCancel.Name = "setPrefixCancel";
            this.setPrefixCancel.Size = new System.Drawing.Size(75, 23);
            this.setPrefixCancel.TabIndex = 2;
            this.setPrefixCancel.Text = "Cancel";
            this.setPrefixCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(207, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "What do you want your voice prefix to be?";
            // 
            // SetPrefixDialog
            // 
            this.AcceptButton = this.setPrefixOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.setPrefixCancel;
            this.ClientSize = new System.Drawing.Size(269, 180);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.setPrefixCancel);
            this.Controls.Add(this.setPrefixOK);
            this.Controls.Add(this.prefixText);
            this.Name = "SetPrefixDialog";
            this.Text = "Set Prefix";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox prefixText;
        private System.Windows.Forms.Button setPrefixOK;
        private System.Windows.Forms.Button setPrefixCancel;
        private System.Windows.Forms.Label label1;
    }
}