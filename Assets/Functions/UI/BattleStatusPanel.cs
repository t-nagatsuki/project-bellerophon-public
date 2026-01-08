using Functions.Data;
using Functions.Data.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Functions.UI
{
    public class BattleStatusPanel
    {
        private VisualTreeAsset templateButton;

        private Color32 colorHpHigh = Color.green;
        private Color32 colorHpLow = Color.yellow;
        private Color32 colorHpDanger = Color.red;
        private Color32 colorEn = Color.cyan;
        private Color32 colorSp = Color.magenta;
        private StyleColor colorNormal;

        private VisualElement backPanel;
        private VisualElement image;
        private Label lblUnitName;
        private Label lblCharacterName;
        private Label lblLv;
        private ProgressBar barHp;
        private ProgressBar barEn;
        private ProgressBar barSp;

        private VisualElement divInfos;
        private VisualElement divMenu;

        private Label lblActionName;
        private Label lblNote;
        private Label lblPercentTitle;
        private Label lblPercent;
        private Label lblCriticalTitle;
        private Label lblCritical;

        public BattleStatusPanel(VisualElement _root, Color32 _colorHpHigh, Color _colorHpLow, Color _colorHpDanger, Color _colorEn, Color _colorSp, VisualTreeAsset _btn)
        {
            colorHpHigh = _colorHpHigh;
            colorHpLow = _colorHpLow;
            colorHpDanger = _colorHpDanger;
            colorEn = _colorEn;
            colorSp = _colorSp;
            templateButton = _btn;

            backPanel = _root.Q<VisualElement>("BackPanel");
            image = _root.Q<VisualElement>("Image");
            lblUnitName = _root.Q<Label>("LblUnitName");
            lblCharacterName = _root.Q<Label>("LblCharacterName");
            lblLv = _root.Q<Label>("LblLV");
            barHp = _root.Q<ProgressBar>("BarHP");
            barEn = _root.Q<ProgressBar>("BarEN");
            barSp = _root.Q<ProgressBar>("BarSP");

            divInfos = _root.Q<VisualElement>("DivInfos");
            divMenu = _root.Q<VisualElement>("DivMenu");

            lblActionName = _root.Q<Label>("LblActionName");
            lblNote = _root.Q<Label>("LblNote");
            lblPercentTitle = _root.Q<Label>("LblPercentTitle");
            lblPercent = _root.Q<Label>("LblPercent");
            lblCriticalTitle = _root.Q<Label>("LblCriticalTitle");
            lblCritical = _root.Q<Label>("LblCritical");

            colorNormal = backPanel.style.backgroundColor;
        }

        public void SetColor(Color32 _color)
        {
            backPanel.style.backgroundColor = new StyleColor( new Color32(_color.r, _color.g, _color.b, 123));
        }

        public void ResetColor()
        {
            backPanel.style.backgroundColor = colorNormal;
        }

        public Button AddAction(string _text, FontStyle _style)
        {
            divMenu.style.display = DisplayStyle.Flex;
            var btn = templateButton.Instantiate();
            var elmtBtn = btn.Q<Button>("BtnAction");
            elmtBtn.text = _text;
            elmtBtn.style.unityFontStyleAndWeight = _style;
            divMenu.Add(btn);
            return elmtBtn;
        }

        public void ResetAction()
        {
            divMenu.Clear();
            divMenu.style.display = DisplayStyle.None;
        }

        public void SetStatus(PermanenceUnitData _unit, PermanenceCharacterData _chara)
        {
            image.style.backgroundImage = _chara.Character.CharacterImage;
            lblUnitName.text = _unit.UnitName;
            lblCharacterName.text = _chara.CharacterName;
            lblLv.text = $"{_chara.LV.Now}";

            barHp.title = _unit.HP.DisplayText;
            barHp.highValue = _unit.HP.Max;
            barHp.value = _unit.HP.Now;
            if ((double)_unit.HP.Now / _unit.HP.Max > 0.5)
            { barHp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor( colorHpHigh); }
            else if ((double)_unit.HP.Now / _unit.HP.Max > 0.2)
            { barHp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorHpLow); }
            else
            { barHp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorHpDanger); }

            barEn.title = _unit.EN.DisplayText;
            barEn.highValue = _unit.EN.Max;
            barEn.value = _unit.EN.Now;
            barEn.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorEn);

            barSp.title = _chara.SP.DisplayText;
            barSp.highValue = _chara.SP.Max;
            barSp.value = _chara.SP.Now;
            barSp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorSp);
        }

        public void SetInfo(string _action, string _note, string _title1, string _value1, string _title2, string _value2)
        {
            divInfos.style.display = DisplayStyle.Flex;
            lblActionName.text = _action;
            lblNote.text = _note;
            lblPercentTitle.text = _title1;
            lblPercent.text = _value1;
            lblCriticalTitle.text = _title2;
            lblCritical.text = _value2;
        }

        public void ResetInfo()
        {
            divInfos.style.display = DisplayStyle.None;
        }
    }
}