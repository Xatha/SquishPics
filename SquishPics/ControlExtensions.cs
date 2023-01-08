using System.Reflection;

namespace SquishPics;

public static class ControlExtensions
{
    //https://stackoverflow.com/a/15268338
    public static void DoubleBuffered(this Control control, bool enable)
    {
        var doubleBufferPropertyInfo =
            control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
        doubleBufferPropertyInfo?.SetValue(control, enable, null);
    }
}