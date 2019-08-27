using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace TreeLibrary.Model
{
    public class TreeNodeModel : BaseNotifyPropertyChanged
    {
        private bool _isExpanded;
        private bool _isSelected;

        private bool? _isChecked;
        private string _name = string.Empty;
        private Visibility _showCheckBox = Visibility.Visible;
        private string _textBoxForeground;
        private string _iconImage;
        private readonly TreeNodeModel _dummyChild;

        private Dictionary<bool, TreeNodeModel> _isVisibility = new Dictionary<bool, TreeNodeModel>();

        private  ObservableCollection<TreeNodeModel> _subNodes = new ObservableCollection<TreeNodeModel>();

        private DataModel _data;

        public DataModel Data
        {
            get => _data;
            set
            {
                this._data = value;
                this.OnPropertyChanged();
            }
        }

        public Dictionary<bool, TreeNodeModel> IsVisibility
        {
            get => _isVisibility;
            set
            {
                this._isVisibility = value;
                this.OnPropertyChanged();
            }
        }


        public TreeNodeModel Parent { get; set; }


        public string IconImage
        {
            get => _iconImage;
            set
            {
                this._iconImage = value;
                this.OnPropertyChanged();
            }
        }

        public string TextBoxForeground
        {
            get => _textBoxForeground;
            set
            {
                this._textBoxForeground = value;
                this.OnPropertyChanged();
            }
        }

        public Visibility ShowCheckBox
        {
            get => _showCheckBox;
            set
            {
                this._showCheckBox = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get => this._isExpanded;
            set
            {
                this._isExpanded = value;
                this.OnPropertyChanged();
                if (!this.HasDummyChild)
                    return;
                this.SubNodes.Remove(_dummyChild);
                this.LoadSubNodes();
            }
        }

        protected virtual object LoadSubNodes()
        {
            return null;
        }

        public bool HasDummyChild
        {
            get => this._subNodes.Count == 0 && _dummyChild != null;
        }

        public bool IsSelected
        {
            get => this._isSelected;
            set
            {
                if (this._isSelected != value)
                {
                    this._isSelected = value;
                    this.OnPropertyChanged();
                }
            }
        }


        public bool? IsChecked
        {
            get => this._isChecked;
            set
            {
                if (this._isChecked != value)
                {
                    this._isChecked = value;
                    this.OnPropertyChanged();

                    if (this._isChecked == true)
                    {
                        if (this._subNodes != null)
                            foreach (var dt in this._subNodes)
                                dt.IsChecked = true;

                        if (this.Parent != null)
                        {
                            bool bExistUncheckedChildren = false;
                            foreach (var dt in this.Parent.SubNodes)
                                if (dt.IsChecked != true)
                                {
                                    bExistUncheckedChildren = true;
                                    break;
                                }

                            if (bExistUncheckedChildren)
                                this.Parent.IsChecked = null;
                            else
                                this.Parent.IsChecked = true;
                        }
                    }
                    else if (this._isChecked == false)
                    {
                        if (this._subNodes != null)
                            foreach (var dt in this._subNodes)
                                dt.IsChecked = false;

                        if (this.Parent != null)
                        {
                            Boolean bExistCheckedChildren = false;
                            foreach (var dt in this.Parent.SubNodes)
                                if (dt.IsChecked != false)
                                {
                                    bExistCheckedChildren = true;
                                    break;
                                }

                            if (bExistCheckedChildren)
                                this.Parent.IsChecked = null;
                            else
                                this.Parent.IsChecked = false;
                        }
                    }
                    else
                    {
                        if (this.Parent != null)
                            this.Parent.IsChecked = null;
                    }
                }
            }
        }


        public string Name
        {
            get => this._name;
            set
            {
                if (this._name != value)
                {
                    this._name = value;
                    this.OnPropertyChanged();
                }
            }
        }


        public TreeNodeModel(bool isLazyLoadSubNodes)
        {
            this._isChecked = false;
            if (isLazyLoadSubNodes)
            {
                this._dummyChild = new TreeNodeModel();
            }
        }

        public TreeNodeModel()
        {
            this._isChecked = false;
        }

        public virtual void AddSubNode(TreeNodeModel subNode)
        {
            if (subNode == null)
            {
                return;
            }

            subNode.Parent = this;
            this._subNodes.Add(subNode);

            subNode.IsVisibility.Add(true, this);
        }

        public ObservableCollection<TreeNodeModel> SubNodes
        {
            get => this._subNodes;
        }
    }
}