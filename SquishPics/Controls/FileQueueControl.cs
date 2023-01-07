using KeyboardHookManager;
using Clipboard = System.Windows.Forms.Clipboard;
using DataFormats = System.Windows.Forms.DataFormats;
using DragDropEffects = System.Windows.Forms.DragDropEffects;
using DragEventArgs = System.Windows.Forms.DragEventArgs;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using ListViewItem = System.Windows.Forms.ListViewItem;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Point = System.Drawing.Point;
using UserControl = System.Windows.Forms.UserControl;

namespace SquishPics.Controls
{
    public partial class FileQueueControl : UserControl
    {
        public List<FileInfo> FileContents { get; private set; }

        private bool _keyHeld;
        
        public FileQueueControl(SortingControl sortingControl)
        {
            InitializeComponent();

            FileContents = new List<FileInfo>();
            CurrentQueueListView.DragEnter += CurrentQueueListView_DragEnter;
            CurrentQueueListView.DragDrop += CurrentQueueListView_DragDrop;
            CurrentQueueListView.DragLeave += CurrentQueueListView_DragLeave;
            CurrentQueueListView.MouseClick += CurrentQueueListViewOnClick;
            ClearQueueButton.Click += ClearQueueButton_Click;
            ImportFilesButton.Click += ImportFilesButton_Click;
            
            CurrentQueueListView.KeyDown += CurrentQueueListView_KeyDown;
            sortingControl.OnSelectedValueChanged += _sortingControl_OnSelectedValueChanged;

            ContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;
            
            // I have to use this because I cannot get the WinForms event KeyUp event to work.
            HookManager.KeyUp += HookManager_KeyUp;
        }
        
        private void HookManager_KeyUp(object? sender, KeyEventArgs e)
        {
            if (_keyHeld) _keyHeld = false;
        }

        private async void CurrentQueueListView_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) return;
            if (_keyHeld) return;
            switch (e)
            {
                case {KeyCode: Keys.Delete}:
                    DeleteCurrentlySelected();
                    return;
                case { KeyCode: Keys.A, Modifiers: Keys.Control }:
                    SelectAll();
                    return;
                case { KeyCode: Keys.V, Modifiers: Keys.Control }:
                    await ImportClipboardAsync();
                    return;
                default:
                    _keyHeld = true;
                    return;
            }
        }

        private async Task ImportClipboardAsync()
        {
            var data = Clipboard.GetDataObject();

            if (data?.GetData(DataFormats.FileDrop) is not string[] files) return;
            foreach (var file in files)
            {
                FileContents.Add(new FileInfo(file));
            }

            await UpdateQueueOrderAsync();
            await UpdateQueueViewAsync();
        }

        private void SelectAll()
        {
            foreach (ListViewItem item in CurrentQueueListView.Items)
            {
                item.Selected = true;
            }
        }

        private void DeleteCurrentlySelected()
        {
            if (CurrentQueueListView.SelectedItems.Count <= 0) return;
            foreach (ListViewItem item in CurrentQueueListView.SelectedItems)
            {
                FileContents.RemoveAt(item.Index);
                CurrentQueueListView.Items.Remove(item);
            }
        }

        private void ContextMenuStrip_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem?.Text != "Remove") return;
            
            var selectedItems = CurrentQueueListView.SelectedItems;
            foreach (ListViewItem item in selectedItems)
            {
                FileContents.RemoveAt(item.Index);
                CurrentQueueListView.Items.Remove(item);
            }
        }

        private void CurrentQueueListViewOnClick(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            ContextMenuStrip.Show(this, new Point(e.X, e.Y + ContextMenuStrip.Height));
        }

        private async void _sortingControl_OnSelectedValueChanged(object? sender, EventArgs e)
        {
            await UpdateQueueOrderAsync();
            await UpdateQueueViewAsync();
        }

        //TODO: Perhaps there's a way to keep order without having to re-add all the items?
        private async Task UpdateQueueOrderAsync() => FileContents = await SortFilesAsync(FileContents);

        //TODO: Perhaps there's a way to keep order without having to re-add all the items?
        private Task UpdateQueueViewAsync()
        {
            Invoke(() =>
            {
                CurrentQueueListView.Items.Clear();
                var listviewItems = new List<ListViewItem>(FileContents.Count + 5);
                foreach (var file in FileContents)
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
                ("Name", "Ascending")  => unsortedFiles.OrderBy(f => f.Name).ToList(),                                //If FileSortType.ByName    
                ("Name", "Descending") => unsortedFiles.OrderByDescending(f => f.Name).ToList(),                      //If FileSortType.ByName 
                ("Size", "Ascending")  => unsortedFiles.OrderBy(f => f.Length).ToList(),                              //If FileSortType.BySize
                ("Size", "Descending") => unsortedFiles.OrderByDescending(f => f.Length).ToList(),                    //If FileSortType.BySize 
                ("Type", "Ascending")  => unsortedFiles.OrderBy(f => f.Extension).ToList(),                           //If FileSortType.Type
                ("Type", "Descending") => unsortedFiles.OrderByDescending(f => f.Extension).ToList(),                 //If FileSortType.Type 
                ("Date Modified", "Ascending")  => unsortedFiles.OrderBy(f => f.LastWriteTimeUtc).ToList(),           //If FileSortType.ByDate
                ("Date Modified", "Descending") => unsortedFiles.OrderByDescending(f => f.LastWriteTimeUtc).ToList(), //If FileSortType.ByDate
                _                      => unsortedFiles.ToList()
            };
            return fileSortType;
        }

        private async void ImportFilesButton_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = @"Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png;", //TODO: We can currently only compress these file types.
                    //@"Image Files (*.jpg;*.jpeg;*.png;*.gif;*.gifv;*.mp4)|*.jpg;*.jpeg;*.png;*.gif;*.gifv;*.mp4",
                InitialDirectory = await GetLastVisitedDirectoryAsync(),
                Title = @"Select files to import"
            };
            
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            
            await GlobalSettings.SafeSetSettingAsync(SettingKeys.LAST_VISITED_DIRECTORY_DIALOGUE, 
                Path.GetDirectoryName(openFileDialog.FileNames[0]) ?? string.Empty);
            
            foreach (var fileName in openFileDialog.FileNames)
            {
                FileContents.Add(new FileInfo(fileName));
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
            FileContents.Clear();
            CurrentQueueListView.Items.Clear();
        }

        private async Task AddFilesToQueueAsync(IEnumerable<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                try
                {
                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.Extension.ToLower() is ".jpg" or ".jpeg" or ".png" or ".gif" or ".gifv" or ".mp4")
                        FileContents.Add(fileInfo);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e); //TODO: Logging.
                    throw;
                }
            }

            await UpdateQueueOrderAsync();
            await UpdateQueueViewAsync();
        }
        
        private Task ResetStyleAsync()
        {
            CurrentQueueListView.BackColor = Color.White;
            return Task.CompletedTask;
        }

        private async void CurrentQueueListView_DragLeave(object? sender, EventArgs e) => await ResetStyleAsync();

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
    }
}
