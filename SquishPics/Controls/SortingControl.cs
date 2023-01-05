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
            
            MaxFileSizeTextBox.TextChanged += MaxFileSizeTextBox_TextChanged;
            MaxFileSizeTextBox.KeyPress += MaxFileSizeTextBox_KeyPress;
        }

        private void InitializeSettings()
        {
            //TODO: Streamline on-first inits?
            SortingModesComboBox.SelectedItem = GlobalSettings.Default.SORTING_MODE;
            SortingOrderComboBox.SelectedItem = GlobalSettings.Default.SORTING_ORDER;
            SortingModesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            SortingOrderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            
            MaxFileSizeTextBox.Text = GlobalSettings.Default.MAX_FILE_SIZE.ToString();
        }
        
        private async void MaxFileSizeTextBox_TextChanged(object? sender, EventArgs e)
        {
            var value = MaxFileSizeTextBox.Text.Length > 0
                ? int.Parse(MaxFileSizeTextBox.Text)
                : await GlobalSettings.SafeGetSettingAsync<int>(SettingKeys.MAX_FILE_SIZE);

            //TODO: Make max and min a config?
            if (value is < 6 or > 200) MaxFileSizeTextBox.Text = value < 6 ? "6" : "200";

            await GlobalSettings.SafeSetSettingAsync(SettingKeys.MAX_FILE_SIZE, value);
        }
        
        private void MaxFileSizeTextBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
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
        

    }
}
