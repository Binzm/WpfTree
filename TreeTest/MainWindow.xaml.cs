using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TreeLibrary;
using TreeLibrary.Converter;
using TreeLibrary.DragDropFramework;
using TreeLibrary.Model;
using TreeLibrary.NodeItem;
using TreeLibrary.NodeItem.BaseItem;
using TreeLibrary.NodeModel;
using TreeTest.Item;
using TreeTest.Model;
using TreeTest.ProductAndCustomer;
using Timer = System.Timers.Timer;

namespace TreeTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CompositionContainer _container = null;

        private TreeControl _treeControl;
        private Timer _timer;
        private Timer _beginOffLineTime;
        private Timer _beginOnLineTime;
        private readonly Random _random = new Random();

        private readonly AsyncStack _asyncStack;
        private readonly Producer _producer;
        private Consumer _consumer;
        private Timer _consumerTimer;


        private readonly string[] _colorStrings = new string[]
        {
            "AliceBlue",
            "DarkGreen",
            "AntiqueWhite",
            "LightSeaGreen",
            "Aqua",
            "Navy"
        };


        public MainWindow()
        {
            InitializeComponent();

            Compose();

            _asyncStack = new AsyncStack();
            _producer = new Producer(_asyncStack);

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            #region NodeOperator

            //treeControl.NodeOperator += TreeNodeOperator;
            //treeControl.TreeHelper.TreeAllNodels[1]=new ChannelsTreeNodeModel()
            //{
            //    Data = new DataModel { Id = "122", Name = "2222" },
            //    Name = "444444",
            //    IconImage = "xsearch_hov_act"
            //};

            #endregion

            #region Task 

            Task.Delay(5000)
                .ContinueWith(
                    task =>
                    {
                        _treeControl.TreeHelper.TreeAllNodels[1].TextBoxForeground = "red";
                        if (_consumer == null)
                            return;
                    });

            #endregion
        }

        private void ExecuteConsumerExpense(object sender, ElapsedEventArgs e)
        {
            _consumer.RunConsume();
        }


        private int GetNodeCount(IEnumerable<TreeNodeModel> nodeList, int counts)
        {
            int count = counts;
            foreach (var nodeItem in nodeList)
            {
                count++;
                if (nodeItem.SubNodes.Count > 0)
                    count = GetNodeCount(nodeItem.SubNodes, count);
            }

            return count;
        }

        //private void TreeNodeOperator(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    switch (e.Action)
        //    {
        //        case NotifyCollectionChangedAction.Add:
        //            break;
        //        case NotifyCollectionChangedAction.Remove:
        //            break;
        //        case NotifyCollectionChangedAction.Replace:
        //            break;
        //        case NotifyCollectionChangedAction.Reset:
        //            break;
        //        default:
        //            break;
        //    }

        //}

        protected override void OnClosing(CancelEventArgs e)
        {
            _container?.Dispose();
            base.OnClosing(e);
        }


        private void Compose()
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            if (dir.Exists)
            {
                var catalog = new DirectoryCatalog(dir.FullName, "TreeLibrary.dll");
                _container = new CompositionContainer(catalog);
                _container.ComposeParts(this);
            }
        }

        private void ButtonBaseSingleAddNodeBySelectNode_OnClick(object sender, RoutedEventArgs e)
        {
            if (_treeControl == null || _treeControl.TreeSelectedNode == null)
                return;

            var addNodeModel = new TwosTreeNodeModel()
            {
                Data = new DataModel {Id = _random.Next(100, 100000).ToString()},
                Name = "在选中节点下添加的子节点" + _random.Next(100, 100000).ToString()
            };
            Application.Current.Dispatcher.Invoke(() =>
            {
                _treeControl.TreeHelper.AddNodeModelToItem(addNodeModel, _treeControl.TreeSelectedNode);
            });
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (_treeControl == null)
                return;

            var addNodeModel = new ChannelsTreeNodeModel()
            {
                Data = new DataModel {Id = _random.Next(100, 100000).ToString()},
                Name = _random.Next(100, 100000).ToString()
            };
            var mountPointModel = new TwosTreeNodeModel()
            {
                Data = new DataModel {Id = "9"},
                Name = "1111"
            };
            Application.Current.Dispatcher.Invoke(() =>
            {
                _treeControl.TreeHelper.AddNodeModelToItem(addNodeModel, mountPointModel);
            });
        }


        private void ButtonBaseMulti_OnClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 30; i++)
            {
                Task.Delay(_random.Next(0, 10000))
                    .ContinueWith(task =>
                    {
                        var addNodelist = new List<TreeNodeModel>()
                        {
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = _random.Next(0, 5000).ToString()},
                                Name = _random.Next(0, 5000).ToString()
                            },
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = _random.Next(5000, 10000).ToString()},
                                Name = _random.Next(5000, 10000).ToString()
                            },
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = _random.Next(10000, 15000).ToString()},
                                Name = _random.Next(10000, 15000).ToString()
                            },
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = _random.Next(15000, 20000).ToString()},
                                Name = _random.Next(15000, 20000).ToString()
                            }
                        };
                        var mountPointModel = new TwosTreeNodeModel()
                        {
                            Data = new DataModel {Id = "7", Name = "2222"},
                            Name = "添加多个子集的节点"
                        };
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _treeControl.TreeHelper.AddNodeModelListToItem(addNodelist, mountPointModel);
                        });
                    });
            }
        }


        private void ButtonBaseRemoveTree_OnClick(object sender, RoutedEventArgs e)
        {
            _treeControl = null;

            //container?.Dispose(); 
            TreeStackPanel.Children.Clear();
            if (_timer != null)
                _timer.Stop();
            if (_beginOffLineTime != null)
                _beginOffLineTime.Stop();

            if (_consumerTimer != null)
                _consumerTimer.Stop();
        }

        private void ButtonBaseCreateTree_OnClick(object sender, RoutedEventArgs e)
        {
            if (_treeControl != null)
                return;

            #region 加载自定义Model 和Item

            Dictionary<Type, Type> modelTypeAndItemType = new Dictionary<Type, Type>
            {
                {typeof(ThreeLevelTreeNodeModel), typeof(ThreeLevelTreeNodeItem)},
                {typeof(ChannelsTreeNodeModel), typeof(ChannelsNodeItem)},
                {typeof(TwosTreeNodeModel), typeof(TwosNodeItem)}
            };
            LoadDataAndTemplate.CurrentUserDefaultModelTypeAndItemType = modelTypeAndItemType;
            LoadDataAndTemplate.CurrentLoadModelType = LoadModelType.UserDefined;

            #endregion

            #region 加载自定义样式

            LoadDataAndTemplate.CurrentLoadStyleType = LoadStyleType.DefaultAll;

            var itemTypeList = new List<Type>()
            {
                typeof(ThreeLevelTreeNodeItem),
                typeof(ChannelsNodeItem),
                typeof(TwosNodeItem)
            };
            var styleList = GetDefaultAllStyle(itemTypeList);
            if (LoadDataAndTemplate.CurrentItemTypeAndStyleList == null)
                LoadDataAndTemplate.CurrentItemTypeAndStyleList = new Dictionary<Type, Style>();
            else
                LoadDataAndTemplate.CurrentItemTypeAndStyleList.Clear();
            itemTypeList.ForEach(f =>
            {
                LoadDataAndTemplate.CurrentItemTypeAndStyleList.Add(itemTypeList[itemTypeList.IndexOf(f)],
                    styleList[itemTypeList.IndexOf(f)]);
            });

            #endregion

            #region Add Menu RouteHandler

            var routeHandler = new Dictionary<RoutedEvent, Delegate>();
            var delegateAllExpandedSubEvent = new RoutedEventHandler((o, args) => { AllExpandedSubNode(); });
            routeHandler.Add(MenuItem.ClickEvent, delegateAllExpandedSubEvent);

            var allCombineRouteRouteHandler = new Dictionary<RoutedEvent, Delegate>();
            var delegateAllCombineSubEvent = new RoutedEventHandler((o, args) => { AllCombineSubNode(); });
            allCombineRouteRouteHandler.Add(MenuItem.ClickEvent, delegateAllCombineSubEvent);

            var menuRouteHandler = new Dictionary<string, Dictionary<RoutedEvent, Delegate>>
            {
                {"全部展开", routeHandler},
                {"全部合并", allCombineRouteRouteHandler}
            };


            LoadDataAndTemplate.CurrentLoadMenuRouteHandler = menuRouteHandler;

            #endregion

            #region 是否可拖拽以及事件

            LoadDataAndTemplate.TreeNodeIsAllowDrop = true;

            #endregion

            _treeControl = _container.GetExportedValue<TreeControl>();

            #region 显示搜索框

            _treeControl.AutoCompleteIsShow = true;

            #endregion


            var iteModels = new ObservableCollection<TreeNodeModel>
            {
                new TwosTreeNodeModel()
                {
                    Data = new DataModel {Id = "12"},
                    Name = "祖节点1"
                },
                new TwosTreeNodeModel
                {
                    Data = new DataModel {Id = "11"},
                    Name = "拥有一级子节点的祖节点2"
                },
                new TwosTreeNodeModel
                {
                    Data = new DataModel {Id = "10"},
                    Name = "祖节点3"
                },
                new TwosTreeNodeModel
                {
                    Data = new DataModel {Id = "9"},
                    Name = "添加单个子集的节点"
                },
                new TwosTreeNodeModel
                {
                    Data = new DataModel {Id = "8"},
                    Name = "祖节点4"
                },
                new TwosTreeNodeModel
                {
                    Data = new DataModel {Id = "7"},
                    Name = "添加多个子集的节点"
                }
            };
            iteModels[1].AddSubNode(new ChannelsTreeNodeModel()
            {
                Data = new DataModel {Id = "1"},
                Name = "一级子节点"
            });


            _treeControl.SetItemsSource(iteModels);

            #region Tree DragDrop

            TreeViewDataProvider<ItemsControl, TreeViewItem> treeViewDataProvider =
                new TreeViewDataProvider<ItemsControl, TreeViewItem>("TreeViewItem");
            TreeViewDataConsumer<ItemsControl, TreeViewItem> treeViewDataConsumer =
                new TreeViewDataConsumer<ItemsControl, TreeViewItem>(new string[] {"TreeViewItem"});

            treeViewDataConsumer.DropDragHandler += DropDragHandler;

            _ = new DragManager(_treeControl.ControlTree, treeViewDataProvider);
            _ = new DropManager(_treeControl.ControlTree,
                new IDataConsumer[]
                {
                    treeViewDataConsumer
                });

            _ = new DropManager(TextTreeDrag,
                new IDataConsumer[]
                {
                    treeViewDataConsumer
                });

            #endregion

            TreeStackPanel.Children.Add(_treeControl);

            ToChangeTreeNodeProperty();

            #region 消费者开始消费

            _consumer = new Consumer(_asyncStack, _treeControl, this);
            _consumerTimer = new Timer
            {
                Enabled = true,
                Interval = 8
            };
            _consumerTimer.Start();
            _consumerTimer.Elapsed += ExecuteConsumerExpense;

            #endregion
        }

        private void DropDragHandler(bool bDrop, object sender, DragEventArgs e)
        {
            var dataProvider =
                (TreeViewDataProvider<ItemsControl, TreeViewItem>) this.GetData(e);

            if (bDrop)
            {
                var treeView = (TreeView) sender;
                if (treeView != null)
                {
                    if (e.Source is TreeView dropTarget && dropTarget.Name == "TextTreeDrag")
                    {
                        if (dataProvider.SourceObject is TreeNodeModel dragSourceObject)
                        {
                            treeView.Items.Add(new TreeViewItem {Header = dragSourceObject.Name});
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A list of formats this data object consumer supports
        /// </summary>
        private string[] _dataFormats = {"TreeViewItem"};

        public virtual object GetData(DragEventArgs e)
        {
            object data = null;
            string[] dataFormats = e.Data.GetFormats();
            foreach (var dataFormat in dataFormats)
            {
                foreach (var dataFormatString in this._dataFormats)
                {
                    if (dataFormat.Equals(dataFormatString))
                    {
                        try
                        {
                            data = e.Data.GetData(dataFormat);
                        }
                        catch
                        {
                            ;
                        }
                    }

                    if (data != null)
                        return data;
                }
            }

            return null;
        }


        private void ToChangeTreeNodeProperty()
        {
            _timer = new Timer
            {
                Enabled = true,
                Interval = 500
            };
            _timer.Start();
            _timer.Elapsed += Timer1_Elapsed;
        }

        private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            this._treeControl.TreeHelper
                .TreeAllNodels[_random.Next(0, this._treeControl.TreeHelper.TreeAllNodels.Count)]
                .TextBoxForeground = _colorStrings[_random.Next(0, _colorStrings.Length)];
        }

        private void ButtonBaseRemoveSelectNodeItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (_treeControl == null)
                return;

            _treeControl.RemoveSelectNodeItem();
        }

        private void ButtonBaseRemoveCheckItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (_treeControl == null)
                return;

            _treeControl.RemoveAllChcekedNodeItem();
        }


        private void AllCombineSubNode(TreeNodeModel operateNode = null)
        {
            var thisChangeNode = operateNode ?? _treeControl.TreeSelectedNode;
            thisChangeNode.IsExpanded = false;
            if (thisChangeNode.SubNodes.Count > 0)
            {
                foreach (var treeNodeModel in thisChangeNode.SubNodes)
                {
                    AllCombineSubNode(treeNodeModel);
                }
            }
        }

        private void AllExpandedSubNode(TreeNodeModel operateNode = null)
        {
            var thisChangeNode = operateNode ?? _treeControl.TreeSelectedNode;
            thisChangeNode.IsExpanded = true;
            if (thisChangeNode.SubNodes.Count > 0)
            {
                foreach (var treeNodeModel in thisChangeNode.SubNodes)
                {
                    AllExpandedSubNode(treeNodeModel);
                }
            }
        }


        private static List<Style> GetDefaultAllStyle(List<Type> modelItemList)
        {
            var resultStyles = new List<Style>();

            modelItemList.ForEach(f =>
            {
                var tempStyle = new Style(f);

                var controlTemplate = new ControlTemplate(f);

                var gridElementFactory = new FrameworkElementFactory(typeof(Grid))
                {
                    Name = "PART_Grid"
                };
                var column1 = new FrameworkElementFactory(typeof(ColumnDefinition));
                column1.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
                var column2 = new FrameworkElementFactory(typeof(ColumnDefinition));
                column2.SetValue(ColumnDefinition.WidthProperty, new GridLength(3, GridUnitType.Pixel));
                var column3 = new FrameworkElementFactory(typeof(ColumnDefinition));
                column3.SetValue(ColumnDefinition.WidthProperty, new GridLength(18, GridUnitType.Pixel));
                var column4 = new FrameworkElementFactory(typeof(ColumnDefinition));
                column4.SetValue(ColumnDefinition.WidthProperty, new GridLength(3, GridUnitType.Pixel));
                var column5 = new FrameworkElementFactory(typeof(ColumnDefinition));
                column5.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));

                gridElementFactory.AppendChild(column1);
                gridElementFactory.AppendChild(column2);
                gridElementFactory.AppendChild(column3);
                gridElementFactory.AppendChild(column4);
                gridElementFactory.AppendChild(column5);

                var checkBoxElementFactory = new FrameworkElementFactory(typeof(CheckBox));
                checkBoxElementFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty,
                    HorizontalAlignment.Center);
                checkBoxElementFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
                checkBoxElementFactory.SetValue(Control.IsTabStopProperty, false);
                checkBoxElementFactory.SetValue(UIElement.FocusableProperty, false);

                checkBoxElementFactory.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
                    new Binding(nameof(TreeNodeModel.IsChecked)));
                checkBoxElementFactory.SetBinding(UIElement.VisibilityProperty,
                    new Binding(nameof(TreeNodeModel.ShowCheckBox)));
                checkBoxElementFactory.SetValue(Grid.ColumnProperty, 0);
                gridElementFactory.AppendChild(checkBoxElementFactory);

                if (ContainProperty(f, "ExitImg"))
                {
                    var imageElementFactory = new FrameworkElementFactory(typeof(Image));
                    imageElementFactory.SetValue(FrameworkElement.NameProperty, "icon");
                    imageElementFactory.SetValue(FrameworkElement.HeightProperty, 18D);
                    imageElementFactory.SetValue(FrameworkElement.WidthProperty, 20D);
                    imageElementFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty,
                        HorizontalAlignment.Stretch);
                    imageElementFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);
                    var sourceBinding = new Binding(nameof(TreeNodeModel.IconImage))
                        {Converter = new ImageNameToPhotoPathConverter()};
                    imageElementFactory.SetValue(Image.SourceProperty, sourceBinding);
                    imageElementFactory.SetValue(Grid.ColumnProperty, 2);
                    gridElementFactory.AppendChild(imageElementFactory);
                }

                var textBlockElementFactory = new FrameworkElementFactory(typeof(TextBlock));
                textBlockElementFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty,
                    HorizontalAlignment.Left);
                textBlockElementFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
                textBlockElementFactory.SetValue(TextBlock.TextProperty,
                    new TemplateBindingExtension(TreeNodeItem.TextProperty));
                var textBlockForeground = new Binding(nameof(TreeNodeModel.TextBoxForeground))
                    {Converter = new ForegroundConverter()};
                textBlockElementFactory.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Red));

                textBlockElementFactory.SetValue(TextBlock.FontSizeProperty, 14D);
                textBlockElementFactory.SetValue(Grid.ColumnProperty, 4);
                gridElementFactory.AppendChild(textBlockElementFactory);

                controlTemplate.VisualTree = gridElementFactory;
                var setter = new Setter
                    {Property = TemplateProperty, Value = controlTemplate};
                tempStyle.Setters.Add(setter);

                resultStyles.Add(tempStyle);
            });

            return resultStyles;
        }

        /// <summary>
        /// 根据类型判断 类型对象 是否包含 属性名称
        /// </summary>
        /// <param name="instance">类型</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>是否包含</returns>
        public static bool ContainProperty(object instance, string propertyName)
        {
            if (instance != null && !string.IsNullOrEmpty(propertyName))
            {
                var findPropertyInfo = ((Type) instance)?.GetProperty(propertyName);
                return (findPropertyInfo != null);
            }

            return false;
        }

        private void ButtonBaseMultiAddNodeBySelectNode_OnClick(object sender, RoutedEventArgs e)
        {
            if (_treeControl == null || _treeControl.TreeSelectedNode == null)
                return;

            for (var i = 0; i < 30; i++)
            {
                Task.Delay(_random.Next(0, 5000))
                    .ContinueWith(task =>
                    {
                        var addNodelist = new List<TreeNodeModel>()
                        {
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = _random.Next(0, 5000).ToString()},
                                Name = _random.Next(0, 5000).ToString()
                            },
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = _random.Next(5000, 10000).ToString()},
                                Name = _random.Next(5000, 10000).ToString()
                            },
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = _random.Next(10000, 15000).ToString()},
                                Name = _random.Next(10000, 15000).ToString()
                            },
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = _random.Next(15000, 20000).ToString()},
                                Name = _random.Next(15000, 20000).ToString()
                            }
                        };
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _treeControl.TreeHelper.AddNodeModelListToItem(addNodelist,
                                _treeControl.TreeSelectedNode);
                        });
                    });
            }
        }

        private void ButtonNodeOffLine_OnClick(object sender, RoutedEventArgs e)
        {
            _beginOffLineTime = new Timer
            {
                Enabled = true,
                Interval = 10
            };
            _beginOffLineTime.Start();
            _beginOffLineTime.Elapsed += NodeOffLine;

            BeginOffLineButton.IsEnabled = false;
        }

        private void NodeOffLine(object sender, ElapsedEventArgs e)
        {
            new Thread(() => { _producer.RunProduction(false); }).Start();
        }

        private void ButtonNodeOffLineEnd_OnClick(object sender, RoutedEventArgs e)
        {
            if (_beginOffLineTime == null)
                return;
            _beginOffLineTime.Enabled = false;
            _beginOffLineTime.Stop();

            BeginOffLineButton.IsEnabled = true;
        }

        private void ButtonNodeOnLine_OnClick(object sender, RoutedEventArgs e)
        {
            _beginOnLineTime = new Timer
            {
                Enabled = true,
                Interval = 5
            };
            _beginOnLineTime.Start();
            _beginOnLineTime.Elapsed += NodeOnLine;

            BeginOnLineButton.IsEnabled = false;
        }

        private void NodeOnLine(object sender, ElapsedEventArgs e)
        {
            new Thread(() => { _producer.RunProduction(true); }).Start();
        }

        private void ButtonNodeOnLineEnd_OnClick(object sender, RoutedEventArgs e)
        {
            if (_beginOnLineTime == null)
                return;
            _beginOnLineTime.Enabled = false;
            _beginOnLineTime.Stop();

            BeginOnLineButton.IsEnabled = true;
        }
    }
}