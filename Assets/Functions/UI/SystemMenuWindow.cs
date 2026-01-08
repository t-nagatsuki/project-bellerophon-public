using System.Collections.Generic;
using Functions.Data;
using Functions.Manager;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Functions.UI
{
    public class SystemMenuWindow : BaseWindow
    {
        [SerializeField]
        private VisualTreeAsset templateButton;

        private int idxSelect;
        private VisualElement divMenu;
        private Dictionary<int, List<TemplateContainer>> dictButton = new Dictionary<int, List<TemplateContainer>>();

        public override void Setup()
        {
            divMenu = document.rootVisualElement.Q<VisualElement>("DivMenu");
        }

        public Button AddMenu(ScriptManager mng, MenuData dat)
        {
            var btn = templateButton.Instantiate();
            btn.name = $"{divMenu.childCount}";
            var elmtBtn = btn.Q<Button>("Button");
            elmtBtn.text = mng.AnalysisEmbeddedvariable(dat.Text);
            if (dat.IsBold && !dat.IsItalic)
            { elmtBtn.style.unityFontStyleAndWeight = FontStyle.Bold; }
            else if (!dat.IsBold && dat.IsItalic)
            { elmtBtn.style.unityFontStyleAndWeight = FontStyle.Italic; }
            else if (dat.IsBold && dat.IsItalic)
            { elmtBtn.style.unityFontStyleAndWeight = FontStyle.BoldAndItalic; }
            divMenu.Add(btn);
            if (!dictButton.ContainsKey(dat.Group))
            {
                dictButton[dat.Group] = new List<TemplateContainer>();
            }
            dictButton[dat.Group].Add(btn);
            return elmtBtn;
        }

        public void ClearButton()
        {
            idxSelect = 0;
            divMenu.Clear();
        }

        public void PrevButton()
        {
            if (divMenu.childCount == 0)
            { return; }
            idxSelect = math.clamp(idxSelect - 1, 0, divMenu.childCount - 1);
            var btn = divMenu.Q<VisualElement>($"{idxSelect}");
            var elmtBtn = btn.Q<Button>("Button");
            elmtBtn.Focus();
        }

        public void NextButton()
        {
            if (divMenu.childCount == 0)
            { return; }
            idxSelect = math.clamp(idxSelect + 1, 0, divMenu.childCount - 1);
            var btn = divMenu.Q<VisualElement>($"{idxSelect}");
            var elmtBtn = btn.Q<Button>("Button");
            elmtBtn.Focus();
        }

        public void SetDisplayButton(int group, bool display)
        {
            if (!dictButton.ContainsKey(group))
            { return; }

            foreach (var btn in dictButton[group])
            {
                btn.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }
}
