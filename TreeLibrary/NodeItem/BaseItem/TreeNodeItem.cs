using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TreeLibrary.Args;
using TreeLibrary.Delegate;
using TreeLibrary.Model;

namespace TreeLibrary.NodeItem.BaseItem
{
    public class TreeNodeItem : System.Windows.Controls.Control
    {
        public readonly static DependencyProperty ModelProperty;

        public readonly static DependencyProperty TextProperty;

        public readonly static DependencyProperty IsCheckedProperty;

        public readonly static DependencyProperty IsSelectedProperty;

        public readonly static DependencyProperty IsExpandedProperty;

        //public readonly static DependencyProperty IsShowMenuProperty;

       

        //private static TreeNodeItem selectedNode;
        private static TreeNodeItem selectedRightNode;

       


        /// <summary>
        /// 树节点控件的复选框
        /// </summary>
        private CheckBox _NodeCheckBox = null;

        public bool IsExpanded
        {
            get => (bool) base.GetValue(TreeNodeItem.IsExpandedProperty);
            set => base.SetValue(TreeNodeItem.IsExpandedProperty, value);
        }

        public bool IsSelected
        {
            get => (bool) base.GetValue(TreeNodeItem.IsSelectedProperty);
            set => base.SetValue(TreeNodeItem.IsSelectedProperty, value);
        }

        //public bool IsShowMenu
        //{
        //    get => (bool) base.GetValue(TreeNodeItem.IsShowMenuProperty);
        //    set => base.SetValue(TreeNodeItem.IsShowMenuProperty, value);
        //}

        public TreeNodeModel Model
        {
            get => (TreeNodeModel) base.GetValue(TreeNodeItem.ModelProperty);
            set => base.SetValue(TreeNodeItem.ModelProperty, value);
        }

        public string Text
        {
            get => (string) base.GetValue(TreeNodeItem.TextProperty);
            set => base.SetValue(TreeNodeItem.TextProperty, value);
        }

        public Boolean? IsChecked
        {
            get => (Boolean) base.GetValue(TreeNodeItem.IsCheckedProperty);
            set => base.SetValue(TreeNodeItem.IsCheckedProperty, value);
        }


        static TreeNodeItem()
        {
            TreeNodeItem.ModelProperty = DependencyProperty.Register("Model", typeof(TreeNodeModel),
                typeof(TreeNodeItem), new PropertyMetadata(null));
            TreeNodeItem.TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TreeNodeItem),
                new PropertyMetadata(string.Empty));
            TreeNodeItem.IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool),
                typeof(TreeNodeItem), new PropertyMetadata(false));
            TreeNodeItem.IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool),
                typeof(TreeNodeItem), new PropertyMetadata(false));
            TreeNodeItem.IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool),
                typeof(TreeNodeItem), new PropertyMetadata(false));
            //TreeNodeItem.IsShowMenuProperty = DependencyProperty.Register("IsShowMenu", typeof(bool),
            //    typeof(TreeNodeItem), new PropertyMetadata(false));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeNodeItem),
                new FrameworkPropertyMetadata(typeof(TreeNodeItem)));
        }

        /// <summary>
        /// 应用模版的处理
        /// </summary>
        public override void OnApplyTemplate()
        {

            #region Node_CheckBox的check值改变事件（单击）

            this._NodeCheckBox = base.Template.FindName("Node_CheckBox", this) as CheckBox;
            if (this._NodeCheckBox != null)
            {
                _NodeCheckBox.Click += this.NodeCheckBox_Clicked;
            } 
            #endregion

        }


        private static void NodeStateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TreeNodeItem treeNodeItem = sender as TreeNodeItem;
            if (treeNodeItem == null)
            {
                return;
            }

            treeNodeItem.StateChanged();
        }

        protected virtual void StateChanged()
        {
        }

        protected void UIElement_DoubleClick(MouseButtonEventArgs e)
        {
            var args = new NodeDoubleClickArgs(NodeDoubleClickEvent, this.Model);
            this.RaiseEvent(args);
        }

        protected void UIElement_Click(MouseButtonEventArgs e)
        {
            this.IsSelected = true;
            var args = new NodeSelectedArgs(NodeSelectedEvent, this.Model, true);
            this.RaiseEvent(args);
        }


        //树节点的复选框的单击事件处理
        protected void NodeCheckBox_Clicked(object sender, RoutedEventArgs e)
        {
            var argsNodeCheckBox = new NodeSelectedArgs(NodeCheckBoxCheckEvent, this.Model, true);
            this.RaiseEvent(argsNodeCheckBox);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                var timer = new Timer(500) {AutoReset = false};
                timer.Elapsed += new ElapsedEventHandler((o, ex) => this.Dispatcher.Invoke(new Action(() =>
                {
                    var timer2 = (Timer) this.Tag;
                    timer2.Stop();
                    timer2.Dispose();
                    UIElement_Click(e);
                })));
                timer.Start();
                this.Tag = timer;
            }

            if (e.ClickCount > 1)
            {
                var timer = this.Tag as Timer;
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    UIElement_DoubleClick(e);
                }
            }
        }


        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }

            this.IsSelected = true;

            TreeNodeItem.selectedRightNode = this;


            var rightButtonSelectedArgs = new NodeSelectedArgs(NodeRightButtonSelectedEvent, this.Model, true);
            this.RaiseEvent(rightButtonSelectedArgs);

            var argsNodeSelectedArgs = new NodeSelectedArgs(NodeSelectedEvent, this.Model, true);
            this.RaiseEvent(argsNodeSelectedArgs);
            //NodeRightButtonSelectedHandler nodeSelectedHandler = this.NodeRightButtonSelected;
            //if (nodeSelectedHandler == null)
            //{
            //    return;
            //}

            // nodeSelectedHandler(this, new NodeSelectedArgs(this.Model, true));
        }



        public static readonly RoutedEvent NodeSelectedEvent = EventManager.RegisterRoutedEvent("NodeSelected",
            RoutingStrategy.Bubble, typeof(NodeSelectedHandler), typeof(TreeNodeItem));
        //public event NodeSelectedHandler NodeSelected
        //{
        //    add => this.AddHandler(TreeNodeItem.NodeSelectedEvent, (System.Delegate) value, false);
        //    remove => this.RemoveHandler(TreeNodeItem.NodeSelectedEvent, (System.Delegate) value);
        //}


        public static readonly RoutedEvent NodeDoubleClickEvent = EventManager.RegisterRoutedEvent(
            "NodeDoubleClick",
            RoutingStrategy.Bubble, typeof(NodeDoubleClickHandler), typeof(TreeNodeItem));
        //public event NodeDoubleClickHandler NodeDoubleClickProperty
        //{
        //    add => AddHandler(NodeDoubleClickEvent, value);
        //    remove => RemoveHandler(NodeDoubleClickEvent, value);
        //}

        public static readonly RoutedEvent NodeRightButtonSelectedEvent = EventManager.RegisterRoutedEvent(
            "NodeRightButtonSelected",
            RoutingStrategy.Bubble, typeof(NodeSelectedHandler), typeof(TreeNodeItem));
        //public event NodeSelectedHandler NodeRightButtonSelected
        //{
        //    add => AddHandler(NodeRightButtonSelectedEvent, value);
        //    remove => RemoveHandler(NodeRightButtonSelectedEvent, value);
        //}

        public static readonly RoutedEvent NodeCheckBoxCheckEvent =
            EventManager.RegisterRoutedEvent("NodeCheckBoxCheck", RoutingStrategy.Bubble, typeof(NodeSelectedHandler),
                typeof(TreeNodeItem));
        

    }
}