using System.Windows;
using TreeLibrary.Model;

namespace TreeLibrary.NodeModel
{
    public class TwosTreeNodeModel : TreeNodeModel
    {
        public TwosTreeNodeModel()
        {
            this.IconImage = this.GetType().Name;
        }
    }
}