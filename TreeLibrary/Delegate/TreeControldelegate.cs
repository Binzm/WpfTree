using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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


    public delegate void DragOverHandler(object sender, DragEventArgs e);

    public delegate void PreviewMouseMoveHandler(object sender, MouseEventArgs e);

    public delegate void MouseLeaveHandler(object sender, MouseEventArgs e);

    public delegate void PreviewMouseUpHandler(object sender, MouseButtonEventArgs e);


    public delegate void DropDragHandler(bool bDrop, object sender, DragEventArgs e);
}