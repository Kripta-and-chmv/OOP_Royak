using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Drawing;

namespace WpfApp2
{
    class CustomGrid
    {
        private Point start;
        private double step = 10;
        private double stepMultiplier = 2;
        private double width = 0;
        private double height = 0;

        public CustomGrid(Canvas canvas)
        {
            width = (int)canvas.Width;
            height = (int)canvas.Height;
            start = new Point(canvas.Width / 2, this.Height / 2);           
        }

        public Point Start
        {
            get { return start; }
            set { start = value;}
        }

        public double StepMultiplier
        {
            get { return stepMultiplier; }
        }

        public double Step
        {
            get { return step;}
            set { if (value >= 10 && value <= 160) step = value; }
        }

        public double Width
        {
            get { return width; }
            set { if (value > 0) width = value; }
        }

        public double Height
        {
            get { return height; }
            set { if (value > 0) height = value; }
        }

        private Line gridLine(double X1, double Y1, double X2, double Y2)
        {
            var line = new Line();
            line.X1 = X1;
            line.Y1 = Y1;
            line.X2 = X2;
            line.Y2 = Y2;
            line.Stroke = Brushes.Black;
            return line;
        }

        private Line axisLine(double X1, double Y1, double X2, double Y2)
        {
            var line = new Line();
            line.X1 = X1;
            line.Y1 = Y1;
            line.X2 = X2;
            line.Y2 = Y2;
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 4;
            return line;
        }

        private Ellipse gridPoint()
        {
            var point = new Ellipse() { Width=10, Height=10, Fill=Brushes.Red };
            return point;
        }

        private TextBlock gridNumber()
        {
            return new TextBlock()
            {
                Foreground = new SolidColorBrush(Colors.Red),
                Height = 30,
                Width = 100,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
            };
        }

        public void Draw(Canvas canvas)
        {
            DrawGrid(canvas);
            DrawAxis(canvas);
            DrawNumbers(canvas);
        }

        private void DrawGrid(Canvas canvas)
        {
            DrawHorizontal(canvas);
            DrawVertical(canvas);
        }

        private void DrawHorizontal(Canvas canvas)
        {
            for (var i = start.X; i <= canvas.Width; i += step)
            {
                var line = gridLine(i, 0, i, this.Height);
                canvas.Children.Add(line);
            }
            for (var i = start.X - step; i >= 0; i -= step)
            {
                var line = gridLine(i, 0, i, this.Height);
                canvas.Children.Add(line);
            }
        }

        private void DrawVertical(Canvas canvas)
        {
            for (var i = start.Y; i >= 0; i -= step)
            {
                var line = gridLine(0, i, this.Width, i);
                canvas.Children.Add(line);
            }

            for (var i = start.Y + step; i <= this.Height; i += step)
            {
                var line = gridLine(0, i, this.Width, i);
                canvas.Children.Add(line);
            }
        }

        private void DrawAxis(Canvas canvas)
        {
            var lineX = axisLine(0, start.Y, this.Width, start.Y);
            lineX.Name = "lineX";
            canvas.Children.Add(lineX);

            var arrowX1 = axisLine(this.Width, start.Y, this.Width - 10, start.Y - 10);
            arrowX1.Name = "lineX";
            canvas.Children.Add(arrowX1);
            var arrowX2 = axisLine(this.Width, start.Y, this.Width  - 10, start.Y + 10);
            arrowX2.Name = "lineX";
            canvas.Children.Add(arrowX2);
            var y = gridNumber();
            y.Name = "axisNameY";
            y.Text = "Y";
            y.Margin = new Thickness(start.X - 20, 0, 0, 0);
            canvas.Children.Add(y);

            var lineY = axisLine(start.X, 0, start.X, this.Height);
            lineY.Name = "lineY";
            canvas.Children.Add(lineY);

            var arrowY1 = axisLine(start.X, 0, start.X - 10, 10);
            arrowY1.Name = "lineY";
            canvas.Children.Add(arrowY1);
            var arrowY2 = axisLine(start.X, 0, start.X + 10, 10);
            arrowY2.Name = "lineY";
            canvas.Children.Add(arrowY2);
            var x = gridNumber();
            x.Name = "axisNameX";
            x.Text = "X";
            x.Margin = new Thickness(canvas.Width - 20, start.Y - 30, 0, 0);
            canvas.Children.Add(x);
        }

        private void DrawNumbers(Canvas canvas)
        {
            for (var i = start.X + Step; i <= canvas.Width; i += step)
                if (this.fromPictureBoxToGrid(new Point(i, 0)).X % 5 == 0)
                    DrawNumber(canvas, 'x', i);

            for (var i = start.X; i >= 0; i -= step)
                if (this.fromPictureBoxToGrid(new Point(i, 0)).X % 5 == 0)
                    DrawNumber(canvas, 'x', i);

            for (var i = start.Y; i >= 0; i -= step)
                if (this.fromPictureBoxToGrid(new Point(0, i)).Y % 5 == 0)
                    DrawNumber(canvas, 'y', i);

            for (var i = start.Y + step; i <= this.Height; i += step)
                if (this.fromPictureBoxToGrid(new Point(0, i)).Y % 5 == 0)
                    DrawNumber(canvas, 'y', i);
        }

        private void DrawNumber(Canvas canvas, char type, double i)
        {
            if (type != 'x' && type != 'y')
                throw (new ArgumentException("wrong type"));

            var dash = type == 'x' 
                ? axisLine(i, start.Y - 6, i, start.Y + 6) 
                : axisLine(start.X - 6, i, start.X + 6, i);
            dash.Name = type == 'x' ? "dashX" : "dashY";
            canvas.Children.Add(dash);

            var number = gridNumber();
            number.Name = type == 'x' ? "numberX" : "numberY";
            var value = type == 'x' 
                ? fromPictureBoxToGrid(new Point(i, 0)).X 
                : fromPictureBoxToGrid(new Point(0, i)).Y;
            number.Text = value.ToString();
            if (number.Text == "0") number.Text = "";
            number.Margin = type == 'x' 
                ? new Thickness(i, start.Y, 0, 0)
                : new Thickness(start.X, i, 0, 0);
            if(type == 'x') number.LayoutTransform = new RotateTransform() { Angle = 90 };
            canvas.Children.Add(number);
        }

        public Point fromGridToPictureBox(Point point)
        {
            double x = point.X,
                y = point.Y;
            return new Point((x * this.Step + this.Start.X), (this.Start.Y - y * this.Step));
        }

        public Point fromPictureBoxToGrid(Point point)
        {
            double x = point.X,
                y = point.Y;
            return new Point((x - this.Start.X) / (double)this.Step, (this.Start.Y - y) / (double)this.Step);
        }

        public void ChangePosition(Canvas canvas, Point difference)
        {
            start.X += difference.X;
            start.Y += difference.Y;
            
            foreach (var figure in canvas.Children)
                ChangeFigurePosition(figure, difference);
        }

        private void ChangeFigurePosition(object figure, Point difference)
        {
            if (figure is Line)
                changeLinePosition((Line)figure, difference);
            if (figure is Ellipse)
                changeDotPosition((Ellipse)figure, difference);
            if (figure is TextBlock)
                changeTextPosition((TextBlock)figure, difference);
        }

        private void changeTextPosition(TextBlock number, Point difference)
        {
            if (number.Name == "numberX")
            {
                var x = number.Margin.Left + difference.X;
                x %= this.Width;
                x = x < 0 ? x + this.Width : x;
                number.Margin = new Thickness(x, number.Margin.Top + difference.Y, 0, 0);
                number.Text = fromPictureBoxToGrid(new Point(x, 0)).X.ToString();
                if (number.Text == "0") number.Text = "";
            }
            if (number.Name == "numberY")
            {
                var y = number.Margin.Top + difference.Y;
                y %= this.Height;
                y = y < 0 ? y + this.Height : y;
                number.Margin = new Thickness(number.Margin.Left + difference.X, y, 0, 0);
                number.Text = fromPictureBoxToGrid(new Point(0, y)).Y.ToString();
            }
            if (number.Name == "axisNameX")
                number.Margin = new Thickness(number.Margin.Left, number.Margin.Top + difference.Y, 0, 0);
            if (number.Name == "axisNameY")
                number.Margin = new Thickness(number.Margin.Left + difference.X, number.Margin.Top, 0, 0);
        }

        private void changeDotPosition(Ellipse dot, Point difference)
        {
            dot.Margin = new Thickness(dot.Margin.Left + difference.X, dot.Margin.Top + difference.Y, 0, 0);
        }

        private void changeLinePosition(Line line, Point difference)
        {
            var begin = MainWindow.pointsAddition(new Point(line.X1, line.Y1), difference);
            var end = MainWindow.pointsAddition(new Point(line.X2, line.Y2), difference);

            if (line.Name == "dashX")
            {
                line.X1 = begin.X % this.Width;
                line.X1 = line.X1 < 0 ? line.X1 + this.Width : line.X1;
                line.X2 = end.X % this.Width;
                line.X2 = line.X2 < 0 ? line.X2 + this.Width : line.X2;
                line.Y1 += difference.Y;
                line.Y2 += difference.Y;
            } else if (line.Name == "dashY")
            {
                line.Y1 = begin.Y % this.Height;
                line.Y1 = line.Y1 < 0 ? line.Y1 + this.Height : line.Y1;
                line.Y2 = end.Y % this.Height;
                line.Y2 = line.Y2 < 0 ? line.Y2 + this.Height : line.Y2;
                line.X1 += difference.X;
                line.X2 += difference.X;
            } else if (line.Name == "Graph")
            {
                line.X1 += difference.X; line.X2 += difference.X;
                line.Y1 += difference.Y; line.Y2 += difference.Y;
            } else if (line.Name == "lineY")
            {
                line.X1 += difference.X;
                line.X2 += difference.X;
            } else if (line.Name == "lineX")
            {
                line.Y1 += difference.Y;
                line.Y2 += difference.Y;
            } else if (line.X1 == line.X2)
            {
                line.X1 = begin.X % this.Width;
                line.X1 = line.X1 < 0 ? line.X1 + this.Width : line.X1;
                line.X2 = end.X % this.Width;
                line.X2 = line.X2 < 0 ? line.X2 + this.Width : line.X2;
            } else if (line.Y1 == line.Y2)
            {
                line.Y1 = begin.Y % this.Height;
                line.Y1 = line.Y1 < 0 ? line.Y1 + this.Height : line.Y1;
                line.Y2 = end.Y % this.Height;
                line.Y2 = line.Y2 < 0 ? line.Y2 + this.Height : line.Y2;
            }
        }
    }
}