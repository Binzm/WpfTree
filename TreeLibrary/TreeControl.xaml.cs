﻿using ICMS.WPFControlsLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TreeLibrary.Args;
using TreeLibrary.Delegate;
using TreeLibrary.Model;
using TreeLibrary.NodeItem.BaseItem;

namespace TreeLibrary
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(TreeControl))]
    public partial class TreeControl : UserControl, INotifyPropertyChanged
    {
        // private ObservableCollection<TreeNodeModel> _treeNodeModels;

        private Dictionary<DataTemplateKey, HierarchicalDataTemplate> _hierarchicalDataTemplateResources;

        private Dictionary<Type, Type> _modelTyeAndItemTypeDictionary;

        private readonly Dictionary<RoutedEvent, System.Delegate> _currentTreeRoutedAddHandler;
        private readonly Dictionary<RoutedEvent, System.Delegate> _currentTreeLibraryControlRoutedAddHandler;
        private readonly Dictionary<string, Dictionary<RoutedEvent, System.Delegate>> _menuRouteAndHandlerDictionary;

        public TreeHelper TreeHelper = new TreeHelper();


        public static readonly DependencyProperty AutoCompleteIsShowProperty;
        public static readonly DependencyProperty AllowTreeDropProperty;


        public bool IsAllowTreeDrop
        {
            get => (bool) GetValue(TreeControl.AllowTreeDropProperty);
            set => SetValue(TreeControl.AllowTreeDropProperty, value);
        }

        public bool AutoCompleteIsShow
        {
            get => (bool) GetValue(TreeControl.AutoCompleteIsShowProperty);
            set => SetValue(TreeControl.AutoCompleteIsShowProperty, value);
        }

        public bool CanDrag { get; set; }

        private TreeNodeModel _treeSelectedNode;

        public TreeNodeModel TreeSelectedNode
        {
            get => this._treeSelectedNode;
            set
            {
                this._treeSelectedNode = value;
                this.OnPropertyChanged();
            }
        }

        public TreeView ControlTree
        {
            get => this.TreeView;
            set => this.TreeView = value;
        }


        [ImportingConstructor]
        public TreeControl([Import(nameof(LoadDataAndTemplate))] ILoadDataAndTemplate loadDataAndTemplate)
        {
            InitializeComponent();

            if (loadDataAndTemplate != null)
            {
                _modelTyeAndItemTypeDictionary = loadDataAndTemplate.GetLogicDictionary();
                _currentTreeRoutedAddHandler = loadDataAndTemplate.GetTreeRoutedHandler();
                _currentTreeLibraryControlRoutedAddHandler = loadDataAndTemplate.GetTreeLibraryRoutedHandler();
                loadDataAndTemplate.SetStyle(_modelTyeAndItemTypeDictionary.Values.ToList(), this.Resources);
                _menuRouteAndHandlerDictionary = loadDataAndTemplate.GetMenuRouteAndHandlerDictionary();
                this.IsAllowTreeDrop = loadDataAndTemplate.GetIsAllowDrop();
            }
            else
            {
                LoadModelTypeAndItemType();
                this.IsAllowTreeDrop = false;
            }

            LoadHierarchicalDataTemplate();
            InitHierarchicalDataTemplateResource();
            base.DataContext = this.TreeHelper;
            CollectionChangedEventManager.AddHandler(TreeHelper.TreeAllNodels, TreeNodeItem_NodeOperator);

            AddMenuItem();
            ThisTreeAddHandler();
            ThisControlAddHandler();
        }

        static TreeControl()
        {
            AutoCompleteIsShowProperty =
                DependencyProperty.Register("AutoCompleteIsShow", typeof(bool), typeof(TreeControl),
                    new PropertyMetadata(false));
            AllowTreeDropProperty = DependencyProperty.Register("IsAllowTreeDrop", typeof(bool), typeof(TreeControl),
                new PropertyMetadata(false));
        }

        #region 初始化构造HierarchicalDataTemplate

        public void InitHierarchicalDataTemplateResource()
        {
            if (_hierarchicalDataTemplateResources == null)
                return;
            foreach (var dicItem in _hierarchicalDataTemplateResources)
            {
                this.Resources.Add(dicItem.Key, dicItem.Value);
            }

            var generic = new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/TreeLibrary;component/Generic.xaml", UriKind.Absolute)
            };
            this.Resources.MergedDictionaries.Add(generic);
        }

        public void LoadHierarchicalDataTemplate()
        {
            _hierarchicalDataTemplateResources = new Dictionary<DataTemplateKey, HierarchicalDataTemplate>();

            foreach (var dictionaryItem in _modelTyeAndItemTypeDictionary)
            {
                HierarchicalDataTemplate heHierarchicalDataTemplate =
                    new HierarchicalDataTemplate(dictionaryItem.Key)
                    {
                        ItemsSource = new Binding("SubNodes")
                    };
                FrameworkElementFactory nodeItemFrameworkElement =
                    new FrameworkElementFactory(dictionaryItem.Value);
                nodeItemFrameworkElement.AddHandler(TreeNodeItem.NodeSelectedEvent,
                    new NodeSelectedHandler(TreeNodeItem_NodeSelected));
                nodeItemFrameworkElement.AddHandler(TreeNodeItem.NodeDoubleClickEvent,
                    new NodeDoubleClickHandler(TreeNodeItem_NodeDoubleClick));
                nodeItemFrameworkElement.AddHandler(TreeNodeItem.NodeRightButtonSelectedEvent,
                    new NodeSelectedHandler(TreeNodeItem_NodeRightSelected));
                nodeItemFrameworkElement.AddHandler(TreeNodeItem.NodeCheckBoxCheckEvent,
                    new NodeSelectedHandler(TreeNodeItem_NodeCheck));
                nodeItemFrameworkElement.SetValue(UIElement.AllowDropProperty, true);
                nodeItemFrameworkElement.SetValue(TreeNodeItem.TextProperty, new Binding("Name"));
                //nodeItemFrameworkElement.SetValue(TreeNodeItem.IsShowMenuProperty, new Binding("IsShowMenu"));
                var isCheckBinding = new Binding("IsChecked")
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                nodeItemFrameworkElement.SetValue(TreeNodeItem.IsCheckedProperty, isCheckBinding);
                nodeItemFrameworkElement.SetValue(TreeNodeItem.IsSelectedProperty, new Binding("IsSelected"));


                nodeItemFrameworkElement.SetBinding(TreeNodeItem.ModelProperty, new Binding(""));

                heHierarchicalDataTemplate.VisualTree = nodeItemFrameworkElement;

                DataTemplateKey hierarchicalDataTemplateKey = new DataTemplateKey(dictionaryItem.Key);
                _hierarchicalDataTemplateResources.Add(hierarchicalDataTemplateKey, heHierarchicalDataTemplate);
            }
        }


        public void LoadModelTypeAndItemType()
        {
            LoadDataAndTemplate.GetNamespace(out var modelTypes, out var itemTypes);

            _modelTyeAndItemTypeDictionary = new Dictionary<Type, Type>();
            foreach (var modelTypeItem in modelTypes)
            {
                var modelName = modelTypeItem.Name
                    .Substring(0, modelTypeItem.Name.Length - nameof(TreeNodeModel).Length);
                var findItemIndex = itemTypes.FindIndex(f =>
                    f.Name.Substring(0, f.Name.Length - nameof(NodeItem).Length) == modelName);
                if (findItemIndex >= 0)
                    _modelTyeAndItemTypeDictionary.Add(modelTypeItem, itemTypes[findItemIndex]);
            }
        }

        #endregion

        #region 事件

        public void SetItemsSource(ObservableCollection<TreeNodeModel> itemSource)
        {
            TreeView.ItemsSource = itemSource;
            this.TreeHelper.SetItemsSource(itemSource);

            InitAddAllTreeNodeModel(itemSource);
        }


        public void RemoveNodeItem(TreeNodeModel removeNodeModel)
        {
            var treeNodeIndex = TreeHelper.TreeAllNodels.ToList().FindIndex(f => f.Data.Id == removeNodeModel.Data.Id);
            if (treeNodeIndex >= 0)
            {
                List<TreeNodeModel> removeNodeList = new List<TreeNodeModel>();
                if (RemoveNodeItemByNodeList(removeNodeModel, null, removeNodeList))
                {
                    TreeHelper.TreeAllNodels.RemoveAt(treeNodeIndex);
                    if (removeNodeList.Count > 0)
                    {
                        foreach (var removeNode in removeNodeList)
                        {
                            TreeHelper.TreeAllNodels.Remove(removeNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 删除选中节点
        /// </summary>
        public void RemoveSelectNodeItem()
        {
            if (TreeSelectedNode == null)
                return;

            var treeNodeIndex = TreeHelper.TreeAllNodels.ToList().FindIndex(f => f.Data.Id == TreeSelectedNode.Data.Id);
            if (treeNodeIndex >= 0)
            {
                List<TreeNodeModel> removeNodeList = new List<TreeNodeModel>();
                if (RemoveNodeItemByNodeList(TreeSelectedNode, null, removeNodeList))
                {
                    TreeHelper.TreeAllNodels.RemoveAt(treeNodeIndex);
                    if (removeNodeList.Count > 0)
                    {
                        foreach (var removeNode in removeNodeList)
                        {
                            TreeHelper.TreeAllNodels.Remove(removeNode);
                        }
                    }
                }
            }
        }

        public void RemoveAllChcekedNodeItem()
        {
            List<TreeNodeModel> checkNodeModelList =
                TreeHelper.TreeAllNodels.ToList().FindAll(f => f.IsChecked == true);
            if (checkNodeModelList.Count > 0)
            {
                checkNodeModelList.ForEach(f =>
                {
                    TreeHelper.TreeAllNodels.Remove(f);
                    RemoveNodeItemByNodeList(f, null, null);
                });
            }
        }

        public bool RemoveNodeItemByNodeList(TreeNodeModel selectNodeModel,
            ObservableCollection<TreeNodeModel> nodeModels, List<TreeNodeModel> removeNodeModelList)
        {
            foreach (var treeNodeModel in nodeModels ?? TreeHelper.NodeList)
            {
                if (treeNodeModel == null)
                    return false;
                if (treeNodeModel.Data.Id == selectNodeModel.Data.Id)
                {
                    if (nodeModels != null)
                    {
                        if (nodeModels[nodeModels.IndexOf(treeNodeModel)].SubNodes.Count > 0 &&
                            removeNodeModelList != null)
                        {
                            foreach (var nodeModel in nodeModels[nodeModels.IndexOf(treeNodeModel)].SubNodes)
                            {
                                removeNodeModelList.Add(nodeModel);
                            }
                        }

                        nodeModels.RemoveAt(nodeModels.IndexOf(treeNodeModel));
                    }
                    else
                    {
                        if (TreeHelper.NodeList[TreeHelper.NodeList.IndexOf(treeNodeModel)].SubNodes.Count > 0 &&
                            removeNodeModelList != null)
                        {
                            foreach (var nodeModel in TreeHelper.NodeList[TreeHelper.NodeList.IndexOf(treeNodeModel)]
                                .SubNodes)
                            {
                                removeNodeModelList.Add(nodeModel);
                            }
                        }

                        this.TreeHelper.NodeList.RemoveAt(TreeHelper.NodeList.IndexOf(treeNodeModel));
                    }

                    return true;
                }

                //TreeHelper.NodeList.Count == (TreeHelper.NodeList.IndexOf(treeNodeModel) + 1) &&
                if (treeNodeModel.SubNodes.Count > 0)
                {
                    RemoveNodeItemByNodeList(selectNodeModel, treeNodeModel.SubNodes, removeNodeModelList);
                }
            }

            return false;
        }

        private void InitAddAllTreeNodeModel(ObservableCollection<TreeNodeModel> itemSource)
        {
            foreach (var itemModel in itemSource)
            {
                TreeHelper.TreeAllNodels.Add(itemModel);
                if (itemModel.SubNodes.Count > 0)
                    InitAddAllTreeNodeModel(itemModel.SubNodes);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void TreeNodeItem_NodeDoubleClick(object sender, Args.NodeDoubleClickArgs e)
        {
            NodeDoubleClick?.Invoke(sender, e);
        }

        private void TreeNodeItem_NodeSelected(object sender, NodeSelectedArgs e)
        {
            TreeSelectedNode = e.NodeModel;
            NodeSelected?.Invoke(sender, e);
        }

        private void TreeNodeItem_NodeCheck(object sender, NodeSelectedArgs e)
        {
            NodeCheckChanged?.Invoke(sender, e);
        }

        private void TreeNodeItem_NodeRightSelected(object sender, NodeSelectedArgs e)
        {
            NodeRightButtonSelected?.Invoke(sender, e);
        }

        private void TreeNodeItem_NodeOperator(object sender, NotifyCollectionChangedEventArgs e)
        {
            NodeOperator?.Invoke(sender, e);
        }

        private void cmbLocation_PatternChanged(object sender, AutoComplete.AutoCompleteArgs args)
        {
            args.CancelBinding = true;
            this.cmbLocation.Items.Clear();
            string mPattern = args.Pattern.ToLower();

            {
                ObservableCollection<TreeNodeModel> mQuery = new ObservableCollection<TreeNodeModel>();
                this.TreeHelper.GetAppointedPatternNode(mPattern, mQuery, null);

                base.Dispatcher.BeginInvoke(new Action(delegate
                {
                    foreach (TreeNodeModel current in mQuery)
                    {
                        ComboBoxItem comboBoxItem = new ComboBoxItem
                        {
                            Content = current.Name,
                            Tag = current
                        };
                        this.cmbLocation.Items.Add(comboBoxItem);
                    }
                }), new object[0]);
            }

            this.cmbLocation.Text = args.Pattern;
            this.cmbLocation.EditableTextBox.CaretIndex = args.Pattern.Length;
        }

        private void cmbLocation_KeyUp(object sender, KeyEventArgs e)
        {
        }


        private void cmbLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbLocation.SelectedItem != null)
            {
                TreeNodeModel searchNode = (TreeNodeModel) ((ComboBoxItem) this.cmbLocation.SelectedItem).Tag;
                this.SelectedNode(searchNode);

                SearchContentSelectChanged?.Invoke(sender, e);
            }
        }

        private void SelectedNode(TreeNodeModel searchNode)
        {
            var node = this.FindNode(searchNode, this.TreeHelper.NodeList);
            if (node != null)
            {
                ExpandTreeViewItem(node);
                node.IsSelected = true;
                TreeViewItem item = FindTreeViewItem(this.TreeView, node);
                if (item != null)
                    item.BringIntoView();
            }
        }

        private void ExpandTreeViewItem(TreeNodeModel item)
        {
            if (item.Parent == null)
                return;
            item.Parent.IsExpanded = true;
            this.ExpandTreeViewItem(item.Parent);
        }

        private static TreeViewItem FindTreeViewItem(ItemsControl item, object data)
        {
            TreeViewItem findItem = null;
            for (int i = 0; i < item.Items.Count; i++)
            {
                TreeViewItem tvItem = (TreeViewItem) item.ItemContainerGenerator.ContainerFromIndex(i);
                if (tvItem == null)
                    continue;
                object itemData = item.Items[i];
                if (itemData == data)
                {
                    findItem = tvItem;
                    break;
                }
                else if (tvItem.Items.Count > 0)
                {
                    findItem = FindTreeViewItem(tvItem, data);
                    if (findItem != null)
                        break;
                }
            }

            return findItem;
        }

        private TreeNodeModel FindNode(TreeNodeModel specNode, IEnumerable<TreeNodeModel> list)
        {
            TreeNodeModel treeNodeModel;
            using (IEnumerator<TreeNodeModel> enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    TreeNodeModel current = enumerator.Current;
                    if (current != specNode && current != null)
                    {
                        if (current.SubNodes.Count <= 0)
                        {
                            continue;
                        }

                        TreeNodeModel treeNodeModel1 = FindNode(specNode, current.SubNodes);
                        if (treeNodeModel1 == null)
                        {
                            continue;
                        }

                        treeNodeModel = treeNodeModel1;
                        return treeNodeModel;
                    }
                    else
                    {
                        treeNodeModel = current;
                        return treeNodeModel;
                    }
                }

                return null;
            }
        }

        public event NodeSelectedHandler NodeSelected;
        public event NodeDoubleClickHandler NodeDoubleClick;
        public event NodeRightButtonSelectedHandler NodeRightButtonSelected;
        public event NodeCheckboxChangeHandler NodeCheckChanged;
        public event SearchContentSelectChangedHandler SearchContentSelectChanged;
        public event NodeOperatorHandler NodeOperator;

        //public event DragOverHandler NodeDragOver;
        //public event PreviewMouseMoveHandler NodePreviewMouseMove;
        //public event MouseLeaveHandler NodeMouseLeave;
        //public event PreviewMouseUpHandler NodePreviewMouseUp;

        #endregion

        #region 自定义方法

        ///// <summary>
        ///// 获取同类型的数节点实体
        ///// </summary>
        ///// <param name="modelType">要获取的实体Type</param>
        ///// <param name="treeNodeModels">开始搜索的节点集合（不填写默认从根节点开始查找）</param>
        ///// <returns></returns>
        //public List<TreeNodeModel> FindModelsTypeTreeItem(Type modelType,
        //    ObservableCollection<TreeNodeModel> treeNodeModels = null)
        //{
        //    List<TreeNodeModel> resultItems = new List<TreeNodeModel>();
        //    foreach (var nodeModelItem in treeNodeModels ?? TreeHelper.NodeList)
        //    {
        //        if (nodeModelItem.GetType() == modelType)
        //        {
        //            resultItems.Add(nodeModelItem);
        //        }

        //        if (nodeModelItem.SubNodes != null)
        //        {
        //            resultItems.AddRange(FindModelsTypeTreeItem(modelType, nodeModelItem.SubNodes));
        //        }
        //    }

        //    return resultItems;
        //}

        #endregion

        private void AddMenuItem()
        {
            if (_menuRouteAndHandlerDictionary == null)
                return;

            var menu = new ContextMenu();
            foreach (var routeAndHandlerItem in _menuRouteAndHandlerDictionary)
            {
                var menuItem = new MenuItem
                {
                    Header = routeAndHandlerItem.Key
                };
                menuItem.AddHandler(routeAndHandlerItem.Value.Keys.First(), routeAndHandlerItem.Value.Values.First());
                menu.Items.Add(menuItem);
            }

            TreeView.ContextMenu = menu;
        }

        /// <summary>
        /// 为树节点添加路由事件
        /// </summary>
        private void ThisTreeAddHandler()
        {
            if (_currentTreeRoutedAddHandler == null)
                return;
            foreach (var treeRoutedAndHandler in _currentTreeRoutedAddHandler)
            {
                TreeView.AddHandler(treeRoutedAndHandler.Key, treeRoutedAndHandler.Value);
            }
        }

        /// <summary>
        /// 为当前控件添加路由事件
        /// </summary>
        private void ThisControlAddHandler()
        {
            if (_currentTreeRoutedAddHandler == null)
                return;
            foreach (var controlRoutedAndHandler in _currentTreeLibraryControlRoutedAddHandler)
            {
                AddHandler(controlRoutedAndHandler.Key, controlRoutedAndHandler.Value);
            }
        }
    }
}