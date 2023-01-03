using SquishPics.Controls;

namespace SquishPics;

public class ControlsInitializer
{
    private static readonly FileQueueControl _fileQueueControl;
    private static readonly SortingControl _sortingControl;
    private static readonly ConnectingControl _connectingControl;

    
    static ControlsInitializer()
    {
        _sortingControl = new SortingControl();
        _fileQueueControl = new FileQueueControl(_sortingControl);
        _connectingControl = new ConnectingControl(Program.DiscordClient);
    }

    public static Task InitializeControls(Form form)
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

        // Add the controls to the form
        form.Controls.Add(_fileQueueControl);
        form.Controls.Add(_sortingControl);
        form.Controls.Add(_connectingControl);
        
        //Finalizing Settings
        _fileQueueControl.DoubleBuffered(true);
        return Task.CompletedTask;
    }
}