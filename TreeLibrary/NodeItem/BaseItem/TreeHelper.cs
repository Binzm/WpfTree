using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using TreeLibrary.Extensions;
using TreeLibrary.Model;

namespace TreeLibrary.NodeItem.BaseItem
{
    public class TreeHelper : BaseNotifyPropertyChanged
    {
        private Visibility _titleVisibility;
        public ObservableCollection<TreeNodeModel> TreeAllNodels = new ObservableCollection<TreeNodeModel>();

        public ObservableCollection<TreeNodeModel> NodeList { get; set; } = new ObservableCollection<TreeNodeModel>();

        public string SearchKey { get; set; }

        public bool ShowTime { get; set; }

        public Visibility TitleVisibility
        {
            get => this._titleVisibility;
            set
            {
                this._titleVisibility = value;
                this.OnPropertyChanged();
            }
        }

        public TreeHelper()
        {
            this.SearchKey = "";
        }

        public void SetItemsSource(ObservableCollection<TreeNodeModel> itemSource)
        {
            NodeList = itemSource;
        }

        public void GetAllCheckedItem(ObservableCollection<TreeNodeModel> resultCol,
            ObservableCollection<TreeNodeModel> subNode)
        {
            foreach (TreeNodeModel model in subNode ?? NodeList)
            {
                if (true == model.IsChecked)
                {
                    resultCol.Add(model);
                }

                if (model.SubNodes.Count>0)
                {
                    GetAllCheckedItem(resultCol, model.SubNodes);
                }
            }
        }

        public void GetAppointedPatternNode(string pattern, ObservableCollection<TreeNodeModel> resultCol,
            ObservableCollection<TreeNodeModel> subNode)
        {
            foreach (TreeNodeModel model in subNode == null ? NodeList : subNode)
            {
                if (model.Name.Contains(pattern) | this.ConvertChineseString(model.Name).ToLower().Contains(pattern))
                {
                    resultCol.Add(model);
                }

                if (model.SubNodes.Count>0)
                {
                    GetAppointedPatternNode(pattern, resultCol, model.SubNodes);
                }
            }
        }

        private string ConvertChineseString(string str)
        {
            string result;
            if (str == null)
            {
                result = "";
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < str.Length; i++)
                {
                    stringBuilder.Append(StringObject.GetChineseFirstChar(str[i]));
                }

                result = stringBuilder.ToString();
            }

            return result;
        }

        /// <summary>
        /// 在树节点上挂载单个子节点
        /// </summary>
        /// <param name="addNode">要添加的子节点</param>
        /// <param name="mountPointModel">挂载的实体</param>
        public void AddNodeModelToItem(TreeNodeModel addNode, TreeNodeModel mountPointModel)
        {
            var mountPointIndex = TreeAllNodels.ToList().FindIndex(f => f.Data.Id == mountPointModel.Data.Id);
            if (mountPointIndex >= 0)
            {
                var treeNode = GetNodeByNodeList(mountPointModel, null);
                if (treeNode != null)
                {
                    if (TreeAllNodels.ToList().FindIndex(f => f.Data.Id == addNode.Data.Id) >= 0)
                        return;
                    treeNode.AddSubNode(addNode);
                    TreeAllNodels.Add(addNode);
                }
            }
        }
        /// <summary>
        /// 在树节点上挂载多个个子节点
        /// </summary>
        /// <param name="addNodeItems">要添加的子节点</param>
        /// <param name="mountPointModel">挂载的实体</param>
        public void AddNodeModelListToItem(List<TreeNodeModel> addNodeItems, TreeNodeModel mountPointModel)
        {
            var mountPointIndex = TreeAllNodels.ToList().FindIndex(f => f.Data.Id == mountPointModel.Data.Id);
            if (mountPointIndex >= 0)
            {
                var treeNode = GetNodeByNodeList(mountPointModel, null);
                if (treeNode != null)
                { 
                    foreach (var nodeItem in addNodeItems)
                    {
                        if(TreeAllNodels.ToList().FindIndex(f => f.Data.Id == nodeItem.Data.Id)>=0)
                            continue;
                        treeNode.AddSubNode(nodeItem);
                        TreeAllNodels.Add(nodeItem);
                    }
                }
            }
        }

      

        private TreeNodeModel GetNodeByNodeList(TreeNodeModel modelItem,
            ObservableCollection<TreeNodeModel> treeNodeModels)
        {
            foreach (var treeNodeModel in treeNodeModels ?? this.NodeList)
            {
                if (modelItem.Data.Id == treeNodeModel.Data.Id)
                    return treeNodeModel;
                if (treeNodeModel.SubNodes.Count>0)
                {
                  var treeNodeModelResult =  GetNodeByNodeList(modelItem, treeNodeModel.SubNodes);
                  if (treeNodeModelResult != null)
                      return treeNodeModelResult;
                }
            }
            return null;
        }

        

        //public event NodeOperatorHandler NodeOperator;
        //public event ResourceTreeDataLoadedHandler ResourceTreeDataLoadedCompleted;
    }
}