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
            SortingModesComboBox.SelectedItem = GlobalSettings.Default.SORTING_MODE;
            SortingOrderComboBox.SelectedItem = GlobalSettings.Default.SORTING_ORDER;
            SortingModesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            SortingOrderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            
            MaxFileSizeTextBox.Text = GlobalSettings.Default.MAX_FILE_SIZE.ToString();
        }
        
        private void MaxFileSizeTextBox_TextChanged(object? sender, EventArgs e)
        {
            GlobalSettings.Default.MAX_FILE_SIZE = MaxFileSizeTextBox.Text.Length > 0
                ? int.Parse(MaxFileSizeTextBox.Text)
                : GlobalSettings.Default.MAX_FILE_SIZE;
            
            //GlobalSettings.Default.Save();
        }
        
        private void MaxFileSizeTextBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void SortingModesComboBox_SelectedValueChanged(object? sender, EventArgs e)
        {
            if (GlobalSettings.Default.SORTING_MODE == SortingModesComboBox.Text) return;

            GlobalSettings.Default.SORTING_MODE = SortingModesComboBox.Text;
        }
        
        private void SortingOrderComboBox_SelectedValueChanged(object? sender, EventArgs e)
        {
            if (GlobalSettings.Default.SORTING_ORDER == SortingOrderComboBox.Text) return;

            GlobalSettings.Default.SORTING_ORDER = SortingOrderComboBox.Text;
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
