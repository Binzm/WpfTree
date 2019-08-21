using System.Windows;
using TreeLibrary.NodeItem.BaseItem;

namespace TreeLibrary.NodeItem
{
    public class TwosNodeItem : TreeNodeItem
    {
       
        public  static DependencyProperty ExitImgProperty;

        public string ExitImg
        {
            get => (string)base.GetValue(TwosNodeItem.ExitImgProperty);
            set => base.SetValue(TwosNodeItem.ExitImgProperty, value);
        }

        static TwosNodeItem()
        {
            TwosNodeItem.ExitImgProperty = DependencyProperty.Register("ExitImg", typeof(TwosNodeItem),
                typeof(TwosNodeItem), new PropertyMetadata(null));
        }

        public TwosNodeItem()
        {

        }


       
    }
}