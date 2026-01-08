using System;
namespace Functions.Data
{
    [Serializable]
    public class MenuData
    {
        public string Text;
        public bool IsBold;
        public bool IsItalic;
        public string Name;
        public string Method;
        public int Group;

        public MenuData(Json.SystemMenuJson json)
        {
            Text = json.text;
            IsBold = json.bold;
            IsItalic = json.italic;
            Name = json.name;
            Method = json.method;
            Group = json.group;
        }
    }
}
