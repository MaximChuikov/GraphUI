using GraphLogic;
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

namespace Graph.Elements
{
    public class Node : INode
    {
        internal readonly Ellipse me;
        internal List<Edge> inMe = new ();
        internal List<Edge> fromMe = new ();
        private readonly Canvas canvas;
        private readonly List<Node> nodes;
        public Node(Canvas canvas, List<Node> nodes, MouseButtonEventArgs e)
        {
            this.canvas = canvas;
            this.nodes = nodes;
            me = new Ellipse();
            me.MouseDown += MouseDown;
            me.MouseUp += MouseUp;
            me.MouseMove += MouseMove;
            me.MouseLeave += MouseLeave;
            canvas.Children.Add(me);
            Canvas.SetLeft(me, e.GetPosition(canvas).X - me.Width / 2);
            Canvas.SetTop(me, e.GetPosition(canvas).Y - me.Height / 2);
        }
        public void ConnectTo(Node node)
        {
            var edge = new Edge(this, node, canvas);
            edge.me.X2 = Canvas.GetLeft(node.me) + node.me.Width / 2;
            edge.me.Y2 = Canvas.GetTop(node.me) + node.me.Height / 2;
            edge.me.X1 = Canvas.GetLeft(me) + me.Width / 2;
            edge.me.Y1 = Canvas.GetTop(me) + me.Height / 2;
        }
        private bool isClicked = false;
        private MouseButtonState lastLeftButtonState;
        private MouseButtonState lastRightButtonState;
        private bool isMoved = false;
        private void DeleteMe()
        {
            nodes.Remove(this);
            foreach (var edge in inMe.ToArray())
                edge.DeleteEdge();
            foreach (var edge in fromMe.ToArray())
                edge.DeleteEdge();
            canvas.Children.Remove(me);
            if (label != null)
                canvas.Children.Remove(label);
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            isClicked = true;
            lastLeftButtonState = e.LeftButton;
            lastRightButtonState = e.RightButton;
            isMoved = false;
            e.Handled = true;
        }
        private static bool isActionSelected = false;
        private static Action<INode>? doAction = null;
        private void CreateContextMenu(MouseButtonEventArgs e)
        {
            var menu = new ContextMenu();
            menu.PlacementTarget = me;
            menu.IsOpen = true;
            var item1 = new MenuItem { Header = "Запустить обход графа в ширину" };
            item1.Click += (_, _) => Search.BreadFirstAlgorithm(this, nodes.ToArray(), 500);
            var item2 = new MenuItem { Header = "Запустить обход графа в глубину" };
            item2.Click += (_, _) => Search.DeapthFirstAlgorithm(this, nodes.ToArray(), 500);
            var item3 = new MenuItem { Header = "Начать поиск максимального транспортного пути" };
            item3.Click += (_, _) =>
            {
                isActionSelected = true;
                doAction = (INode secondNode) => Search.FindMaximumFlow(this, secondNode, nodes.ToArray(), 2000);
            };
            var item4 = new MenuItem { Header = "Удалить вершину" };
            item4.Click += (_, _) => DeleteMe();
            menu.Items.Add(item1);
            menu.Items.Add(item2);
            menu.Items.Add(item3);
            menu.Items.Add(item4);
        }
        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isClicked && !isMoved)
            {
                if (lastRightButtonState.HasFlag(MouseButtonState.Pressed))
                {
                    CreateContextMenu(e);
                    e.Handled = true;
                }
                else if (lastLeftButtonState.HasFlag(MouseButtonState.Pressed))
                {
                    if (isActionSelected)
                    {
                        isActionSelected = false;
                        if (doAction != null)
                            doAction(this);
                        doAction = null;
                        e.Handled = true;
                    }
                    else
                        e.Handled = false;
                }

            }
            else
                e.Handled = true;
            isClicked = false;
            isMoved = false;
        }
        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (isClicked)
            {
                isMoved = true;
                Point newMousePosition = e.GetPosition(canvas);
                double x = newMousePosition.X;
                double y = newMousePosition.Y;
                Canvas.SetLeft(me, x - me.Width / 2);
                Canvas.SetTop(me, y - me.Height / 2);

                foreach(var line in inMe)
                {
                    line.me.X2 = x;
                    line.me.Y2 = y;
                    line.RedrawLabel();
                }
                foreach (var line in fromMe)
                {
                    line.me.X1 = x;
                    line.me.Y1 = y;
                    line.RedrawLabel();
                }

                if (label != null)
                {
                    Canvas.SetLeft(label, Canvas.GetLeft(me));
                    Canvas.SetTop(label, Canvas.GetTop(me));
                }
            }
            e.Handled= true;
        }
        private void MouseLeave(object sender, MouseEventArgs e)
        {
            isClicked = false;
            isMoved = false;
            e.Handled = true;
        }
        Brush? lastColor;
        public void ShowPass()
        {
            lastColor = me.Fill;
            me.Fill = Brushes.IndianRed;
        }
        public void HidePass()
        {
            if (lastColor != null)
                me.Fill = lastColor;
        }
        private Label? label;
        public void ShowValue(float val)
        {
            label = new Label { Content = val, Width = me.Width, Height = me.Height, HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center, FontSize = 20, Background = Brushes.Transparent };
            Panel.SetZIndex(label, 5);
            Canvas.SetLeft(label, Canvas.GetLeft(me));
            Canvas.SetTop(label, Canvas.GetTop(me));
            canvas.Children.Add(label);
        }
        public void HideValue()
        {
            canvas.Children.Remove(label);
            label = null;
        }
        public IEdge[] GetOutputEgdes() => fromMe.ToArray();
    }
}
