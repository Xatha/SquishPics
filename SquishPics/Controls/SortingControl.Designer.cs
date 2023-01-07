namespace SquishPics.Controls
{
    partial class SortingControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SortingOrderComboBox = new System.Windows.Forms.ComboBox();
            this.SortingModesLabel = new System.Windows.Forms.Label();
            this.SortingModesComboBox = new System.Windows.Forms.ComboBox();
            this.MaxFileSizeTextBox = new System.Windows.Forms.TextBox();
            this.MaxFileSizeLabel = new System.Windows.Forms.Label();
            this.MaxFileSizeNUD = new CleanNumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.MaxFileSizeNUD)).BeginInit();
            this.SuspendLayout();
            // 
            // SortingOrderComboBox
            // 
            this.SortingOrderComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SortingOrderComboBox.FormattingEnabled = true;
            this.SortingOrderComboBox.Items.AddRange(new object[] {
            "Ascending",
            "Descending"});
            this.SortingOrderComboBox.Location = new System.Drawing.Point(109, 18);
            this.SortingOrderComboBox.Name = "SortingOrderComboBox";
            this.SortingOrderComboBox.Size = new System.Drawing.Size(90, 23);
            this.SortingOrderComboBox.TabIndex = 20;
            // 
            // SortingModesLabel
            // 
            this.SortingModesLabel.AutoSize = true;
            this.SortingModesLabel.BackColor = System.Drawing.SystemColors.Control;
            this.SortingModesLabel.Location = new System.Drawing.Point(0, 0);
            this.SortingModesLabel.Name = "SortingModesLabel";
            this.SortingModesLabel.Size = new System.Drawing.Size(44, 15);
            this.SortingModesLabel.TabIndex = 19;
            this.SortingModesLabel.Text = "Sort by";
            // 
            // SortingModesComboBox
            // 
            this.SortingModesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SortingModesComboBox.FormattingEnabled = true;
            this.SortingModesComboBox.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.SortingModesComboBox.Items.AddRange(new object[] {
            "Name",
            "Date Modified",
            "Type",
            "Size"});
            this.SortingModesComboBox.Location = new System.Drawing.Point(0, 18);
            this.SortingModesComboBox.Name = "SortingModesComboBox";
            this.SortingModesComboBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SortingModesComboBox.Size = new System.Drawing.Size(107, 23);
            this.SortingModesComboBox.TabIndex = 18;
            // 
            // MaxFileSizeTextBox
            // 
            this.MaxFileSizeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.MaxFileSizeTextBox.Location = new System.Drawing.Point(205, 18);
            this.MaxFileSizeTextBox.Name = "MaxFileSizeTextBox";
            this.MaxFileSizeTextBox.Size = new System.Drawing.Size(103, 23);
            this.MaxFileSizeTextBox.TabIndex = 21;
            // 
            // MaxFileSizeLabel
            // 
            this.MaxFileSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MaxFileSizeLabel.AutoSize = true;
            this.MaxFileSizeLabel.BackColor = System.Drawing.SystemColors.Control;
            this.MaxFileSizeLabel.Location = new System.Drawing.Point(205, 0);
            this.MaxFileSizeLabel.Name = "MaxFileSizeLabel";
            this.MaxFileSizeLabel.Size = new System.Drawing.Size(103, 15);
            this.MaxFileSizeLabel.TabIndex = 22;
            this.MaxFileSizeLabel.Text = "Max File Size (MB)";
            // 
            // MaxFileSizeNUD
            // 
            this.MaxFileSizeNUD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MaxFileSizeNUD.Location = new System.Drawing.Point(205, 18);
            this.MaxFileSizeNUD.Maximum = 200;
            this.MaxFileSizeNUD.Minimum = 7;
            this.MaxFileSizeNUD.Name = "MaxFileSizeNUD";
            this.MaxFileSizeNUD.Size = new System.Drawing.Size(103, 23);
            this.MaxFileSizeNUD.TabIndex = 23;
            this.MaxFileSizeNUD.Value = 7;
            // 
            // SortingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MaxFileSizeNUD);
            this.Controls.Add(this.MaxFileSizeLabel);
            this.Controls.Add(this.MaxFileSizeTextBox);
            this.Controls.Add(this.SortingOrderComboBox);
            this.Controls.Add(this.SortingModesLabel);
            this.Controls.Add(this.SortingModesComboBox);
            this.Name = "SortingControl";
            this.Size = new System.Drawing.Size(310, 42);
            ((System.ComponentModel.ISupportInitialize)(this.MaxFileSizeNUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private ComboBox SortingOrderComboBox;
        private Label SortingModesLabel;
        private ComboBox SortingModesComboBox;
        private TextBox MaxFileSizeTextBox;
        private Label MaxFileSizeLabel;
        private CleanNumericUpDown MaxFileSizeNUD;
    }
}
