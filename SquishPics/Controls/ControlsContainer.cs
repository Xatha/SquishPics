using SquishPics.Controllers;
using SquishPics.Hooks;
using SquishPicsDiscordBackend;

namespace SquishPics.Controls;

public sealed class ControlsContainer : IDisposable
{
    private readonly ConnectingControl _connectingControl;
    private readonly FileQueueControl _fileQueueControl;
    private readonly ServerChannelSelectorControl _serverChannelSelectorControl;
    private readonly SortingControl _sortingControl;
    private readonly StartStopButtonControl _startStopButtonControl;
    private readonly StatusControl _statusControl;

    public ControlsContainer(DiscordClient client, ApiController apiController,
        GlobalKeyboardHook keyboardHook)
    {
        _sortingControl = new SortingControl();
        _fileQueueControl = new FileQueueControl(_sortingControl, keyboardHook);
        _connectingControl = new ConnectingControl(client);
        _startStopButtonControl = new StartStopButtonControl(apiController, _fileQueueControl);
        _serverChannelSelectorControl = new ServerChannelSelectorControl(client);
        _statusControl = new StatusControl(apiController);
    }

    public async Task InitializeControlsAsync(Form form)
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
        _serverChannelSelectorControl.Name = "serverChannelSelector";
        _serverChannelSelectorControl.Size = new Size(340, 203);
        _serverChannelSelectorControl.TabIndex = 21;

        // StatusControl
        _statusControl.Location = new Point(12, 368);
        _statusControl.Name = "statusProgressBar";
        _statusControl.Size = new Size(237, 70);
        _statusControl.TabIndex = 0;
        _statusControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        await AddControlsToFormAsync(form);
    }

    private Task AddControlsToFormAsync(Form form)
    {
        // Add the controls to the form
        form.Controls.Add(_fileQueueControl);
        form.Controls.Add(_sortingControl);
        form.Controls.Add(_connectingControl);
        form.Controls.Add(_startStopButtonControl);
        form.Controls.Add(_serverChannelSelectorControl);
        form.Controls.Add(_statusControl);
        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _statusControl.Dispose();
        _sortingControl.Dispose();
        _fileQueueControl.Dispose();
        _connectingControl.Dispose();
        _startStopButtonControl.Dispose();
        _serverChannelSelectorControl.Dispose();
    }
}