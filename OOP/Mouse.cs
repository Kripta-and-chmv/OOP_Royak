using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    class Mouse
    {
        private Point position;
        private bool isPushed = false;
        private bool isMoveMode = true;
        private bool isDrawing = false;

        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool IsPushed
        {
            get { return isPushed; }
            set { isPushed = value; }
        }

        public bool IsMoveMode
        {
            get { return isMoveMode; }
            set { isMoveMode = value; }
        }

        public bool IsDrawing
        {
            get { return isDrawing; }
            set { isDrawing = value; }
        }
    }
}
