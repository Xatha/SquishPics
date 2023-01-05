using Timer = System.Threading.Timer;

namespace SquishPics;

internal partial class GlobalSettings
{
    private static readonly object _locker = new();
    private static Timer? _timer;

    internal static void ForceSave() => Default.Save();
    
    internal static void StartAutoSave()
    {
        _timer = new Timer(
            _ => Default.Save(),
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(10));
    }

    internal static void StopAutoSave()
    {
        _timer?.Dispose();
    }

    internal static Task<T?> SafeGetSettingAsync<T>(SettingKeys key)
    {
        try
        {
            ForceSave();
            return Task.FromResult((T?)Default[key.ToString()]);
        }
        catch (Exception)
        {
            Console.WriteLine(@"Error getting setting: " + key);
            return Task.FromResult<T?>(default);
        }
    }
    internal static T? SafeGetSetting<T>(SettingKeys key)
    {
        try
        {
            ForceSave();
            return (T?)Default[key.ToString()];
        }
        catch (Exception)
        {
            Console.WriteLine(@"Error getting setting: " + key);
            return default;
        }
    }
    

    //TODO, This might be wrong but hopefully this won't lead to concurrency issues.
    internal static Task SafeSetSettingAsync<T>(SettingKeys key, T value)
    {
        lock (_locker)
        {
            try
            {
                Default[key.ToString()] = value;
                ForceSave();
            }
            catch (Exception)
            {
                Console.WriteLine(@"Error setting setting: " + key);
            }
        }
        return Task.CompletedTask;
    }
}

internal enum SettingKeys
{
    API_KEY,
    SORTING_MODE,
    SORTING_ORDER,
    MAX_FILE_SIZE,
    LAST_VISITED_DIRECTORY_DIALOGUE
}