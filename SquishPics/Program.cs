using System.Diagnostics;
using Discord.Net;
using SquishPics.APIHelpers;
using SquishPics.Controllers;
using SquishPics.Hooks;
using SquishPicsDiscordBackend;

namespace SquishPics;

internal static class Program
{
    private static readonly DiscordClient _client = new(GlobalSettings.Default.API_KEY);
    private static readonly GlobalKeyboardHook _keyboardHook = new();
    private static ApiController? _controller;

    [STAThread]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.ThreadException += Application_ThreadException;
        Application.ApplicationExit += async (_, _) => await Application_ApplicationExitAsync();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        try
        {
            if (!File.Exists(GetExternalDependencies()))
                throw new FileNotFoundException("Pingo.exe not found in libs folder");

            var messageServiceHelper = new MessageServiceHelper(_client);
            var compressionServiceHelper = new CompressionServiceHelper();
            _controller = new ApiController(_client, messageServiceHelper, compressionServiceHelper);
            GlobalSettings.StartAutoSave();
            _keyboardHook.Hook();
            Application.Run(new SquishPicsForm(_client, _controller, _keyboardHook));
        }
        catch (FileNotFoundException e)
        {
            MessageBox.Show($"Error on startup... Could not resolve libs/pingo.exe\n{e}", "Fatal Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception e)
        {
            MessageBox.Show($"Unspecified error on startup...\n{e}", @"Fatal Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            Application.Exit();
        }
    }

    private static string GetExternalDependencies()
    {
        var assemblyPath = Process.GetCurrentProcess().MainModule?.FileName ??
                           throw new InvalidOperationException("Could not get entry assembly location");
        var appRoot = Path.GetDirectoryName(assemblyPath) ?? throw new InvalidOperationException(assemblyPath);
        var pingoPath = Path.Combine(appRoot, @"libs\pingo.exe");
        return pingoPath;
    }

    private static async Task Application_ApplicationExitAsync()
    {
        Console.WriteLine("Application is exiting");
        await _client.StopAsync();
        GlobalSettings.ForceSave();
        GlobalSettings.StopAutoSave();
        _keyboardHook.Unhook();
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        if (e.Exception is HttpException) return;

        var message = $"An unhandled exception occurred: {e.Exception.Message}\n {e.Exception.StackTrace}";
        MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        Application.Exit();
    }
}