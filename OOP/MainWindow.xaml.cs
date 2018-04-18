using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using System.Threading.Tasks;
using System.Composition.Hosting;
using System.Reflection;
using Interface;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly List<IFunction> functions;
        public readonly List<IOptimizator> optimizers;

        public MainWindow()
        {
            InitializeComponent();
            functions = new List<IFunction>();
            optimizers = new List<IOptimizator>();
            getFunctionsOptimizers();
        }

        public void getFunctionsOptimizers()
        {
            string path = Assembly.GetEntryAssembly().Location;
            path = path.Replace("OOP.exe", "");
            string contentFolder = path + @"content\";

            DirectoryInfo directory = new DirectoryInfo(contentFolder);//Assuming Test is your Folder
            FileInfo[] functionDlls = directory.GetFiles("Func*.dll"); //Getting Text files
            foreach (FileInfo file in functionDlls)
            {
                var types = Assembly.LoadFrom(contentFolder + file.Name).GetTypes();
                functions.Add((IFunction)Activator.CreateInstance(types[0]));
            }

            FileInfo[] optimizerDlls = directory.GetFiles("Opt*.dll"); //Getting Text files
            foreach (FileInfo file in optimizerDlls)
            {
                var types = Assembly.LoadFrom(contentFolder + file.Name).GetTypes();
                optimizers.Add((IOptimizator)Activator.CreateInstance(types[0]));
            }

            var interpolation = optimizers[1];
            interpolation.Points = new List<Tuple<double, double>> { new Tuple<double, double>(1, 1), new Tuple<double, double>(2, 2), new Tuple<double, double>(40, 40) };
            var res2 = interpolation.Optimize();
            var points = new List<Point>();
            foreach(var point in res2)
            {
                points.Add(new Point(point.Item1, point.Item2));
            }
        }

        public class tableParameters
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        public static Point pointsAddition(Point x, Point y)
        {
            return new Point(x.X + y.X, x.Y + y.Y);
        }

        public static Point pointsSubstraction(Point x, Point y)
        {
            return new Point(x.X - y.X, x.Y - y.Y);
        }

        CustomGrid grid;
        Mouse mouse;
        int startObjectsAmount = 0;

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mouse.Position = e.GetPosition(canvas);
                mouse.IsPushed = true;
                if (!mouse.IsMoveMode)
                {
                    var point = grid.fromPictureBoxToGrid(e.GetPosition(canvas));
                    dgrdPoints.Items.Add(new tableParameters() { X = point.X, Y = point.Y });
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                mouse.IsMoveMode = !mouse.IsMoveMode;

                if (mouse.IsMoveMode)
                    this.Cursor = Cursors.Hand;
                else
                    this.Cursor = Cursors.Cross;
            }
        }

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            mouse = new Mouse();
            grid = new CustomGrid(canvas);
            grid.Draw(canvas);
            startObjectsAmount = canvas.Children.Count;
            foreach (var function in Functions.All)
                cmbbxFunctions.Items.Add(function);
            foreach (var method in Methods.All)
                cmbbxMethods.Items.Add(method);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var coords = grid.fromPictureBoxToGrid(e.GetPosition(canvas));
            lblX.Content = coords.X.ToString();
            lblY.Content = coords.Y.ToString();

            if (mouse.IsPushed && mouse.IsMoveMode)
            {
                Point difference = pointsSubstraction(mouse.Position, e.GetPosition(canvas));
                mouse.Position = e.GetPosition(canvas);
                grid.ChangePosition(canvas, difference);
            }
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                mouse.IsPushed = false;
            }
        }

        private void stepDec_Click(object sender, RoutedEventArgs e)
        {
            var stepOld = grid.Step;
            grid.Step /= grid.StepMultiplier;
            if (stepOld == grid.Step)
                return;

            var center = new Point(canvas.Width / 2, canvas.Height / 2);
            var oldStart = grid.Start;
            Point difference = pointsSubstraction(center, oldStart);
            grid.ChangePosition(canvas, difference);

            grid.Width /= grid.StepMultiplier;
            grid.Height /= grid.StepMultiplier;

            foreach (var figure in canvas.Children)
            {
                if (figure is Line)
                {
                    var lineStart = grid.fromPictureBoxToGrid(new Point(((Line)figure).X1, ((Line)figure).Y1));
                    var lineEnd = grid.fromPictureBoxToGrid(new Point(((Line)figure).X2, ((Line)figure).Y2));
                    lineStart = grid.fromGridToPictureBox(new Point(lineStart.X / grid.StepMultiplier,
                                                                    lineStart.Y / grid.StepMultiplier));
                    lineEnd = grid.fromGridToPictureBox(new Point(lineEnd.X / grid.StepMultiplier,
                                                                    lineEnd.Y / grid.StepMultiplier));

                    ((Line)figure).X1 = lineStart.X;
                    ((Line)figure).Y1 = lineStart.Y;
                    ((Line)figure).X2 = lineEnd.X;
                    ((Line)figure).Y2 = lineEnd.Y;
                }
                if (figure is Ellipse)
                {
                    var newDot = new Point(((Ellipse)figure).Margin.Left, ((Ellipse)figure).Margin.Top);
                    newDot = grid.fromPictureBoxToGrid(newDot);
                    newDot = grid.fromGridToPictureBox(new Point(newDot.X / grid.StepMultiplier,
                                                                 newDot.Y / grid.StepMultiplier));
                    ((Ellipse)figure).Margin = new Thickness(newDot.X, newDot.Y, 0, 0);
                }
                if (figure is TextBlock)
                {
                    var newDot = new Point(((TextBlock)figure).Margin.Left, ((TextBlock)figure).Margin.Top);
                    newDot = grid.fromPictureBoxToGrid(newDot);
                    newDot = grid.fromGridToPictureBox(new Point(newDot.X / grid.StepMultiplier,
                                                                 newDot.Y / grid.StepMultiplier));
                    ((TextBlock)figure).Margin = new Thickness(newDot.X, newDot.Y, 0, 0);
                }
            }

            grid.ChangePosition(canvas, new Point(-difference.X, -difference.Y));
        }

        private void stepInc_Click(object sender, RoutedEventArgs e)
        {
            var stepOld = grid.Step;
            grid.Step *= grid.StepMultiplier;
            if (stepOld == grid.Step)
                return;

            var center = new Point(canvas.Width / 2, canvas.Height / 2);
            var oldStart = grid.Start;
            Point difference = pointsSubstraction(center, oldStart);
            grid.ChangePosition(canvas, difference);

            grid.Width *= grid.StepMultiplier;
            grid.Height *= grid.StepMultiplier;

            foreach (var figure in canvas.Children)
            {
                if (figure is Line)
                {
                    var lineStart = grid.fromPictureBoxToGrid(new Point(((Line)figure).X1, ((Line)figure).Y1));
                    var lineEnd = grid.fromPictureBoxToGrid(new Point(((Line)figure).X2, ((Line)figure).Y2));
                    lineStart = grid.fromGridToPictureBox(new Point(lineStart.X * grid.StepMultiplier,
                                                                    lineStart.Y * grid.StepMultiplier));
                    lineEnd = grid.fromGridToPictureBox(new Point(lineEnd.X * grid.StepMultiplier,
                                                                    lineEnd.Y * grid.StepMultiplier));

                    ((Line)figure).X1 = lineStart.X;
                    ((Line)figure).Y1 = lineStart.Y;
                    ((Line)figure).X2 = lineEnd.X;
                    ((Line)figure).Y2 = lineEnd.Y;
                }
                if (figure is Ellipse)
                {
                    var newDot = new Point(((Ellipse)figure).Margin.Left, ((Ellipse)figure).Margin.Top);
                    newDot = grid.fromPictureBoxToGrid(newDot);
                    newDot = grid.fromGridToPictureBox(new Point(newDot.X * grid.StepMultiplier,
                                                                 newDot.Y * grid.StepMultiplier));
                    ((Ellipse)figure).Margin = new Thickness(newDot.X, newDot.Y, 0, 0);
                }
                if (figure is TextBlock)
                {
                    var newDot = new Point(((TextBlock)figure).Margin.Left, ((TextBlock)figure).Margin.Top);
                    newDot = grid.fromPictureBoxToGrid(newDot);
                    newDot = grid.fromGridToPictureBox(new Point(newDot.X * grid.StepMultiplier,
                                                                 newDot.Y * grid.StepMultiplier));
                    ((TextBlock)figure).Margin = new Thickness(newDot.X, newDot.Y, 0, 0);
                }
            }

            grid.ChangePosition(canvas, new Point(-difference.X, -difference.Y));
        }

        private void dgrdPoints_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //if (dgrdPoints.Items.Count < 2)
            //    return;
            //var rows = dgrdPoints.Items.OfType<tablePoints>().ToList();
            //rows.Sort((x, y) => x.X.CompareTo(y.X));

            //canvas.Children.RemoveRange(startObjectsAmount, canvas.Children.Count);

            //for (int i = 1; i < rows.Count; i++)
            //{
            //    var start = grid.fromGridToPictureBox(new Point(rows[i - 1].X, rows[i - 1].Y));
            //    var end = grid.fromGridToPictureBox(new Point(rows[i].X, rows[i].Y));
            //    canvas.Children.Add(new Line()
            //    {
            //        Name = "Graph",
            //        Stroke = Brushes.Red,
            //        StrokeThickness = 4,
            //        X1 = start.X,
            //        Y1 = start.Y,
            //        X2 = end.X,
            //        Y2 = end.Y
            //    });
            //}
        }

        private void canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            mouse.IsMoveMode = true;
            mouse.IsPushed = false;
            this.Cursor = Cursors.Hand;
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            string fileText = "";
            var rows = dgrdPoints.Items.OfType<tableParameters>().ToList();

            foreach (var row in rows)
                fileText += row.X + " " + row.Y + "\n";

            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "Graphs(*.grph;*.txt;)|*.grph;*.txt;|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
                File.WriteAllText(dialog.FileName, fileText);
        }

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open_dialog = new OpenFileDialog()
            {
                Filter = "Graphs(*.grph;*.txt;)|*.grph;*.txt;|All files (*.*)|*.*"
            };
            if (open_dialog.ShowDialog() == true)
            {
                try
                {
                    using (var file = new StreamReader(open_dialog.FileName))
                    {
                        dgrdPoints.Items.Clear();
                        while (!file.EndOfStream)
                        {
                            var coord = file.ReadLine().Split(' ');
                            try
                            {
                                double x, y;
                                double.TryParse(coord[0], out x);
                                double.TryParse(coord[1], out y);
                                dgrdPoints.Items.Add(new tableParameters() { X = x, Y = y });
                            }
                            catch
                            {
                                return;
                            }
                        }
                    }
                }
                catch
                { }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //double x, y;
            //if (!double.TryParse(txtbxCoordX.Text, out x))
            //    return;
            //if (!double.TryParse(txtbxCoordY.Text, out y))
            //    return;
            //dgrdPoints.Items.Add(new tableParameters() { X = x, Y = y });
        }

        private void drawing(List<Point> rows)
        {
            canvas.Children.RemoveRange(startObjectsAmount, canvas.Children.Count);

            for (int i = 1; i < rows.Count; i++)
            {
                var start = grid.fromGridToPictureBox(new Point(rows[i - 1].X, rows[i - 1].Y));
                var end = grid.fromGridToPictureBox(new Point(rows[i].X, rows[i].Y));
                canvas.Children.Add(new Line()
                {
                    Name = "Graph",
                    Stroke = Brushes.Red,
                    StrokeThickness = 4,
                    X1 = start.X,
                    Y1 = start.Y,
                    X2 = end.X,
                    Y2 = end.Y
                });
            }
        }

        private void btnDoTask_Click(object sender, RoutedEventArgs e)
        {
            //var buf = dgrdPoints.Items.OfType<tableParameters>().ToList();

            //var k = Optimizer.optimize((string)cmbbxFunctions.SelectedValue, (string)cmbbxMethods.SelectedValue, new List<Point> { new Point(0, 0) });
            //foreach (var dts in k)
            //{
            //    drawing(dts);
            //    //System.Threading.Thread.Sleep(1000);
            //}
        }
    }
}
