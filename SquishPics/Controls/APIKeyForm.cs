namespace SquishPics.Controls
{
    public partial class APIKeyForm : Form
    {
        public APIKeyForm()
        {
            InitializeComponent();
        }

        private async void APIKeyForm_Load(object sender, EventArgs e) =>
            APIKEYTextBox.Text = await GlobalSettings.SafeGetSettingAsync<string>(SettingKeys.API_KEY);

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            var apiKey = await GlobalSettings.SafeGetSettingAsync<string>(SettingKeys.API_KEY);
            if (apiKey != APIKEYTextBox.Text) await GlobalSettings.SafeSetSettingAsync(SettingKeys.API_KEY, APIKEYTextBox.Text);

            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            e.Cancel = true;
            Hide();
        }
    }
}
