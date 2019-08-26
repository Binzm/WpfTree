using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TreeLibrary.DragDropFramework
{
    public class DefaultAdorner : Adorner
    {
        private UIElement _child;
        private Point _adornerOrigin;
        private Point _adornerOffset;

        /// <summary>
        /// Create an adorner with default opacity.
        /// The created adorner must then be added to the AdornerLayer.
        /// </summary>
        /// <param name="adornedElement">Element whose AdornerLayer will be use for displaying the adorner</param>
        /// <param name="adornerElement">Element used as adorner</param>
        /// <param name="adornerOrigin">Origin offset within the adorner</param>
        public DefaultAdorner(UIElement adornedElement, UIElement adornerElement, Point adornerOrigin)
            : this(adornedElement, adornerElement, adornerOrigin, 0.3)
        {
        }

        /// <summary>
        /// Create an adorner.
        /// The created adorner must then be added to the AdornerLayer.
        /// </summary>
        /// <param name="adornedElement">Element whose AdornerLayer will be use for displaying the adorner</param>
        /// <param name="adornerElement">Element used as adorner</param>
        /// <param name="adornerOrigin">Origin offset within the adorner</param>
        /// <param name="opacity">Adorner's opacity</param>
        public DefaultAdorner(UIElement adornedElement, UIElement adornerElement, Point adornerOrigin, double opacity)
            : base(adornedElement)
        {
            Rectangle rect = new Rectangle
            {
                Width = adornerElement.RenderSize.Width,
                Height = adornerElement.RenderSize.Height
            };

            VisualBrush visualBrush = new VisualBrush(adornerElement)
            {
                Opacity = opacity,
                Stretch = Stretch.None
            };
            rect.Fill = visualBrush;

            this._child = rect;

            this._adornerOrigin = adornerOrigin;
        }

        /// <summary>
        /// Set the position of and redraw the adorner.
        /// Call when the mouse cursor position changes.
        /// </summary>
        /// <param name="position">Adorner's new position relative to AdornerLayer origin</param>
        public void SetMousePosition(Point position)
        {
            this._adornerOffset.X = position.X - this._adornerOrigin.X;
            this._adornerOffset.Y = position.Y - this._adornerOrigin.Y;
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            AdornerLayer adornerLayer = (AdornerLayer) this.Parent;
            if (adornerLayer != null)
            {
                adornerLayer.Update(this.AdornedElement);
            }
        }
    }
}