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

namespace SoundConsole.Pages.WavePage
{
    public partial class WavePage : Page
    {
        public WaveForm waveForm;

        public WavePage()
        {
            InitializeComponent();

            Label[] labels = new Label[4];
            labels[0] = Label1;
            labels[1] = Label2;
            labels[2] = Label3;
            labels[3] = Label4;

            this.waveForm = new WaveForm(Graphic, GraphicCanvas, labels);
        }

        private void Page_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.waveForm.ChangeTimeWidth(e.Delta);
        }
    }

    public class WaveForm
    {
        public double Width { get { return this.GraphicCanvas.Width; } }

        Polyline Graphic;
        Canvas GraphicCanvas;
        Label[] labels;

        private TimeSpan timeWidth = TimeSpan.FromSeconds(2);
        private readonly TimeSpan TimeUnit = TimeSpan.FromMilliseconds(1);
        private readonly TimeSpan TimeMin = TimeSpan.FromMilliseconds(10);

        public WaveForm(Polyline Graphic, Canvas GraphicCanvas, Label[] labels)
        {
            this.Graphic = Graphic;
            this.GraphicCanvas = GraphicCanvas;
            this.labels = labels;

            this.DisplayLabel();
        }

        public void Display(TimeSpan timeSpan, TimeSpan totalTime, InfiniteSequence data)
        {
            Graphic.Points = new PointCollection();

            for (int i = 0; i < Width; i++)
            {
                Graphic.Points.Add(NewPoint(i, data[(int)
                (
                    ((double)data.Size) * (timeSpan.Ticks - timeWidth.Ticks * i / Width) / totalTime.Ticks
                )]));
            }
        }

        public void ChangeTimeWidth(int value)
        {
            this.timeWidth = this.timeWidth.Add(TimeSpan.FromTicks(value * TimeUnit.Ticks));
            this.DisplayLabel();
        }

        private void DisplayLabel()
        {
            for(int i = 0; i < labels.Length; i++)
            {
                labels[i].Content = this.timeWidth.TotalMilliseconds / (labels.Length + 1) * (i + 1);
            }
        }

        private Point NewPoint(int n, short y)
        {
            return new Point(n, this.GraphicCanvas.Height * (0.5 - ((double)y) / short.MaxValue));
        }
    }
}
