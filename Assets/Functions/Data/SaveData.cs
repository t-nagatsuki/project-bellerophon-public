using System;
using System.Collections.Generic;
using System.Diagnostics;
using Functions.Data.Maps;
using Functions.Data.Scripts;
using Functions.Data.Units;
using Functions.Enum;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
namespace Functions.Data
{
    [Serializable]
    public class SaveData
    {
        public int MajorVersion;
        public int MinorVersion;
        public ProcessMode Mode;
        public string MapId;
        /// <summary>画面中心座標</summary>
        public Vector2Int CenterPosition;
        /// <summary>現在行動中のグループ</summary>
        public int NowGroup;
        /// <summary>現在実行中スクリプトクラス</summary>
        public RunningScriptData RunningScript;
        public bool IsIntermission;
        public string NextStageClass;
        public string NextStageMethod;
        public bool IsBackImage;
        public string PathBackImage;
        public Color32 ColorBackImage;
        public int ArrangementUnitNum = 0;
        public int ArrangementUnitMinNum = 0;
        public int ArrangementUnitMaxNum = 0;
        public DirectionType ArrangementUnitDirection = DirectionType.LeftDown;
        public string NextCallClass;
        public string NextCallMethod;
        public bool IsMusicStop;
        
        public List<SerializableKeyValuePair<string, ScriptsVariableData>> Variables;
        public List<SerializableKeyValuePair<string, PermanenceCharacterSaveData>> Characters;
        public List<SerializableKeyValuePair<string, PermanenceUnitSaveData>> Units;
        public List<SerializableKeyValuePair<string, ArrangementData>> Arrangements;
        public List<SerializableKeyValuePair<Vector2Int, string>> ArrangementPositions;
        public List<SerializableKeyValuePair<Vector3Int, TileData>> MapTiles;
        public List<SerializableKeyValuePair<int, GroupData>> Groups;
        public List<SerializableKeyValuePair<Vector2Int, int>> Heights;
        public List<SerializableKeyValuePair<Vector2Int, bool>> CanCursors;
        public List<Vector2Int> CanArrangementPositionList;
        public List<Vector2Int> HighlightTilePositionList;
        public List<ConditionData> VictoryConditionList;
        public List<ConditionData> DefeatConditionList;
        public List<ConditionData> EventConditionList;

        public void SetPermanenceCharacters(Dictionary<string, PermanenceCharacterData> _dict)
        {
            var lst = new List<SerializableKeyValuePair<string, PermanenceCharacterSaveData>>();
            foreach (var pair in _dict)
            {
                var serializablePair = new SerializableKeyValuePair<string, PermanenceCharacterSaveData>(pair.Key, new PermanenceCharacterSaveData(pair.Value));
                lst.Add(serializablePair);
            }
            Characters = lst;
        }

        public void SetPermanenceUnits(Dictionary<string, PermanenceUnitData> _dict)
        {
            var lst = new List<SerializableKeyValuePair<string, PermanenceUnitSaveData>>();
            foreach (var pair in _dict)
            {
                var serializablePair = new SerializableKeyValuePair<string, PermanenceUnitSaveData>(pair.Key, new PermanenceUnitSaveData(pair.Value));
                lst.Add(serializablePair);
            }
            Units = lst;
        }

        public Dictionary<string, PermanenceCharacterData> GetPermanenceCharacters(SlgSceneManager _mng)
        {
            var deserializedDict = new Dictionary<string, PermanenceCharacterData>();
            foreach (var serializablePair in Characters)
            {
                var dat = new PermanenceCharacterData(serializablePair.Value)
                {
                    Character = _mng.GetCharacterData(serializablePair.Value.CharacterDataId),
                };
                deserializedDict.Add(serializablePair.Key, dat);
            }
            return deserializedDict;
        }

        public Dictionary<string, PermanenceUnitData> GetPermanenceUnits(SlgSceneManager _mng)
        {
            var deserializedDict = new Dictionary<string, PermanenceUnitData>();
            foreach (var serializablePair in Units)
            {
                var dat = new PermanenceUnitData(serializablePair.Value)
                {
                    Unit = _mng.GetUnitData(serializablePair.Value.UnitDataId),
                };
                deserializedDict.Add(serializablePair.Key, dat);
            }
            return deserializedDict;
        }
    }

    [Serializable]
    public class PermanenceCharacterSaveData
    {
        public string CharacterId;
        public string CharacterDataId;
        public string CharacterName;
        public List<SerializableKeyValuePair<string, ResourceValueData>> Statuses;
        public SuitableData Space;
        public SuitableData Air;
        public SuitableData Ground;
        public SuitableData Underwater;

        public PermanenceCharacterSaveData(PermanenceCharacterData _dat)
        {
            CharacterId = _dat.CharacterId;
            CharacterDataId = _dat.Character.CharacterId;
            CharacterName = _dat.CharacterName;
            Statuses = DataUtil.SerializeDictionary(_dat.Statuses);
            Space = _dat.Space;
            Air = _dat.Air;
            Ground = _dat.Ground;
            Underwater = _dat.Underwater;
        }
    }

    [Serializable]
    public class PermanenceUnitSaveData
    {
        public string UnitId;
        public string UnitDataId;
        public string UnitName;
        public List<SerializableKeyValuePair<string, ResourceValueData>> Statuses;
        public List<SerializableKeyValuePair<string, ResourceValueData>> Weapons;

        public PermanenceUnitSaveData(PermanenceUnitData _dat)
        {
            UnitId = _dat.UnitId;
            UnitDataId = _dat.Unit.UnitId;
            UnitName = _dat.UnitName;

            Statuses = DataUtil.SerializeDictionary(_dat.Statuses);
            Weapons = DataUtil.SerializeDictionary(_dat.Weapons);
        }
    }

    [Serializable]
    public class SerializableKeyValuePair<TKey, TValue>
    {
        public TKey  Key;
        public TValue Value;
    
        public SerializableKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}