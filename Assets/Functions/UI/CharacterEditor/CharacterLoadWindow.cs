using System.IO;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Functions.UI.CharacterEditor
{
    public class CharacterLoadWindow : BaseWindow
    {
        private Button btnAccept;
        private Button btnCancel;
        private DropdownField drpLoadFile;

        public override void Setup()
        {
            btnAccept = document.rootVisualElement.Q<Button>("BtnAccept");
            btnCancel = document.rootVisualElement.Q<Button>("BtnCancel");
            drpLoadFile = document.rootVisualElement.Q<DropdownField>("DrpLoadFile");
        }

        public void SetManager(CharacterEditorManager mng)
        {
            btnAccept.clicked += () =>
            {
                if (drpLoadFile.index == -1)
                { return; }
                mng.WaitNavigate = 0;
                HiddenDisplay();
                mng.LoadCharacters(drpLoadFile.value);
            };
            btnCancel.clicked += () =>
            {
                mng.WaitNavigate = 0;
                HiddenDisplay();
            };
        }

        public void InitializeDisplay(CharacterEditorManager mng)
        {
            drpLoadFile.choices.Clear();
            drpLoadFile.index = -1;
            foreach (var path in DataUtil.GetCharacters("*.json"))
            { drpLoadFile.choices.Add(Path.GetFileNameWithoutExtension(path)); }
            foreach (var path in DataUtil.GetCharacters("*.yml"))
            { drpLoadFile.choices.Add(Path.GetFileNameWithoutExtension(path)); }
        }
    }
}
