
namespace ICMS.WPFControlsLibrary
{
    /// <summary>
    /// 自动检索实体
    /// </summary>
    public class AutoCompleteEntry
    {
        public string[] KeywordStrings
        {
            get
            {
                if (this._keywordStrings == null)
                {
                    this._keywordStrings = new string[]
                    {
                        this._displayText
                    };
                }

                return this._keywordStrings;
            }
        }


        public string DisplayName
        {
            get => this._displayText; 
            set  =>this._displayText = value; 
        }


        public AutoCompleteEntry(string name, params string[] keywords)
        {
            this._displayText = name;
            this._keywordStrings = keywords;
        }


        public object ValueText
        {
            get =>  this._valueText; 
            set => this._valueText = value; 
        }


        public AutoCompleteEntry(object value, string name, params string[] keywords) : this(name, keywords)
        {
            this._valueText = value;
        }


        public override string ToString()
        {
            return this._displayText;
        }


        private string[] _keywordStrings;


        private string _displayText;


        private object _valueText;
    }
}