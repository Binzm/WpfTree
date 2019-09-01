using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TreeLibrary;
using TreeLibrary.Model;

namespace TreeTest.ProductAndCustomer
{
    public class Consumer
    {
        private readonly AsyncStack _asyncStack = null;
        private readonly TreeControl _treeControl;
        private readonly Random _random = new Random();
        private readonly Window _window;

        public Consumer(AsyncStack asyncStack, TreeControl treeControl, Window window)
        {
            _asyncStack = asyncStack;
            _treeControl = treeControl;
            _window = window;
        }

        public void RunConsume()
        {
            if(!_asyncStack.IsHaveProduction())
                return;
            lock (_treeControl)
            {
                var runMethodOnLineOrOffLineBool = _asyncStack.Pop();
                if (runMethodOnLineOrOffLineBool)
                {
                    NodeOnLine();
                }
                else
                {
                    NodeOffLine();
                }
            }
           
        }

      

        private void NodeOnLine()
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var offLineNode = _treeControl.TreeHelper
                        .TreeAllNodels.Where(f => f.IsVisibility.ContainsKey(false)).ToList();
                    if (offLineNode.Count <= 0)
                        return;

                    var onLineNode = offLineNode[_random.Next(0, offLineNode.Count - 1)];
                    var nodeParent = onLineNode.IsVisibility.Values.First();
                    onLineNode.IsVisibility = new Dictionary<bool, TreeNodeModel>
                    {
                        {true, nodeParent}
                    };
                    if (!nodeParent.SubNodes.Contains(onLineNode))
                        nodeParent.AddSubNode(onLineNode);

                    ((TextBlock) _window.FindName("OnLineEquipmentTextBlock")??new TextBlock()).Text =
                        $"当前在线设备数：{(_treeControl.TreeHelper.TreeAllNodels.Count - offLineNode.Count + 1)}";
                    ((TextBlock) _window.FindName("OffLineEquipmentTextBlock")??new TextBlock()).Text =
                        $"当前离线设备数：{offLineNode.Count - 1}";
                    GetThisTreeNodeCount();
                });
            });
        }

        private void NodeOffLine()
        {
            var onLineNodeList = _treeControl.TreeHelper
                .TreeAllNodels.Where(f => f.IsVisibility.ContainsKey(true)).ToList();
            if (onLineNodeList.Count <= 0)
                return;

            var offLineNode = onLineNodeList[_random.Next(0, onLineNodeList.Count - 1)];

            Task.Run(() =>
            {
                var nodeParent = offLineNode.IsVisibility.Values.First();
                offLineNode.IsVisibility.Clear();
                offLineNode.IsVisibility.Add(false, nodeParent);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _treeControl.RemoveNodeItem(offLineNode);

                    ((TextBlock) _window.FindName("OnLineEquipmentTextBlock")??new TextBlock()).Text =
                        $"当前在线设备数：{(onLineNodeList.Count > 1 ? onLineNodeList.Count : 0)}";

                    var offLineEquipmentCount = onLineNodeList.Count > 1
                        ? (_treeControl.TreeHelper.TreeAllNodels.Count - onLineNodeList.Count)
                        : (_treeControl.TreeHelper.TreeAllNodels.Count - onLineNodeList.Count) + 1;
                    ((TextBlock) _window.FindName("OffLineEquipmentTextBlock")??new TextBlock()).Text =
                        $"当前离线设备数：{offLineEquipmentCount}";
                    GetThisTreeNodeCount();
                });
            });
        }

        private void GetThisTreeNodeCount()
        {
            if (_treeControl == null)
                return;
            try
            {
                int nodeCount = 0;
                ((TextBlock)_window.FindName("TreeNodeCountTextBlock") ?? new TextBlock()).Text =
                    $"当前树节点个数为：{GetNodeCount(_treeControl.TreeHelper.NodeList, nodeCount)}";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
    }
}