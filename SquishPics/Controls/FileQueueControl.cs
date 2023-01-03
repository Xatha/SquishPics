namespace SquishPics.Controls
{
    public partial class FileQueueControl : UserControl
    {
        private List<FileInfo> _fileList;

        public FileQueueControl(SortingControl sortingControl)
        {
            InitializeComponent();

            _fileList = new List<FileInfo>();
            
            CurrentQueueListView.DragEnter += CurrentQueueListView_DragEnter;
            CurrentQueueListView.DragDrop  += CurrentQueueListView_DragDrop;
            CurrentQueueListView.DragLeave += CurrentQueueListView_DragLeave;
            
            ClearQueueButton.Click         += ClearQueueButton_Click;
            ImportFilesButton.Click        += ImportFilesButton_Click;

            sortingControl.OnSelectedValueChanged += _sortingControl_OnSelectedValueChanged;
        }

        private async void _sortingControl_OnSelectedValueChanged(object? sender, EventArgs e)
        {
            await UpdateQueueOrderAsync();
            await UpdateQueueViewAsync();
        }

        //TODO: Perhaps there's a way to keep order without having to re-add all the items?
        private async Task UpdateQueueOrderAsync() => _fileList = await SortFiles(_fileList);

        //TODO: Perhaps there's a way to keep order without having to re-add all the items?
        private Task UpdateQueueViewAsync()
        {
            Invoke(() =>
            {
                CurrentQueueListView.Items.Clear();
                var listviewItems = new List<ListViewItem>(_fileList.Count + 5);
                foreach (var file in _fileList)
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

        private static Task<List<FileInfo>> SortFiles(IEnumerable<FileInfo> unsortedFiles)
        {
            var fileSortType = (GlobalSettings.Default.SORTING_MODE, GlobalSettings.Default.SORTING_ORDER) switch
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
            return Task.FromResult(fileSortType);
        }

        private async void ImportFilesButton_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = @"Image Files (*.jpg;*.jpeg;*.png;*.gif;*.gifv;*.mp4)|*.jpg;*.jpeg;*.png;*.gif;*.gifv;*.mp4",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };
            
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            
            foreach (var fileName in openFileDialog.FileNames)
            {
                _fileList.Add(new FileInfo(fileName));
            }

            await UpdateQueueOrderAsync();
            await UpdateQueueViewAsync();
        }

        private void ClearQueueButton_Click(object? sender, EventArgs e)
        {
            _fileList.Clear();
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
                        _fileList.Add(fileInfo);
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
