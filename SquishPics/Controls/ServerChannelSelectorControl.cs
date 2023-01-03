using Discord.Rest;
using Discord.WebSocket;

namespace SquishPics.Controls
{
    public partial class ServerChannelSelectorControl : UserControl
    {
        private IReadOnlyCollection<RestGuild>? _guilds;
        private IEnumerable<SocketTextChannel>? _textChannels;

        public RestGuild? SelectedGuild { get; private set; }
        public SocketTextChannel? SelectedTextChannel { get; private set; }

        public ServerChannelSelectorControl()
        {
            InitializeComponent();
            Task.Run(Initialize);
        }

        private async Task Initialize()
        {
            _guilds = await Program.DiscordClient.GetServersAsync();
            if (_guilds is null) 
            {
                Console.WriteLine(@"No guilds found. Retrying...");
                await Task.Delay(2000); // Retry in 2 seconds
                _guilds = await Program.DiscordClient.GetServersAsync();
            }
            if (_guilds is null) 
            {
                var buttons = MessageBoxButtons.YesNo;
                Invoke( ()=> MessageBox.Show(@"Failed to get guilds from Discord API. Please try again later.", @"Error", buttons));
                return; // Still null, give up TODO: Add better error.
            }
                
            Invoke(() => ServerListBox.Items.AddRange(_guilds.Select(guild => guild.Name).ToArray<object>()));
        }
        
        private async void ServerListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedServer = ServerListBox.SelectedItem?.ToString();
            if (selectedServer is null || selectedServer == SelectedGuild?.Name ) return;
            
            SelectedTextChannel = null;
            SelectedGuild = _guilds?.FirstOrDefault(guild => guild.Name == selectedServer);
            if (SelectedGuild is null) return;

            _textChannels = await Program.DiscordClient.GetChannelsAsync(SelectedGuild)!;

            var textChannels = await Program.DiscordClient.GetChannelsAsync(SelectedGuild)!;
            Invoke(() =>
            {
                ChannelListBox.Items.Clear();
                ChannelListBox.Items.AddRange(textChannels.Select(textChannel => textChannel.Name).ToArray<object>());
            });
        }

        private void ChannelListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedChannel = ChannelListBox.SelectedItem?.ToString();
            if (selectedChannel is null || selectedChannel == SelectedTextChannel?.Name) return;
            SelectedTextChannel = _textChannels?.FirstOrDefault(textChannel => textChannel.Name == selectedChannel);
        }
    }
}
