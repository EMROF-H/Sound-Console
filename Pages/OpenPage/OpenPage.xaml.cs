using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Audio;
using Audio.WAVE;

namespace SoundConsole.Pages.OpenPage
{
    public partial class OpenPage : Page
    {
        private Uri path;
        public Player player;

        public OpenPage(Player player)
        {
            InitializeComponent();

            this.player = player;
            this.player.SetInformationListBox(InformationListBox);
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.Cancel) { return; }
            this.path = new Uri(folderBrowserDialog.SelectedPath.Trim());
            this.FolderGroupBoxContent.Content = Path.GetFileName(this.path.OriginalString);
            this.RefreshButton.Visibility = Visibility.Visible;
            ReadFile();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ReadFile();
        }

        private void ReadFile()
        {
            this.FilesListBox.Items.Clear();
            string[] files = Directory.GetFiles(this.path.OriginalString, "*.wav");
            foreach (string filename in files)
            {
                try
                {
                    FileInfo file = new FileInfo(filename);

                    this.FilesListBox.Items.Add(new FileListBoxItem(file, player));
                }
                catch (FileNotFoundException exception)
                {
                    System.Windows.MessageBox.Show(exception.Message);
                    continue;
                }
            }
        }
    }

    public class FileListBoxItem : ListBoxItem
    {
        private FileInfo file;
        private Player player;

        public FileListBoxItem(FileInfo file, Player player)
        {
            this.file = file;
            this.Content = file.Name;
            this.player = player;
            this.MouseLeftButtonUp += FileListBoxItem_MouseLeftButtonUp;
        }

        private void FileListBoxItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.player.Load(new Uri(this.file.ToString()));
        }
    }
}
