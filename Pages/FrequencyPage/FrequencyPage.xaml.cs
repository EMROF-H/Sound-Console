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

using DigitalSignalProcess;

namespace SoundConsole.Pages.FrequencyPage
{
    public partial class FrequencyPage : Page
    {
        private Histogram histogram;

        public FrequencyPage(Histogram histogram)
        {
            InitializeComponent();

            this.histogram = histogram;
            this.histogram.SetHistogramGrid(HistogramGrid);
            this.histogram.length = MainWindow.Length;
        }
    }

    public class Histogram
    {
        private class Bar
        {
            private Rectangle rectangle;

            private double _value;
            public double Value
            {
                get
                {
                    return this._value;
                }
                set
                {
                    if (value <= 0) value = 1;

                    this.rectangle.Height = value;
                    this._value = value;
                }
            }

            public Bar(Grid histogramGrid, double width, int serial)
            {
                this.rectangle = new Rectangle();
                histogramGrid.Children.Add(this.rectangle);

                this.rectangle.Fill = new SolidColorBrush(Colors.DarkBlue);
                this.rectangle.Width = width;
                this.Value = 1;
                this.rectangle.SetValue(Grid.ColumnProperty, serial);
                this.rectangle.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Bottom);
            }
        }

        private Bar[] bars;

        public double Width { get { return this.histogramGrid.Width; } }
        public double Height { get { return this.histogramGrid.Height; } }

        private double barWidth;

        private double MaxValue;

        private Grid histogramGrid;

        private int _length;
        public int length
        {
            get
            {
                return this._length;
            }

            set
            {
                if (value <= 0) return;

                barWidth = this.Width / value;

                bars = new Bar[value];

                for (int i = 0; i < value; i++)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition();
                    this.histogramGrid.ColumnDefinitions.Add(columnDefinition);
                    bars[i] = new Bar(this.histogramGrid, this.barWidth, i);
                }

                this._length = value;
            }
        }

        public Histogram(double maxValue)
        {
            this.MaxValue = maxValue;
        }

        public void SetHistogramGrid(Grid histogramGrid)
        {
            this.histogramGrid = histogramGrid;
        }

        public void SetZero()
        {
            for (int i = 0; i < length; i++)
            {
                this.bars[i].Value = 0;
            }
        }

        public void Display(double[] values)
        {
            for (int i = 0; i < length; i++)
            {
                double value = values[i];
                if (value > this.MaxValue) value = this.MaxValue;
                if (value < 0) value = 0;

                this.bars[i].Value = value / this.MaxValue * this.Height;
            }
        }

        public void Display(Complex[] values)
        {
            for (int i = 0; i < length; i++)
            {
                double value = values[i].Absolute;
                if (value > this.MaxValue) value = this.MaxValue;
                if (value < 0) value = 0;

                this.bars[i].Value = value / this.MaxValue * this.Height;
            }
        }
    }
}
