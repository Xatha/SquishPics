namespace SquishPics.Controls;

internal static class StandardResponses
{
    public static Task NoConnectionAsync()
    {
        MessageBox.Show(@"The application is not connected to discord. Please check your connection and try again by clicking the red connection button.");
        return Task.CompletedTask;
    }
    
    public static Task NoFilesSelectedAsync()
    {
        MessageBox.Show(@"No files were selected. Please select at least one file or more files and try again.");
        return Task.CompletedTask;
    }
    
    public static Task NoChannelSelectedAsync()
    {
        MessageBox.Show(@"No server or channel was selected. Please select a server and channel and try again.");
        return Task.CompletedTask;
    }

    public static Task<bool> ConfirmationAsync(string serverName, string channelName)
    {
        var message =
            $"Do you want to begin posting images? The images will be send in:\nServer: {serverName}\nChannel: {channelName}";
        return Task.FromResult(MessageBox.Show(message, @"Start Process?", MessageBoxButtons.YesNo) == DialogResult.No);
    }

    


}