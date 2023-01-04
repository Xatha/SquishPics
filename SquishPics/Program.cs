using SquishPicsDiscordBackend;

namespace SquishPics
{
    internal static class Program
    {
        private static readonly DiscordClient _client = new(GlobalSettings.Default.API_KEY);

        [STAThread]
        private static void Main()
        {
            GlobalSettings.StartAutoSave(); 
            
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new SquishPicsForm(_client));
            
            GlobalSettings.ForceSave();
            GlobalSettings.StopAutoSave();
        }
    }
}