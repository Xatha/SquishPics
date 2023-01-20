using System.Diagnostics;

namespace SquishPicsDiscordBackend.OAuth2;

public class DiscordOAuth2
{
    public void Start()
    {
        var psiNpmRunDist = new ProcessStartInfo
        {
            FileName = "cmd",
            RedirectStandardInput = true,
            WorkingDirectory = @"C:\Users\Luca\source\repos\SquishPics\SquishPicsDiscordBackend\OAuth2"
        };
        var pNpmRunDist = Process.Start(psiNpmRunDist);
        pNpmRunDist?.StandardInput.WriteLine("npm start index.js");
    }
    
}