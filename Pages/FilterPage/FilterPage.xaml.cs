using DigitalSignalProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Filter;
using Audio;
using System.IO;

namespace SoundConsole.Pages.FilterPage
{
    public partial class FilterPage : Page
    {
        private Player player;

        public FilterPage(Player player)
        {
            InitializeComponent();
            MainWindow.HideBoundingBox(this);

            this.player = player;
        }

        private async void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            ButtonFilter.IsEnabled = false;

            Filter.Filter filter = new Filter.Filter
            (
                Filter.Filter.ToFilterType(ComboBoxFilterType.Text),
                Convert.ToInt32(TextBoxOrder.Text),
                new Frequency(Convert.ToDouble(TextBoxFrequencyLow.Text), this.player.file.Format.SampleRate),
                new Frequency(Convert.ToDouble(TextBoxFrequencyHigh.Text), this.player.file.Format.SampleRate),
                Windows.ToWindowTyoe(ComboBoxWindowType.Text)
            );

            this.ProgressBarLeft.Visibility = Visibility.Visible;
            this.ProgressBarRight.Visibility = Visibility.Visible;
            this.ProgressBarWrite.Visibility = Visibility.Visible;

            short[] leftData  = await filter.Process(player.file.Data.LeftData,  this.ProgressBarLeft);
            short[] rightData = await filter.Process(player.file.Data.RightData, this.ProgressBarRight);

            //short[] leftData  = filter.Process(player.file.Data.LeftData,  this.ProgressBarLeft);
            //short[] rightData = filter.Process(player.file.Data.RightData, this.ProgressBarRight);

            if (File.Exists(Player.ProcessedUri.OriginalString))
            {
                File.Delete(Player.ProcessedUri.OriginalString);
            }
            File.Copy(Player.OriginUri.OriginalString, Player.ProcessedUri.OriginalString);

            this.ProgressBarWrite.Visibility = Visibility.Visible;
            await Task.Run(() =>
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(new FileStream(Player.ProcessedUri.OriginalString, FileMode.Open, FileAccess.Write), Encoding.UTF8))
                {
                    binaryWriter.Seek(this.player.file.GetDataLocation(), SeekOrigin.Current);
                    int node = this.player.file.Data.Size / 500;
                    for (int i = 0; i < this.player.file.Data.Size; i++)
                    {
                        binaryWriter.Write(leftData[i]);
                        binaryWriter.Write(rightData[i]);

                        if (i % node == 0)
                        {
                            MainWindow.MainDispatcher.Invoke(() =>
                            {
                                this.ProgressBarWrite.Value = 100.0 * i / this.player.file.Data.Size;
                            });
                        }
                    }

                    binaryWriter.Close();
                }
            });
            player.Load(true);

            this.ProgressBarLeft.Visibility = Visibility.Hidden;
            this.ProgressBarRight.Visibility = Visibility.Hidden;
            this.ProgressBarWrite.Visibility = Visibility.Hidden;
            this.ProgressBarLeft.Value = 0;
            this.ProgressBarRight.Value = 0;
            this.ProgressBarWrite.Value = 0;
            ButtonFilter.IsEnabled = true;
        }

        private void ComboBoxFilterType_DropDownClosed(object sender, EventArgs e)
        {
            if(ComboBoxFilterType.SelectedItem == null)
            {
                TextBoxFrequencyLow.IsEnabled  = false;
                TextBoxFrequencyHigh.IsEnabled = false;
                return;
            }
            switch(Filter.Filter.ToFilterType(ComboBoxFilterType.Text))
            {
                case Filter.Type.LowPass:
                    TextBoxFrequencyLow.IsEnabled  = true;
                    TextBoxFrequencyHigh.IsEnabled = false;
                    break;
                case Filter.Type.HighPass:
                    TextBoxFrequencyLow.IsEnabled  = false;
                    TextBoxFrequencyHigh.IsEnabled = true;
                    break;
                case Filter.Type.BandPass:
                case Filter.Type.BandStop:
                    TextBoxFrequencyLow.IsEnabled  = true;
                    TextBoxFrequencyHigh.IsEnabled = true;
                    break;
            }
        }
    }
}
