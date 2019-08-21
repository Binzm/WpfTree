using System.Windows;
using TreeLibrary.Model;

namespace TreeLibrary.Args
{
    public class NodeSelectedArgs : RoutedEventArgs
    {
        public bool IsSelected { get; set; }

        public TreeNodeModel NodeModel { get; set; }

        public NodeSelectedArgs(RoutedEvent routedEvent, TreeNodeModel nodeModel, bool isSelected) : base(routedEvent,
            nodeModel)
        {
            this.NodeModel = nodeModel;
            this.IsSelected = isSelected;
        }
    }
}