using System.ComponentModel;
using System.Diagnostics;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SquishPicsDiscordBackend.OAuth2;

public class DiscordOAuth2
{
    private static Process _OAuth2Server;
    private static OAuth2Token? _cachedToken;

    private static bool _tokenExpired;
    
    public bool TokenExpired => _tokenExpired;
    public OAuth2Token? Token => _cachedToken;

    static DiscordOAuth2()
    {
        RetrieveToken();
    }

    public DiscordOAuth2()
    {
    }
    
    public void StartServer()
    {
        _OAuth2Server = Process.Start(new ProcessStartInfo
        {
            FileName = "libs\\OAuth2Server",
            Arguments = "--port 53134", //TODO: Get port from config
            UseShellExecute = false
        }) ?? throw new NullReferenceException("OAuth2Server process could no start.");
    }

    public void SetToken(string json)
    {
        var rawOAuth2Token = JsonConvert.DeserializeObject<RawOAuth2Token>(json);
        if (rawOAuth2Token == null) throw new JsonSerializationException("Failed to deserialize token");
        

        _cachedToken = new OAuth2Token(
            rawOAuth2Token.Username, rawOAuth2Token.Discriminator, rawOAuth2Token.UserId, rawOAuth2Token.ExpiresAt);
        _tokenExpired = false;
        CacheToken(_cachedToken);
    }
    
    private static void RetrieveToken()
    {
        try
        {
            var json = File.ReadAllText("token.json");
            var oAuth2Token = JsonConvert.DeserializeObject<OAuth2Token>(json);

            if (oAuth2Token != null && oAuth2Token.ExpiresAt > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                _cachedToken = oAuth2Token;
                return;
            }
        
            // Token is expired, refresh it
            _tokenExpired = true;
        }
        catch (JsonSerializationException) 
        {
            _tokenExpired = true;
        }
        catch (FileNotFoundException)
        {
            _tokenExpired = true;
        }
    }
    
    private void CacheToken(OAuth2Token token)
    {
        var jsonString = JsonSerializer.Serialize(token);
        File.WriteAllText("token.json", jsonString);
    }
}


public record RawOAuth2Token(string Username, string Discriminator, ulong UserId, long ExpiresAt)
{
    [JsonConstructor]
    public RawOAuth2Token(string username, string discriminator, string id) 
        : this(username, discriminator, 0, DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds())
    {
        UserId = ulong.Parse(id);
    }
}

public record OAuth2Token(string Username, string Discriminator, ulong UserId, long ExpiresAt);
