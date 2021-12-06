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
    public class Edge : IEdge
    {
        private Node fromNode;
        private Node toNode;
        public Line me;
        private readonly Canvas canvas;
        public float Price { get; set; }
        public Edge(Node from, Node to, Canvas canvas, float price = 1)
        {
            this.fromNode = from;
            this.toNode = to;
            this.Price = price;
            this.canvas = canvas;
            me = new Line();
            me.MouseDown += Me_MouseDown;
            me.MouseUp += Me_MouseUp;
            CreateEdge();
        }
        private bool isClicked = false;
        private MouseButtonState lastLeftState;
        private MouseButtonState lastRightState;
        private void Me_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastLeftState = e.LeftButton;
            lastRightState = e.RightButton;
            isClicked = true;
        }
        private void Me_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isClicked)
            {
                if (lastLeftState.HasFlag(MouseButtonState.Pressed))
                {
                    if (label == null)
                        CreateLabel();

                }
                else if (lastRightState.HasFlag(MouseButtonState.Pressed))
                {
                    DeleteEdge();
                }
                e.Handled = true;
            }
            else
                e.Handled = false;

            isClicked = false;
        }

        private void CreateEdge()
        {
            fromNode.fromMe.Add(this);
            toNode.inMe.Add(this);
            canvas.Children.Add(me);
        }
        public void DeleteEdge()
        {
            fromNode.fromMe.Remove(this);
            toNode.inMe.Remove(this);
            canvas.Children.Remove(me);
            if (label != null)
                canvas.Children.Remove(label);
        }
        private Label? label;
        private void CreateLabel()
        {
            label = new Label
            {
                Content = Price,
                Width = 50,
                Height = 40,
                FontSize = 20,
                Cursor = Cursors.Hand
            };
            RedrawLabel();
            label.MouseDown += (object sender, MouseButtonEventArgs e) =>
            {
                if (e.LeftButton.HasFlag(MouseButtonState.Pressed))
                    Price++;
                else if (e.RightButton.HasFlag(MouseButtonState.Pressed))
                    Price--;
                RedrawLabel();
                e.Handled = true;
            };
            label.MouseUp += (object sender, MouseButtonEventArgs e) => e.Handled = true;
            canvas.Children.Add(label);
        }
        internal void RedrawLabel()
        {
            if (label != null)
            {
                Canvas.SetLeft(label, (me.X1 + me.X2) / 2 - label.Width / 2);
                Canvas.SetTop(label, (me.Y1 + me.Y2) / 2 - label.Height - 5);
                if (me.X2 - me.X1 > 0)
                    label.Content = $"{Price} >";
                else
                    label.Content = $"{Price} <";

            }
        }
        public void ShowValue(float val)
        {
            if (label == null)
                CreateLabel();
        }
        public Tuple<INode, INode> GetFromToNodes() => new Tuple<INode, INode>(fromNode, toNode);
        public float GetPrice() => Price;
    }
}
