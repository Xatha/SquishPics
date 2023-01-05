using SquishPics.APIHelpers;
using SquishPics.Controllers;
using SquishPicsDiscordBackend;

namespace SquishPics
{
    internal static class Program
    {
        
        
        
        [STAThread]
        private static void Main()
        {
            var client = new DiscordClient(GlobalSettings.Default.API_KEY);
            
            var messageServiceHelper = new MessageServiceHelper(client);
            var compressionServiceHelper = new CompressionServiceHelper();
            var controller = new ApiController(messageServiceHelper, compressionServiceHelper);
            
            GlobalSettings.StartAutoSave(); 
            
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.ThreadException += Application_ThreadException;
            
            Application.Run(new SquishPicsForm(client, controller));
            
            GlobalSettings.ForceSave();
            GlobalSettings.StopAutoSave();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var message = $"An unhandled exception occurred: {e.Exception.Message}\n {e.Exception.StackTrace}";
            MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }
    }
}