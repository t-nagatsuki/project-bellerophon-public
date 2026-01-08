using System.IO;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
namespace Functions.UI.TileEditor
{
    public class TileLoadWindow : BaseWindow
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

        public void SetManager(TileEditorManager mng)
        {
            btnAccept.clicked += () =>
            {
                if (drpLoadFile.index == -1)
                { return; }
                mng.WaitNavigate = 0;
                HiddenDisplay();
                mng.LoadTiles(drpLoadFile.value);
            };
            btnCancel.clicked += () =>
            {
                mng.WaitNavigate = 0;
                HiddenDisplay();
            };
        }

        public void InitializeDisplay(TileEditorManager mng)
        {
            drpLoadFile.choices.Clear();
            drpLoadFile.index = -1;
            foreach (var path in DataUtil.GetTiles("*.json"))
            { drpLoadFile.choices.Add(Path.GetFileNameWithoutExtension(path)); }
            foreach (var path in DataUtil.GetTiles("*.yml"))
            { drpLoadFile.choices.Add(Path.GetFileNameWithoutExtension(path)); }
        }
    }
}
