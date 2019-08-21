using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TreeLibrary.Control
{
    public class SkinButton : Button
    {
        static SkinButton()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SkinButton),
                new FrameworkPropertyMetadata(typeof(SkinButton)));
        }


        public ImageSource NorImage
        {
            get => (ImageSource) base.GetValue(SkinButton.NorImageProperty);
            set => base.SetValue(SkinButton.NorImageProperty, value);
        }


        public ImageSource HovImage
        {
            get => (ImageSource) base.GetValue(SkinButton.HovImageProperty);
            set => base.SetValue(SkinButton.HovImageProperty, value);
        }


        public ImageSource ActImage
        {
            get => (ImageSource) base.GetValue(SkinButton.ActImageProperty);
            set => base.SetValue(SkinButton.ActImageProperty, value);
        }


        public ImageSource DisImage
        {
            get => (ImageSource) base.GetValue(SkinButton.DisImageProperty);
            set => base.SetValue(SkinButton.DisImageProperty, value);
        }


        public static DependencyProperty NorImageProperty = DependencyProperty.Register("NorImage", typeof(ImageSource),
            typeof(SkinButton), new PropertyMetadata(null));


        public static DependencyProperty HovImageProperty = DependencyProperty.Register("HovImage", typeof(ImageSource),
            typeof(SkinButton), new PropertyMetadata(null));


        public static DependencyProperty ActImageProperty = DependencyProperty.Register("ActImage", typeof(ImageSource),
            typeof(SkinButton), new PropertyMetadata(null));


        public static DependencyProperty DisImageProperty = DependencyProperty.Register("DisImage", typeof(ImageSource),
            typeof(SkinButton), new PropertyMetadata(null));
    }
}