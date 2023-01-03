namespace SquishPics.Controls
{
    partial class APIKeyForm
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
            this.APIKEYTextBox = new System.Windows.Forms.TextBox();
            this.APIKEYLabel = new System.Windows.Forms.Label();
            this.APIKEYDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // APIKEYTextBox
            // 
            this.APIKEYTextBox.Location = new System.Drawing.Point(28, 38);
            this.APIKEYTextBox.Name = "APIKEYTextBox";
            this.APIKEYTextBox.Size = new System.Drawing.Size(175, 23);
            this.APIKEYTextBox.TabIndex = 0;
            this.APIKEYTextBox.UseSystemPasswordChar = true;
            // 
            // APIKEYLabel
            // 
            this.APIKEYLabel.AutoSize = true;
            this.APIKEYLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.APIKEYLabel.Location = new System.Drawing.Point(28, 20);
            this.APIKEYLabel.Name = "APIKEYLabel";
            this.APIKEYLabel.Size = new System.Drawing.Size(48, 15);
            this.APIKEYLabel.TabIndex = 1;
            this.APIKEYLabel.Text = "API KEY";
            // 
            // APIKEYDescription
            // 
            this.APIKEYDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.APIKEYDescription.AutoSize = true;
            this.APIKEYDescription.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.APIKEYDescription.Location = new System.Drawing.Point(20, 65);
            this.APIKEYDescription.Name = "APIKEYDescription";
            this.APIKEYDescription.Size = new System.Drawing.Size(189, 30);
            this.APIKEYDescription.TabIndex = 2;
            this.APIKEYDescription.Text = "The API KEY is very secret and you \r\nshould not show this to anyone";
            // 
            // APIKeyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(234, 130);
            this.Controls.Add(this.APIKEYDescription);
            this.Controls.Add(this.APIKEYLabel);
            this.Controls.Add(this.APIKEYTextBox);
            this.ForeColor = System.Drawing.SystemColors.Control;
            this.MaximumSize = new System.Drawing.Size(250, 169);
            this.MinimumSize = new System.Drawing.Size(250, 169);
            this.Name = "APIKeyForm";
            this.Text = "APIKeyForm";
            this.Load += new System.EventHandler(this.APIKeyForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox APIKEYTextBox;
        private Label APIKEYLabel;
        private Label APIKEYDescription;
    }
}