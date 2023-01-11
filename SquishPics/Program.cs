using System.Diagnostics;
using Discord.Net;
using log4net;
using SquishPics.APIHelpers;
using SquishPics.Controllers;
using SquishPics.Controls;
using SquishPics.Hooks;
using SquishPicsDiscordBackend;
using SquishPicsDiscordBackend.Logging;

namespace SquishPics;

internal sealed class Program
{
    private static readonly ILog _log = LogProvider.GetLogger<Program>();
    private static readonly DiscordClient _client = new();
    private static readonly GlobalKeyboardHook _keyboardHook = new();
    private static ApiController? _controller;

    [STAThread]
    private static void Main()
    {
        var messageServiceHelper = new MessageServiceHelper(_client);
        var compressionServiceHelper = new CompressionServiceHelper();
        _controller = new ApiController(_client, messageServiceHelper, compressionServiceHelper);
        
        try
        {
            ValidateDependencies();
            InitializeConfigurations();
            InitializeEvents();
            
            GlobalSettings.StartAutoSave();
            _keyboardHook.Hook();
            _client.StartAsync(GlobalSettings.Default.API_KEY).Wait();
            
            var container = new ControlsContainer(_client, _controller, _keyboardHook);
            Application.Run(new SquishPicsForm(container));
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
            Cleanup();
        }
    }
    
    private static void ValidateDependencies()
    {
        var assemblyPath = 
            Process.GetCurrentProcess().MainModule?.FileName 
            ?? throw new InvalidOperationException("Could not get entry assembly location");
        
        var appRoot = 
            Path.GetDirectoryName(assemblyPath) 
            ?? throw new InvalidOperationException("Could not retrieve appRoot.");
        
        var pingoPath = Path.Combine(appRoot, @"libs\pingo.exe");
        var log4NetConfigPath = Path.Combine(appRoot, @"log4net.config");

        if (! File.Exists(pingoPath) || !File.Exists(log4NetConfigPath)) 
            throw new FileNotFoundException("Could not find dependencies");
    }

    private static void InitializeConfigurations()
    {
        ApplicationConfiguration.Initialize();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.SetDefaultFont(new Font("Segoe UI", 9));
    }

    private static void InitializeEvents() => Application.ThreadException += Application_ThreadException;
    
    private static void Cleanup()
    {
        _client.StopAsync().Wait();
        GlobalSettings.ForceSave();
        GlobalSettings.StopAutoSave();
        _keyboardHook.Unhook();
    }

    #region Events

    private static async void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        if (e.Exception is HttpException) return;

        var message = $"An unhandled exception occurred: {e.Exception.Message}\n {e.Exception.StackTrace}";
        MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        await _log.FatalAsync("Application is exiting because of an unhandled exception", e.Exception);
        Application.Exit();
    }
    
    #endregion
}