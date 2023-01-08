//using KeyboardHookManager;

using SquishPics.Hooks;

namespace SquishPics.Controls;

public partial class FileQueueControl : UserControl
{
    private readonly GlobalKeyboardHook _keyboardHook;
    private readonly SortingControl _sortingControl;

    private bool _keyHeld;

    public FileQueueControl(SortingControl sortingControl, GlobalKeyboardHook keyboardHook)
    {
        _sortingControl = sortingControl;
        _keyboardHook = keyboardHook;
        InitializeComponent();

        Items = new List<FileInfo>();
        CurrentQueueListView.DragEnter += CurrentQueueListView_DragEnter;
        CurrentQueueListView.DragDrop += CurrentQueueListView_DragDrop;
        CurrentQueueListView.DragLeave += CurrentQueueListView_DragLeave;
        CurrentQueueListView.MouseClick += CurrentQueueListViewOnClick;
        ClearQueueButton.Click += ClearQueueButton_Click;
        ImportFilesButton.Click += ImportFilesButton_Click;

        CurrentQueueListView.KeyDown += CurrentQueueListView_KeyDown;
        RemoveImageContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;

        _sortingControl.OnSelectedValueChanged += _sortingControl_OnSelectedValueChanged;
        _keyboardHook.KeyUp += HookManager_KeyUp;
    }

    public List<FileInfo> Items { get; private set; }

    ~FileQueueControl()
    {
        _sortingControl.OnSelectedValueChanged -= _sortingControl_OnSelectedValueChanged;
        _keyboardHook.KeyUp -= HookManager_KeyUp;
    }

    private void HookManager_KeyUp(object? sender, KeyEventArgs e)
    {
        _keyHeld = false;
    }

    private async void CurrentQueueListView_KeyDown(object? sender, KeyEventArgs e)
    {
        Console.WriteLine(_keyHeld);
        if (e.KeyCode == Keys.ControlKey) return;
        if (_keyHeld) return;
        switch (e)
        {
            case { KeyCode: Keys.Delete }:
                DeleteCurrentlySelected();
                break;
            case { KeyCode: Keys.A, Modifiers: Keys.Control }:
                SelectAll();
                break;
            case { KeyCode: Keys.V, Modifiers: Keys.Control }:
                await ImportClipboardAsync();
                break;
        }

        _keyHeld = true;
    }

    private async Task ImportClipboardAsync()
    {
        var data = Clipboard.GetDataObject();

        // Only allow files to be imported from the clipboard.
        if (data?.GetData(DataFormats.FileDrop) is not string[] files) return;
        foreach (var file in files) Items.Add(new FileInfo(file));

        await UpdateQueueOrderAsync();
        await UpdateQueueViewAsync();
    }

    private void SelectAll()
    {
        foreach (ListViewItem item in CurrentQueueListView.Items) item.Selected = true;
    }

    private void DeleteCurrentlySelected()
    {
        if (CurrentQueueListView.SelectedItems.Count <= 0) return;
        foreach (ListViewItem item in CurrentQueueListView.SelectedItems)
        {
            Items.RemoveAt(item.Index);
            CurrentQueueListView.Items.Remove(item);
        }
    }

    private void ContextMenuStrip_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
    {
        if (e.ClickedItem?.Text != @"Remove") return;

        var selectedItems = CurrentQueueListView.SelectedItems;
        foreach (ListViewItem item in selectedItems)
        {
            Items.RemoveAt(item.Index);
            CurrentQueueListView.Items.Remove(item);
        }
    }

    private void CurrentQueueListViewOnClick(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right) return;
        RemoveImageContextMenuStrip.Show(this, new Point(e.X, e.Y + RemoveImageContextMenuStrip.Height));
    }

    private async void _sortingControl_OnSelectedValueChanged(object? sender, EventArgs e)
    {
        await UpdateQueueOrderAsync();
        await UpdateQueueViewAsync();
    }

    //TODO: Perhaps there's a way to keep order without having to re-add all the items?
    private async Task UpdateQueueOrderAsync()
    {
        Items = await SortFilesAsync(Items);
    }

    //TODO: Perhaps there's a way to keep order without having to re-add all the items?
    private Task UpdateQueueViewAsync()
    {
        Invoke(() =>
        {
            CurrentQueueListView.Items.Clear();
            var listviewItems = new List<ListViewItem>(Items.Count + 5);
            foreach (var file in Items)
            {
                var listview = new ListViewItem(file.Name);
                listview.SubItems.Add($"{file.Length / 1024}Kb");
                listview.SubItems.Add(file.CreationTime.ToShortDateString());
                listview.SubItems.Add(file.Extension);
                listviewItems.Add(listview);
            }

            CurrentQueueListView.BeginUpdate();
            CurrentQueueListView.Items.AddRange(listviewItems.ToArray());
            CurrentQueueListView.EndUpdate();
        });
        return Task.CompletedTask;
    }

    private static async Task<List<FileInfo>> SortFilesAsync(IEnumerable<FileInfo> unsortedFiles)
    {
        var sortingMode = await GlobalSettings.SafeGetSettingAsync<string>(SettingKeys.SORTING_MODE);
        var sortingOrder = await GlobalSettings.SafeGetSettingAsync<string>(SettingKeys.SORTING_ORDER);
        var fileSortType = (sortingMode, sortingOrder) switch
        {
            ("Name", "Ascending") => unsortedFiles.OrderBy(f => f.Name).ToList(), //If FileSortType.ByName    
            ("Name", "Descending") => unsortedFiles.OrderByDescending(f => f.Name).ToList(), //If FileSortType.ByName 
            ("Size", "Ascending") => unsortedFiles.OrderBy(f => f.Length).ToList(), //If FileSortType.BySize
            ("Size", "Descending") => unsortedFiles.OrderByDescending(f => f.Length).ToList(), //If FileSortType.BySize 
            ("Type", "Ascending") => unsortedFiles.OrderBy(f => f.Extension).ToList(), //If FileSortType.Type
            ("Type", "Descending") => unsortedFiles.OrderByDescending(f => f.Extension)
                .ToList(), //If FileSortType.Type 
            ("Date Modified", "Ascending") => unsortedFiles.OrderBy(f => f.LastWriteTimeUtc)
                .ToList(), //If FileSortType.ByDate
            ("Date Modified", "Descending") => unsortedFiles.OrderByDescending(f => f.LastWriteTimeUtc)
                .ToList(), //If FileSortType.ByDate
            _ => unsortedFiles.ToList()
        };
        return fileSortType;
    }

    private async void ImportFilesButton_Click(object? sender, EventArgs e)
    {
        using var openFileDialog = new OpenFileDialog
        {
            Multiselect = true,
            Filter =
                @"Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png;", //TODO: We can currently only compress these file types.
            //@"Image Files (*.jpg;*.jpeg;*.png;*.gif;*.gifv;*.mp4)|*.jpg;*.jpeg;*.png;*.gif;*.gifv;*.mp4",
            InitialDirectory = await GetLastVisitedDirectoryAsync(),
            Title = @"Select files to import"
        };

        if (openFileDialog.ShowDialog() != DialogResult.OK) return;

        await GlobalSettings.SafeSetSettingAsync(SettingKeys.LAST_VISITED_DIRECTORY_DIALOGUE,
            Path.GetDirectoryName(openFileDialog.FileNames[0]) ?? string.Empty);

        foreach (var fileName in openFileDialog.FileNames)
        {
            if (Items.Exists(x => x.FullName == fileName)) continue;
            Items.Add(new FileInfo(fileName));
        }

        await UpdateQueueOrderAsync();
        await UpdateQueueViewAsync();
    }

    private static async Task<string> GetLastVisitedDirectoryAsync()
    {
        var directory = await GlobalSettings.SafeGetSettingAsync<string>(SettingKeys.LAST_VISITED_DIRECTORY_DIALOGUE);
        if (!Directory.Exists(directory)) directory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        return directory;
    }

    private void ClearQueueButton_Click(object? sender, EventArgs e)
    {
        Items.Clear();
        CurrentQueueListView.Items.Clear();
    }

    private async Task AddFilesToQueueAsync(IEnumerable<string> filePaths)
    {
        foreach (var filePath in filePaths)
            try
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Extension.ToLower() is ".jpg" or ".jpeg" or ".png" or ".gif" or ".gifv" or ".mp4")
                    Items.Add(fileInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); //TODO: Logging.
                throw;
            }

        await UpdateQueueOrderAsync();
        await UpdateQueueViewAsync();
    }

    private Task ResetStyleAsync()
    {
        CurrentQueueListView.BackColor = Color.White;
        return Task.CompletedTask;
    }

    private async void CurrentQueueListView_DragLeave(object? sender, EventArgs e)
    {
        await ResetStyleAsync();
    }

    private void CurrentQueueListView_DragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data != null && !e.Data.GetDataPresent(DataFormats.FileDrop)) return;
        CurrentQueueListView.BackColor = Color.AliceBlue;
        e.Effect = DragDropEffects.Link;
    }

    private async void CurrentQueueListView_DragDrop(object? sender, DragEventArgs e)
    {
        await ResetStyleAsync();
        if (e.Data?.GetData(DataFormats.FileDrop) is string[] files) await AddFilesToQueueAsync(files);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            _sortingControl.OnSelectedValueChanged -= _sortingControl_OnSelectedValueChanged;
            //HookManager.KeyUp -= HookManager_KeyUp;
        }

        base.Dispose(disposing);
    }
}