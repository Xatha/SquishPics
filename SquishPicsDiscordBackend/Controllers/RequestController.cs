using Discord;
using log4net;
using SquishPicsDiscordBackend.Logging;
using SquishPicsDiscordBackend.Messaging;

namespace SquishPicsDiscordBackend.Controllers;

public class RequestController
{
    private readonly ILog _log;
    private readonly MessageService _messageService;
    private readonly DataProcessor _validatedData;

    public event EventHandler? RequestReceived;
    public event EventHandler? RequestCompleted;
    public event EventHandler? RequestCancelled; 
    public event EventHandler<RequestFailureReason>? RequestFailed;
    public bool IsBusy { get; set; }
    
    
    public RequestController(ILog logger, MessageService messageService, DataProcessor validatedData)
    {
        _log = logger;
        _messageService = messageService;
        _validatedData = validatedData;
        _messageService.Completed += MessageService_Completed;
    }
    ~RequestController()
    {
        _messageService.Completed -= MessageService_Completed;
    }

    public async Task<bool> SendRequestAsync(ITextChannel channel)
    {
        if (!CheckPreconditions()) return false;
        IsBusy = true;

        try
        {
            await _messageService.SetupAsync(
                _validatedData.Data 
                ?? throw new InvalidOperationException("Data is null."), channel);
        }
        catch (InvalidOperationException e)
        {
            OnRequestFailed(RequestFailureReason.MessageServiceError);
            await _log.ErrorAsync("Failed to start message service.", e);
            return false;
        }

        _messageService.StartForget();
        return true;
    }

    public async Task CancelRequestAsync()
    {
        if (!IsBusy) return;
        await _messageService.StopAsync();
        //OnRequestCancelled();
    }

    private bool CheckPreconditions()
    {
        if (IsBusy)
        {
            OnRequestFailed(RequestFailureReason.AlreadyHandlingRequest);
            return false;
        }
        if (!_validatedData.IsValid)
        {
            OnRequestFailed(RequestFailureReason.InvalidData);
            return false;
        }

        return true;
    }


    #region Events
    
    private async void MessageService_Completed(object? sender, MessageServiceCompleteState e)
    {
        await _validatedData.CleanUpAsync();
        if (e == MessageServiceCompleteState.Success)
        {
            OnRequestCompleted();
        }
        else if (e == MessageServiceCompleteState.Cancelled)
        {
            OnRequestCancelled();
        }
        else
        {
            OnRequestFailed(RequestFailureReason.MessageServiceError);
        }

        IsBusy = false;
    }

    protected virtual void OnRequestReceived() => RequestReceived?.Invoke(this, EventArgs.Empty);
    protected virtual void OnRequestCompleted() => RequestCompleted?.Invoke(this, EventArgs.Empty);
    protected virtual void OnRequestCancelled() => RequestCancelled?.Invoke(this, EventArgs.Empty);
    protected virtual void OnRequestFailed(RequestFailureReason reason) => RequestFailed?.Invoke(this, reason);

    #endregion
}