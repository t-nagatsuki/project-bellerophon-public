using System.Collections.Generic;
using System.Linq;
using Functions.Data;
using Functions.Data.Units;
using Functions.Enum;
using Functions.Manager;
using UnityEngine;
using UnityEngine.UIElements;
namespace Functions.UI
{
    public class ArrangementUnitsWindow : BaseWindow
    {
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
        private DirectionType direction;

        public override void Setup()
        {
            list = document.rootVisualElement.Q<ScrollView>("List");
        }

        public void SetupDisplay(SlgSceneManager mng, ArrangementData[] lst, SortedDictionary<int, GroupData> grp, Dictionary<string, PermanenceUnitData> unit, Dictionary<string, PermanenceCharacterData> chara, DirectionType dir)
        {
            direction = dir;
            list.Clear();
            foreach (var dat in lst)
            {
                if (!grp.TryGetValue(dat.GroupId, out var grpData))
                { continue; }
                if (!unit.TryGetValue(dat.UnitId, out var unitData))
                { continue; }
                chara.TryGetValue(dat.CharacterId, out var charaData);
                if (!grpData.Player)
                { continue; }
                if (unitData.Hidden || charaData == null)
                { continue; }
                if (dat.IsArrangement || dat.IsTemporaryArrangement)
                { continue; }
                var record = SetUnitRecord(grpData, unitData, charaData);
                record.RegisterCallback<ClickEvent>(v =>
                {
                    mng.ArrangementUnit(unitData.UnitId, mng.CursorPosition, dir);
                    mng.ProcessMode = ProcessMode.ArrangementUnits;
                    HiddenDisplay();
                });
            }
        }

        private TemplateContainer SetUnitRecord(GroupData grp, PermanenceUnitData unit, PermanenceCharacterData chara)
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
            return record;
        }
    }
}
