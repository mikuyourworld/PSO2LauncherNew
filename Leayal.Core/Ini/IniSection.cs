namespace Leayal.Ini
{
    public class IniSection
    {
        private bool m_IsComment;
        public bool IsComment
        {
            get { return this.m_IsComment; }
            set { this.m_IsComment = value; }
        }

        private System.Collections.Generic.List<string> m_CommentList;
        public System.Collections.Generic.List<string> CommentList
        {
            get { return this.m_CommentList; }
        }

        private System.Collections.Generic.Dictionary<string, IniKeyValue> m_ListOfIniKeyValue;
        public System.Collections.Generic.Dictionary<string, IniKeyValue> IniKeyValues
        {
            get { return this.m_ListOfIniKeyValue; }
        }

        public IniSection() : this(false)
        {
        }

        public IniSection(bool IsComment)
        {
            this.m_IsComment = IsComment;
            this.m_ListOfIniKeyValue = new System.Collections.Generic.Dictionary<string, IniKeyValue>();
            this.m_CommentList = new System.Collections.Generic.List<string>();
        }

        public void Clear()
        {
            this.m_ListOfIniKeyValue.Clear();
            this.m_CommentList.Clear();
        }
    }
}
