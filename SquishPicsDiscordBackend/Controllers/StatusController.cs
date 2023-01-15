namespace SquishPicsDiscordBackend.Controllers;

public class StatusController
{
    public event EventHandler<Status>? StatusChanged;

    protected virtual void OnStatusChanged(Status e) => StatusChanged?.Invoke(this, e);
}