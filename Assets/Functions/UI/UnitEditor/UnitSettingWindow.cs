using Functions.Data.Units;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
using UnityEngine.UIElements;

namespace Functions.UI.UnitEditor
{
    public class UnitSettingWindow : MonoBehaviour
    {
        private UIDocument document;
        private Button btnAccept;
        private Button btnCancel;
        private TextField txtFileName;
        private TextField txtUnitId;

        private bool isNewUnits;
        private bool isAddUnit;

        private UnitData unit;
        
        private void Awake()
        {
            document = GetComponent<UIDocument>();
            document.rootVisualElement.style.display = DisplayStyle.None;
            btnAccept = document.rootVisualElement.Q<Button>("BtnAccept");
            btnCancel = document.rootVisualElement.Q<Button>("BtnCancel");
            txtFileName = document.rootVisualElement.Q<TextField>("TxtFileName");
            txtUnitId = document.rootVisualElement.Q<TextField>("TxtUnitId");
        }

        public void SetManager(UnitEditorManager mng)
        {
            btnAccept.clicked += () =>
            {
                if (string.IsNullOrWhiteSpace(txtFileName.value))
                {
                    mng.EditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_S0001", LocaleUtil.GetEntry("lbl_name_file")));
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtUnitId.value))
                {
                    mng.EditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_S0001", LocaleUtil.GetEntry("lbl_id_unit")));
                    return;
                }
                mng.FileName = txtFileName.value;
                if (isNewUnits)
                {
                    mng.InitializeUnit(new UnitData(txtUnitId.value));
                }
                else
                {
                    if (isAddUnit)
                    {
                        if (mng.Units.ContainsKey(unit.UnitId))
                        {
                            mng.EditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_C0002", LocaleUtil.GetEntry("lbl_id_unit")));
                            return;
                        }
                        unit = new UnitData(txtUnitId.value);
                    }
                    else
                    {
                        if (mng.Unit.UnitId != unit.UnitId && mng.Units.ContainsKey(unit.UnitId))
                        {
                            mng.EditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_C0002", LocaleUtil.GetEntry("lbl_id_unit")));
                            return;
                        }
                        mng.RemoveUnit(mng.Unit.UnitId);
                    }
                    mng.EditUnit(unit);
                }
                mng.WaitNavigate = 0;
                document.rootVisualElement.style.display = DisplayStyle.None;
            };
            btnCancel.clicked += () =>
            {
                mng.WaitNavigate = 0;
                document.rootVisualElement.style.display = DisplayStyle.None;
            };
        }

        public void InitializeDisplay(UnitEditorManager mng)
        {
            isNewUnits = true;
            isAddUnit = false;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            txtFileName.value = "default_units";
            txtUnitId.value = "default_unit";
        }

        public void AddDisplay(UnitEditorManager mng)
        {
            isNewUnits = false;
            isAddUnit = true;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            txtFileName.value = mng.FileName;
            txtUnitId.value = "default_unit";
        }

        public void EditDisplay(UnitEditorManager mng)
        {
            isNewUnits = false;
            isAddUnit = false;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            txtFileName.value = mng.FileName;
            unit = mng.Unit;
            txtUnitId.value = unit.UnitId;
        }

        public bool IsDisplay()
        {
            return document.rootVisualElement.style.display == DisplayStyle.Flex;
        }
    }
}
