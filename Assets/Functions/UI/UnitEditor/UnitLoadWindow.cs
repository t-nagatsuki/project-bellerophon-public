using System.IO;
using Functions.Manager;
using Functions.Util;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Functions.UI.UnitEditor
{
    public class UnitLoadWindow : BaseWindow
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

        public void SetManager(UnitEditorManager mng)
        {
            btnAccept.clicked += () =>
            {
                if (drpLoadFile.index == -1)
                { return; }
                mng.WaitNavigate = 0;
                HiddenDisplay();
                mng.LoadUnits(drpLoadFile.value);
            };
            btnCancel.clicked += () =>
            {
                mng.WaitNavigate = 0;
                HiddenDisplay();
            };
        }

        public void InitializeDisplay(UnitEditorManager mng)
        {
            drpLoadFile.choices.Clear();
            drpLoadFile.index = -1;
            foreach (var path in DataUtil.GetUnits("*.json"))
            { drpLoadFile.choices.Add(Path.GetFileNameWithoutExtension(path)); }
            foreach (var path in DataUtil.GetUnits("*.yml"))
            { drpLoadFile.choices.Add(Path.GetFileNameWithoutExtension(path)); }
        }
    }
}
