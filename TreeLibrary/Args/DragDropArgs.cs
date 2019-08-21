using System;
using TreeLibrary.Model;

namespace TreeLibrary.Args
{
    public class DragDropArgs : EventArgs
    {
        public TreeNodeModel NodeModel { get; set; }


        public DragDropArgs(TreeNodeModel nodeModel)
        {
            this.NodeModel = nodeModel;
        }
    }
}