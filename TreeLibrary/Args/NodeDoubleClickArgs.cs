using System.Windows;
using TreeLibrary.Model;

namespace TreeLibrary.Args
{
    public class NodeDoubleClickArgs : RoutedEventArgs
    {
        public TreeNodeModel NodeModel { get; set; }

        public NodeDoubleClickArgs(RoutedEvent routedEvent, TreeNodeModel nodeModel) : base(routedEvent, nodeModel)
        {
            this.NodeModel = nodeModel;
        }
    }
}