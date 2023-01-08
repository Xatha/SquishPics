using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SquishPics.Hooks;

//Refactored from https://github.com/CodeSleeker/Helper
public sealed class GlobalKeyboardHook
{
    private const int WM_KEYDOWN = 0x100;
    private const int WM_KEYUP = 0x101;
    private const int WM_SYSKEYDOWN = 0x104;
    private const int WM_SYSKEYUP = 0x105;

    private static nint _ptrHook;
    private LowLevelKeyboardProc? _keyboardProcess;

    public event KeyEventHandler? KeyUp;
    public event KeyEventHandler? KeyDown;

    ~GlobalKeyboardHook()
    {
        Unhook();
    }

    public void Hook()
    {
        if (_ptrHook != nint.Zero) return;

        var currentModule = Process.GetCurrentProcess().MainModule ??
                            throw new InvalidOperationException("MainModule is null");
        _keyboardProcess = CaptureKey;
        _ptrHook = SetWindowsHookEx(13, _keyboardProcess, GetModuleHandle(currentModule.ModuleName), 0);
    }

    public void Unhook()
    {
        if (_ptrHook == nint.Zero) return;
        UnhookWindowsHookEx(_ptrHook);
    }

    private nint CaptureKey(int nCode, int wParam, nint lParam)
    {
        if (nCode < 0) return CallNextHookEx(_ptrHook, nCode, wParam, lParam);

        var keyInfo = (DllHookStruct)(Marshal.PtrToStructure(lParam, typeof(DllHookStruct))
                                      ?? throw new InvalidOperationException());
        var eventArgs = new KeyEventArgs(keyInfo.key);
        switch (wParam)
        {
            case WM_KEYDOWN or WM_SYSKEYDOWN:
                OnKeyDown(eventArgs);
                break;
            case WM_KEYUP or WM_SYSKEYUP:
                OnKeyUp(eventArgs);
                break;
        }

        return eventArgs.Handled ? 1 : CallNextHookEx(_ptrHook, nCode, wParam, lParam);
    }

    private void OnKeyDown(KeyEventArgs e)
    {
        KeyDown?.Invoke(this, e);
    }

    private void OnKeyUp(KeyEventArgs e)
    {
        KeyUp?.Invoke(this, e);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint SetWindowsHookEx(int id, LowLevelKeyboardProc callback, nint hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint CallNextHookEx(nint hook, int nCode, int wp, nint lp);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint GetModuleHandle(string name);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(nint hook);

    private delegate nint LowLevelKeyboardProc(int nCode, int wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct DllHookStruct
    {
        public readonly Keys key;
        private readonly int vkCode;
        private readonly int scanCode;
        private readonly int flags;
        private readonly int time;
        private readonly nint extra;
    }
}