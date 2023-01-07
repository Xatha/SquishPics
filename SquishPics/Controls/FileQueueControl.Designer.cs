namespace SquishPics.Controls
{
    partial class FileQueueControl
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
            this.components = new System.ComponentModel.Container();
            this.ClearQueueButton = new System.Windows.Forms.Button();
            this.ImportFilesButton = new System.Windows.Forms.Button();
            this.CurrentQueueListView = new System.Windows.Forms.ListView();
            this.FilePathHeader = new System.Windows.Forms.ColumnHeader();
            this.SizeHeader = new System.Windows.Forms.ColumnHeader();
            this.DateModifiedHeader = new System.Windows.Forms.ColumnHeader();
            this.TypeHeader = new System.Windows.Forms.ColumnHeader();
            this.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ClearQueueButton
            // 
            this.ClearQueueButton.Location = new System.Drawing.Point(78, 0);
            this.ClearQueueButton.Name = "ClearQueueButton";
            this.ClearQueueButton.Size = new System.Drawing.Size(95, 23);
            this.ClearQueueButton.TabIndex = 16;
            this.ClearQueueButton.Text = "Clear Queue";
            this.ClearQueueButton.UseVisualStyleBackColor = true;
            // 
            // ImportFilesButton
            // 
            this.ImportFilesButton.Location = new System.Drawing.Point(0, 0);
            this.ImportFilesButton.Name = "ImportFilesButton";
            this.ImportFilesButton.Size = new System.Drawing.Size(75, 23);
            this.ImportFilesButton.TabIndex = 15;
            this.ImportFilesButton.Text = "Import";
            this.ImportFilesButton.UseVisualStyleBackColor = true;
            // 
            // CurrentQueueListView
            // 
            this.CurrentQueueListView.AllowDrop = true;
            this.CurrentQueueListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FilePathHeader,
            this.SizeHeader,
            this.DateModifiedHeader,
            this.TypeHeader});
            this.CurrentQueueListView.FullRowSelect = true;
            this.CurrentQueueListView.Location = new System.Drawing.Point(0, 26);
            this.CurrentQueueListView.Name = "CurrentQueueListView";
            this.CurrentQueueListView.Size = new System.Drawing.Size(306, 310);
            this.CurrentQueueListView.TabIndex = 18;
            this.CurrentQueueListView.UseCompatibleStateImageBehavior = false;
            this.CurrentQueueListView.View = System.Windows.Forms.View.Details;
            // 
            // FilePathHeader
            // 
            this.FilePathHeader.Text = "File Path";
            this.FilePathHeader.Width = 80;
            // 
            // SizeHeader
            // 
            this.SizeHeader.Text = "Size";
            // 
            // DateModifiedHeader
            // 
            this.DateModifiedHeader.Text = "Date Modified";
            this.DateModifiedHeader.Width = 100;
            // 
            // TypeHeader
            // 
            this.TypeHeader.Text = "Type";
            // 
            // ContextMenuStrip
            // 
            this.ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem});
            this.ContextMenuStrip.Name = "contextMenuStrip1";
            this.ContextMenuStrip.Size = new System.Drawing.Size(181, 48);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            // 
            // FileQueueControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CurrentQueueListView);
            this.Controls.Add(this.ClearQueueButton);
            this.Controls.Add(this.ImportFilesButton);
            this.Name = "FileQueueControl";
            this.Size = new System.Drawing.Size(308, 338);
            this.ContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private Button ClearQueueButton;
        private Button ImportFilesButton;
        private ListView CurrentQueueListView;
        private ColumnHeader FilePathHeader;
        private ColumnHeader SizeHeader;
        private ColumnHeader DateModifiedHeader;
        private ColumnHeader TypeHeader;
        private ContextMenuStrip ContextMenuStrip;
        private ToolStripMenuItem removeToolStripMenuItem;
    }
}
