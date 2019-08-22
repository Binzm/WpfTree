using System.Collections.Specialized;
using System.Windows.Controls;
using TreeLibrary.Args;

namespace TreeLibrary.Delegate
{
    public delegate void NodeDoubleClickHandler(object sender, NodeDoubleClickArgs e);

    public delegate void NodeSelectedHandler(object sender, NodeSelectedArgs e);

    public delegate void NodeRightButtonSelectedHandler(object sender, NodeSelectedArgs e);

    public delegate void NodeCheckboxChangeHandler(object sender, NodeSelectedArgs e);

    public delegate void NodeOperatorHandler(object sender, NotifyCollectionChangedEventArgs e);

    public delegate void ResourceTreeDataLoadedHandler(object sender, bool IsScussed);

    public delegate void SearchContentSelectChangedHandler(object sender, SelectionChangedEventArgs e);
}