using System.Windows;
using TreeLibrary.Model;

namespace TreeLibrary.NodeModel
{
    public class ChannelsTreeNodeModel : TreeNodeModel
    {
        public ChannelsTreeNodeModel(Visibility visibility = Visibility.Visible)
        {
            this.IconImage = this.GetType().Name;
            this.ShowCheckBox = visibility;
        }
    }
}