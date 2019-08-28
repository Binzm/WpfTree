using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using TreeLibrary.Delegate;
using TreeLibrary.Extensions;
using TreeLibrary.Model;
using TreeLibrary.NodeItem.BaseItem;

namespace TreeLibrary.DragDropFramework
{
    /// <summary>
    /// This Data Provider represents TreeViewItems.
    /// 
    /// Note that a TreeViewItem's container can be
    /// either a TreeView or another TreeViewItem.
    /// </summary>
    /// <typeparam name="TContainer">Drag source container type</typeparam>
    /// <typeparam name="TObject">Drag source object type</typeparam>
    public class TreeViewDataProvider<TContainer, TObject> : DataProviderBase<TContainer, TObject>, IDataProvider
        where TContainer : ItemsControl
        where TObject : ItemsControl
    {
        public TreeViewDataProvider(string dataFormatString)
            : base(dataFormatString)
        {
        }

        public override DragDropEffects AllowedEffects
        {
            get =>
                DragDropEffects.Move | // Move TreeItem
                DragDropEffects.Link | // Move TreeItem as sibling
                DragDropEffects.None;
        }

        public override DataProviderActions DataProviderActions
        {
            get =>
                DataProviderActions.QueryContinueDrag | // Need Shift key info
                DataProviderActions.GiveFeedback |
                DataProviderActions.None;
        }

        public override void DragSource_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (e.Effects == DragDropEffects.Move)
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
            else if (e.Effects == DragDropEffects.Link)
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
        }

        public override void Unparent()
        {
            if ((this.SourceContainer as TreeView) == null)
                return;

            var treeNodeList = ((TreeHelper) ((TreeView) this.SourceContainer).DataContext)?.NodeList;
            if (treeNodeList == null)
                return;

            TreeNodeModel removeItem = this.SourceObject as TreeNodeModel;
            RemoveItemByNodeModel(removeItem, treeNodeList);
        }

        private static bool RemoveItemByNodeModel(TreeNodeModel removeItem,
            ObservableCollection<TreeNodeModel> treeNodeList)
        {
            if (treeNodeList.Contains(removeItem))
            {
                treeNodeList.Remove(removeItem);
                removeItem.Parent = null;
                return true;
            }

            foreach (var itemNodeModel in treeNodeList)
            {
                if (itemNodeModel.SubNodes.Count > 0)
                {
                    RemoveItemByNodeModel(removeItem, itemNodeModel.SubNodes);
                }
            }

            return false;
        }
    }

    /// <summary>
    /// This data consumer looks for TreeViewItems.
    /// The TreeViewItem is added as either a sibling or
    /// a child, depending on the state of the Shift key.
    /// </summary>
    /// <typeparam name="TContainer">Drag source and drop destination container type</typeparam>
    /// <typeparam name="TObject">Drag source and drop destination object type</typeparam>
    public class TreeViewDataConsumer<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : ItemsControl
        where TObject : ItemsControl
    {
        public TreeViewDataConsumer(string[] dataFormats)
            : base(dataFormats)
        {
        }

        public override DataConsumerActions DataConsumerActions
        {
            get => DataConsumerActions.DragEnter |
                   DataConsumerActions.DragOver |
                   DataConsumerActions.Drop |
                   DataConsumerActions.None;
        }

        public override void DropTarget_DragEnter(object sender, DragEventArgs e)
        {
            DropDragHandler?.Invoke(false, sender, e);
            this.DragOverOrDrop(false, sender, e);
        }

        public override void DropTarget_DragOver(object sender, DragEventArgs e)
        {
            DropDragHandler?.Invoke(false, sender, e);
            this.DragOverOrDrop(false, sender, e);
        }

        public override void DropTarget_Drop(object sender, DragEventArgs e)
        {
            DropDragHandler?.Invoke(true, sender, e);
            this.DragOverOrDrop(true, sender, e);
        }


        /// <summary>
        /// First determine whether the drag data is supported.
        /// Finally handle the actual drop when <code>bDrop</code> is true.
        /// Add the item as the drop target's child when Shift is not pressed,
        /// or insert the item before the drop target when Shift is pressed.
        /// When there is no drop target (dropped on empty space),
        /// add to the end of the items.
        /// 
        /// Note that the source object cannot be an ancestor of the drop target.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e)
        {
            TreeViewDataProvider<TContainer, TObject> dataProvider =
                this.GetData(e) as TreeViewDataProvider<TContainer, TObject>;
            if (dataProvider != null)
            {
                TContainer dragSourceContainer = dataProvider.SourceContainer as TContainer;
                TreeNodeModel dragSourceObject = dataProvider.SourceObject as TreeNodeModel;

                Debug.Assert(dragSourceContainer != null);
                //Debug.Assert(dragSourceObject != null);

                TContainer dropContainer =
                    Utilities.FindParentControlIncludingMe<TContainer>(sender as DependencyObject);
                Debug.Assert(dropContainer != null);
                TObject dropTarget = e.Source as TObject;

                if (dropTarget == null)
                {
                    if (bDrop)
                    {
                        if (IsRejectRemove(dragSourceObject, e.OriginalSource))
                            return;
                        dataProvider.Unparent();

                        if ((e.OriginalSource as Grid) != null)
                        {
                            ((TreeHelper) ((Grid) e.OriginalSource).DataContext)?.NodeList.Add(
                                dragSourceObject);
                            e.Effects = DragDropEffects.Move;
                            e.Handled = true;
                            return;
                        }

                        if ((e.OriginalSource as FrameworkElement) != null)
                        {
                            ((TreeNodeModel) ((FrameworkElement) e.OriginalSource).DataContext)?.AddSubNode(
                                dragSourceObject);
                            return;
                        }
                    }

                    e.Effects = DragDropEffects.Move;
                    e.Handled = true;
                }
                else
                {
                    // bool IsAncestor = dragSourceObject.IsAncestorOf(dropTarget);
                    if ((dataProvider.KeyStates & DragDropKeyStates.ShiftKey) != 0)
                    {
                        //ItemsControl shiftDropTarget = Utilities.FindParentControlExcludingMe<ItemsControl>(dropTarget);
                        //Debug.Assert(shiftDropTarget != null);
                        //if (!IsAncestor)
                        //{
                        //    if (bDrop)
                        //    {
                        //        dataProvider.Unparent();
                        //        Debug.Assert(shiftDropTarget != null);
                        //        shiftDropTarget.Items.Insert(shiftDropTarget.Items.IndexOf(dropTarget),
                        //            dragSourceObject);
                        //    }

                        //    e.Effects = DragDropEffects.Link;
                        //    e.Handled = true;
                        //}
                        //else
                        //{
                        //    e.Effects = DragDropEffects.None;
                        //    e.Handled = true;
                        //}
                    }
                    else
                    {
                        //if (!IsAncestor && (dragSourceObject != dropTarget))
                        //{
                        //    if (bDrop)
                        //    {
                        //        dataProvider.Unparent();
                        //        dropTarget.Items.Add(dragSourceObject);
                        //    }

                        //    e.Effects = DragDropEffects.Move;
                        //    e.Handled = true;
                        //}
                        //else
                        //{
                        //    e.Effects = DragDropEffects.None;
                        //    e.Handled = true;
                        //}
                    }
                }

                if (bDrop && e.Handled && (e.Effects != DragDropEffects.None))
                {
                    dragSourceObject.IsSelected = true;
                    //dragSourceObject.BringIntoView();
                }
            }
        }

        private bool IsRejectRemove(TreeNodeModel dragSourceObject, object originalSource)
        {
            try
            {
                if (dragSourceObject == null)
                    return false;
                if ((originalSource as FrameworkElement) == null)
                    return false;
                if ((((FrameworkElement) originalSource).DataContext as TreeHelper)!=null)
                    return false;

                var treeNodeModel = ((TreeNodeModel) ((FrameworkElement) originalSource).DataContext);
                if (dragSourceObject == treeNodeModel)
                    return true;

                if (dragSourceObject.SubNodes.Count <= 0)
                    return false;

                return IsDragDropSubNode(dragSourceObject.SubNodes, treeNodeModel);
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        private bool IsDragDropSubNode(ObservableCollection<TreeNodeModel> subNodes, TreeNodeModel dragSourceObject)
        {
            foreach (var nodeItem in subNodes)
            {
                if (nodeItem == dragSourceObject)
                    return true;
                if (nodeItem.SubNodes.Count > 0)
                    if (IsDragDropSubNode(nodeItem.SubNodes, dragSourceObject))
                        return true;
            }

            return false;
        }

        public event DropDragHandler DropDragHandler;
    }
}