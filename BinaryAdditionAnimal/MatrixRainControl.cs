using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace BinaryAdditionAnimal;

public class MatrixRainControl : Canvas
    {
        private readonly Random _random = new Random();
        private readonly List<MatrixColumn> _columns = new List<MatrixColumn>();
        private readonly DispatcherTimer _timer;
        
        public MatrixRainControl()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            _timer.Tick += Timer_Tick;
            
            Loaded += MatrixRainControl_Loaded;
            Unloaded += MatrixRainControl_Unloaded;
        }

        private void MatrixRainControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeColumns();
            _timer.Start();
        }

        private void MatrixRainControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private void InitializeColumns()
        {
            const int columnWidth = 20;
            int columnCount = (int)ActualWidth / columnWidth;

            for (int i = 0; i < columnCount; i++)
            {
                _columns.Add(new MatrixColumn
                {
                    X = i * columnWidth,
                    Speed = _random.Next(5, 15),
                    Y = _random.Next(-1000, 0),
                    Length = _random.Next(5, 20)
                });
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Children.Clear();

            foreach (var column in _columns)
            {
                column.Y += column.Speed;
                if (column.Y > ActualHeight)
                {
                    column.Y = _random.Next(-1000, 0);
                    column.Speed = _random.Next(5, 15);
                    column.Length = _random.Next(5, 20);
                }

                for (int i = 0; i < column.Length; i++)
                {
                    var text = new TextBlock
                    {
                        Text = _random.Next(2).ToString(),
                        Foreground = new SolidColorBrush(Color.FromArgb(
                            (byte)(255 - (i * (255 / column.Length))),
                            0,
                            (byte)(255 - (i * 10)),
                            0)),
                        FontFamily = new FontFamily("Consolas"),
                        FontSize = 16
                    };

                    SetLeft(text, column.X);
                    SetTop(text, column.Y - (i * 20));
                    Children.Add(text);
                }
            }
        }

        private class MatrixColumn
        {
            public double X { get; set; }
            public double Y { get; set; }
            public int Speed { get; set; }
            public int Length { get; set; }
        }
    }