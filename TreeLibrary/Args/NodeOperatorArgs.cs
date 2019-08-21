using System;
using TreeLibrary.Model;

namespace TreeLibrary.Args
{
    public enum NodeOperatorType
    {
        Add,
        Remove
    }

    public class NodeOperatorArgs : EventArgs
    {
        public TreeNodeModel NodeModel { get; set; }

        public NodeOperatorType OpType { get; set; }

        public NodeOperatorArgs(TreeNodeModel nodeModel, NodeOperatorType opType)
        {
            this.NodeModel = nodeModel;
            this.OpType = opType;
        }
    }
}