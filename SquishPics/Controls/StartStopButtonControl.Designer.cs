using System.ComponentModel;

namespace SquishPics.Controls;

partial class StartStopButtonControl
{
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

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
            this.StartStopButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StartStopButton
            // 
            this.StartStopButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StartStopButton.Location = new System.Drawing.Point(0, 0);
            this.StartStopButton.Name = "StartStopButton";
            this.StartStopButton.Size = new System.Drawing.Size(103, 23);
            this.StartStopButton.TabIndex = 6;
            this.StartStopButton.Text = "Start Process";
            this.StartStopButton.BackColor = Color.PaleGreen;
            this.StartStopButton.FlatStyle = FlatStyle.Flat;
            this.StartStopButton.UseVisualStyleBackColor = false;
            // 
            // StartStopButtonControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.StartStopButton);
            this.Name = "StartStopButtonControl";
            this.Size = new System.Drawing.Size(103, 23);
            this.ResumeLayout(false);
    }

    #endregion

    private Button StartStopButton;
}