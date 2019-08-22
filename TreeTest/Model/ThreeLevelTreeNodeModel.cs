using System.Windows;
using TreeLibrary.Model;

namespace TreeTest.Model
{
    public class ThreeLevelTreeNodeModel : TreeNodeModel
    {
        public ThreeLevelTreeNodeModel() : base(true)
        {
            this.IconImage = this.GetType().Name;
        }

        protected override object LoadSubNodes()
        {
            return null;
        }
    }
}