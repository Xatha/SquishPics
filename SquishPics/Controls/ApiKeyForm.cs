namespace SquishPics.Controls;

public partial class ApiKeyForm : Form
{
    public ApiKeyForm()
    {
        InitializeComponent();
        MaximizeBox = false;
    }

    private async void APIKeyForm_Load(object sender, EventArgs e)
    {
        APIKEYTextBox.Text = await GlobalSettings.SafeGetSettingAsync<string>(SettingKeys.API_KEY);
    }

    protected override async void OnFormClosing(FormClosingEventArgs e)
    {
        var apiKey = await GlobalSettings.SafeGetSettingAsync<string>(SettingKeys.API_KEY);
        var newApiKey = APIKEYTextBox.Text.Trim();
        if (apiKey != newApiKey)
            await GlobalSettings.SafeSetSettingAsync(SettingKeys.API_KEY, newApiKey);

        base.OnFormClosing(e);
        if (e.CloseReason == CloseReason.WindowsShutDown) return;

        e.Cancel = true;
        Hide();
    }
}