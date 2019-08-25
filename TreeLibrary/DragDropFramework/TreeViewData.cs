using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TreeLibrary.Extensions;
using TreeLibrary.Model;

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
            var treeNodeList = ((this.SourceContainer as TreeView).DataContext as NodeItem.BaseItem.TreeHelper)
                .NodeList;
            if (treeNodeList == null)
                return;

            TreeNodeModel removeItem = this.SourceObject as TreeNodeModel;
            RemoveItemByNodeModel(removeItem, treeNodeList);
        }

        private bool RemoveItemByNodeModel(TreeNodeModel removeItem, ObservableCollection<TreeNodeModel> treeNodeList)
        {
            if (treeNodeList.Contains(removeItem))
            {
                treeNodeList.Remove(removeItem);
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
            this.DragOverOrDrop(false, sender, e);
        }

        public override void DropTarget_DragOver(object sender, DragEventArgs e)
        {
            this.DragOverOrDrop(false, sender, e);
        }

        public override void DropTarget_Drop(object sender, DragEventArgs e)
        {
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
                Debug.Assert(dragSourceObject != null);

                TContainer dropContainer =
                    Utilities.FindParentControlIncludingMe<TContainer>(sender as DependencyObject);
                Debug.Assert(dropContainer != null);
                TObject dropTarget = e.Source as TObject;

                if (dropTarget == null)
                {
                    if (bDrop)
                    {
                        dataProvider.Unparent();

                        if ((e.OriginalSource as Grid) != null &&
                            (e.OriginalSource as Grid).DataContext as TreeLibrary.NodeItem.BaseItem.TreeHelper !=
                            null && (e.Source as TreeView) != null)
                        {
                            ((e.Source as TreeView).DataContext as NodeItem.BaseItem.TreeHelper).NodeList.Add(
                                dragSourceObject);
                            return;
                        }

                        if ((e.OriginalSource as FrameworkElement) != null)
                        {
                            ((e.OriginalSource as FrameworkElement).DataContext as TreeNodeModel).AddSubNode(
                                dragSourceObject);
                            return;
                        }

                        if ((e.OriginalSource as Image) != null)
                        {
                            ((e.OriginalSource as Image).DataContext as TreeNodeModel).AddSubNode(dragSourceObject);
                        }


                        if ((e.OriginalSource as TextBlock) == null ||
                            ((e.OriginalSource as TextBlock).DataContext as TreeNodeModel) == null)
                            return;
                        ((e.OriginalSource as TextBlock).DataContext as TreeNodeModel).AddSubNode(dragSourceObject);
                        //dropContainer.Items.Add(dragSourceObject);
                    }

                    e.Effects = DragDropEffects.Move;
                    e.Handled = true;
                }
                else
                {
                    // bool IsAncestor = dragSourceObject.IsAncestorOf(dropTarget);
                    if ((dataProvider.KeyStates & DragDropKeyStates.ShiftKey) != 0)
                    {
                        ItemsControl shiftDropTarget = Utilities.FindParentControlExcludingMe<ItemsControl>(dropTarget);
                        Debug.Assert(shiftDropTarget != null);
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
    }
}