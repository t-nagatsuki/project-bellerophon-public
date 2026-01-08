using Functions.Data;
using Functions.Data.Units;
using UnityEngine;
using UnityEngine.UIElements;
namespace Functions.UI
{
    public class SimpleStatusWindow : BaseWindow
    {
        [SerializeField]
        private Color32 colorHpHigh = Color.green;
        [SerializeField]
        private Color32 colorHpLow = Color.yellow;
        [SerializeField]
        private Color32 colorHpDanger = Color.red;
        [SerializeField]
        private Color32 colorEn = Color.cyan;
        [SerializeField]
        private Color32 colorSp = Color.magenta;

        private VisualElement image;
        private Label lblUnitName;
        private Label lblCharacterName;
        private Label lblLv;
        private ProgressBar barExp;
        private ProgressBar barHp;
        private ProgressBar barEn;
        private ProgressBar barSp;

        public override void Setup()
        {
            image = document.rootVisualElement.Q<VisualElement>("Image");
            lblUnitName = document.rootVisualElement.Q<Label>("LblUnitName");
            lblCharacterName = document.rootVisualElement.Q<Label>("LblCharacterName");
            lblLv = document.rootVisualElement.Q<Label>("LblLV");
            barExp = document.rootVisualElement.Q<ProgressBar>("BarEXP");
            barHp = document.rootVisualElement.Q<ProgressBar>("BarHP");
            barEn = document.rootVisualElement.Q<ProgressBar>("BarEN");
            barSp = document.rootVisualElement.Q<ProgressBar>("BarSP");
        }

        public void SetStatus(PermanenceUnitData unit, PermanenceCharacterData chara)
        {
            image.style.backgroundImage = chara.Character.CharacterImage;
            lblUnitName.text = unit.UnitName;
            lblCharacterName.text = chara.CharacterName;
            lblLv.text = $"{chara.LV.Now}";

            barExp.title = chara.EXP.DisplayText;
            barExp.highValue = chara.EXP.Max;
            barExp.value = chara.EXP.Now;

            barHp.title = unit.HP.DisplayText;
            barHp.highValue = unit.HP.Max;
            barHp.value = unit.HP.Now;
            if ((double)unit.HP.Now / unit.HP.Max > 0.5)
            { barHp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorHpHigh); }
            else if ((double)unit.HP.Now / unit.HP.Max > 0.2)
            { barHp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorHpLow); }
            else
            { barHp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorHpDanger); }

            barEn.title = unit.EN.DisplayText;
            barEn.highValue = unit.EN.Max;
            barEn.value = unit.EN.Now;
            barEn.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorEn);

            barSp.title = chara.SP.DisplayText;
            barSp.highValue = chara.SP.Max;
            barSp.value = chara.SP.Now;
            barSp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorSp);
        }
    }
}
