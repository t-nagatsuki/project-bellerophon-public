using Functions.Data;
using Functions.Manager;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Functions.UI
{
    public class CommandMenuWindow : BaseWindow
    {
        [SerializeField]
        private VisualTreeAsset templateButton;

        private VisualElement divMenu;

        public override void Setup()
        {
            divMenu = document.rootVisualElement.Q<VisualElement>("DivMenu");
        }

        public Button AddMenu(ScriptManager mng, MenuData dat)
        {
            var btn = templateButton.Instantiate();
            var elmtBtn = btn.Q<Button>("Button");
            elmtBtn.text = mng.AnalysisEmbeddedvariable(dat.Text);
            if (dat.IsBold && !dat.IsItalic)
            { elmtBtn.style.unityFontStyleAndWeight = FontStyle.Bold; }
            else if (!dat.IsBold && dat.IsItalic)
            { elmtBtn.style.unityFontStyleAndWeight = FontStyle.Italic; }
            else if (dat.IsBold && dat.IsItalic)
            { elmtBtn.style.unityFontStyleAndWeight = FontStyle.BoldAndItalic; }
            divMenu.Add(btn);
            return elmtBtn;
        }

        public Button AddMenu(ScriptManager mng, string text, FontStyle style)
        {
            var btn = templateButton.Instantiate();
            var elmtBtn = btn.Q<Button>("Button");
            elmtBtn.text =  mng.AnalysisEmbeddedvariable(text);
            elmtBtn.style.unityFontStyleAndWeight = style;
            divMenu.Add(btn);
            return elmtBtn;
        }

        public void ClearButton()
        {
            divMenu.Clear();
        }

        public int Count
        {
            get => divMenu.childCount;
        }
    }
}
