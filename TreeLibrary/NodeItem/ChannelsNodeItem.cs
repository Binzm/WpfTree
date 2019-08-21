using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TreeLibrary.Args;
using TreeLibrary.NodeItem.BaseItem;

namespace TreeLibrary.NodeItem
{
    public class ChannelsNodeItem : TreeNodeItem
    {
        private Grid _parthGrid;

        protected bool IsDraging = false;
        protected Point DragStartPoint;

        public static DependencyProperty ExitImgProperty;

        public string ExitImg
        {
            get => (string)base.GetValue(ChannelsNodeItem.ExitImgProperty);
            set => base.SetValue(ChannelsNodeItem.ExitImgProperty, value);
        }

        static ChannelsNodeItem()
        {
            TwosNodeItem.ExitImgProperty = DependencyProperty.Register("ExitImg", typeof(ChannelsNodeItem),
                typeof(ChannelsNodeItem), new PropertyMetadata(null));
        }

        public ChannelsNodeItem()
        {

        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._parthGrid = base.Template.FindName("PART_Grid", this) as Grid;
            if (this._parthGrid != null)
            {
                this._parthGrid.MouseMove += this.PART_Grid_MouseMove;
                this._parthGrid.MouseLeftButtonDown += this.PART_Grid_MouseLeftButtonDown;
            }
        }



        private void PART_Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragStartPoint = e.GetPosition(null);
        }

        private void PART_Grid_MouseMove(object sender, MouseEventArgs e)
        {
            var lv = sender as Grid;
            if (e.LeftButton == MouseButtonState.Pressed && !IsDraging)
            {
                if (e.LeftButton == MouseButtonState.Pressed && !IsDraging)
                {
                    Point position = e.GetPosition(null);

                    if (Math.Abs(position.X - DragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(position.Y - DragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        IsDraging = true;
                        DragDropEffects de = DragDrop.DoDragDrop(lv, new DragDropArgs(base.Model), DragDropEffects.Copy); //zjm
                        IsDraging = false;
                    }
                }
            }
        }
    }
}
