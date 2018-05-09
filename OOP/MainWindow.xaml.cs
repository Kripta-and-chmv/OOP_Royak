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
        public readonly Dictionary<string, IFunction> functions;
        public readonly Dictionary<string, IOptimizator> optimizers;
        List<Control> controls;

        public MainWindow()
        {
            InitializeComponent();
            functions = new Dictionary<string, IFunction>();
            optimizers = new Dictionary<string, IOptimizator>();
            getFunctionsOptimizers();
            controls = new List<Control> { dgrdPoints, txtbxBorderFrom, txtbxBorderTo, txtbxCoordX, txtbxCoordY, cmbbxFunctions, btnAdd, btnDelete, lblFunction, lblBorders, lblAddX, lblAddY };
            foreach (var ctrl in controls)
                ctrl.Visibility = Visibility.Hidden;
        }

        public void getFunctionsOptimizers()
        {
            try
            {
                string path = Assembly.GetEntryAssembly().Location;
                path = path.Replace("OOP.exe", "");
                string contentFolder = path + @"content\";

                DirectoryInfo directory = new DirectoryInfo(contentFolder);//Assuming Test is your Folder
                FileInfo[] functionDlls = directory.GetFiles("Func*.dll"); //Getting Text files
                foreach (FileInfo file in functionDlls)
                {
                    var types = Assembly.LoadFrom(contentFolder + file.Name).GetTypes();
                    var func = (IFunction)Activator.CreateInstance(types[0]);
                    functions.Add(func.Name, func);
                }

                FileInfo[] optimizerDlls = directory.GetFiles("Opt*.dll"); //Getting Text files
                foreach (FileInfo file in optimizerDlls)
                {
                    var types = Assembly.LoadFrom(contentFolder + file.Name).GetTypes();
                    var opt = (IOptimizator)Activator.CreateInstance(types[0]);
                    optimizers.Add(opt.Name, opt);
                }
            } catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        public class tablePoints
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

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            mouse = new Mouse();
            grid = new CustomGrid(canvas);
            grid.Draw(canvas);
            startObjectsAmount = canvas.Children.Count;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var coords = grid.fromPictureBoxToGrid(e.GetPosition(canvas));
            lblX.Content = coords.X.ToString();
            lblY.Content = coords.Y.ToString();

            if (mouse.IsPushed)
            {
                Point difference = pointsSubstraction(mouse.Position, e.GetPosition(canvas));
                mouse.Position = e.GetPosition(canvas);
                grid.ChangePosition(canvas, difference);
            }
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mouse.Position = e.GetPosition(canvas);
                mouse.IsPushed = true;
            }
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                mouse.IsPushed = false;
        }

        private void canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            mouse.IsPushed = false;
            this.Cursor = Cursors.Hand;
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
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
            var rows = dgrdPoints.Items.OfType<tablePoints>().ToList();
            rows.Sort((x, y) => x.X.CompareTo(y.X));

            canvas.Children.RemoveRange(startObjectsAmount, canvas.Children.Count);
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            string fileText = "";
            var rows = dgrdPoints.Items.OfType<tablePoints>().ToList();

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
                                dgrdPoints.Items.Add(new tablePoints() { X = x, Y = y });
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            double x, y;
            if (!double.TryParse(txtbxCoordX.Text, out x) || !double.TryParse(txtbxCoordY.Text, out y))
                return;

            dgrdPoints.Items.Add(new tablePoints() { X = x, Y = y });
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

        private void btnDoTask_Click(object sender , RoutedEventArgs e)
        {
            if (dgrdPoints.IsVisible && dgrdPoints.Items.Count < 2 || cmbbxFunctions.IsVisible && cmbbxFunctions.SelectedValue == null || cmbbxMethods.SelectedValue == null)
                return;

            var opt = optimizers[cmbbxMethods.SelectedValue.ToString()];
            
            if (opt.needPoints)
                opt.Points = (from row in dgrdPoints.Items.OfType<tablePoints>().ToList()
                              select new Tuple<double, double>(row.X, row.Y)).ToList<Tuple<double, double>>();
            else
                opt.Function = functions[cmbbxFunctions.SelectedValue.ToString()];
            if (opt.needBorders)
            {
                double from;
                double to;
                if (!double.TryParse(txtbxBorderFrom.Text, out from) || !double.TryParse(txtbxBorderTo.Text, out to))
                    return;
                if (from >= to)
                    return;
                opt.Borders = new Tuple<double, double>(from, to);
            }

            var points = opt.Optimize();

            try
            {
                drawing((from point in points
                         select new Point(point.Item1, point.Item2)).ToList<Point>());
            } catch(Exception err)
            {
                MessageBox.Show("Add more point");
            }
        }

        private void dgrdPoints_Loaded(object sender, RoutedEventArgs e)
        {
            addInfoInCmbbx();
        }

        private Visibility boolToVisibility(bool b)
        {
            return b ? Visibility.Visible : Visibility.Hidden;
        }

        private void cmbbxMethods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string methodName = e.AddedItems[0].ToString();
            var usePoints = new List<Control> { dgrdPoints, txtbxCoordX, txtbxCoordY, btnAdd, btnDelete, lblAddX,lblAddY };
            var useBorder = new List<Control> { txtbxBorderFrom, txtbxBorderTo, lblBorders };
            var useFunction = new List<Control> { cmbbxFunctions, lblFunction };
            foreach (var control in usePoints)
                control.Visibility = boolToVisibility(optimizers[methodName].needPoints);
            foreach (var control in useBorder)
                control.Visibility = boolToVisibility(optimizers[methodName].needBorders);
            foreach (var control in useFunction)
                control.Visibility = boolToVisibility(!optimizers[methodName].needPoints);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            dgrdPoints.Items.Remove(dgrdPoints.SelectedItem);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            cmbbxMethods.Items.Clear();
            cmbbxFunctions.Items.Clear();
            functions.Clear();
            optimizers.Clear();
            getFunctionsOptimizers();
            addInfoInCmbbx();
        }

        void addInfoInCmbbx()
        {
            foreach (var func in functions.Keys)
                cmbbxFunctions.Items.Add(func);
            foreach (var opt in optimizers.Keys)
                cmbbxMethods.Items.Add(opt);
        }
    }
}
