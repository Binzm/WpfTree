using System.ComponentModel.Composition;
using System.Windows;
using TreeLibrary.NodeItem.BaseItem;

namespace TreeTest.Item
{
    public class ThreeLevelTreeNodeItem: TreeNodeItem
    {
        public static DependencyProperty ExitImgProperty;
        public string ExitImg
        {
            get => (string)base.GetValue(ThreeLevelTreeNodeItem.ExitImgProperty);
            set => base.SetValue(ThreeLevelTreeNodeItem.ExitImgProperty, value);
        }

        public ThreeLevelTreeNodeItem()
        {
            if (ThreeLevelTreeNodeItem.ExitImgProperty != null)
                return;

            ThreeLevelTreeNodeItem.ExitImgProperty = DependencyProperty.Register("ExitImg", typeof(ThreeLevelTreeNodeItem),
                typeof(ThreeLevelTreeNodeItem), new PropertyMetadata(null));
        }
    }
}
