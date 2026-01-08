using System.Collections.Generic;
using System.IO;
using System.Linq;
using Functions.Data.Units;
using Functions.Json;
using Functions.Util;
using UnityEngine;

namespace Functions.Manager
{
    public class UnitEditorManager : MonoBehaviour
    {
        [SerializeField] private UnitEditorWindowManager mngWindow;

        private string nameFile = "default_units";
        private Dictionary<string, UnitData> dictUnits = new();
        /// <summary>キャラクター情報</summary>
        private UnitData unit;
        
        private BasicAction action;
        /// <summary>カーソル移動入力待機時間</summary>
        private float waitNavigate = 999;
        /// <summary>カーソル移動入力制限時間</summary>
        private const float WaitNavigateTime = 0.2f;

        private void Awake()
        {
            action = new BasicAction();
            action.Enable();
            mngWindow.Initialize(this);
            unit = null;
            InitializeUnit(new UnitData("default_unit"));
        }

        private void OnDisable()
        {
            action.Disable();
        }

        private void Update()
        {
            if (mngWindow.WindowProcess(this))
            { return; }
        }

        public async void LoadUnits(string path)
        {
            // ユニット定義読込
            nameFile = path;
            mngWindow.EditorToolBar.UnitDropdown.choices.Clear();
            var pathBase = Path.Combine(DataUtil.PathBase, "data", "unit");
            if (File.Exists(Path.Combine(pathBase, $"{path}.json"))) dictUnits = await DataUtil.LoadUnitJson(DataUtil.PathBase, Path.Combine(pathBase, $"{path}.json"), dictUnits);
            else dictUnits = await DataUtil.LoadUnitYaml(DataUtil.PathBase, Path.Combine(pathBase, $"{path}.yml"), dictUnits);
            unit = null;
            foreach (var key in dictUnits.Keys)
            { mngWindow.EditorToolBar.UnitDropdown.choices.Add(key); }
            if (dictUnits.Count > 0)
            { SetUnit(dictUnits.Keys.First()); }
        }

        public void InitializeUnit(UnitData dat)
        {
            dictUnits.Clear();
            mngWindow.EditorToolBar.UnitDropdown.choices.Clear();
            EditUnit(dat);
        }

        public void EditUnit(UnitData dat)
        {
            dictUnits[dat.UnitId] = dat;
            if (!mngWindow.EditorToolBar.UnitDropdown.choices.Contains(dat.UnitId))
            { mngWindow.EditorToolBar.UnitDropdown.choices.Add(dat.UnitId); }
            mngWindow.EditorToolBar.UnitDropdown.value = dat.UnitId;
        }
        
        public void RemoveUnit(string unitId)
        {
            dictUnits.Remove(unitId);
            mngWindow.EditorToolBar.UnitDropdown.choices.Remove(unitId);
        }
        
        public void SetUnit(string newUnit)
        {
            if (!dictUnits.TryGetValue(newUnit, out var datUnit))
            {
                return;
            }
            mngWindow.EditorToolBar.UpdateUnit(unit);
            unit = datUnit;
            mngWindow.EditorToolBar.SetUnit(datUnit);
        }

        public void SaveUnits()
        {
            mngWindow.EditorToolBar.UpdateUnit(unit);
            var junits = new List<UnitJson>();
            foreach (var key in dictUnits.Keys)
            {
                var data = dictUnits[key];
                junits.Add(new UnitJson()
                {
                    id = key,
                    display = data.UnitImagePath,
                    name = data.UnitName,
                    suitability = new SuitabilityJson()
                    {
                        space = data.Space.suitable.ToString(),
                        air = data.Air.suitable.ToString(),
                        ground = data.Ground.suitable.ToString(),
                        underwater = data.Underwater.suitable.ToString()
                    },
                    status = new UnitStatusJson()
                    {
                        accuracy = data.Accuracy.status,
                        maneuver = data.Maneuver.status,
                        power = data.Power.status,
                        armor = data.Armor.status,
                        reduction = data.Reduction.status,
                        move = data.Move.status,
                        hp = data.HP.status,
                        en = data.EN.status
                    }
                    // TODO : タイル設定
                    // TODO : 特殊能力設定
                    // TODO : 武装設定
                });
            }
            var units = new UnitsJson()
            {
                unit = junits.ToArray()
            };
            DataUtil.SaveData<UnitsJson>(Path.Combine(DataUtil.PathBase, "data", "unit", $"{nameFile}.json"), units);
        }

        public void DeleteUnit()
        {
            if (dictUnits.Count < 2)
            { return; }
            dictUnits.Remove(unit.UnitId);
            mngWindow.EditorToolBar.UnitDropdown.choices.Remove(unit.UnitId);
            mngWindow.EditorToolBar.UnitDropdown.value = dictUnits.Keys.ToArray()[0];
        }

        public BasicAction Action => action;
        public UnitEditorWindowManager EditorWindowManager => mngWindow;
        public Dictionary<string, UnitData> Units => dictUnits;
        public UnitData Unit => unit;
        
        public float WaitNavigate
        {
            set => waitNavigate = value;
        }

        public string FileName
        {
            get => nameFile;
            set => nameFile = value;
        }
    }
}