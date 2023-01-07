using System.Reflection;
using KeyboardHookManager;
using SquishPics.APIHelpers;
using SquishPics.Controllers;
using SquishPicsDiscordBackend;
using Task = System.Threading.Tasks.Task;

namespace SquishPics
{
    internal static class Program
    {
        private static readonly DiscordClient _client = new(GlobalSettings.Default.API_KEY);
        [STAThread]
        private static void Main()
        {
            // HookManager is bad. It captures all keyboard input, and invades the privacy of the user,
            // and it's not possible to disable this behavior.
            // Nevertheless I can not get WinForm's keyboard events to work on FileQueueControl,
            // so I will be using this but filtering out all keys except for the ones that is conducted in the program.
            var thisModule = Assembly.GetExecutingAssembly().GetModules().FirstOrDefault();
            HookManager.Init(thisModule);
            Console.WriteLine(thisModule?.Name);
            
            var messageServiceHelper = new MessageServiceHelper(_client);
            var compressionServiceHelper = new CompressionServiceHelper();
            var controller = new ApiController(_client, messageServiceHelper, compressionServiceHelper);
            
            GlobalSettings.StartAutoSave(); 
            
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.ThreadException += Application_ThreadException;
            Application.ApplicationExit += async (_, _) => await Application_ApplicationExitAsync();
            Application.Run(new SquishPicsForm(_client, controller));
            Application.Exit();
        }

        private static async Task Application_ApplicationExitAsync()
        {
            Console.WriteLine("Application is exiting");
            await _client.StopAsync();
            GlobalSettings.ForceSave();
            GlobalSettings.StopAutoSave();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (e.Exception is Discord.Net.HttpException _) return;

            var message = $"An unhandled exception occurred: {e.Exception.Message}\n {e.Exception.StackTrace}";
            MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }
    }
}