using SquishPicsDiscordBackend;

namespace SquishPics
{
    internal static class Program
    {
        public static DiscordClient DiscordClient { get; private set; } = null!;

        [STAThread]
        private static void Main()
        {
            DiscordClient = new DiscordClient(GlobalSettings.Default.API_KEY);
            GlobalSettings.StartAutoSave(); 
            
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new SquishPicsForm());
            
            GlobalSettings.ForceSave();
            GlobalSettings.StopAutoSave();
        }
    }
}