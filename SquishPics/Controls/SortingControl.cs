namespace SquishPics.Controls
{
    public partial class SortingControl : UserControl
    {
        public SortingControl()
        {
            InitializeComponent();
            InitializeSettings();
            
            SortingModesComboBox.SelectedValueChanged += SortingModesComboBox_SelectedValueChanged;
            SortingOrderComboBox.SelectedValueChanged += SortingOrderComboBox_SelectedValueChanged;

            MaxFileSizeNUD.ValueChanged += MaxFileSizeNUD_ValueChanged;
        }
        
        public event EventHandler? OnSelectedValueChanged
        {
            add 
            { 
                SortingModesComboBox.SelectedValueChanged += value;
                SortingOrderComboBox.SelectedValueChanged += value;
            }
            remove
            {
                SortingModesComboBox.SelectedValueChanged -= value; 
                SortingOrderComboBox.SelectedValueChanged -= value;
            }
        }


        private void InitializeSettings()
        {
            //TODO: Streamline on-first inits?
            SortingModesComboBox.SelectedItem = GlobalSettings.SafeGetSetting<string>(SettingKeys.SORTING_MODE);
            SortingOrderComboBox.SelectedItem = GlobalSettings.SafeGetSetting<string>(SettingKeys.SORTING_ORDER);
            SortingModesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            SortingOrderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            
            MaxFileSizeNUD.Value = GlobalSettings.SafeGetSetting<int>(SettingKeys.MAX_FILE_SIZE);
        }
        
        private async void MaxFileSizeNUD_ValueChanged(object? sender, EventArgs e)
        {
            var value = (int)MaxFileSizeNUD.Value;
            await GlobalSettings.SafeSetSettingAsync(SettingKeys.MAX_FILE_SIZE, value);
        }

        private async void SortingModesComboBox_SelectedValueChanged(object? sender, EventArgs e)
        {
            if (await GlobalSettings.SafeGetSettingAsync<string>(SettingKeys.SORTING_MODE) == SortingModesComboBox.Text) return;
            await GlobalSettings.SafeSetSettingAsync(SettingKeys.SORTING_MODE, SortingModesComboBox.Text);
        }
        
        private async void SortingOrderComboBox_SelectedValueChanged(object? sender, EventArgs e)
        {
            if (await GlobalSettings.SafeGetSettingAsync<string>(SettingKeys.SORTING_ORDER) == SortingOrderComboBox.Text) return;
            await GlobalSettings.SafeSetSettingAsync(SettingKeys.SORTING_ORDER, SortingOrderComboBox.Text);
        }

    }
}
