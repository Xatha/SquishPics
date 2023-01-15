using SquishPicsDiscordBackend.Controllers;
using SquishPicsDiscordBackend.Messaging;
namespace SquishPics.Controls;

public partial class StatusControl : UserControl
{
    private readonly RequestController _requestController;
    private readonly MessageService _messageService;
    private readonly DataProcessor _dataProcessor;

    public StatusControl(RequestController requestController, MessageService messageService, DataProcessor dataProcessor)
    {
        _requestController = requestController;
        _messageService = messageService;
        _dataProcessor = dataProcessor;
        _requestController.RequestCompleted += RequestControllerOnRequestCompleted;
        _requestController.RequestFailed += RequestControllerOnRequestFailed;
        _requestController.RequestCancelled += RequestControllerOnRequestCancelled;        
        
        _messageService.MessageSent += MessageServiceOnMessageSent;
        _dataProcessor.FileProcessed += DataProcessorOnFileProcessed;
            
            
        InitializeComponent();
        StatusProgressBar.Style = ProgressBarStyle.Blocks;
    }

    private void DataProcessorOnFileProcessed(object? sender, Status e)
    {
        StatusChanged(e);
    }

    private void MessageServiceOnMessageSent(object? sender, Status e)
    {
        StatusChanged(e);
    }

    private void RequestControllerOnRequestCancelled(object? sender, EventArgs e)
    {
        Invoke(() => StatusProgressBarLabel.Text = "Status: Cancelled");
    }

    private void RequestControllerOnRequestFailed(object? sender, RequestFailureReason e)
    {
        Invoke(() => StatusProgressBarLabel.Text = $"Status: Failed: {e}");
    }

    private void RequestControllerOnRequestCompleted(object? sender, EventArgs e)
    {
        Invoke(() => StatusProgressBarLabel.Text = $"Status: Completed!");
    }
    
    private void StatusChanged(Status e)
    {
        Invoke(() => StatusProgressBarLabel.Text = $"Status: {e.Message} | {e.WorkDone} / {e.TotalWork}");
        if (e.TotalWork == 0)
        {
            Invoke(() => StatusProgressBar.Value = 0);
            return;
        }

        var progress = 100 * (float)(e.TotalWork - Math.Abs(e.WorkDone - e.TotalWork)) / e.TotalWork;
        Invoke(() => StatusProgressBar.Value = (int)progress);
    }
}