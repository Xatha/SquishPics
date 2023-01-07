namespace SquishPics.Controls;

public class CleanNumericUpDown : NumericUpDown
{
    public CleanNumericUpDown()
    {
        Controls.RemoveAt(0);
    }

    protected override void OnTextBoxResize(object? source, EventArgs e)
    {
        Controls[0].Width = Width - 4;
    }
}