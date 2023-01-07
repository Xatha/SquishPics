using SquishPics.Controllers;
using SquishPics.Controls;
using SquishPicsDiscordBackend;

namespace SquishPics;

public sealed class ControlsContainer : IDisposable
{
    private readonly Form _form;
    private readonly SortingControl _sortingControl;
    private readonly FileQueueControl _fileQueueControl;
    private readonly ConnectingControl _connectingControl;
    private readonly StartStopButtonControl _startStopButtonControl;
    private readonly ServerChannelSelectorControl _serverChannelSelectorControl;

    public ControlsContainer(Form form, DiscordClient client, ApiController apiController)
    {
        _form = form;
        _sortingControl = new SortingControl();
        _fileQueueControl = new FileQueueControl(_sortingControl);
        _connectingControl = new ConnectingControl(client);
        _startStopButtonControl = new StartStopButtonControl(apiController, _fileQueueControl);
        _serverChannelSelectorControl = new ServerChannelSelectorControl(client);
    }

    public async Task InitializeControlsAsync()
    {
        // fileQueueControl
        _fileQueueControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom 
                                                    | AnchorStyles.Left 
                                                    | AnchorStyles.Right;
        _fileQueueControl.AutoSize = true;
        _fileQueueControl.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _fileQueueControl.Location = new Point(12, 12);
        _fileQueueControl.Name = "fileQueueControl";
        _fileQueueControl.Size = new Size(309, 339);
        _fileQueueControl.TabIndex = 19;
        _fileQueueControl.DoubleBuffered(true);
        
        // sortingControl
        _sortingControl.Anchor = AnchorStyles.Right;
        _sortingControl.Location = new Point(324, 38);
        _sortingControl.Name = "sortingControl";
        _sortingControl.Size = new Size(310, 42);
        _sortingControl.TabIndex = 20;
        
        // connectingControl
        _connectingControl.Location = new Point(273, 412);
        _connectingControl.Name = "connectingControl";
        _connectingControl.Size = new Size(213, 26);
        _connectingControl.TabIndex = 19;

        // startStopButtonControl
        _startStopButtonControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _startStopButtonControl.Location = new Point(556, 412);
        _startStopButtonControl.Name = "StartStopButton";
        _startStopButtonControl.Size = new Size(103, 26);
        _startStopButtonControl.TabIndex = 5;
        
        // serverChannelSelector
        _serverChannelSelectorControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _serverChannelSelectorControl.Location = new Point(326, 147);
        _serverChannelSelectorControl.Name = "serverChannelSelector1";
        _serverChannelSelectorControl.Size = new Size(340, 203);
        _serverChannelSelectorControl.TabIndex = 21;
        
        await AddControlsToFormAsync();
    }

    private Task AddControlsToFormAsync()
    {
        // Add the controls to the form
        _form.Controls.Add(_fileQueueControl);
        _form.Controls.Add(_sortingControl);
        _form.Controls.Add(_connectingControl);
        _form.Controls.Add(_startStopButtonControl);
        _form.Controls.Add(_serverChannelSelectorControl);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _sortingControl.Dispose();
        _fileQueueControl.Dispose();
        _connectingControl.Dispose();
        _startStopButtonControl.Dispose();
        _serverChannelSelectorControl.Dispose();
        
    }
}