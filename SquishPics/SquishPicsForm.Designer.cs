namespace SquishPics
{
    partial class SquishPicsForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StatusProgressBar = new System.Windows.Forms.ProgressBar();
            this.StatusProgressBarLabel = new System.Windows.Forms.Label();
            this.StartStopButton = new System.Windows.Forms.Button();
            this.ApiKeyButton = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.serverChannelSelector1 = new SquishPics.Controls.ServerChannelSelector();
            this.SuspendLayout();
            // 
            // StatusProgressBar
            // 
            this.StatusProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StatusProgressBar.Location = new System.Drawing.Point(12, 412);
            this.StatusProgressBar.Name = "StatusProgressBar";
            this.StatusProgressBar.Size = new System.Drawing.Size(237, 26);
            this.StatusProgressBar.TabIndex = 0;
            // 
            // StatusProgressBarLabel
            // 
            this.StatusProgressBarLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StatusProgressBarLabel.AutoSize = true;
            this.StatusProgressBarLabel.Location = new System.Drawing.Point(12, 394);
            this.StatusProgressBarLabel.Name = "StatusProgressBarLabel";
            this.StatusProgressBarLabel.Size = new System.Drawing.Size(42, 15);
            this.StatusProgressBarLabel.TabIndex = 1;
            this.StatusProgressBarLabel.Text = "Status:";
            // 
            // StartStopButton
            // 
            this.StartStopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StartStopButton.Location = new System.Drawing.Point(556, 412);
            this.StartStopButton.Name = "StartStopButton";
            this.StartStopButton.Size = new System.Drawing.Size(103, 23);
            this.StartStopButton.TabIndex = 5;
            this.StartStopButton.Text = "Start";
            this.StartStopButton.UseVisualStyleBackColor = true;
            // 
            // ApiKeyButton
            // 
            this.ApiKeyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ApiKeyButton.Location = new System.Drawing.Point(586, 12);
            this.ApiKeyButton.Name = "ApiKeyButton";
            this.ApiKeyButton.Size = new System.Drawing.Size(75, 23);
            this.ApiKeyButton.TabIndex = 10;
            this.ApiKeyButton.Text = "API KEY";
            this.ApiKeyButton.UseVisualStyleBackColor = true;
            this.ApiKeyButton.Click += new System.EventHandler(this.ApiKeyButton_Click);
            // 
            // serverChannelSelector1
            // 
            this.serverChannelSelector1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.serverChannelSelector1.Location = new System.Drawing.Point(326, 147);
            this.serverChannelSelector1.Name = "serverChannelSelector1";
            this.serverChannelSelector1.Size = new System.Drawing.Size(340, 203);
            this.serverChannelSelector1.TabIndex = 21;
            // 
            // SquishPicsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 450);
            this.Controls.Add(this.serverChannelSelector1);
            this.Controls.Add(this.ApiKeyButton);
            this.Controls.Add(this.StartStopButton);
            this.Controls.Add(this.StatusProgressBarLabel);
            this.Controls.Add(this.StatusProgressBar);
            this.MinimumSize = new System.Drawing.Size(689, 489);
            this.Name = "SquishPicsForm";
            this.Text = "SquishPics";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ProgressBar StatusProgressBar;
        private Label StatusProgressBarLabel;
        private Button StartStopButton;
        private Button ApiKeyButton;
        private ColorDialog colorDialog1;
        private Controls.ServerChannelSelector serverChannelSelector1;
    }
}