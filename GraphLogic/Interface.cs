using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLogic
{
    public interface INode
    {
        public void ShowPass();
        public void HidePass();
        public void ShowValue(float val);
        public void HideValue();
        public IEdge[] GetOutputEgdes();
    }
    public interface IEdge
    {
        public void ShowValue(float val);
        public float GetPrice();
        public Tuple<INode, INode> GetFromToNodes();
    }
}
