namespace SquishPics.Controls
{
    partial class ServerChannelSelectorControl
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
            this.ServerListBox = new System.Windows.Forms.ListBox();
            this.ChannelListBox = new System.Windows.Forms.ListBox();
            this.ChannelsLabel = new System.Windows.Forms.Label();
            this.ServersLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ServerListBox
            // 
            this.ServerListBox.FormattingEnabled = true;
            this.ServerListBox.ItemHeight = 15;
            this.ServerListBox.Location = new System.Drawing.Point(0, 17);
            this.ServerListBox.Name = "ServerListBox";
            this.ServerListBox.Size = new System.Drawing.Size(132, 184);
            this.ServerListBox.TabIndex = 17;
            this.ServerListBox.SelectedIndexChanged += new System.EventHandler(this.ServerListBox_SelectedIndexChanged);
            // 
            // ChannelListBox
            // 
            this.ChannelListBox.FormattingEnabled = true;
            this.ChannelListBox.ItemHeight = 15;
            this.ChannelListBox.Location = new System.Drawing.Point(138, 17);
            this.ChannelListBox.Name = "ChannelListBox";
            this.ChannelListBox.Size = new System.Drawing.Size(200, 184);
            this.ChannelListBox.TabIndex = 16;
            this.ChannelListBox.SelectedIndexChanged += new System.EventHandler(this.ChannelListBox_SelectedIndexChanged);
            // 
            // ChannelsLabel
            // 
            this.ChannelsLabel.AutoSize = true;
            this.ChannelsLabel.Location = new System.Drawing.Point(138, -1);
            this.ChannelsLabel.Name = "ChannelsLabel";
            this.ChannelsLabel.Size = new System.Drawing.Size(56, 15);
            this.ChannelsLabel.TabIndex = 15;
            this.ChannelsLabel.Text = "Channels";
            // 
            // ServersLabel
            // 
            this.ServersLabel.AutoSize = true;
            this.ServersLabel.Location = new System.Drawing.Point(0, -1);
            this.ServersLabel.Name = "ServersLabel";
            this.ServersLabel.Size = new System.Drawing.Size(44, 15);
            this.ServersLabel.TabIndex = 14;
            this.ServersLabel.Text = "Servers";
            // 
            // ServerChannelSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ServerListBox);
            this.Controls.Add(this.ChannelListBox);
            this.Controls.Add(this.ChannelsLabel);
            this.Controls.Add(this.ServersLabel);
            this.Name = "ServerChannelSelectorControl";
            this.Size = new System.Drawing.Size(340, 203);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListBox ServerListBox;
        private ListBox ChannelListBox;
        private Label ChannelsLabel;
        private Label ServersLabel;
    }
}
