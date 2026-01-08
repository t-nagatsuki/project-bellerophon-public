using System.Collections.Generic;
using System.Linq;
using Functions.Data;
using Functions.Data.Units;
using UnityEngine;
using UnityEngine.UIElements;
namespace Functions.UI
{
    public class ListWindow : BaseWindow
    {
        [SerializeField]
        private VisualTreeAsset charaRecord;
        [SerializeField]
        private VisualTreeAsset unitRecord;
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

        private ScrollView list;

        public override void Setup()
        {
            list = document.rootVisualElement.Q<ScrollView>("List");
        }

        public void SetUnitDisplay(bool intermission, ArrangementData[] lst, SortedDictionary<int, GroupData> grp, Dictionary<string, PermanenceUnitData> unit, Dictionary<string, PermanenceCharacterData> chara)
        {
            list.Clear();
            foreach (var dat in lst)
            {
                if (!grp.TryGetValue(dat.GroupId, out var grpData))
                { continue; }
                if (!unit.TryGetValue(dat.UnitId, out var unitData))
                { continue; }
                chara.TryGetValue(dat.CharacterId, out var charaData);
                if (intermission)
                {
                    if (!grpData.Player)
                    { continue; }
                    if (unitData.Hidden)
                    { continue; }
                }
                else
                {
                    if (!dat.IsArrangement)
                    { continue; }
                }
                SetUnitRecord(grpData, unitData, charaData);
            }
        }

        public void SetCharaDisplay(bool intermission, ArrangementData[] lst, SortedDictionary<int, GroupData> grp, Dictionary<string, PermanenceCharacterData> chara)
        {
            list.Clear();
            if (intermission)
            {
                var grpData = grp.Values.First(x => x.Player);
                foreach (var dat in chara.Values)
                {
                    if (dat.Hidden)
                    { continue; }
                    SetCharaRecord(grpData, dat);
                }
            }
            else
            {
                foreach (var dat in lst)
                {
                    if (!dat.IsArrangement)
                    { continue; }
                    SetCharaRecord(grp[dat.GroupId], chara[dat.CharacterId]);
                }
            }
        }

        private void SetUnitRecord(GroupData grp, PermanenceUnitData unit, PermanenceCharacterData chara)
        {
            var record = unitRecord.Instantiate();
            var back = record.Q<VisualElement>("UnitRecord");
            var img = record.Q<VisualElement>("Image");
            var unitName = record.Q<Label>("LblUnitName");
            var charaName = record.Q<Label>("LblCharacterName");
            var lv = record.Q<Label>("LblLV");
            var hp = record.Q<ProgressBar>("BarHP");
            var en = record.Q<ProgressBar>("BarEN");
            var sp = record.Q<ProgressBar>("BarSP");
            SetColor(back, grp.GroupColor);
            img.style.backgroundImage = unit.Unit.UnitImage;
            unitName.text = unit.UnitName;

            hp.title = unit.HP.DisplayText;
            hp.highValue = unit.HP.Max;
            hp.value = unit.HP.Now;
            if ((double)unit.HP.Now / unit.HP.Max > 0.5)
            { hp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor( colorHpHigh); }
            else if ((double)unit.HP.Now / unit.HP.Max > 0.2)
            { hp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorHpLow); }
            else
            { hp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorHpDanger); }

            en.title = unit.EN.DisplayText;
            en.highValue = unit.EN.Max;
            en.value = unit.EN.Now;
            en.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorEn);

            if (chara != null)
            {
                charaName.text = chara.CharacterName;
                lv.text = $"{chara.LV.Now}";
                
                sp.title = chara.SP.DisplayText;
                sp.highValue = chara.SP.Max;
                sp.value = chara.SP.Now;
                sp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorSp);
            }
            else
            {
                charaName.text = "未搭乗";
                lv.text = "－";
                sp.title = string.Empty;
                sp.highValue = 0;
                sp.value = 0;
            }

            list.Add(record);
        }

        private void SetCharaRecord(GroupData _grp, PermanenceCharacterData _chara)
        {
            var record = charaRecord.Instantiate();
            var back = record.Q<VisualElement>("CharaRecord");
            var img = record.Q<VisualElement>("Image");
            var charaName = record.Q<Label>("LblCharacterName");
            var lv = record.Q<Label>("LblLV");
            var exp = record.Q<ProgressBar>("BarEXP");
            var sp = record.Q<ProgressBar>("BarSP");

            SetColor(back, _grp.GroupColor);
            img.style.backgroundImage = _chara.Character.CharacterImage;
            charaName.text = _chara.CharacterName;

            lv.text = $"Lv. {_chara.LV.Now}";
            exp.title = _chara.EXP.DisplayText;
            exp.highValue = _chara.EXP.Max;
            exp.value = _chara.EXP.Now;

            sp.title = _chara.SP.DisplayText;
            sp.highValue = _chara.SP.Max;
            sp.value = _chara.SP.Now;
            sp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorSp);
            list.Add(record);
        }
        
        private void SetColor(VisualElement _elm, Color32 _color)
        {
            _elm.style.backgroundColor = new StyleColor( new Color32(_color.r, _color.g, _color.b, 123));
        }
    }
}
