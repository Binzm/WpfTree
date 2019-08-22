using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TreeLibrary;
using TreeLibrary.Converter;
using TreeLibrary.Model;
using TreeLibrary.NodeItem;
using TreeLibrary.NodeItem.BaseItem;
using TreeLibrary.NodeModel;
using TreeTest.Item;
using TreeTest.Model;

namespace TreeTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CompositionContainer container = null;

        private TreeControl treeControl;

        public MainWindow()
        {
            InitializeComponent();

            Compose();

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

            Task.Delay(10000)
                .ContinueWith(task => { treeControl.TreeHelper.TreeAllNodels[1].TextBoxForeground = "red"; });

            //Task.Delay(3000).ContinueWith(task =>
            //{
            //    for (int i = 0; i < 50; i++)
            //    {
            //        var addNodeModel = new ChannelsTreeNodeModel()
            //        {
            //            Data = new DataModel { Id =i.ToString(), Name = "2222" },
            //            Name = "666",
            //            IconImage = "xsearch_hov_act"
            //        };
            //        var mountPointModel = new TwosTreeNodeModel()
            //        {
            //            Data = new DataModel { Id = "9", Name = "2222" },
            //            Name = "1111"
            //        };
            //        Application.Current.Dispatcher.Invoke(() =>
            //        {
            //            treeControl.TreeHelper.AddNodeModelToItem(addNodeModel, mountPointModel);
            //        });
            //    }

            //});


            //Task.Delay(3000).ContinueWith(task =>
            //{
            //    for (int i = 0; i < 10; i++)
            //    {
            //        var addNodelist = new List<TreeNodeModel>()
            //        {
            //            new TwosTreeNodeModel()
            //            {
            //                Data = new DataModel {Id = new Random().Next(0, 5000).ToString(), Name = "2222"},
            //                Name = "123"
            //            },
            //            new TwosTreeNodeModel()
            //            {
            //                Data = new DataModel {Id = new Random().Next(5000, 10000).ToString(), Name = "2222"},
            //                Name = "234"
            //            },
            //            new TwosTreeNodeModel()
            //            {
            //                Data = new DataModel {Id = new Random().Next(10000, 15000).ToString(), Name = "2222"},
            //                Name = "345"
            //            },
            //            new TwosTreeNodeModel()
            //            {
            //                Data = new DataModel {Id = new Random().Next(15000, 20000).ToString(), Name = "2222"},
            //                Name = "456"
            //            }
            //        };
            //        var mountPointModel = new TwosTreeNodeModel()
            //        {
            //            Data = new DataModel {Id = "7", Name = "2222"},
            //            Name = "1111"
            //        };
            //        Application.Current.Dispatcher.Invoke(() =>
            //        {
            //            treeControl.TreeHelper.AddNodeModelListToItem(addNodelist, mountPointModel);
            //        });
            //    }
            //});

            #endregion
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            container?.Dispose();
            base.OnClosing(e);
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


        private void Compose()
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            if (dir.Exists)
            {
                var catalog = new DirectoryCatalog(dir.FullName, "TreeLibrary.dll");
                container = new CompositionContainer(catalog);
                container.ComposeParts(this);
            }
        }

        private void ButtonBaseSingleAddNodeBySelectNode_OnClick(object sender, RoutedEventArgs e)
        {
            if (treeControl.TreeSelectedNode == null)
                return;

            var addNodeModel = new TwosTreeNodeModel()
            {
                Data = new DataModel {Id = new Random().Next(100, 100000).ToString(), Name = "2222"},
                Name = "在选中节点下添加的子节点" + new Random().Next(100, 100000).ToString()
            };
            Application.Current.Dispatcher.Invoke(() =>
            {
                treeControl.TreeHelper.AddNodeModelToItem(addNodeModel, treeControl.TreeSelectedNode);
            });
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var addNodeModel = new ChannelsTreeNodeModel()
            {
                Data = new DataModel {Id = new Random().Next(100, 100000).ToString(), Name = "2222"},
                Name = new Random().Next(100, 100000).ToString()
            };
            var mountPointModel = new TwosTreeNodeModel()
            {
                Data = new DataModel {Id = "9", Name = "2222"},
                Name = "1111"
            };
            Application.Current.Dispatcher.Invoke(() =>
            {
                treeControl.TreeHelper.AddNodeModelToItem(addNodeModel, mountPointModel);
            });
        }


        private void ButtonBaseMulti_OnClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 30; i++)
            {
                Task.Delay(new Random().Next(0, 10000))
                    .ContinueWith(task =>
                    {
                        var addNodelist = new List<TreeNodeModel>()
                        {
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = new Random().Next(0, 5000).ToString(), Name = "2222"},
                                Name = new Random().Next(0, 5000).ToString()
                            },
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = new Random().Next(5000, 10000).ToString(), Name = "2222"},
                                Name = new Random().Next(5000, 10000).ToString()
                            },
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = new Random().Next(10000, 15000).ToString(), Name = "2222"},
                                Name = new Random().Next(10000, 15000).ToString()
                            },
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = new Random().Next(15000, 20000).ToString(), Name = "2222"},
                                Name = new Random().Next(15000, 20000).ToString()
                            }
                        };
                        var mountPointModel = new TwosTreeNodeModel()
                        {
                            Data = new DataModel {Id = "7", Name = "2222"},
                            Name = "添加多个子集的节点"
                        };
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            treeControl.TreeHelper.AddNodeModelListToItem(addNodelist, mountPointModel);
                        });
                    });
            }
        }


        private void ButtonBaseRemoveTree_OnClick(object sender, RoutedEventArgs e)
        {
            treeControl = null;
            //container?.Dispose();
            TreeStackPanel.Children.Clear();
        }

        private void ButtonBaseCreateTree_OnClick(object sender, RoutedEventArgs e)
        {
            if (treeControl != null)
                return;

            //Compose();


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

            treeControl = container.GetExportedValue<TreeControl>();

            #region 显示搜索框

            treeControl.AutoCompleteIsShow = true;

            #endregion

            ObservableCollection<TreeNodeModel> iteModels = new ObservableCollection<TreeNodeModel>
            {
                new TwosTreeNodeModel()
                {
                    Data = new DataModel {Id = "12", Name = "2222"},
                    Name = "祖节点1"
                },
                new TwosTreeNodeModel
                {
                    Data = new DataModel {Id = "11", Name = "2222"},
                    Name = "拥有一级子节点的祖节点2"
                },
                new TwosTreeNodeModel
                {
                    Data = new DataModel {Id = "10", Name = "2222"},
                    Name = "祖节点3"
                },
                new TwosTreeNodeModel
                {
                    Data = new DataModel {Id = "9", Name = "2222"},
                    Name = "添加单个子集的节点"
                },
                new TwosTreeNodeModel
                {
                    Data = new DataModel {Id = "8", Name = "2222"},
                    Name = "祖节点4"
                },
                new TwosTreeNodeModel
                {
                    Data = new DataModel {Id = "7", Name = "2222"},
                    Name = "添加多个子集的节点"
                }
            };
            iteModels[1].AddSubNode(new ThreeLevelTreeNodeModel()
            {
                Data = new DataModel {Id = "1", Name = "2222"},
                Name = "一级子节点"
            });

            treeControl.SetItemsSource(iteModels);

            TreeStackPanel.Children.Add(treeControl);
        }


        private void ButtonBaseRemoveSelectNodeItem_OnClick(object sender, RoutedEventArgs e)
        {
            treeControl.RemoveSelectNodeItem();
        }

        private void ButtonBaseRemoveCheckItem_OnClick(object sender, RoutedEventArgs e)
        {
            treeControl.RemoveAllChcekedNodeItem();
        }


        private void AllCombineSubNode(TreeNodeModel operateNode = null)
        {
            var thisChangeNode = operateNode ?? treeControl.TreeSelectedNode;
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
            var thisChangeNode = operateNode ?? treeControl.TreeSelectedNode;
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
                checkBoxElementFactory.SetValue(System.Windows.Controls.Control.IsTabStopProperty, false);
                checkBoxElementFactory.SetValue(UIElement.FocusableProperty, false);


                checkBoxElementFactory.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
                    new Binding("IsChecked"));
                checkBoxElementFactory.SetBinding(UIElement.VisibilityProperty, new Binding("ShowCheckBox"));
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
                    var sourceBinding = new Binding("IconImage") {Converter = new ImageNameToPhotoPathConverter()};
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
                var textBlockForeground = new Binding("TextBoxForeground") {Converter = new ForegroundConverter()};
                textBlockElementFactory.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Red));

                textBlockElementFactory.SetValue(TextBlock.FontSizeProperty, 14D);
                textBlockElementFactory.SetValue(Grid.ColumnProperty, 4);
                gridElementFactory.AppendChild(textBlockElementFactory);

                controlTemplate.VisualTree = gridElementFactory;
                var setter = new Setter
                    {Property = System.Windows.Controls.Control.TemplateProperty, Value = controlTemplate};
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
                var findPropertyInfo = (instance as Type).GetProperty(propertyName);
                return (findPropertyInfo != null);
            }

            return false;
        }

        private void ButtonBaseMultiAddNodeBySelectNode_OnClick(object sender, RoutedEventArgs e)
        {
            if (treeControl.TreeSelectedNode == null)
                return;

            for (var i = 0; i < 3000; i++)
            {
                Task.Delay(new Random().Next(0, 10000))
                    .ContinueWith(task =>
                    {
                        var addNodelist = new List<TreeNodeModel>()
                        {
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = new Random().Next(0, 5000).ToString(), Name = "2222"},
                                Name = new Random().Next(0, 5000).ToString()
                            },
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = new Random().Next(5000, 10000).ToString(), Name = "2222"},
                                Name = new Random().Next(5000, 10000).ToString()
                            },
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = new Random().Next(10000, 15000).ToString(), Name = "2222"},
                                Name = new Random().Next(10000, 15000).ToString()
                            },
                            new TwosTreeNodeModel()
                            {
                                Data = new DataModel {Id = new Random().Next(15000, 20000).ToString(), Name = "2222"},
                                Name = new Random().Next(15000, 20000).ToString()
                            }
                        };
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            treeControl.TreeHelper.AddNodeModelListToItem(addNodelist,
                                treeControl.TreeSelectedNode);
                        });
                    });
            }
        }
    }
}