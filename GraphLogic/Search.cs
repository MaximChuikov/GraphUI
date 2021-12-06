using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLogic
{
    public static class Search
    {
        public static async void DeapthFirstAlgorithm(INode startNode, INode[] nodes, int delay)
        {
            List<INode> visited = new List<INode> { startNode };
            Stack<INode> stack = new Stack<INode>();
            stack.Push(startNode);
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                var neighbours = node.GetOutputEgdes().Select(e => e.GetFromToNodes().Item2);
                foreach (var n in neighbours)
                    if (!visited.Contains(n))
                    {
                        stack.Push(n);
                        visited.Add(n);
                    }
                node.ShowPass();
                await Task.Delay(delay);
            }
            await Task.Delay(delay * 6);
            foreach (var n in visited)
                n.HidePass();
        }
        public static async void BreadFirstAlgorithm(INode startNode, INode[] nodes, int delay)
        {
            List<INode> visited = new List<INode> { startNode };
            Queue<INode> stack = new Queue<INode>();
            stack.Enqueue(startNode);
            while (stack.Count > 0)
            {
                var node = stack.Dequeue();
                var neighbours = node.GetOutputEgdes().Select(e => e.GetFromToNodes().Item2);
                foreach (var n in neighbours)
                    if (!visited.Contains(n))
                    {
                        stack.Enqueue(n);
                        visited.Add(n);
                    }
                node.ShowPass();
                await Task.Delay(delay);
            }
            await Task.Delay(delay * 6);
            foreach (var n in visited)
                n.HidePass();
        }
        public static async void FindMaximumFlow(INode startNode, INode endNode, INode[] nodes, int delay)
        {
            var allPaths = new List<Tuple<List<INode>, float>>();
            NewFlow(float.MaxValue, startNode, new());
            var bestPath = allPaths.MaxBy(x => x.Item2).Item1;
            foreach (INode n in bestPath)
            {
                n.ShowPass();
            }
            await Task.Delay(delay);
            foreach (INode n in bestPath)
            {
                n.HidePass();
            }


            async void NewFlow(float currentPrice, INode me, List<INode> visited)
            {
                if (me == endNode)
                {
                    visited.Add(me);
                    allPaths.Add(new(visited.ToList(), currentPrice));
                }
                else
                {
                    visited.Add(me);
                    var neighbours = me.GetOutputEgdes().Where(n => !visited.Contains(n.GetFromToNodes().Item2));
                    foreach (var n in neighbours)
                        NewFlow(currentPrice > n.GetPrice() ? n.GetPrice() : currentPrice, n.GetFromToNodes().Item2, visited.ToList());
                }
            }
        }
    }
}
