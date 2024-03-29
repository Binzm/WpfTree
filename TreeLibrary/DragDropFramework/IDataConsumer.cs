﻿using System;
using System.Windows;

namespace TreeLibrary.DragDropFramework
{
    public interface IDataConsumer
    {
        DataConsumerActions DataConsumerActions { get; }

        void DropTarget_DragEnter(object sender, DragEventArgs e);
        void DropTarget_DragOver(object sender, DragEventArgs e);
        void DropTarget_Drop(object sender, DragEventArgs e);
        void DropTarget_DragLeave(object sender, DragEventArgs e);
    }

    /// <summary>
    /// Actions that can be defined by a data consumer
    /// </summary>
    [Flags]
    public enum DataConsumerActions
    {
        DragEnter = 0x01,
        DragOver = 0x02,
        Drop = 0x04,
        DragLeave = 0x08,

        None = 0x00,

        AllowDropMask = DragEnter | DragOver | Drop | DragLeave,
    }

    public class DropManager
    {
        private FrameworkElement _dropTarget;
        private IDataConsumer[] _dragDropConsumers;


        /// <summary>
        /// Manage data that is dragged over and dropped on the <code>dropTarget</code>.
        /// Supported data is defined as one or more classes that implement IDataConsumer.
        /// </summary>
        /// <param name="dropTarget">FrameworkElement monitored for drag events</param>
        /// <param name="dragDropConsumers">Array of supported data objects</param>
        public DropManager(FrameworkElement dropTarget, IDataConsumer[] dragDropConsumers)
        {
            this._dropTarget = dropTarget;
            System.Diagnostics.Debug.Assert(dropTarget != null);

            this._dragDropConsumers = dragDropConsumers;
            System.Diagnostics.Debug.Assert(dragDropConsumers != null);

            bool hookDragEnter = false;
            bool hookDragOver = false;
            bool hookDrop = false;
            bool hookDragLeave = false;

            // Determine which events to hook
            foreach (IDataConsumer dragDropConsumer in this._dragDropConsumers)
            {
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.DragEnter) != 0)
                    hookDragEnter = true;
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.DragOver) != 0)
                    hookDragOver = true;
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.Drop) != 0)
                    hookDrop = true;
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.DragLeave) != 0)
                    hookDragLeave = true;
            }

            if ((hookDragEnter == true) || (hookDragOver == true) || (hookDrop == true) || (hookDragLeave == true))
                this._dropTarget.AllowDrop = true;

            // Hook only the events needed
            if (hookDragEnter == true)
                this._dropTarget.DragEnter += new DragEventHandler(this.DropTarget_DragEnter);
            if (hookDragOver == true)
                this._dropTarget.DragOver += new DragEventHandler(this.DropTarget_DragOver);
            if (hookDrop == true)
                this._dropTarget.Drop += new DragEventHandler(this.DropTarget_Drop);
            if (hookDragLeave == true)
                this._dropTarget.DragLeave += new DragEventHandler(this.DropTarget_DragLeave);
        }

        /// <summary>
        /// Initial call, after DoDragDrop is called, has Effects and AllowedEffects set to
        /// allowedEffects as passed to DoDragDrop.  Subsequent Effects and AllowedEffects
        /// are set to the Effects returned by DragLeave.  Note that DragLeave can return
        /// effects that are not defined in allowedEffects (as passed to DoDragDrop).
        /// Source and Original source are set to dragSource as passed to DoDragDrop.
        /// </summary>
        private void DropTarget_DragEnter(object sender, DragEventArgs e)
        {
            foreach (IDataConsumer dragDropConsumer in this._dragDropConsumers)
            {
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.DragEnter) != 0)
                {
                    dragDropConsumer.DropTarget_DragEnter(sender, e);
                    if (e.Handled)
                        break;
                }
            }

            if (e.Handled)
                return;

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when mouse is over the area occupied
        /// by the dropTarget (specified in the constructor).
        /// You must likely will provide your own method; make sure
        /// to define DragOver in DataConsumerActions.
        /// </summary>
        private void DropTarget_DragOver(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException("DragOver not implemented");
            foreach (IDataConsumer dragDropConsumer in this._dragDropConsumers)
            {
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.DragOver) != 0)
                {
                    dragDropConsumer.DropTarget_DragOver(sender, e);
                    if (e.Handled)
                        break;
                }
            }

            if (e.Handled)
                return;

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the left mouse button is released in the area
        /// occupied by the dropTarget (specified in the constructor).
        /// You must likely will provide your own method; make sure
        /// to define Drop in DataConsumerActions.
        /// 
        /// See DropTarget_DragEnter in DropManager for additional comments.
        /// </summary>
        private void DropTarget_Drop(object sender, DragEventArgs e)
        {
            foreach (IDataConsumer dragDropConsumer in this._dragDropConsumers)
            {
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.Drop) != 0)
                {
                    dragDropConsumer.DropTarget_Drop(sender, e);
                    if (e.Handled)
                        break;
                }
            }

            if (e.Handled)
                return;

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>
        /// Retured effects are passed to *_DragEnter in both Effects and AllowedEffects;
        /// even effects not included in DoDragDrop's allowedEffects can be used.
        /// </summary>
        private void DropTarget_DragLeave(object sender, DragEventArgs e)
        {
            foreach (IDataConsumer dragDropConsumer in this._dragDropConsumers)
            {
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.DragLeave) != 0)
                {
                    dragDropConsumer.DropTarget_DragLeave(sender, e);
                    if (e.Handled)
                        break;
                }
            }

            if (e.Handled)
                return;

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
    }
}