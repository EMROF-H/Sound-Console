using Audio.WAVE;
using SoundConsole.Pages.FrequencyPage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

using SoundConsole;
using DigitalSignalProcess;
using Menu;
using SoundConsole.Pages.WavePage;
using System.Windows.Controls;

namespace Audio
{
    public class Player : MediaPlayer
    {
        public static readonly Uri OriginUri = new Uri("C:\\Windows\\Temp\\_SoundConsoleOrigin.wav");
        public static readonly Uri ProcessedUri = new Uri("C:\\Windows\\Temp\\_SoundConsoleProcessed.wav");

        public WaveFile file;
        TimeSpan totalTime;

        Histogram histogram;
        MusicMenuButton musicMenuButton;
        WavePage wavePage;
        ListBox informationListBox;

        public Player(Histogram histogram)
        {
            this.histogram = histogram;

            timer = new Timer(Plot, null, 0, 30);

            this.MediaEnded += Player_MediaEnded;
        }

        public void SetMusicMenuButton(MusicMenuButton musicMenuButton)
        {
            this.musicMenuButton = musicMenuButton;
        }

        public void SetWavePage(WavePage wavePage)
        {
            this.wavePage = wavePage;
        }

        public void SetInformationListBox(ListBox informationListBox)
        {
            this.informationListBox = informationListBox;
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            this.musicMenuButton.Pause();
        }

        Timer timer;
        TimeSpan timeSpan = TimeSpan.FromSeconds(0);
        TimeSpan lastTimeSpan = TimeSpan.FromSeconds(0);
        private void Plot(object sender)
        {
            if (this.file == null) return;
            MainWindow.MainDispatcher.Invoke(() =>
            {

                lastTimeSpan = timeSpan;
                timeSpan = base.Position;

                InfiniteSequence data = new InfiniteSequence(this.file.Data.LeftData);

                wavePage.waveForm.Display(timeSpan, totalTime, data);

                if (timeSpan == lastTimeSpan)
                {
                    histogram.SetZero();
                }
                else
                {
                    short[] input = data.Truncation((int)(((double)this.file.Data.Size) * timeSpan.Ticks / totalTime.Ticks), 2 * MainWindow.Length);
                    histogram.Display
                    (
                        FourierTransforms.FastFourierTransform(input).Take(MainWindow.Length).ToArray()
                    );
                    /*
                    histogram.Display
                    (
                        //TransformsFunctions.LogarithmTransform
                        //(
                            FourierTransforms.FastFourierTransform
                            (
                                this.file.Data.LeftData.Skip
                                (
                                    (int)(((double)this.file.Data.Size) * timeSpan.Ticks / totalTime.Ticks)
                                )
                                .Take(2 * MainWindow.Length).ToArray()
                            )
                            .Take(MainWindow.Length).ToArray()
                        //)
                    );
                    */
                }
            });
        }

        public void Load(Uri source)
        {
            if (File.Exists(OriginUri.OriginalString))
            {
                File.Delete(OriginUri.OriginalString);
            }
            File.Copy(source.OriginalString, OriginUri.OriginalString);
            Load(false);
        }

        public void Load(bool isProcessed)
        {
            TimeSpan timeSpan = base.Position;
            this.Stop();
            if (isProcessed)
            {
                Open(ProcessedUri);
                if(timeSpan != TimeSpan.Zero)
                {
                    base.Position = timeSpan;
                    this.musicMenuButton.Play();
                }
            }
            else
            {
                Open(OriginUri);
            }
        }

        private void DisplayInformation()
        {
            this.informationListBox.Items.Clear();
            this.informationListBox.Items.Add($"Type: {Encoding.ASCII.GetString(BitConverter.GetBytes(this.file.RIFF.Type).Reverse().ToArray())}");
            this.informationListBox.Items.Add($"Size: {this.file.RIFF.Size + 8} Byte");
            this.informationListBox.Items.Add($"Sample rate: {this.file.Format.SampleRate} Hz");
            this.informationListBox.Items.Add($"Bits per sample: {this.file.Format.BitsPerSample} bit");
            this.informationListBox.Items.Add($"Code rate: {this.file.Format.ByteRate * 8} bps");
            this.informationListBox.Items.Add($"Channels: {this.file.Format.NumChannels} channel(s)");
            this.informationListBox.Items.Add($"Data size: {this.file.Data.Size} Byte");
        }

        public new void Open(Uri source)
        {
            base.Open(source);
            this.file = new WaveFile(source);
            totalTime = TimeSpan.FromSeconds(this.file.Data.Size / this.file.Format.SampleRate);
            DisplayInformation();
        }

        public new void Stop()
        {
            this.musicMenuButton.Pause();
            base.Stop();
        }
    }
}
