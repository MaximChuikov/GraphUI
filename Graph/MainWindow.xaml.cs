using Graph.Elements;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Graph
{
    public partial class MainWindow : Window
    {
        public List<Node> nodes = new List<Node>();
        public MainWindow()
        {
            InitializeComponent();
        }
        private Node? requestToConnect;
        private Line? tempLine;
        private void DeleteTempLine()
        {
            if (tempLine != null)
            {
                canvas.Children.Remove(tempLine);
                requestToConnect = null;
                tempLine = null;
            }
        }
        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Ellipse)
            {
                Ellipse ellipse = (Ellipse)e.Source;
                if (requestToConnect == null)
                {
                    tempLine = new Line
                    {
                        X1 = Canvas.GetLeft(ellipse) + ellipse.Width / 2,
                        Y1 = Canvas.GetTop(ellipse) + ellipse.Height / 2,
                        X2 = Canvas.GetLeft(ellipse) + ellipse.Width / 2,
                        Y2 = Canvas.GetTop(ellipse) + ellipse.Height / 2
                    };
                    canvas.Children.Add(tempLine);
                    requestToConnect = nodes.Where(n => n.me == ellipse).First();
                }
                else
                {
                    var toConnect = nodes.Where(n => n.me == ellipse).First();
                    if (toConnect != requestToConnect)
                        requestToConnect.ConnectTo(toConnect);
                    DeleteTempLine();
                }
            }
            else if (sender is Action)
            {

            }
            else
            {
                if (tempLine == null)
                {
                    var node = new Node(canvas, nodes, e);
                    nodes.Add(node);
                }
                else
                    DeleteTempLine();
            }
        }
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (tempLine != null)
            {
                tempLine.X2 = e.GetPosition(canvas).X;
                tempLine.Y2 = e.GetPosition(canvas).Y;
            }
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            DeleteTempLine();
        }
    }
}
