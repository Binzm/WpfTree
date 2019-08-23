using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TreeLibrary.Converter;
using TreeLibrary.NodeItem.BaseItem;

namespace TreeLibrary.Model
{
    public enum LoadModelType
    {
        DefaultAll = 0,
        First = 1,
        UserDefined = 2
    }

    public enum LoadStyleType
    {
        DefaultAll = 0,
        First = 1,
        UserDefaultStyle = 2
    }


    [Export(nameof(LoadDataAndTemplate), typeof(ILoadDataAndTemplate))]
    public class LoadDataAndTemplate : ILoadDataAndTemplate
    {
        public static LoadModelType CurrentLoadModelType = LoadModelType.DefaultAll;
        public static LoadStyleType CurrentLoadStyleType = LoadStyleType.DefaultAll;

        public static Dictionary<Type, Type> CurrentUserDefaultModelTypeAndItemType;
        public static Dictionary<Type, Style> CurrentItemTypeAndStyleList;

        public static bool TreeNodeIsAllowDrop;

        public static Dictionary<string, Dictionary<RoutedEvent, System.Delegate>> CurrentLoadMenuRouteHandler;



        public bool GetIsAllowDrop()
        {
            return TreeNodeIsAllowDrop;
        }

        public Dictionary<Type, Type> GetLogicDictionary()
        {
            switch (CurrentLoadModelType)
            {
                case LoadModelType.DefaultAll:
                    return LoadModelTypeAndItemType();
                case LoadModelType.First:
                    return LoadModelTypeFirst();
                case LoadModelType.UserDefined:
                    return CurrentUserDefaultModelTypeAndItemType;
                default:
                    return LoadModelTypeAndItemType();
            }
        }

        public void SetStyle(List<Type> modelItemList, ResourceDictionary controlResources)
        {
            switch (CurrentLoadStyleType)
            {
                case LoadStyleType.DefaultAll:
                    SetAllDefaultStyle(modelItemList, controlResources);
                    break;
                case LoadStyleType.First:
                    SetFirstStyle(modelItemList, controlResources);
                    break;
                case LoadStyleType.UserDefaultStyle:
                    ResourceDictionaryAddStyle(CurrentItemTypeAndStyleList.Values.ToList(),
                        CurrentItemTypeAndStyleList.Keys.ToList(), controlResources);
                    break;
                default:
                    break;
            }
        }


        public Dictionary<string, Dictionary<RoutedEvent, System.Delegate>> GetMenuRouteAndHandlerDictionary()
        {
            return CurrentLoadMenuRouteHandler;
        }

        private Dictionary<Type, Type> LoadModelTypeAndItemType()
        {
            GetNamespace(out var modelTypes, out var itemTypes);

            var modelTyeAndItemTypeDictionary = new Dictionary<Type, Type>();
            foreach (var modelTypeItem in modelTypes)
            {
                var modelName = modelTypeItem.Name
                    .Substring(0, modelTypeItem.Name.Length - nameof(TreeNodeModel).Length);
                var findItemIndex = itemTypes.FindIndex(f =>
                    f.Name.Substring(0, f.Name.Length - nameof(NodeItem).Length) == modelName);
                if (findItemIndex >= 0)
                    modelTyeAndItemTypeDictionary.Add(modelTypeItem, itemTypes[findItemIndex]);
            }

            return modelTyeAndItemTypeDictionary;
        }

        private Dictionary<Type, Type> LoadModelTypeFirst()
        {
            GetNamespace(out var modelTypes, out var itemTypes);

            var modelTyeAndItemTypeDictionary = new Dictionary<Type, Type>();
            var modelName = modelTypes[1].Name
                .Substring(0, modelTypes[1].Name.Length - nameof(TreeNodeModel).Length);
            var findItemIndex = itemTypes.FindIndex(f =>
                f.Name.Substring(0, f.Name.Length - nameof(NodeItem).Length) == modelName);
            if (findItemIndex >= 0)
                modelTyeAndItemTypeDictionary.Add(modelTypes[1], itemTypes[findItemIndex]);


            return modelTyeAndItemTypeDictionary;
        }


        private static void SetFirstStyle(List<Type> modelItemList, System.Collections.IDictionary controlResources)
        {
            var styles = GetDefaultAllStyle(modelItemList);
            if (styles.Count > 0 && modelItemList.Count == styles.Count)
            {
                controlResources[modelItemList[0]] = styles[0];
            }
        }

        private static void SetAllDefaultStyle(List<Type> modelItemList,
            System.Collections.IDictionary controlResources)
        {
            var styles = GetDefaultAllStyle(modelItemList);
            ResourceDictionaryAddStyle(styles, modelItemList, controlResources);
        }

        private static void ResourceDictionaryAddStyle(IReadOnlyList<Style> styles, IReadOnlyList<Type> modelItemList,
            System.Collections.IDictionary controlResources)
        {
            if (styles.Count > 0 && modelItemList.Count == styles.Count)
            {
                for (var i = 0; i < styles.Count; i++)
                {
                    controlResources[modelItemList[i]] = styles[i];
                }
            }
        }


        #region 私有共用方法

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
                textBlockElementFactory.SetValue(TextBlock.ForegroundProperty, textBlockForeground);

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

        public static void GetNamespace(out List<Type> modelTypeList, out List<Type> itemTypeList)
        {
            modelTypeList = new List<Type>();
            itemTypeList = new List<Type>();
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (item.Namespace != null && item.Namespace.Equals("TreeLibrary.NodeModel"))
                {
                    modelTypeList.Add(item);
                }

                if (item.Namespace != null && item.Namespace.Equals("TreeLibrary.NodeItem"))
                {
                    itemTypeList.Add(item);
                }
            }
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

        #endregion
    }
}