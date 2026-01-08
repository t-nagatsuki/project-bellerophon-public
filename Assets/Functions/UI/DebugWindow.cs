using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace Functions.UI
{
    public class DebugWindow : BaseWindow
    {
        [SerializeField]
        private VisualTreeAsset templateDisplay;

        private VisualElement divDebug;
        private readonly Dictionary<string, TemplateContainer> dictData = new();

        public override void Setup()
        {
            document.rootVisualElement.style.display = DisplayStyle.None;
            divDebug = document.rootVisualElement.Q<VisualElement>("DivDebug");
        }
        
        public void SetData(string key, string value)
        {
            if (!dictData.ContainsKey(key))
            {
                var dat = templateDisplay.Instantiate();
                dat.Q<Label>("LblData").text = key;
                dictData[key] = dat;
                divDebug.Add(dat);
            }
            dictData[key].Q<Label>("TxtData").text = value;
        }

        public void RemoveData(string key)
        {
            dictData.Remove(key);
        }
    }
}
