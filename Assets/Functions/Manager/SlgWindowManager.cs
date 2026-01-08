using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Functions.Data;
using Functions.Data.Maps;
using Functions.Data.Units;
using Functions.Enum;
using Functions.Json;
using Functions.UI;
using Functions.Util;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
namespace Functions.Manager
{
    public class SlgWindowManager : MonoBehaviour
    {
        [SerializeField] private SystemMenuWindow systemMenuWindow;
        [SerializeField] private CommandMenuWindow commandMenuWindow;
        [SerializeField] private WeaponMenuWindow weaponMenuWindow;
        [SerializeField] private SimpleStatusWindow simpleStatusWindow;
        [SerializeField] private StatusWindow statusWindow;
        [SerializeField] private BattleConfirmWindow battleConfirmWindow;
        [SerializeField] private MessageWindow messageWindowUp;
        [SerializeField] private MessageWindow messageWindowDown;
        [SerializeField] private TelopWindow telopWindow;
        [SerializeField] private VariableInputWindow variableInputWindow;
        [SerializeField] private ChoiceWindow choiceWindow;
        [SerializeField] private ArrangementUnitsWindow arrangementUnitsListWindow;
        [SerializeField] private ListWindow listWindow;
        [SerializeField] private ConditionListWindow conditionListWindow;
        [SerializeField] private DebugWindow debugWindow;
        [SerializeField] private ErrorWindow errorWindow;
        [SerializeField] private BackImageWindow backImageWindow;
        [SerializeField] private IntermissionWindow intermissionWindow;
        [SerializeField] private SettingWindow settingWindow;
    
        private bool isDisplayCommandMenu;
        private WeaponData selectWeapon;
        private float waitMessage;
        private bool isWaitMessage;
        private string pathBackImage;
        private Color32 colorBackImage;

        public void Initialize(SlgSceneManager mng)
        {
            systemMenuWindow.HiddenDisplay();
            commandMenuWindow.HiddenDisplay();
            weaponMenuWindow.HiddenDisplay();
            simpleStatusWindow.HiddenDisplay();
            statusWindow.HiddenDisplay();
            battleConfirmWindow.HiddenDisplay();
            messageWindowUp.HiddenDisplay();
            messageWindowDown.HiddenDisplay();
            telopWindow.ClearTelop();
            telopWindow.HiddenDisplay();
            variableInputWindow.HiddenDisplay();
            choiceWindow.HiddenDisplay();
            arrangementUnitsListWindow.HiddenDisplay();
            listWindow.HiddenDisplay();
            conditionListWindow.HiddenDisplay();
            debugWindow.HiddenDisplay();
            errorWindow.HiddenDisplay();
            backImageWindow.HiddenDisplay();
            intermissionWindow.HiddenDisplay();
            settingWindow.HiddenDisplay();
            settingWindow.OnChange += mng.LoadSystemSettings;
        }

        public void InitializeSystemMenu(ScriptManager _mng, CommonSettingsData _dat)
        {
            // システムメニュー構築
            foreach (var dat in _dat.Menus)
            {
                systemMenuWindow.AddMenu(_mng, dat).clicked += (() => SystemMenuClicked(_mng, dat));
            }
        }

        public async UniTask<bool> WindowProcess(SlgSceneManager _mng)
        {
            debugWindow.HiddenDisplay();
            simpleStatusWindow.HiddenDisplay();
            commandMenuWindow.HiddenDisplay();
            // エラー処理
            if (errorWindow.IsDisplay())
            {
                if (_mng.Action.UI.Enter.triggered || _mng.Action.UI.Click.WasPressedThisFrame())
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                }
                return true;
            }
            if (variableInputWindow.IsDisplay())
            { return true; }
            if (choiceWindow.IsDisplay())
            { return true; }
            // システム設定
            if (settingWindow.IsDisplay())
            { return true; }
            // テロップ処理
            if (backImageWindow.IsTransition)
            { return true; }
            // テロップ処理
            if (telopWindow.UpdateProcess(Time.deltaTime))
            { return true; }
            // メッセージ処理
            if (messageWindowUp.IsDisplay() || messageWindowDown.IsDisplay())
            {
                if (isWaitMessage)
                {
                    if (waitMessage > 0)
                    {
                        waitMessage -= Time.deltaTime;
                    }
                    else
                    {
                        isWaitMessage = false;
                        messageWindowUp.HiddenDisplay();
                        messageWindowDown.HiddenDisplay();
                    }
                    return true;
                }
                if (_mng.Action.UI.Enter.triggered || _mng.Action.UI.Click.WasPressedThisFrame())
                {
                    messageWindowUp.HiddenDisplay();
                    messageWindowDown.HiddenDisplay();
                }
                return true;
            }
            // 戦闘開始処理
            if (battleConfirmWindow.IsDisplay())
            {
                switch (_mng.ProcessMode)
                {
                    case ProcessMode.SelectReaction:
                        break;
                    case ProcessMode.SelectCounterWeapon:
                        if (_mng.IsCancel)
                        {
                            _mng.BattleInfo.Reaction = ReactionMode.None;
                            _mng.ProcessMode = ProcessMode.SelectReaction;
                            weaponMenuWindow.HiddenDisplay();
                            await DisplayBattleConfirmWindow(_mng);
                        }
                        break;
                    case ProcessMode.AcceptReaction:
                        if (_mng.Action.UI.Enter.triggered || _mng.Action.UI.Click.WasPressedThisFrame())
                        {
                            _mng.BattleStart();
                            battleConfirmWindow.HiddenDisplay();
                        }
                        else if (_mng.IsCancel)
                        {
                            _mng.BattleInfo.Reaction = ReactionMode.None;
                            _mng.ProcessMode = ProcessMode.SelectReaction;
                            await DisplayBattleConfirmWindow(_mng);
                        }
                        break;
                    default:
                        if (_mng.Action.UI.Enter.triggered || _mng.Action.UI.Click.WasPressedThisFrame())
                        {
                            _mng.BattleStart();
                            battleConfirmWindow.HiddenDisplay();
                        }
                        else if (_mng.IsCancel)
                        {
                            battleConfirmWindow.HiddenDisplay();
                            _mng.DisplayAttackArea(_mng.BattleInfo.AttackWeapon);
                        }
                        break;
                }
                return true;
            }

            if (_mng.ProcessMode != ProcessMode.None && _mng.ProcessMode != ProcessMode.ArrangementUnits && _mng.ProcessMode != ProcessMode.Intermission)
            { return false; }
            if (isDisplayCommandMenu) commandMenuWindow.VisibleDisplay();
            else commandMenuWindow.HiddenDisplay();
            if (!_mng.IsCancel)
            {
                return commandMenuWindow.IsDisplay() || weaponMenuWindow.IsDisplay() ||
                       battleConfirmWindow.IsDisplay() || statusWindow.IsDisplay() || systemMenuWindow.IsDisplay() ||
                       arrangementUnitsListWindow.IsDisplay() || listWindow.IsDisplay() ||
                       conditionListWindow.IsDisplay(); }
            if (commandMenuWindow.IsDisplay())
            {
                isDisplayCommandMenu = false;
                if (_mng.ProcessMode != ProcessMode.ArrangementUnits)
                {
                    var dat = _mng.GetArrangementData(_mng.SelectUnitPosition);
                    if (!dat.IsEndAction && dat.IsEndMove)
                    {
                        dat.IsEndMove = false;
                        _mng.MoveUnit(dat.UnitId, dat.BeforePosition, dat.BeforeDirection, false);
                        _mng.DisplayMoveArea();
                    }
                    else
                    {
                        _mng.UnselectUnit();
                    }
                }
            }
            else if (weaponMenuWindow.IsDisplay())
            {
                weaponMenuWindow.HiddenDisplay();
                DisplayCommandMenu(_mng);
            }
            else if (battleConfirmWindow.IsDisplay())
            {
                battleConfirmWindow.HiddenDisplay();
                _mng.DisplayAttackArea(_mng.BattleInfo.AttackWeapon);
            }
            else if (statusWindow.IsDisplay())
            {
                statusWindow.HiddenDisplay();
                if (_mng.ProcessMode != ProcessMode.ArrangementUnits)
                {
                    DisplayCommandMenu(_mng);
                }
                else
                {
                    var dat = _mng.GetArrangementData(_mng.SelectUnitPosition);
                    DisplayArangementUnitMenu(_mng, dat);
                }
            }
            else if (systemMenuWindow.IsDisplay())
            {
                systemMenuWindow.HiddenDisplay();
            }
            else if (listWindow.IsDisplay())
            {
                listWindow.HiddenDisplay();
            }
            else if (arrangementUnitsListWindow.IsDisplay())
            {
                CancelArrangementUnitsList(_mng);
            }
            else if (conditionListWindow.IsDisplay())
            {
                conditionListWindow.HiddenDisplay();
            }
            else
            {
                // システムメニュー表示
                systemMenuWindow.VisibleDisplay();
                if (_mng.ProcessMode != ProcessMode.ArrangementUnits)
                {
                    systemMenuWindow.SetDisplayButton(1, true);
                }
                else
                {
                    systemMenuWindow.SetDisplayButton(1, false);
                }
            }
            return true;
        }

        private void SystemMenuClicked(ScriptManager _mng, MenuData dat)
        {
            _mng.CallMethod(dat.Name, dat.Method, 0);
            systemMenuWindow.HiddenDisplay();
        }
    
        public void DisplayCommandMenu(SlgSceneManager _mng)
        {
            var dat = _mng.GetArrangementData(_mng.SelectUnitPosition);
            if (dat == null)
            { return; }
            _mng.BattleInfo.Attacker = dat;
            isDisplayCommandMenu = true;
            var unit = _mng.GetPermanenceUnitData(dat.UnitId);
            var chara = _mng.GetPermanenceCharacterData(dat.CharacterId);
            commandMenuWindow.ClearButton();
            if (_mng.IsNowGroup(dat.GroupId))
            {
                // 行動メニュー表示
                if (!dat.IsEndAction)
                {
                    // 移動メニュー表示
                    if (!dat.IsEndMove && unit.Unit.Move.status > 0)
                    {
                        commandMenuWindow.AddMenu(_mng.ScriptManager, "#{cmd_move}", FontStyle.Normal).clicked += (() =>
                        {
                            isDisplayCommandMenu = false;
                            _mng.DisplayMoveArea();
                        });
                    }
                    // 武装メニュー表示
                    if (unit.Unit.Weapons.Count > 0)
                    {
                        commandMenuWindow.AddMenu(_mng.ScriptManager, "#{cmd_attack}", FontStyle.Normal).clicked += (() =>
                        {
                            isDisplayCommandMenu = false;
                            DisplayWeaponMenu(_mng);
                        });
                    }
                    // 移動状態変更メニュー表示
                    if (!dat.IsEndMove)
                    {
                        ChangeMoveTypeMenu(_mng, dat, unit);
                    }
                    // 待機メニュー表示
                    commandMenuWindow.AddMenu(_mng.ScriptManager, "#{cmd_wait}", FontStyle.Normal).clicked += (() =>
                    {
                        isDisplayCommandMenu = false;
                        dat.IsEndAction = true;
                        _mng.DrawUnit();
                    });
                }
            }
            // 共通メニュー表示
            commandMenuWindow.AddMenu(_mng.ScriptManager, "#{lbl_status}", FontStyle.Normal).clicked += (() =>
            {
                isDisplayCommandMenu = false;
                statusWindow.SetStatus(unit, chara);
                statusWindow.VisibleDisplay();
            });
            isDisplayCommandMenu = true;
        }

        private void ChangeMoveTypeMenu(SlgSceneManager _mng, ArrangementData dat, PermanenceUnitData unit)
        {
            var tile = _mng.GetTile(_mng.SelectUnitPosition);
            foreach (var move in tile.MoveCost.Values)
            {
                if (move.MoveType == dat.MoveType)
                {
                    continue;
                }

                var text = "";
                switch (move.MoveType)
                {
                    case MoveType.Space:
                        if (unit.Unit.Space.suitable == Suitable.E)
                        {
                            continue;
                        }

                        text = "#{cmd_change_space}";
                        break;
                    case MoveType.Air:
                        if (unit.Unit.Air.suitable == Suitable.E)
                        {
                            continue;
                        }

                        text = "#{cmd_change_air}";
                        break;
                    case MoveType.Ground:
                        if (unit.Unit.Ground.suitable == Suitable.E)
                        {
                            continue;
                        }

                        text = "#{cmd_change_ground}";
                        break;
                    case MoveType.Underwater:
                        if (unit.Unit.Underwater.suitable == Suitable.E)
                        {
                            continue;
                        }

                        text = "#{cmd_change_water}";
                        break;
                }

                commandMenuWindow.AddMenu(_mng.ScriptManager, text, FontStyle.Normal).clicked += (() =>
                {
                    dat.MoveType = move.MoveType;
                    _mng.DrawUnit();
                    if (_mng.ProcessMode != ProcessMode.ArrangementUnits)
                    {
                        DisplayCommandMenu(_mng);
                    }
                    else
                    {
                        DisplayArangementUnitMenu(_mng, dat);
                    }
                });
            }
        }

        public void DisplayArangementUnitMenu(SlgSceneManager _mng, ArrangementData dat)
        {
            Debug.Log("DisplayArangementUnitMenu");
            Debug.Log($"dat : {dat != null}");
            commandMenuWindow.ClearButton();
            if (_mng.ArrangmentUnitNum >= _mng.ArrangmentUnitMinNum)
            {
                commandMenuWindow.AddMenu(_mng.ScriptManager, "#{lbl_placement_completed}", FontStyle.Bold).clicked += (() =>
                {
                    isDisplayCommandMenu = false;
                    _mng.ArrangementUnitsAccept();
                });
            }
            if (dat != null)
            {
                var unit = _mng.GetPermanenceUnitData(dat.UnitId);
                var chara = _mng.GetPermanenceCharacterData(dat.CharacterId);
                if (dat.IsTemporaryArrangement)
                {
                    commandMenuWindow.AddMenu(_mng.ScriptManager, "#{lbl_placement_withdrawal}", FontStyle.Normal).clicked += (() =>
                    {
                        isDisplayCommandMenu = false;
                        _mng.CancelArrangementUnit(dat.UnitId);
                    });
                    ChangeMoveTypeMenu(_mng, dat, unit);
                }
                commandMenuWindow.AddMenu(_mng.ScriptManager, "#{lbl_status}", FontStyle.Normal).clicked += (() =>
                {
                    isDisplayCommandMenu = false;
                    statusWindow.SetStatus(unit, chara);
                    statusWindow.VisibleDisplay();
                });
            }
            if (commandMenuWindow.Count > 0)
            {
                isDisplayCommandMenu = true;
            }
        }

        public void DisplayWeaponMenu(SlgSceneManager mng)
        {
            weaponMenuWindow.ClearButton();
            var unit = mng.GetPermanenceUnitData(mng.BattleInfo.Attacker.UnitId);
            weaponMenuWindow.VisibleDisplay();
            foreach (var wep in unit.Unit.Weapons)
            {
                weaponMenuWindow.AddMenu(wep, FontStyle.Normal).clicked += (() =>
                {
                    weaponMenuWindow.HiddenDisplay();
                    mng.BattleInfo.AttackWeapon = wep;
                    mng.DisplayAttackArea(wep);
                });
            }
        }

        public async UniTask DisplayBattleConfirmWindow(SlgSceneManager mng)
        {
            await mng.CalcBattleInfo();
            battleConfirmWindow.VisibleDisplay();
            SetOwnBattleInfo(mng);
            await SetTargetBattleInfo(mng);
        }

        private void SetOwnBattleInfo(SlgSceneManager mng)
        {
            SetBattleInfo(
                mng, 
                mng.SelectUnitPosition, 
                battleConfirmWindow.Own, 
                mng.BattleInfo.AttackWeapon.WeaponName,
                $"ATK {mng.BattleInfo.AttackWeapon.AttackPower:N0}", 
                LocaleUtil.GetEntry("bcw_lbl_hit_percent"),
                $"{Math.Floor(mng.BattleInfo.Hit)} %", 
                LocaleUtil.GetEntry("bcw_lbl_critical_percent"),
                $"{Math.Floor(mng.BattleInfo.Critical)} %"
            );
        }

        private async UniTask SetTargetBattleInfo(SlgSceneManager _mng)
        {
            var reaction = string.Empty;
            var note = string.Empty;
            var title1 = string.Empty;
            var title2 = string.Empty;
            var value1 = string.Empty;
            var value2 = string.Empty;
            if (_mng.ProcessMode == ProcessMode.SelectReaction)
            {
                battleConfirmWindow.Target.ResetInfo();
                battleConfirmWindow.Target.ResetAction();
                var weapons = _mng.GetPermanenceUnitData(_mng.BattleInfo.Defender.UnitId)
                    .GetAvailableWeapons(_mng.Distance(_mng.TargetUnitPosition, _mng.SelectUnitPosition), _mng.GetArrangementData(_mng.SelectUnitPosition)); 
                if (weapons.Any())
                {
                    battleConfirmWindow.Target.AddAction(LocaleUtil.GetEntry("cmd_counter"), FontStyle.Normal).clicked += () =>
                    {
                        battleConfirmWindow.Target.ResetAction();
                        _mng.BattleInfo.Reaction = ReactionMode.Counter;
                        _mng.ProcessMode = ProcessMode.SelectCounterWeapon;
                        weaponMenuWindow.ClearButton();
                        weaponMenuWindow.VisibleDisplay();
                        foreach (var wep in weapons)
                        {
                            weaponMenuWindow.AddMenu(wep, FontStyle.Normal).clicked += (async () =>
                            {
                                weaponMenuWindow.HiddenDisplay();
                                _mng.BattleInfo.CounterWeapon = wep;
                                _mng.ProcessMode = ProcessMode.AcceptReaction;
                                await DisplayBattleConfirmWindow(_mng);
                            });
                        }
                    };
                }
                battleConfirmWindow.Target.AddAction(LocaleUtil.GetEntry("cmd_guard"), FontStyle.Normal).clicked += async () =>
                {
                    battleConfirmWindow.Target.ResetAction();
                    _mng.BattleInfo.Reaction = ReactionMode.Defense;
                    _mng.ProcessMode = ProcessMode.AcceptReaction;
                    await DisplayBattleConfirmWindow(_mng);
                };
                battleConfirmWindow.Target.AddAction(LocaleUtil.GetEntry("cmd_avoidance"), FontStyle.Normal).clicked += async () =>
                {
                    battleConfirmWindow.Target.ResetAction();
                    _mng.BattleInfo.Reaction = ReactionMode.Avoidance;
                    _mng.ProcessMode = ProcessMode.AcceptReaction;
                    await DisplayBattleConfirmWindow(_mng);
                };
            }
            else if (_mng.ProcessMode == ProcessMode.SelectCounterWeapon)
            {
                
            }
            else
            {
                switch (_mng.BattleInfo.Reaction)
                {
                    case ReactionMode.Counter:
                        reaction = _mng.BattleInfo.CounterWeapon.WeaponName;
                        note = $"ATK {_mng.BattleInfo.CounterWeapon.AttackPower:N0}";
                        title1 = LocaleUtil.GetEntry("bcw_lbl_hit_percent");
                        value1 = $"{Math.Floor(_mng.BattleInfo.CounterHit)} %";
                        title2 = LocaleUtil.GetEntry("bcw_lbl_critical_percent");
                        value2 = $"{Math.Floor(_mng.BattleInfo.CounterCritical)} %";
                        break;
                    case ReactionMode.Defense:
                        reaction = LocaleUtil.GetEntry("cmd_guard");
                        // title1 = LocalizationSettings.StringDatabase.GetTableEntry("UIText", "bcw_lbl_guard_percent").Entry.Value;
                        // value1 = $"{Math.Floor(_mng.BattleInfo.DefenceRate)} %";
                        break;
                    case ReactionMode.Avoidance:
                        reaction = LocaleUtil.GetEntry("cmd_avoidance");
                        break;
                }
            }

            SetBattleInfo(
                _mng, 
                _mng.TargetUnitPosition, 
                battleConfirmWindow.Target,
                reaction,
                note,
                title1,
                value1,
                title2,
                value2
            );
        }

        private void SetBattleInfo(SlgSceneManager _mng, Vector2Int _pos, BattleStatusPanel _pnl, string _action, string _note, string _title1, string _value1, string _title2, string _value2)
        {
            var dat = _mng.GetArrangementData(_pos);
            if (dat == null)
            { return; }
            var unit = _mng.GetPermanenceUnitData(dat.UnitId);
            var chara = _mng.GetPermanenceCharacterData(dat.CharacterId);
            _pnl.SetColor(_mng.GetGroupColor(dat.GroupId));
            _pnl.SetStatus(unit, chara);
            _pnl.SetInfo(_action, _note, _title1, _value1, _title2, _value2);
        }

        public void DisplaySimpleStatus(SlgSceneManager _mng)
        {
            var dat = _mng.GetArrangementData(_mng.CursorPosition);
            if (dat == null)
            { return; }
            var unit = _mng.GetPermanenceUnitData(dat.UnitId);
            var chara = _mng.GetPermanenceCharacterData(dat.CharacterId);
            simpleStatusWindow.SetStatus(unit, chara);
            simpleStatusWindow.VisibleDisplay();
        }

        public void SetMessage(SlgSceneManager _mng, string _charaId, string _chara, string _name, string[] _text,
            string _position, float _wait)
        {
            isWaitMessage = true;
            waitMessage = _wait;
            SetMessage(_mng, _charaId, _chara, _name, _text, _position);
        }

        public void SetMessage(SlgSceneManager _mng, string _charaId, string _chara, string _name, string[] _text, string _position)
        {
            Texture2D img = null;
            var name = string.Empty;
            if (!String.IsNullOrWhiteSpace(_charaId))
            {
                var dat = _mng.GetPermanenceCharacterData(_charaId);
                if (dat != null)
                {
                    img = dat.Character.CharacterImage;
                    name = dat.CharacterName;
                }
            }
            else if (!String.IsNullOrWhiteSpace(_chara))
            {
                var dat = _mng.GetCharacterData(_chara);
                if (dat != null)
                {
                    img = dat.CharacterImage;
                    name = dat.CharacterName;
                }
            }
            // 表示名の指定がある場合は強制上書き
            if (!String.IsNullOrWhiteSpace(_name))
            {
                name = _name;
            }

            if (_position.ToLower() == "up")
            {
                messageWindowUp.SetMessage(img, name, _text);
                messageWindowUp.VisibleDisplay();
            }
            else
            {
                messageWindowDown.SetMessage(img, name, _text);
                messageWindowDown.VisibleDisplay();
            }
        }

        public void SetTelop(string[] _text, float _time, float _fade)
        {
            telopWindow.SetTelop(_text, _time, _fade);
            telopWindow.VisibleDisplay();
        }

        public void SetVariableInputWindow(SlgSceneManager _mng, string[] _text, string _scope, string _name, int _min, int _max)
        {
            variableInputWindow.SetupDisplay(_mng, _text, _scope, _name, _min, _max);
            variableInputWindow.VisibleDisplay();
        }

        public void SetChoiceWindow(SlgSceneManager _mng, string[] _text, ChoiceJson[] _choices)
        {
            choiceWindow.SetupChoice(_mng, _text, _choices);
            choiceWindow.VisibleDisplay();
        }

        public void SetArrangementUnitsList(SlgSceneManager _mng, ArrangementData[] _lst, SortedDictionary<int, GroupData> _grp, Dictionary<string, PermanenceUnitData> _unit, Dictionary<string, PermanenceCharacterData> _chara, DirectionType dir)
        {
            _mng.ProcessMode = ProcessMode.ArrangementUnitsList;
            arrangementUnitsListWindow.SetupDisplay(_mng, _lst, _grp, _unit, _chara, dir);
            arrangementUnitsListWindow.VisibleDisplay();
        }

        public void CancelArrangementUnitsList(SlgSceneManager _mng)
        {
            _mng.ProcessMode = ProcessMode.ArrangementUnits;
            arrangementUnitsListWindow.HiddenDisplay();
        }

        public void SetUnitList(bool intermission, ArrangementData[] _lst, SortedDictionary<int, GroupData> _grp, Dictionary<string, PermanenceUnitData> _unit, Dictionary<string, PermanenceCharacterData> _chara)
        {
            listWindow.SetUnitDisplay(intermission, _lst, _grp, _unit, _chara);
            listWindow.VisibleDisplay();
        }

        public void SetCharaList(bool intermission, ArrangementData[] _lst, SortedDictionary<int, GroupData> _grp, Dictionary<string, PermanenceCharacterData> _chara)
        {
            listWindow.SetCharaDisplay(intermission, _lst, _grp, _chara);
            listWindow.VisibleDisplay();
        }

        public void SetBackImage(string _path, Color32 _color)
        {
            if (String.IsNullOrWhiteSpace(_path))
            {
                backImageWindow.HiddenDisplay();
            }
            else
            {
                var img = DataUtil.GetImage(Path.Combine(DataUtil.PathBase, "resource", "image", "back", _path));
                if (img == null)
                {
                    SetError($"{_path}は表示可能なイメージファイルとして存在していません。");
                    return;
                }
                pathBackImage = _path;
                colorBackImage = _color;
                backImageWindow.SetImage(img, _color);
            }
        }

        public void SetBackImageColor(Color32 _color, float _fade)
        {
            colorBackImage = _color;
            backImageWindow.SetImageColor(_color, _fade);
        }

        public void SetCondition(SlgSceneManager _mng)
        {
            conditionListWindow.SetCondition(_mng.VictoryConditionList, _mng.DefeatConditionList);
            conditionListWindow.VisibleDisplay();
        }

        public void SetDebug(Vector3Int _pos, TileData _dat, int _zoom, ProcessMode _mode, string _script)
        {
            debugWindow.SetData("Position", $"({_pos.x}, {_pos.y}, {_pos.z})");
            debugWindow.SetData("Tile", $"{_dat.TileSetId}, {_dat.TileId}");
            debugWindow.SetData("Zoom", $"{_zoom}");
            debugWindow.SetData("Mode", $"{_mode}");
            debugWindow.SetData("Script", $"{_script}");
            debugWindow.VisibleDisplay();
        }

        public void SetError(string _err)
        {
            errorWindow.SetError(_err);
        }

        public void DisplayIntermission(SlgSceneManager _mng, string _name, string _method)
        {
            _mng.ProcessMode = ProcessMode.Intermission;
            intermissionWindow.Display(_mng, _name, _method);
        }

        public void DisplaySystemSettings(SlgSceneManager _mng)
        {
            settingWindow.VisibleDisplay();
        }

        public bool IsDisplayCommandMenu => isDisplayCommandMenu;
        public bool IsDisplayIntermission => intermissionWindow.IsDisplay();
        public string NextStageClass => intermissionWindow.NextStageClass;
        public string NextStageMethod => intermissionWindow.NextStageMethod;
        public bool IsDisplayBackImage => backImageWindow.IsDisplay();
        public string PathBackImage => pathBackImage;
        public Color32 ColorBackImage => colorBackImage;
    }
}
