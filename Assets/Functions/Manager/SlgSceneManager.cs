using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Functions.Data;
using Functions.Data.Battle;
using Functions.Data.Maps;
using Functions.Data.Scripts;
using Functions.Data.Units;
using Functions.Effect;
using Functions.Enum;
using Functions.Util;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using TileData = Functions.Data.Maps.TileData;

namespace Functions.Manager
{
    public class SlgSceneManager : MonoBehaviour
    {
        public const int MajorVersion = 1;
        public const int MinorVersion = 1;

        public const int RequireMajorVersion = 1;
        public const int RequireMinorVersion = 1;

        [SerializeField] private GameObject gridGroup;
        [SerializeField] private TileMapManager mngStage;
        [SerializeField] private TileMapManager mngSelect;
        [SerializeField] private TileMapManager mngCursor;
        [SerializeField] private TileMapManager mapUnit;
        [SerializeField] private AudioSource music;
        [SerializeField] private AudioSource sound;
        [SerializeField] private ScriptManager mngScript;
        [FormerlySerializedAs("mngWindow")] [SerializeField] private SlgWindowManager mngSlgWindow;

        [SerializeField] private GameObject effectBlast;

        private Dictionary<string, TilesData> dictTiles;
        private Dictionary<string, MapData> dictMaps;
        private Dictionary<string, CharacterData> dictCharacterData;
        private Dictionary<string, MessageData> dictMessageData;
        private Dictionary<string, UnitData> dictUnitData;
        private Dictionary<string, PermanenceCharacterData> dictCharacters;
        private Dictionary<string, PermanenceUnitData> dictUnits;
        private Dictionary<string, ArrangementData> dictArrangement;
        private Dictionary<Vector2Int, string> dictArrangementPosition;
        private Dictionary<Vector3Int, TileData> dictMapTiles;
        private SortedDictionary<int, GroupData> dictGroup;
        private Dictionary<Vector2Int, int> dictHeight;
        private Dictionary<Vector2Int, bool> canCursor;
        private List<ConditionData> lstVictoryCondition;
        private List<ConditionData> lstDefeatCondition;
        private List<ConditionData> lstEventCondition;
        private List<Vector2Int> lstCanArrangementPosition;
        private List<Vector2Int> lstHighlightTilePosition;

        private BattleInfoData datBattleInfo = new BattleInfoData();
        private BattleResultData datBattleResult = new BattleResultData();

        private Camera camMain;
        /// <summary>共通設定情報</summary>
        private CommonSettingsData datCommonSettings;
        /// <summary>マップ情報</summary>
        private MapData map;
        /// <summary>カーソル座標</summary>
        private Vector2Int posCursor;
        /// <summary>画面中心座標</summary>
        private Vector2Int posCenter;
        /// <summary>現在表示スケール</summary>
        private int nowScale = 4;
        /// <summary>選択中ユニット座標</summary>
        private Vector2Int posSelectUnit;
        /// <summary>対象ユニット座標</summary>
        private Vector2Int posTargetUnit;
        /// <summary>現在行動中のグループ</summary>
        private int nowGroup;
        private MondUtil utilMond;

        private bool isClick;
        private bool isDrag;
        private Vector3 posCameraMoveStart;
        private Vector3 posMoveStart;

        private BasicAction action;
        /// <summary>カーソル移動入力待機時間</summary>
        private float waitNavigate = 999;
        /// <summary>カーソル移動入力制限時間</summary>
        private const float WaitNavigateTime = 0.2f;

        private ProcessMode mode;
        private ProcessMode beforeMode;
        private float waitTime;
        private float deltaTime;

        private string targetUnit;
        private Vector2Int targetPos;
        private Vector3 baseViewPos;
        private Vector3 targetViewPos;
        private List<Vector2Int> moveRoute;

        private readonly Vector2Int nonePosition = new Vector2Int(-1, -1);
        private string loadingData;
        private string nowMusic;
        private bool isInitialized;

        private int arrangementUnitNum = 0;
        private int arrangementUnitMinNum = 0;
        private int arrangementUnitMaxNum = 0;
        private DirectionType arrangementUnitDirection = DirectionType.LeftDown;
        private string nextCallClass;
        private string nextCallMethod;
        private bool isMusicStop;

        private void Awake()
        {
            camMain = Camera.main;
            action = new BasicAction();
            action.Enable();
            mngStage.Initialize(camMain, action);
            if (!string.IsNullOrWhiteSpace(DataUtil.ErrorMessage))
            {
                mngSlgWindow.SetError(DataUtil.ErrorMessage);
                return;
            }
            //var logWriter = new LogWriter(Path.Combine(DataUtil.PathBase, "logs", "debug.log"), this.GetCancellationTokenOnDestroy());

            dictHeight = new Dictionary<Vector2Int, int>();
            canCursor = new Dictionary<Vector2Int, bool>();
            dictGroup = new SortedDictionary<int, GroupData>();
            dictCharacters = new Dictionary<string, PermanenceCharacterData>();
            dictUnits = new Dictionary<string, PermanenceUnitData>();
            dictArrangement = new Dictionary<string, ArrangementData>();
            dictArrangementPosition = new Dictionary<Vector2Int, string>();
            dictMapTiles = new Dictionary<Vector3Int, TileData>();
            moveRoute = new List<Vector2Int>();
            lstVictoryCondition = new List<ConditionData>();
            lstDefeatCondition = new List<ConditionData>();
            lstEventCondition = new List<ConditionData>();
            lstCanArrangementPosition = new List<Vector2Int>();
            lstHighlightTilePosition = new List<Vector2Int>();
        }

        private async void Start()
        {
            try
            {
                //// 各種定義の読込
                var pathBase = DataUtil.PathBase;
                // 初期化
                mngScript.Variable["system"] = new ScriptsVariableData("system");
                mngScript.Variable["system"].SetVariable("now_turn", "1");

                // マップチップ定義読込
                LoadSystemSettings();
            
                dictTiles = await DataUtil.LoadTiles(pathBase);
	
                // マップ定義読込
                dictMaps = await DataUtil.LoadMaps(pathBase);
	
                // キャラクター定義読込
                dictCharacterData = await DataUtil.LoadCharacters(pathBase);
                
                // メッセージ定義読込
                dictMessageData = await DataUtil.LoadMessages(pathBase);
	
                // ユニット定義読込
                dictUnitData = await DataUtil.LoadUnits(pathBase);
	        
                // 共通設定読込
                datCommonSettings = await DataUtil.LoadCommonSettings(pathBase);
                if (datCommonSettings == null)
                {
                    mngSlgWindow.SetError("data/common/settings.jsonが正常に読み込めませんでした。");
                    return;
                }
	        
                // スクリプト読込
                await mngScript.InitializeScripts(pathBase);
                // システムメニュー初期化
                mngSlgWindow.InitializeSystemMenu(mngScript, datCommonSettings);
                // 組込スクリプト初期化
                utilMond = new MondUtil(pathBase, mngScript);

                isInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                mngSlgWindow.SetError(ex.Message);
            }
        }

        private void OnDestroy()
        {
            action.Disable();
        }

        // Update is called once per frame
        private async void Update()
        {
            if (!isInitialized)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(loadingData))
            {
                Load(1);
            }

            mngCursor.ClearAllTiles();
            if (beforeMode != mode)
            {
                ViewHilightTiles();
            }

            beforeMode = mode;
            if (await mngSlgWindow.WindowProcess(this)
                || WaitProcess()
                || MoveUnitProcess()
                || MoveViewProcess()
                || mngScript.ScriptProcess(this)
                || BattleViewProcess())
            {
                return;
            }

            if (mode == ProcessMode.ArrangementUnitsList)
            {
                if (IsCancel)
                {
                    SlgWindowManager.CancelArrangementUnitsList(this);
                }

                return;
            }

            if (dictGroup.Count == 0)
            {
                return;
            }

            var grp = dictGroup.Values.ToArray()[nowGroup];
            if (mode == ProcessMode.None && !isMusicStop && !music.isPlaying)
            {
                StartGroupMusic();
            }

            // プレイヤーターン
            if (grp.Player)
            {
                if (action.UI.Click.WasPressedThisFrame())
                {
                    isClick = true;
                }
                else if (!action.UI.Click.IsPressed())
                {
                    isClick = false;
                }

                if (action.UI.MiddleClick.WasPressedThisFrame())
                {
                    isDrag = true;
                    posCameraMoveStart = camMain.transform.position;
                    var mpos = action.UI.Point.ReadValue<Vector2>();
                    posMoveStart = camMain.ScreenToWorldPoint(new Vector3(mpos.x, mpos.y, 0));
                }
                else if (!action.UI.MiddleClick.IsPressed())
                {
                    isDrag = false;
                }

                if (action.UI.ScrollWheel.WasPerformedThisFrame())
                {
                    // 拡大・縮小
                    if (action.UI.ScrollWheel.ReadValue<Vector2>().y > 0)
                    {
                        nowScale = math.clamp(nowScale + 1, 3, 7);
                    }
                    else if (action.UI.ScrollWheel.ReadValue<Vector2>().y < 0)
                    {
                        nowScale = math.clamp(nowScale - 1, 3, 7);
                    }

                    gridGroup.transform.localScale =
                        new Vector3(nowScale * 0.25f, nowScale * 0.25f, nowScale * 0.25f);
                }

                if (isDrag)
                {
                    var mpos = action.UI.Point.ReadValue<Vector2>();
                    var after = camMain.ScreenToWorldPoint(new Vector3(mpos.x, mpos.y, 0));
                    var pos = camMain.transform.position;
                    pos.x = posCameraMoveStart.x - (after.x - posMoveStart.x);
                    pos.y = posCameraMoveStart.y - (after.y - posMoveStart.y);
                    camMain.transform.position = Vector3.Lerp(camMain.transform.position, pos, 0.5f);
                }

                if (DisplayMoveAreaProcess())
                {
                    return;
                }

                if (DisplayAttackAreaProcess())
                {
                    return;
                }

                mngSlgWindow.DisplaySimpleStatus(this);
                SetCursor(posCursor);
                ControlCursor();
            }
            // AIターン
            else
            {
                await NpcProcess();
            }
        }

        private bool WaitProcess()
        {
            if (mode != ProcessMode.Wait)
            {
                return false;
            }

            if (mode == ProcessMode.EffectWait)
            {
                return true;
            }
            deltaTime += Time.deltaTime;
            if (waitTime <= 0)
            {
                if (action.UI.Enter.triggered || action.UI.Click.WasPressedThisFrame())
                {
                    mode = ProcessMode.None;
                    mngScript.Variable["system"].SetVariable("wait_time", deltaTime.ToString(CultureInfo.InvariantCulture));
                }
            }
            else
            {
                if (deltaTime >= waitTime)
                {
                    mode = ProcessMode.None;
                }
            }
            return true;
        }

        private bool MoveUnitProcess()
        {
            if (mode != ProcessMode.MoveUnit)
            {
                return false;
            }
            var nowPos = GetPosition(targetUnit);
            if (nowPos == targetPos)
            {
                moveRoute.Clear();
                mode = ProcessMode.None;
                CheckConditionProcess(dictArrangement[targetUnit]);
            }
            else
            {
                if (deltaTime < waitTime)
                {
                    deltaTime += Time.deltaTime;
                }
                else
                {
                    deltaTime = 0;
                    var dat = dictArrangement[targetUnit];
                    var direction = dat.Direction;
                    if (moveRoute.Count == 0)
                    {
                        if (nowPos.x < targetPos.x)
                        {
                            nowPos.x += 1;
                            direction = DirectionType.RightUp;
                        }
                        else if (nowPos.x > targetPos.x)
                        {
                            nowPos.x -= 1;
                            direction = DirectionType.LeftDown;
                        }
                        else if (nowPos.y < targetPos.y)
                        {
                            nowPos.y += 1;
                            direction = DirectionType.LeftUp;
                        }
                        else if (nowPos.y > targetPos.y)
                        {
                            nowPos.y -= 1;
                            direction = DirectionType.RightDown;
                        }
                    }
                    else
                    {
                        var before = nowPos; 
                        nowPos = moveRoute[0];
                        if (before.x < nowPos.x)
                        { direction = DirectionType.RightUp; }
                        else if (before.x > nowPos.x)
                        { direction = DirectionType.LeftDown; }
                        else if (before.y < nowPos.y)
                        { direction = DirectionType.LeftUp; }
                        else if (before.y > nowPos.y)
                        { direction = DirectionType.RightDown; }
                        moveRoute.RemoveAt(0);
                    }

                    DeletePosition(targetUnit);
                    dictArrangementPosition[nowPos] = targetUnit;
                    dictArrangement[targetUnit].Direction = direction;
                    posSelectUnit = nowPos;
                    DrawUnit();
                }
            }
            return true;
        }

        private bool MoveViewProcess()
        {
            if (mode != ProcessMode.MoveView)
            {
                return false;
            }
            deltaTime += Time.deltaTime;
            camMain.transform.position = Vector3.Lerp(baseViewPos, targetViewPos, deltaTime / waitTime);
            if (deltaTime > waitTime)
            {
                mode = ProcessMode.None;
            }
            return true;
        }

        private bool BattleViewProcess()
        {
            switch (mode)
            {
                case ProcessMode.BattleAttackMessage:
                case ProcessMode.BattleAttack:
                case ProcessMode.BattleAttackResultMessage:
                case ProcessMode.BattleCounterMessage:
                case ProcessMode.BattleCounter:
                case ProcessMode.BattleCounterResultMessage:
                    break;
                default:
                    return false;
            }
            var text = new List<string>();
            var atkChara = GetPermanenceCharacterData(BattleResult.Attacker.CharacterId);
            var defChara = GetPermanenceCharacterData(BattleResult.Defender.CharacterId);
            var atkUnit = GetPermanenceUnitData(BattleResult.Attacker.UnitId);
            var defUnit = GetPermanenceUnitData(BattleResult.Defender.UnitId);
            dictMessageData.TryGetValue(atkChara.Character.CharacterId, out var atkMessage);
            dictMessageData.TryGetValue(defChara.Character.CharacterId, out var defMessage);
            switch (mode)
            {
                case ProcessMode.BattleAttackMessage:
                    mode = ProcessMode.BattleAttack;
                    if (atkMessage == null || !atkMessage.HaveAttack(BattleInfo.Hit, atkUnit.HP.Percent))
                    { break; }
                    
                    text.AddRange(atkMessage.GetAttack(BattleInfo.Hit, atkUnit.HP.Percent));
                    mngSlgWindow.SetMessage(this, BattleResult.Attacker.CharacterId, string.Empty, string.Empty,
                        text.ToArray(), "down", 5);
                    break;
                case ProcessMode.BattleAttack:
                    text.Add($"{atkChara.CharacterName}の攻撃！");
                    if (BattleResult.Hit)
                    {
                        if (BattleResult.Critical)
                        { text.Add($"クリティカル！！"); }
                        text.Add($"{defChara.CharacterName}に{BattleResult.Damage}のダメージ！");
                    }
                    else
                    { text.Add($"{defChara.CharacterName}は回避した！"); }

                    mode = ProcessMode.BattleAttackResultMessage;
                    mngSlgWindow.SetMessage(this, string.Empty, string.Empty, "System",
                        text.ToArray(), "down", 3);
                    break;
                case ProcessMode.BattleAttackResultMessage:
                    if (BattleResult.Destroy)
                    {
                        DeleteUnit(BattleResult.Defender.UnitId, true, false);
                        mode = ProcessMode.None;
                        if (defMessage == null || !defMessage.HaveDestroy(BattleInfo.Hit, defUnit.HP.Percent))
                        {
                            text.Add($"{defChara.CharacterName}は撃破された！");
                            mngSlgWindow.SetMessage(this, string.Empty, string.Empty, "System",
                                text.ToArray(), "down", 3);
                        }
                        else
                        {
                            text.AddRange(defMessage.GetDestroy(BattleInfo.Hit, defUnit.HP.Percent));
                            mngSlgWindow.SetMessage(this, BattleResult.Defender.CharacterId, string.Empty, string.Empty,
                                text.ToArray(), "down", 5);
                        }
                        break;
                    }
                    mode = BattleResult.CounterWeapon == null ? ProcessMode.None : ProcessMode.BattleCounterMessage;
                    if (BattleResult.Hit)
                    {
                        if (defMessage == null || !defMessage.HaveDamage(BattleInfo.Hit, defUnit.HP.Percent))
                        { break; }
                        text.AddRange(defMessage.GetDamage(BattleInfo.Hit, defUnit.HP.Percent));
                    }
                    else
                    {
                        if (defMessage == null || !defMessage.HaveAvoid(BattleInfo.Hit, defUnit.HP.Percent))
                        { break; }
                        text.AddRange(defMessage.GetAvoid(BattleInfo.Hit, defUnit.HP.Percent));
                    }
                    mngSlgWindow.SetMessage(this, BattleResult.Defender.CharacterId, string.Empty, string.Empty,
                        text.ToArray(), "down", 5);
                    break;
                case ProcessMode.BattleCounterMessage:
                    mode = ProcessMode.BattleCounter;
                    if (defMessage == null || !defMessage.HaveAttack(BattleInfo.Hit, defUnit.HP.Percent))
                    { break; }
                    
                    text.AddRange(defMessage.GetAttack(BattleInfo.Hit, defUnit.HP.Percent));
                    mngSlgWindow.SetMessage(this, BattleResult.Defender.CharacterId, string.Empty, string.Empty,
                        text.ToArray(), "down", 5);
                    break;
                case ProcessMode.BattleCounter:
                    text.Add($"{defChara.CharacterName}の攻撃！");
                    if (BattleResult.CounterHit)
                    {
                        if (BattleResult.CounterCritical)
                        { text.Add($"クリティカル！！"); }
                        text.Add($"{atkChara.CharacterName}に{BattleResult.CounterDamage}のダメージ！");
                    }
                    else
                    { text.Add($"{atkChara.CharacterName}は回避した！"); }

                    mngSlgWindow.SetMessage(this, string.Empty, string.Empty, "System",
                        text.ToArray(), "down", 3);
                    mode = ProcessMode.BattleCounterResultMessage;
                    break;
                case ProcessMode.BattleCounterResultMessage:
                    mode = ProcessMode.None;
                    if (BattleResult.CounterDestroy)
                    {
                        DeleteUnit(BattleResult.Attacker.UnitId, true, false);
                        mode = ProcessMode.None;
                        if (atkMessage == null || !atkMessage.HaveDestroy(BattleInfo.CounterHit, atkUnit.HP.Percent))
                        {
                            text.Add($"{atkChara.CharacterName}は撃破された！");
                            mngSlgWindow.SetMessage(this, string.Empty, string.Empty, "System",
                                text.ToArray(), "down", 3);
                        }
                        else
                        {
                            text.AddRange(atkMessage.GetDestroy(BattleInfo.CounterHit, atkUnit.HP.Percent));
                            mngSlgWindow.SetMessage(this, BattleResult.Attacker.CharacterId, string.Empty, string.Empty,
                                text.ToArray(), "down", 5);
                        }
                        break;
                    }
                    if (BattleResult.CounterHit)
                    {
                        if (atkMessage == null || !atkMessage.HaveDamage(BattleInfo.CounterHit, atkUnit.HP.Percent))
                        { break; }
                        text.AddRange(atkMessage.GetDamage(BattleInfo.CounterHit, atkUnit.HP.Percent));
                    }
                    else
                    {
                        if (atkMessage == null || !atkMessage.HaveAvoid(BattleInfo.CounterHit, atkUnit.HP.Percent))
                        { break; }
                        text.AddRange(atkMessage.GetAvoid(BattleInfo.CounterHit, atkUnit.HP.Percent));
                    }
                    mngSlgWindow.SetMessage(this, BattleResult.Attacker.CharacterId, string.Empty, string.Empty,
                        text.ToArray(), "down", 5);
                    break;
            }
            return true;
        }

        private async UniTask NpcProcess()
        {
            if (nowGroup >= dictGroup.Count)
            { return; }
            var grp = dictGroup.Keys.ToArray()[nowGroup];
            foreach (var dat in dictArrangement.Values.Where(d => d.GroupId == grp && !d.IsEndAction))
            {
                var pos = GetPosition(dat.UnitId);
                posSelectUnit = pos;
                // 目標選択ルーチン
                SearchTargetRoutine(dat, pos);
                // 移動ルーチン
                if (MoveRoutine(dat, pos))
                { return; }
                // 攻撃ルーチン
                AttackRoutine(dat, pos);
                if (await NpcAttackProcess(dat))
                { return; }
                dat.IsEndAction = true;
                DrawUnit();
                SetWait(0.5f);
                return;
            }
            EndTurn();
        }

        private async UniTask<bool> NpcAttackProcess(ArrangementData dat)
        {
            if (dat.IsEndAction || BattleInfo.AttackWeapon == null)
            { return false; }
            Debug.Log($"NpcAttackProcess : {dat.UnitId}, {BattleInfo.AttackWeapon.WeaponName}");
            await SelectAttackArea(datBattleInfo.Attacker, datBattleInfo.Defender);
            dat.IsEndAction = true;
            return true;
        }

        private void SearchTargetRoutine(ArrangementData dat, Vector2Int pos)
        {
            if (dat.IsEndMove)
            { return; }

            Debug.Log($"SearchTargetRoutine : {dat.UnitId}, {pos}");
            datBattleInfo.Attacker = null;
            datBattleInfo.Defender = null;
            datBattleInfo.AttackWeapon = null;
            dat.RouteData = GetRoute(pos, dat);
            switch (dat.ActionType)
            {
                case ActionType.Normal:
                    var target = SearchEnemyRoutine(dat, pos);
                    if (target == null)
                    { return; }
                    var targetPosition = GetPosition(target.UnitId);
                    foreach (var wep in GetPermanenceUnitData(dat.UnitId).GetAvailableWeapons())
                    {
                        var range = GetRange(pos, wep).Where(
                            x => x.Key == targetPosition);
                        if (range.Any())
                        {
                            dat.TargetPosition = pos;
                            posTargetUnit = targetPosition;
                            datBattleInfo.Attacker = dat;
                            datBattleInfo.Defender = target;
                            datBattleInfo.AttackWeapon = wep;
                            return;
                        }

                        var move = dat.RouteData.Where(
                            x => Distance(x.Key, targetPosition) >= wep.RangeMin && Distance(x.Key, targetPosition) <= wep.RangeMax 
                        ).OrderBy(x => Distance(x.Key, pos));
                        if (move.Any())
                        {
                            dat.TargetPosition = move.First().Key;
                            posTargetUnit = targetPosition;
                            datBattleInfo.Attacker = dat;
                            datBattleInfo.Defender = target;
                            datBattleInfo.AttackWeapon = wep;
                            return;
                        }
                    }
                    var route = dat.RouteData.Where(
                        x => x.Value.IsStop
                    ).OrderBy(x => Distance(x.Value.Position, targetPosition));
                    if (route.Any())
                    {
                        dat.TargetPosition = route.First().Key;
                    }
                    break;
                case ActionType.Destination:
                case ActionType.Guard:
                case ActionType.Peace:
                    break;
            }
        }

        private ArrangementData SearchEnemyRoutine(ArrangementData dat, Vector2Int pos)
        {
            var potentialTargets = dictArrangement.Values.Where(
                x => x.GroupId != dat.GroupId && !dictGroup[x.GroupId].Friendly.Contains(dat.GroupId));
            if (!potentialTargets.Any())
            { return null; }
            var target = potentialTargets.OrderBy(
                x => Distance(pos, GetPosition(x.UnitId)));
            return target.First();
        }

        private bool MoveRoutine(ArrangementData dat, Vector2Int pos)
        {
            Debug.Log($"MoveRoutine : {dat.UnitId}, {pos}");
            if (!dat.IsEndMove && pos != dat.TargetPosition)
            {
                // 移動ルーチン
                if (dat.RouteData.Count > 0 && dat.RouteData.Count(x => x.Value.IsStop) > 0)
                {
                    var route = dat.RouteData.Where(
                        x => x.Value.IsStop
                    ).OrderBy(x => Distance(x.Value.Position, dat.TargetPosition)).First();
                    dat.IsEndMove = true;
                    Debug.Log($"{route.Value.Position}, {dat.TargetPosition}, {Distance(route.Value.Position, dat.TargetPosition)}");
                    MoveUnit(dat.UnitId, route.Value.Position, DirectionType.LeftDown, true, dat.RouteData);
                    return true;
                }
            }
            return false;
        }

        private void AttackRoutine(ArrangementData dat, Vector2Int pos)
        {
            if (dat.IsEndAction || datBattleInfo.AttackWeapon != null)
            { return; }
            Debug.Log($"AttackRoutine : {dat.UnitId}, {pos}");
            var target = SearchEnemyRoutine(dat, pos);
            var targetPosition = Vector2Int.zero;
            if (target != null)
            { targetPosition = GetPosition(target.UnitId); }
            switch (dat.ActionType)
            {
                case ActionType.Destination:
                case ActionType.Guard:
                    if (target == null)
                    { return; }
                    foreach (var wep in GetPermanenceUnitData(dat.UnitId).GetAvailableWeapons(Distance(pos, targetPosition), GetArrangementData(targetPosition)))
                    {
                        posTargetUnit = targetPosition;
                        datBattleInfo.Attacker = dat;
                        datBattleInfo.Defender = target;
                        datBattleInfo.AttackWeapon = wep;
                        break;
                    }
                    break;
            }
        }

        private void ControlCursor()
        {
            if (waitNavigate >= WaitNavigateTime)
            {
                var navigate = action.UI.Navigate.ReadValue<Vector2>();
                var before = posCursor;
                if (navigate.sqrMagnitude != 0)
                {
                    waitNavigate = 0;
                    if (navigate.x < 0)
                    {
                        posCursor.y += 1;
                    }
                    else if (navigate.x > 0)
                    {
                        posCursor.y -= 1;
                    }
                    if (navigate.y < 0)
                    {
                        posCursor.x -= 1;
                    }
                    else if (navigate.y > 0)
                    {
                        posCursor.x += 1;
                    }
                    if (!GetCanCursor(posCursor))
                    {
                        posCursor = before;
                    }
                    SetViewCenter(posCursor, 0);
                }
                else
                {
                    waitNavigate = 0.15f;
                    posCursor = mngStage.GetMouseTile(nowScale);
                    if (!GetCanCursor(posCursor))
                    {
                        posCursor = before;
                        return;
                    }
                    if (!isClick)
                    { return; }
                    Debug.Log($"Click : {posCursor}");
                    isClick = false;
                    if (mode == ProcessMode.DisplayMoveArea)
                    {
                        var dat = GetArrangementData(posSelectUnit);
                        SelectMoveArea(dat);
                    }
                    else if (mode == ProcessMode.DisplayAttackArea)
                    {
                        var dat = GetArrangementData(posSelectUnit);
                        SelectAttackArea(dat);
                    }
                    else if (!mngSlgWindow.IsDisplayCommandMenu)
                    {
                        if (mode != ProcessMode.ArrangementUnits)
                        {
                            if (dictArrangementPosition.ContainsKey(posCursor))
                            {
                                posSelectUnit = posCursor;
                                mngSlgWindow.DisplayCommandMenu(this);
                            }
                        }
                        else
                        {
                            var dat = GetArrangementData(posCursor);
                            if (dat == null && arrangementUnitNum < arrangementUnitMaxNum && lstCanArrangementPosition.Contains(posCursor) && dictArrangement.Values.Count(v => !v.IsArrangement && !v.IsTemporaryArrangement && v.CharacterId != string.Empty) > 0)
                            {
                                mngSlgWindow.SetArrangementUnitsList(this, dictArrangement.Values.ToArray(), dictGroup, dictUnits, dictCharacters, arrangementUnitDirection);
                                return;
                            }
                            posSelectUnit = posCursor;
                            mngSlgWindow.DisplayArangementUnitMenu(this, dat);
                        }
                    }
                }
            }
            else
            {
                waitNavigate += Time.deltaTime;
            }
        }

        private bool DisplayMoveAreaProcess()
        {
            if (mode != ProcessMode.DisplayMoveArea)
            {
                return false;
            }
            if (CancelSelectArea())
            {
                return true;
            }
            mngSlgWindow.DisplaySimpleStatus(this);
            SetCursor(posCursor);
            SelectMoveArea();
            ControlCursor();
            return true;
        }

        private bool DisplayAttackAreaProcess()
        {
            if (mode != ProcessMode.DisplayAttackArea)
            {
                return false;
            }
            if (CancelSelectArea())
            {
                return true;
            }
            mngSlgWindow.DisplaySimpleStatus(this);
            SetCursor(posCursor);
            SelectAttackArea();
            ControlCursor();
            return true;
        }

        private bool CheckConditionProcess(ArrangementData caller)
        {
            // 勝利・敗北・イベント条件の確認
            if (CheckCondition(lstVictoryCondition, caller) || CheckCondition(lstDefeatCondition, caller) || CheckCondition(lstEventCondition, caller))
            { return true; }
            return false;
        }

        private bool CheckCondition(List<ConditionData> list, ArrangementData caller)
        {
            foreach (var condition in list)
            {
                var flg = true;
                switch (condition.ObjectiveType)
                {
                    case ObjectiveType.Reach:
                        // 特定ユニットの視点地点への到達
                        flg = false;
                        foreach (var unitId in condition.Targets)
                        {
                            var pos = GetPosition(unitId);
                            if (pos.x >= condition.Position.x && pos.x <= condition.Position.x + condition.AreaSize.x &&
                                pos.y >= condition.Position.y && pos.y <= condition.Position.y + condition.AreaSize.y)
                            {
                                flg = true;
                                break;
                            }
                        }
                        // 特定勢力の指定地点への到達
                        if (condition.GroupId != 0 && caller != null && caller.GroupId == condition.GroupId)
                        {
                            var pos = GetPosition(caller.UnitId);
                            if (pos.x >= condition.Position.x && pos.x <= condition.Position.x + condition.AreaSize.x &&
                                pos.y >= condition.Position.y && pos.y <= condition.Position.y + condition.AreaSize.y)
                            {
                                flg = true;
                            }
                        }
                        break;
                    case ObjectiveType.ReachAll:
                        // 全指定ユニットの視点地点への到達
                        foreach (var unitId in condition.Targets)
                        {
                            var pos = GetPosition(unitId);
                            if (pos.x < condition.Position.x || pos.x > condition.Position.x + condition.AreaSize.x &&
                                pos.y < condition.Position.y || pos.y > condition.Position.y + condition.AreaSize.y)
                            {
                                flg = false;
                                break;
                            }
                        }
                        // 特定勢力の指定地点への到達
                        if (condition.GroupId != 0 || dictArrangement.Values.Where(
                                x => x.GroupId == condition.GroupId).Select(
                                x => GetPosition(x.UnitId)).Count(
                                x => x.x < condition.Position.x || x.x > condition.Position.x + condition.AreaSize.x &&
                                x.y < condition.Position.y || x.y > condition.Position.y + condition.AreaSize.y) > 0)
                        { flg = false; }
                        break;
                    case ObjectiveType.Destroy:
                        // 特定ユニットの撃破
                        flg = false;
                        foreach (var unitId in condition.Targets)
                        {
                            if (!dictArrangement.ContainsKey(unitId))
                            {
                                flg = true;
                                break;
                            }
                        }
                        // 特定勢力の撃破
                        if (condition.GroupId != 0 && caller != null && caller.GroupId == condition.GroupId)
                        { flg = true; }
                        break;
                    case ObjectiveType.DestroyAll:
                        // 特定ユニットの全滅
                        foreach (var unitId in condition.Targets)
                        {
                            if (dictArrangement.ContainsKey(unitId))
                            {
                                flg = false;
                                break;
                            }
                        }
                        // 特定勢力の全滅
                        if (condition.GroupId != 0 || dictArrangement.Values.Count(x => x.GroupId == condition.GroupId) > 0)
                        { flg = false; }
                        break;
                }
                if (flg)
                {
                    mngScript.Variable["system"].SetVariable("trigger_event_id", condition.EventId);
                    mngScript.CallMethod(condition.Name, condition.Method);
                    return true;
                }
            }
            return false;
        }

        private bool CancelSelectArea()
        {
            if (!IsCancel)
            {
                return false;
            }
            mngSelect.ClearAllTiles();
            SetViewCenter(posSelectUnit, 0);
            switch (mode)
            {
                case ProcessMode.DisplayMoveArea:
                    mngSlgWindow.DisplayCommandMenu(this);
                    break;
                case ProcessMode.DisplayAttackArea:
                    mngSlgWindow.DisplayWeaponMenu(this);
                    break;
            }
            mode = ProcessMode.None;
            return true;
        }

        public void DisplayMoveArea()
        {
            mngSelect.ClearAllTiles();
            if (!dictArrangementPosition.ContainsKey(posSelectUnit))
            {
                return;
            }
            mode = ProcessMode.DisplayMoveArea;
            var dat = datBattleInfo.Attacker;
            dat.RouteData = GetRoute(posSelectUnit, dat);
            foreach (var pos in dat.RouteData.Keys)
            {
                var cpos = Get3dPosition(pos);
                var tile = datCommonSettings.MoveArea(dictMapTiles[cpos].CursorIndex);
                if (!dat.RouteData[pos].IsStop || tile == null)
                {
                    continue;
                }
                mngSelect.SetTile(cpos, tile);
            }
            mngSelect.SetTile(Get3dPosition(posSelectUnit), datCommonSettings.SelfMarker());
        }

        private void SelectMoveArea()
        {
            if (!action.UI.Enter.triggered)
            { return; }
            SelectMoveArea(datBattleInfo.Attacker);
        }

        private void SelectMoveArea(ArrangementData dat)
        {
            if (!dat.RouteData.ContainsKey(posCursor) || !dat.RouteData[posCursor].IsStop)
            { return; }
            dat.BeforePosition = posSelectUnit;
            dat.BeforeDirection = dat.Direction;
            dat.IsEndMove = true;
            mngSelect.ClearAllTiles();
            mode = ProcessMode.None;
            MoveUnit(dat.UnitId, posCursor, DirectionType.LeftDown, true, dat.RouteData);
            mngSlgWindow.DisplayCommandMenu(this);
        }

        public void DisplayAttackArea(WeaponData weapon)
        {
            mngSelect.ClearAllTiles();
            mode = ProcessMode.DisplayAttackArea;
            var dat = datBattleInfo.Attacker;
            dat.RangeData = GetRange(posSelectUnit, weapon);
            foreach (var pos in dat.RangeData.Keys)
            {
                var cpos = Get3dPosition(pos);
                var tile = datCommonSettings.AttackArea(dictMapTiles[cpos].CursorIndex);
                if (!dat.RangeData[pos].IsAttack || tile == null)
                {
                    continue;
                }
                mngSelect.SetTile(cpos, tile);
            }
            mngSelect.SetTile(Get3dPosition(posSelectUnit), datCommonSettings.SelfMarker());
        }

        private void SelectAttackArea()
        {
            if (!isClick)
            { return; }
            SelectAttackArea(datBattleInfo.Attacker);
        }

        private void SelectAttackArea(ArrangementData dat)
        {
            Debug.Log($"SlgSceneManager.SelectAttackArea : {dat.UnitId}");
            if (!dat.RangeData.ContainsKey(posCursor) || !dat.RangeData[posCursor].IsTarget)
            { return; }
            posTargetUnit = posCursor;
            SelectAttackArea(dat, GetArrangementData(posTargetUnit));
        }
        
        private async UniTask SelectAttackArea(ArrangementData dat, ArrangementData target)
        {
            Debug.Log($"SlgSceneManager.SelectAttackArea : {dat.UnitId}, {target.UnitId}");
            // 戦闘BGMスタート
            var atkChara = dictCharacters[datBattleInfo.Attacker.CharacterId];
            if (!String.IsNullOrWhiteSpace(atkChara.Character.Music))
            {
                SetMusic(atkChara.Character.Music, true);
            }
            datBattleInfo.Defender = target;
            if (dictGroup[datBattleInfo.Defender.GroupId].Player)
            {
                // リアクション選択
                // ToDo : 要実装
                mode = ProcessMode.SelectReaction;
            }
            else
            {
                // AI思考
                // ToDo : 高度な思考
                mode = ProcessMode.None;
                datBattleInfo.CounterWeapon = null;
                switch (dat.ActionType)
                {
                    case ActionType.Peace:
                        datBattleInfo.Reaction = ReactionMode.Defense;
                        break;
                    default:
                        var distance = Distance(posSelectUnit, posTargetUnit);
                        var unit = dictUnits[datBattleInfo.Defender.UnitId];
                        foreach (var wep in unit.GetAvailableWeapons(distance, GetArrangementData(posTargetUnit)))
                        {
                            datBattleInfo.Reaction = ReactionMode.Counter;
                            datBattleInfo.CounterWeapon = wep;
                            break;
                        }
                        if (datBattleInfo.CounterWeapon == null)
                        {
                            datBattleInfo.Reaction = ReactionMode.Avoidance;
                        }
                        break;
                }
            }
            await mngSlgWindow.DisplayBattleConfirmWindow(this);
            mngSelect.ClearAllTiles();
            mngSelect.SetTile(Get3dPosition(posSelectUnit), datCommonSettings.SelfMarker());
            mngSelect.SetTile(Get3dPosition(posTargetUnit), datCommonSettings.TargetMarker());
        }

        public async UniTask CalcBattleInfo()
        {
            try
            {
                var attacker = datBattleInfo.Attacker;
                var defender = datBattleInfo.Defender;
                var atkChara = dictCharacters[attacker.CharacterId];
                var defChara = dictCharacters[defender.CharacterId];
                var atkUnit = dictUnits[attacker.UnitId];
                var defUnit = dictUnits[defender.UnitId];
                var atkTile = GetTile(GetPosition(attacker.UnitId));
                var defTile = GetTile(GetPosition(defender.UnitId));
                datBattleInfo = await BattleUtil.CalcBattleInfo(utilMond, datBattleInfo, mode, attacker, defender, atkChara, atkUnit, defChara, defUnit, atkTile, defTile);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                mngSlgWindow.SetError(ex.Message);
            }
        }

        public void BattleStart()
        {
            // 攻撃
            mode = ProcessMode.BattleAttackMessage;
            //mode = ProcessMode.None;
            mngSelect.ClearAllTiles();
            datBattleResult.Attacker = datBattleInfo.Attacker;
            datBattleResult.Defender = datBattleInfo.Defender;
            var hit = Random.Range(1, 100);
            datBattleResult.AttackWeapon = datBattleInfo.AttackWeapon;
            datBattleResult.CounterWeapon = datBattleInfo.CounterWeapon;
            datBattleResult.Reaction = datBattleInfo.Reaction;
            datBattleResult.Hit = hit < datBattleInfo.Hit;
            if (datBattleResult.Hit)
            {
                var min = 0.9f;
                var max = 1.1f;
                // クリティカルチェック
                datBattleResult.Critical = hit < datBattleInfo.Critical;
                if (datBattleResult.Critical)
                {
                    min = 1.8f;
                    max = 2.2f;
                }
                var damage = datBattleInfo.AttackPower * Random.Range(min, max);
                damage -= damage * datBattleInfo.DefenceRate / 100.0f;
                datBattleResult.Damage = Convert.ToInt32(math.clamp(math.ceil(damage), 10, int.MaxValue));
                var unit = dictUnits[datBattleInfo.Defender.UnitId];
                unit.HP.Calc("-", datBattleResult.Damage);
                dictUnits[datBattleInfo.Defender.UnitId] = unit;
                // 撃破
                datBattleResult.Destroy = unit.HP.Now == 0;
                if (datBattleResult.Destroy)
                { return; }
            }
            // 反撃
            if (datBattleInfo.Reaction != ReactionMode.Counter)
            { return; }
            hit = Random.Range(1, 100);
            datBattleResult.CounterHit = hit < datBattleInfo.CounterHit;
            if (datBattleResult.CounterHit)
            {
                var min = 0.9f;
                var max = 1.1f;
                // クリティカルチェック
                datBattleResult.Critical = hit < datBattleInfo.CounterCritical;
                if (datBattleResult.CounterCritical)
                {
                    min = 1.8f;
                    max = 2.2f;
                }
                var damage = datBattleInfo.CounterPower * Random.Range(min, max);
                damage -= damage * datBattleInfo.CounterDefenceRate / 100.0f;
                datBattleResult.CounterDamage = Convert.ToInt32(math.clamp(math.ceil(damage), 10, int.MaxValue));
                var unit = dictUnits[datBattleInfo.Attacker.UnitId];
                unit.HP.Calc("-", datBattleResult.CounterDamage);
                dictUnits[datBattleInfo.Attacker.UnitId] = unit;
                // 撃破
                datBattleResult.CounterDestroy = unit.HP.Now == 0;
            }
        }

        public void SetCursor(Vector2Int pos)
        {
            var cursor = Get3dPosition(pos);
            if (!dictMapTiles.TryGetValue(cursor, out var dat))
            { return; }

            var tile = datCommonSettings.Cursor(dat.CursorIndex);
            if (tile == null)
            {
                return;
            }
            mngCursor.SetTile(cursor, tile);
            if (datCommonSettings.DebugMode)
            {
                mngSlgWindow.SetDebug(cursor, dat, nowScale, mode, $"{mngScript.NowClass}-{mngScript.NowScriptName}-{mngScript.NowScriptLine}");
            }
        }

        public void SetTile(Vector3Int pos, string chipId)
        {
            SetTile(pos, map.TileSet, chipId);
        }

        public void SetTile(Vector3Int posTile, string chipSetId, string chipId)
        {
            if (!dictTiles.ContainsKey(chipSetId) || !dictTiles[chipSetId].Tiles.ContainsKey(chipId))
            {
                return;
            }
            dictMapTiles[posTile] = dictTiles[chipSetId].Tiles[chipId];
            mngStage.SetColor(posTile, Color.white);
            mngStage.SetTile(posTile, dictTiles[chipSetId].Tiles[chipId]);
            var pos = new Vector2Int(posTile.x, posTile.y);
            if (dictTiles[chipSetId].Tiles[chipId].Collision)
            {
                if (dictHeight.GetValueOrDefault(pos, 0) < posTile.z)
                {
                    dictHeight[pos] = posTile.z;
                }
            }
            if (dictHeight.GetValueOrDefault(pos, 0) == posTile.z)
            {
                canCursor[pos] = !dictTiles[chipSetId].Tiles[chipId].VoidTile;
            }
        }

        public void SetMap(string newMap)
        {
            if (!dictMaps.ContainsKey(newMap))
            {
                return;
            }
            ClearMap();
            map = dictMaps[newMap];
            for (var x = -10; x <= map.Max.x + 10; x++)
            {
                for (var y = -10; y <= map.Max.y + 10; y++)
                {
                    if (x < 0 || x > map.Max.x || y < 0 || y > map.Max.y)
                    {
                        SetTile(new Vector3Int(x, y, 0), map.TileSet, "void");
                    }
                    else
                    {
                        SetTile(new Vector3Int(x, y, 0), map.TileSet, map.InitTile);
                    }
                }
            }

            foreach (var tile in map.Tiles)
            {
                SetTile(new Vector3Int(tile.Position.x, tile.Position.y, tile.Position.z * 2), tile.TileSetId, tile.TileId);
            }
        }

        public void ClearMap()
        {
            map = null;
            dictMapTiles.Clear();
            dictHeight.Clear();
            mngStage.ClearAllTiles();
            canCursor.Clear();
        }

        public void EndTurn()
        {
            nowGroup++;
            if (nowGroup >= dictGroup.Count)
            {
                nowGroup = 0;
                mngScript.Variable["system"].SetVariable(
                    "now_turn", 
                    (Convert.ToInt32(mngScript.Variable["system"].GetVariable("now_turn")) + 1).ToString()
                );
            }
            var grp = dictGroup.Keys.ToArray()[nowGroup];
            foreach (var dat in dictArrangement.Values.Where(d => d.GroupId == grp))
            {
                dat.IsEndMove = false;
                dat.IsEndAction = false;
            }
            DrawUnit();
            StartGroupMusic(grp);
        }

        public void StartGroupMusic()
        {
            if (nowGroup >= dictGroup.Count)
            { return; }
            var grp = dictGroup.Keys.ToArray()[nowGroup];
            StartGroupMusic(grp);
        }

        private void StartGroupMusic(int grp)
        {
            if (!String.IsNullOrWhiteSpace(dictGroup[grp].Music))
            { SetMusic(dictGroup[grp].Music, true); }
        }

        public void ArrangementUnitsAccept()
        {
            mode = ProcessMode.None;
            mngSelect.ClearAllTiles();
            mngScript.CallMethod(nextCallClass, nextCallMethod, 0);
        }

        public void DrawUnit()
        {
            mapUnit.ClearAllTiles();
            foreach (var pos in dictArrangementPosition.Keys)
            {
                var dat = dictArrangement[dictArrangementPosition[pos]];
                var unit = dictUnits[dat.UnitId].Unit;
                var tpos = Get3dPosition(pos);
                mapUnit.SetTile(tpos, unit.GetBaseTile(dat.Direction), datCommonSettings.GroupMarker().OverlayBackLayer.Tile,  unit.GetFrontTile(dat.Direction), dat.MoveType);
                mapUnit.SetTileFlags(tpos, TileFlags.None, dat.MoveType);
                var color = dictGroup[dat.GroupId].GroupColor;
                mapUnit.SetColor(tpos, color, LayerType.OverlayBack);
                // 行動済みユニットは薄暗く表示
                mapUnit.SetColor(tpos, dat.IsEndAction ? Color.gray : Color.white);
            }
        }

        public void SetGroup(int id, string grpName, Color32 grpColor, bool isPlayer, int[] lstFriend, string grpMusic)
        {
            dictGroup[id] = new GroupData(id, grpName, grpColor, isPlayer, lstFriend, grpMusic);
        }

        public void EditGroup(int id, string grpName, Color32 grpColor, bool isPlayer, int[] lstFriend, string grpMusic)
        {
            if (!dictGroup.ContainsKey(id))
            { throw new Exception(LocaleUtil.GetMessage("E_C0001", "group_id")); }
            dictGroup[id].SetData(grpName, grpColor, isPlayer, lstFriend, grpMusic);
        }

        public void DeleteGroup(int id)
        {
            dictGroup.Remove(id);
        }

        public void ClearGroup()
        {
            dictGroup.Clear();
        }

        public Color32 GetGroupColor(int id)
        {
            if (!dictGroup.TryGetValue(id, out var value))
            { return Color.white; }
            return value.GroupColor;
        }

        public void AddUnit(
            int group,
            string uid,
            string cid,
            Vector2Int pos,
            int direction,
            MoveType move,
            string udata,
            string cdata,
            int lv,
            bool nonArrangement,
            bool anim,
            bool hidden,
            string unitName,
            string charaName,
            ActionType actionType,
            Vector2Int targetPosition)
        {
            Debug.Log($"add_unit Group:{group} UnitID:{uid} CharaID:{cid} UnitData:{udata} CharaData:{cdata}");
            ArrangementData dat = null;
            if (!string.IsNullOrWhiteSpace(uid))
            { dictArrangement.TryGetValue(uid, out dat); }
            // 永続情報がなく、ユニットデータもキャラクターデータも指定されていない場合は何もしない
            if (dat == null && string.IsNullOrWhiteSpace(udata) && string.IsNullOrWhiteSpace(cdata))
            { return; }
            // 指定されたグループが定義されていない場合は処理を中断
            if (!dictGroup.ContainsKey(group))
            { return; }
            // 指定されたユニットデータが定義されていない場合は処理を中断
            if (!string.IsNullOrWhiteSpace(udata) && !dictUnitData.ContainsKey(udata))
            { return; }
            // 指定されたキャラクターデータが定義されていない場合は処理を中断
            if (!string.IsNullOrWhiteSpace(cdata) && !dictCharacterData.ContainsKey(cdata))
            { return; }
            if (!string.IsNullOrWhiteSpace(udata) && !string.IsNullOrWhiteSpace(uid) && !dictUnits.ContainsKey(uid))
            {
                dictUnits[uid] = new PermanenceUnitData(uid, dictUnitData[udata], unitName, hidden);
            }
            if (!string.IsNullOrWhiteSpace(cdata) && !string.IsNullOrWhiteSpace(cid) && !dictCharacters.ContainsKey(cid))
            {
                dictCharacters[cid] = new PermanenceCharacterData(cid, lv, dictCharacterData[cdata], charaName, hidden);
            }
            // 永続情報がなく、ユニットデータかユニットIDが指定されていない場合は処理中断
            if (dat == null && (string.IsNullOrWhiteSpace(udata) || string.IsNullOrWhiteSpace(uid)))
            { return; }

            if (dat == null && dictArrangement.Values.Count(x => !string.IsNullOrWhiteSpace(cid) && x.CharacterId == cid && !x.IsClone) == 0)
            {
                dat = new ArrangementData(group, uid, cid);
                dictArrangement[dat.UnitId] = dat;
            }
            // 非配置の場合は処理中断
            if (nonArrangement)
            { return; }

            dat.IsArrangement = true;
            // TODO : 同一座標の考慮
            // TODO : 障害物の考慮
            dictArrangementPosition[pos] = dat.UnitId;
            var unit = dictUnits[dat.UnitId].Unit;
            var tile = GetTile(pos);
            if (move == MoveType.None)
            {
                move = unit.GetMostOptimalMoveType(tile);
            }
            dat.MoveType = SetMoveType(unit, tile, move);
            dat.Direction = (DirectionType)direction;
            DrawUnit();
            if (anim)
            {
                SetViewCenter(pos, 0);
                SetWait(0.25f);
            }

            dat.ActionType = actionType;
            dat.TargetPosition = targetPosition;
        }

        public void ArrangementUnit(string uid, Vector2Int pos, DirectionType direction)
        {
            Debug.Log($"arrange_unit UnitID:{uid}");
            dictArrangement.TryGetValue(uid, out var dat);
            dat.IsTemporaryArrangement = true;
            dictArrangementPosition[pos] = dat.UnitId;
            dat.MoveType = SetMoveType(dictUnits[uid].Unit, GetTile(pos), MoveType.Space);
            dat.Direction = direction;
            dictArrangement[uid] = dat;
            DrawUnit();
            arrangementUnitNum += 1;
        }

        private MoveType SetMoveType(UnitData unit, TileData tile, MoveType move)
        {
            switch (move)
            {
                case MoveType.Space:
                    if (GetCanMove(unit.Space, tile, MoveType.Space))
                    { return MoveType.Space; }
                    break;
                case MoveType.Air:
                    if (GetCanMove(unit.Air, tile, MoveType.Air))
                    { return MoveType.Air; }
                    break;
                case MoveType.Ground:
                    if (GetCanMove(unit.Ground, tile, MoveType.Ground))
                    { return MoveType.Ground; }
                    break;
                case MoveType.Underwater:
                    if (GetCanMove(unit.Underwater, tile, MoveType.Underwater))
                    { return MoveType.Underwater; }
                    break;
            }
            if (GetCanMove(unit.Space, tile, MoveType.Space))
            { return MoveType.Space; }
            if (GetCanMove(unit.Air, tile, MoveType.Air))
            { return MoveType.Air; }
            if (GetCanMove(unit.Ground, tile, MoveType.Ground))
            { return MoveType.Ground; }
            if (GetCanMove(unit.Underwater, tile, MoveType.Underwater))
            { return MoveType.Underwater; }

            return MoveType.Space;
        }

        private bool GetCanMove(SuitableData suitable, TileData tile, MoveType move)
        {
            Debug.Log($"GetCanMove : {suitable}, {tile}, {tile.MoveCost}, {move}");
            return suitable.suitable > Suitable.E && tile.MoveCost.GetValueOrDefault(move, new MoveCostData(move, -1)).MoveCost > 0;
        }

        public void EditUnit(string uid, string[] statuses, string op, string[] values, string unitName, bool hidden)
        {
            if (!dictUnits.ContainsKey(uid))
            { return; }
            Debug.Log($"edit_unit {uid}");
            var unit = dictUnits[uid];
            for (var i=0;i<statuses.Length;i++)
            {
                if (i >= values.Length)
                { break; }
                unit.SetStatus(statuses[i], op, values[i]);
            }
            if (!String.IsNullOrWhiteSpace(unitName))
            { unit.UnitName = unitName; }
            unit.Hidden = hidden;
        }

        public void EditCharacter(string cid, string[] statuses, string op, string[] values, string charaName, bool hidden)
        {
            if (!dictCharacters.ContainsKey(cid))
            { return; }
            Debug.Log($"edit_character {cid}");
            var chara = dictCharacters[cid];
            for (var i = 0; i < statuses.Length; i++)
            {
                if (i >= values.Length)
                { break; }
                chara.SetStatus(statuses[i], op, values[i]);
            }
            if (!String.IsNullOrWhiteSpace(charaName))
            { chara.CharacterName = charaName; }
            chara.Hidden = hidden;
        }

        public void CancelArrangementUnit(string uid)
        {
            Debug.Log($"cancel_arrange_unit {uid}");
            var pos = GetPosition(uid);
            dictArrangement.TryGetValue(uid, out var dat);
            if (pos != nonePosition)
            {
                DeletePosition(uid);
                DrawUnit();
            }
            dat.IsTemporaryArrangement = false;
            arrangementUnitNum -= 1;
        }
        
        public bool DeleteUnit(string uid, bool anim, bool permanent)
        {
            var result = false;
            Debug.Log($"delete_unit {uid} {anim} {permanent}");
            var pos = GetPosition(uid);
            dictArrangement.TryGetValue(uid, out var dat);
            if (pos != nonePosition)
            {
                if (anim)
                {
                    result = true;
                    SetViewCenter(pos, 0);
                    if (GetUnitData(uid).Machine)
                    { BlastEffect(pos); }
                }
                DeletePosition(uid);
                DrawUnit();
                result = CheckConditionProcess(dat) || result;
            }
            if (permanent)
            {
                dictArrangement.Remove(uid);
                dictUnits.Remove(uid);
            }
            else
            {
                dat.IsArrangement = false;
            }
            return result;
        }

        public void MoveUnit(string uid, Vector2Int pos, DirectionType direction, bool anim, Dictionary<Vector2Int, MoveData> route=null)
        {
            // TODO : 同一座標の考慮
            // TODO : 移動経路の考慮
            if (dictArrangementPosition.ContainsKey(pos))
            {
                return;
            }
            if (!anim)
            {
                DeletePosition(uid);
                dictArrangementPosition[pos] = uid;
                dictArrangement[uid].Direction = direction;
                posSelectUnit = pos;
                DrawUnit();
            }
            else
            {
                mode = ProcessMode.MoveUnit;
                targetUnit = uid;
                targetPos = pos;
                if (route != null)
                { moveRoute = route[pos].Route; }
                waitTime = 0.25f;
                deltaTime = 0;
            }
        }

        private Vector2Int GetPosition(string uid)
        {
            var keyValue = dictArrangementPosition.FirstOrDefault(x => x.Value == uid);
            return keyValue.Equals(default(KeyValuePair<Vector2Int, string>)) ? nonePosition : keyValue.Key;
        }

        private void DeletePosition(string uid)
        {
            var target = GetPosition(uid);
            if (target != nonePosition)
            {
                dictArrangementPosition.Remove(target);
            }
        }

        public void ClearUnit(bool permanent)
        {
            Debug.Log($"clear_unit");
            dictArrangementPosition.Clear();
            if (permanent)
            {
                dictArrangement.Clear();
                dictUnits.Clear();
            }
            else
            {
                foreach (var keyValuePair in dictArrangement)
                { keyValuePair.Value.IsArrangement = false; }
            }

            DrawUnit();
        }

        public void SetWait(float wait)
        {
            Debug.Log($"wait {wait}");
            mode = ProcessMode.Wait;
            waitTime = wait;
            deltaTime = 0;
        }

        public void SetViewCenter(Vector2Int pos, float time)
        {
            posCenter = pos;
            posCursor = pos;
            var cpos = mngStage.GetCellCenterWorld(Get3dPosition(pos));
            cpos.z = -10;
            if (time > 0)
            {
                mode = ProcessMode.MoveView;
                baseViewPos = camMain.transform.position;
                targetViewPos = cpos;
                waitTime = time;
                deltaTime = 0;
            }
            else
            {
                camMain.transform.position = cpos;
            }
        }

        public void SetUnitList(bool intermission = false)
        {
            mngSlgWindow.SetUnitList(intermission, dictArrangement.Values.ToArray(), dictGroup, dictUnits, dictCharacters);
        }

        public void SetCharaList(bool intermission = false)
        {
            mngSlgWindow.SetCharaList(intermission, dictArrangement.Values.ToArray(), dictGroup, dictCharacters);
        }

        public async void SetMusic(string nameMusic, bool isLoop)
        {
            if (String.IsNullOrWhiteSpace(nameMusic))
            {
                var grp = dictGroup[dictGroup.Keys.ToArray()[nowGroup]];
                if (!String.IsNullOrWhiteSpace(grp.Music))
                { nameMusic = grp.Music; }
                else if (!String.IsNullOrWhiteSpace(map.Music))
                { nameMusic = map.Music; }
                else
                { return; }
            }
            if (nameMusic == nowMusic)
            { return; }
            nowMusic = nameMusic;
            music.clip = await DataUtil.GetAudio(nameMusic);
            music.loop = isLoop;
            music.Play();
            isMusicStop = false;
        }

        public void StopMusic()
        {
            isMusicStop = true;
            music.Stop();
        }

        public async void SetSound(string nameSound)
        {
            sound.PlayOneShot(await DataUtil.GetSound(nameSound));
        }

        public void StopSound()
        {
            sound.Stop();
        }

        public bool GetCanCursor(Vector2Int pos)
        {
            return canCursor.GetValueOrDefault(pos, false);
        }

        public int GetHeight(Vector2Int pos)
        {
            return dictHeight.GetValueOrDefault(pos, 0);
        }

        public Dictionary<Vector2Int, MoveData> GetRoute(Vector2Int basePosition, ArrangementData dat)
        {
            var unit = dictUnits[dat.UnitId].Unit;
            var result = new Dictionary<Vector2Int, MoveData>();
            GetRoute(dat, result, new Vector2Int(basePosition.x-1, basePosition.y), unit.Move.status, new List<Vector2Int>());
            GetRoute(dat, result, new Vector2Int(basePosition.x+1, basePosition.y), unit.Move.status, new List<Vector2Int>());
            GetRoute(dat, result, new Vector2Int(basePosition.x, basePosition.y-1), unit.Move.status, new List<Vector2Int>());
            GetRoute(dat, result, new Vector2Int(basePosition.x, basePosition.y+1), unit.Move.status, new List<Vector2Int>());

            return result;
        }

        private void GetRoute(ArrangementData dat, Dictionary<Vector2Int, MoveData> routes, Vector2Int basePosition, float cost, List<Vector2Int> route)
        {
            // Map外か逆進したら計算終了
            if (!GetCanCursor(basePosition) || route.Contains(basePosition))
            { return; }
    
            // Map情報取得
            var moveCost = GetMoveCost(dat, basePosition);
            // 移動不可の場合は計算終了
            if (moveCost <= 0)
            { return; }
        
            // 移動コスト計算
            cost -= moveCost;
            if (cost < 0)
            { return; }
        
            // 算出済みルートの場合はコスト比較を実施し、移動コストが劣っていたら計算終了
            var move = routes.GetValueOrDefault(
                basePosition, 
                new MoveData()
                {
                    Position = basePosition,
                    Cost = cost
                });
            if (move.Cost > cost)
            { return; }
            route.Add(basePosition);
            move.Cost = cost;
            move.Route = route;

            // 別ユニットが存在したら停止不可とする
            move.IsStop = !dictArrangementPosition.ContainsKey(basePosition);
            routes[basePosition] = move;

            GetRoute(dat, routes, new Vector2Int(basePosition.x - 1, basePosition.y), cost, new List<Vector2Int>(route));
            GetRoute(dat, routes, new Vector2Int(basePosition.x + 1, basePosition.y), cost, new List<Vector2Int>(route));
            GetRoute(dat, routes, new Vector2Int(basePosition.x, basePosition.y - 1), cost, new List<Vector2Int>(route));
            GetRoute(dat, routes, new Vector2Int(basePosition.x, basePosition.y + 1), cost, new List<Vector2Int>(route));
        }

        private float GetMoveCost(ArrangementData dat, Vector2Int basePosition)
        {
            var tile = GetTile(basePosition);
            var unit = GetPermanenceUnitData(dat.UnitId);
            var move = tile.MoveCost.GetValueOrDefault(dat.MoveType, new MoveCostData(dat.MoveType, -1));
            var cost = -1.0f;
            // 適性値を逆転させて計算 (S=5 → 1, E=0 → 6)
            switch (move.MoveType)
            {
                case MoveType.Space:
                    cost = move.MoveCost * (6 - (int)unit.Unit.Space.suitable);
                    break;
                case MoveType.Air:
                    cost = move.MoveCost * (6 - (int)unit.Unit.Air.suitable);
                    break;
                case MoveType.Ground:
                    cost = move.MoveCost * (6 - (int)unit.Unit.Ground.suitable);
                    break;
                case MoveType.Underwater:
                    cost = move.MoveCost * (6 - (int)unit.Unit.Underwater.suitable);
                    break;
            }
            return cost;
        }

        public Dictionary<Vector2Int, RangeData> GetRange(Vector2Int basePosition, WeaponData dat)
        {
            var result = new Dictionary<Vector2Int, RangeData>();
            result = GetRange(result, basePosition, new Vector2Int(basePosition.x - 1, basePosition.y), dat, dat.RangeMin, dat.RangeMax);
            result = GetRange(result, basePosition, new Vector2Int(basePosition.x + 1, basePosition.y), dat, dat.RangeMin, dat.RangeMax);
            result = GetRange(result, basePosition, new Vector2Int(basePosition.x, basePosition.y - 1), dat, dat.RangeMin, dat.RangeMax);
            result = GetRange(result, basePosition, new Vector2Int(basePosition.x, basePosition.y + 1), dat, dat.RangeMin, dat.RangeMax);
            return result;
        }

        private Dictionary<Vector2Int, RangeData> GetRange(Dictionary<Vector2Int, RangeData> range, Vector2Int basePosition, Vector2Int targetPosition, WeaponData weapon, int min, int cost)
        {
            // Map外だったら計算終了
            if (!GetCanCursor(targetPosition))
            { return range; }

            // Map情報取得
            // TODO : 要実装

            // 移動コスト計算
            cost -= 1;
            if (cost < 0)
            { return range; }

            // 算出済みルートの場合はコスト比較を実施し、移動コストが劣っていたら計算終了
            if (range.ContainsKey(targetPosition))
            {
                if (range[targetPosition].Cost > cost)
                { return range; }
                range[targetPosition].Cost = cost;
            }
            else
            {
                range[targetPosition] = new RangeData()
                {
                    Position = targetPosition,
                    Cost = cost,
                    IsTarget = false,
                    IsAttack = Distance(basePosition, targetPosition) >= min
                };
            }

            // 味方ユニットが存在したら攻撃不可とする
            if (range[targetPosition].IsAttack && dictArrangementPosition.ContainsKey(targetPosition))
            {
                // 攻撃適正外の場合は攻撃不可とする
                var dat = GetArrangementData(targetPosition);
                range[targetPosition].IsAttack = weapon.CheckCanAttack(dat.MoveType);
                // 攻撃対象外の場合は更新しない
                if (range[targetPosition].IsAttack)
                {
                    var ownGrp = dictGroup.Values.ToArray()[nowGroup];
                    var grp = dictGroup[dat.GroupId];
                    range[targetPosition].IsTarget = !grp.Friendly.Contains(ownGrp.GroupId);
                }
            }

            range = GetRange(range, basePosition, new Vector2Int(targetPosition.x - 1, targetPosition.y), weapon, min, cost);
            range = GetRange(range, basePosition, new Vector2Int(targetPosition.x + 1, targetPosition.y), weapon, min, cost);
            range = GetRange(range, basePosition, new Vector2Int(targetPosition.x, targetPosition.y - 1), weapon, min, cost);
            range = GetRange(range, basePosition, new Vector2Int(targetPosition.x, targetPosition.y + 1), weapon, min, cost);

            return range;
        }

        private Vector3Int Get3dPosition(Vector2Int pos)
        {
            return new Vector3Int(pos.x, pos.y, GetHeight(pos));
        }

        public int Distance(Vector2Int basePosition, Vector2Int targetPosition)
        {
            return math.abs(basePosition.x - targetPosition.x) + math.abs(basePosition.y - targetPosition.y);
        }

        public void UnselectUnit()
        {
            posSelectUnit = nonePosition;
        }

        public TileData GetTile(Vector2Int pos)
        {
            return dictMapTiles.GetValueOrDefault(Get3dPosition(pos));
        }

        public Dictionary<Vector2Int, string> ArrangementPositions
        {
            get => dictArrangementPosition;
        }

        public ArrangementData GetArrangementData(Vector2Int pos)
        {
            if (!dictArrangementPosition.TryGetValue(pos, out var value))
            { return null; }
            return dictArrangement[value];
        }

        public PermanenceUnitData GetPermanenceUnitData(string id)
        {
            return dictUnits.GetValueOrDefault(id);
        }

        public PermanenceCharacterData GetPermanenceCharacterData(string id)
        {
            return dictCharacters.GetValueOrDefault(id);
        }

        public UnitData GetUnitData(string id)
        {
            return dictUnitData.GetValueOrDefault(id);
        }

        public CharacterData GetCharacterData(string id)
        {
            return dictCharacterData.GetValueOrDefault(id);
        }

        public bool IsNowGroup(int grp)
        {
            return grp == dictGroup.Keys.ToArray()[nowGroup];
        }

        public void BlastEffect(Vector2Int pos)
        {
            var pos3d = mngStage.GetCellCenterWorld(Get3dPosition(pos) + Vector3Int.forward);
            var obj = Instantiate(effectBlast, pos3d, Quaternion.identity);
            obj.GetComponent<BlastControl>().OnStop.AddListener(() =>
            {
                mode = ProcessMode.None;
            }); 
        }

        public void Save(int no)
        {
            var path = Path.Combine(DataUtil.PathBase, "save", $"dat_{no:0000}.json");
            var dat = new SaveData()
            {
                MajorVersion = MajorVersion,
                MinorVersion = MinorVersion,
                Mode = mode,
                MapId = NowMapId,
                CenterPosition = posCenter,
                NowGroup = nowGroup,
                RunningScript = mngScript.RunningScript,
                Variables = DataUtil.SerializeDictionary(mngScript.Variable),
                Arrangements = DataUtil.SerializeDictionary(dictArrangement),
                ArrangementPositions = DataUtil.SerializeDictionary(dictArrangementPosition),
                MapTiles = DataUtil.SerializeDictionary(dictMapTiles),
                Groups = DataUtil.SerializeDictionary(dictGroup),
                Heights = DataUtil.SerializeDictionary(dictHeight),
                CanCursors = DataUtil.SerializeDictionary(canCursor),
                VictoryConditionList = lstVictoryCondition,
                DefeatConditionList = lstDefeatCondition,
                EventConditionList = lstEventCondition,
                CanArrangementPositionList = lstCanArrangementPosition,
                HighlightTilePositionList = lstHighlightTilePosition,
                IsIntermission = mngSlgWindow.IsDisplayIntermission,
                NextStageClass = mngSlgWindow.NextStageClass,
                NextStageMethod = mngSlgWindow.NextStageMethod,
                IsBackImage = mngSlgWindow.IsDisplayBackImage,
                PathBackImage = mngSlgWindow.PathBackImage,
                ColorBackImage = mngSlgWindow.ColorBackImage,
                ArrangementUnitNum = arrangementUnitNum,
                ArrangementUnitMinNum = arrangementUnitMinNum,
                ArrangementUnitMaxNum = arrangementUnitMaxNum,
                ArrangementUnitDirection = arrangementUnitDirection,
                NextCallClass = nextCallClass,
                NextCallMethod = nextCallMethod,
                IsMusicStop = isMusicStop
            };
            dat.SetPermanenceCharacters(dictCharacters);
            dat.SetPermanenceUnits(dictUnits);
            DataUtil.SaveData(path, dat);
        }

        public async void Load(int no)
        {
            loadingData = string.Empty;
            var path = Path.Combine(DataUtil.PathBase, "save", $"dat_{no:0000}.json");
            var dat = await DataUtil.LoadData<SaveData>(path);
            if (dat == null)
            {
                mngSlgWindow.SetMessage(this, string.Empty, string.Empty, "System Message", new []{ "セーブデータが存在しません。" }, "down");
                return;
            }

            if (dat.MajorVersion < RequireMajorVersion ||
                dat.MajorVersion == RequireMajorVersion && dat.MinorVersion < RequireMinorVersion)
            {
                mngSlgWindow.SetError("互換性の無いセーブデータが指定されました。");
                return;
            }

            ClearMap();
            music.Stop();
            sound.Stop();
            mngSlgWindow.Initialize(this);
            mode = dat.Mode;
            if (dat.IsBackImage)
            { mngSlgWindow.SetBackImage(dat.PathBackImage, dat.ColorBackImage); }
            if (dat.IsIntermission)
            { mngSlgWindow.DisplayIntermission(this, dat.NextStageClass, dat.NextStageMethod); }
            SetMap(dat.MapId);
            SetViewCenter(dat.CenterPosition, 0);
            nowGroup = dat.NowGroup;
            isMusicStop = dat.IsMusicStop;
            mngScript.Variable = DataUtil.DeserializeDictionary(dat.Variables);
            dictCharacters = dat.GetPermanenceCharacters(this);
            dictUnits = dat.GetPermanenceUnits(this);
            dictArrangement = DataUtil.DeserializeDictionary(dat.Arrangements);
            dictArrangementPosition = DataUtil.DeserializeDictionary(dat.ArrangementPositions);
            dictGroup = DataUtil.DeserializeSortedDictionary(dat.Groups);
            dictHeight = DataUtil.DeserializeDictionary(dat.Heights);
            canCursor = DataUtil.DeserializeDictionary(dat.CanCursors);
            var tiles = DataUtil.DeserializeDictionary(dat.MapTiles);
            foreach (var key in tiles.Keys)
            {
                dictMapTiles[key] = dictTiles[tiles[key].TileSetId].Tiles[tiles[key].TileId];
            }
            lstVictoryCondition = dat.VictoryConditionList;
            lstDefeatCondition = dat.DefeatConditionList;
            lstEventCondition = dat.EventConditionList;
            lstCanArrangementPosition = dat.CanArrangementPositionList;
            lstHighlightTilePosition = dat.HighlightTilePositionList;
            DrawUnit();
            ViewHilightTiles();
            if (mode == ProcessMode.ArrangementUnits)
            {
                ArrangementUnits(dat.ArrangementUnitMinNum, dat.ArrangementUnitMaxNum, (int)dat.ArrangementUnitDirection, dat.NextCallClass, dat.NextCallMethod);
                arrangementUnitNum = dat.ArrangementUnitNum;
            }
            mngScript.RunningScript = dat.RunningScript;
        }

        public void SetArrangementArea(Vector2Int pos, Vector2Int area)
        {
            Debug.Log($"SetArrangementArea : ({pos.x}, {pos.y}), ({area.x}, {area.y})");
            for (var x = pos.x - area.x; x <= pos.x + area.x; x++)
            {
                for (var y = pos.y - area.y; y <= pos.y + area.y; y++)
                {
                    var target = new Vector2Int(x, y);
                    var tile = GetTile(target);
                    if (tile == null || tile.VoidTile)
                    { continue; }
                    lstCanArrangementPosition.Add(target);
                }
            }
        }

        public void ClearArrangementArea()
        {
            Debug.Log($"ClearArrangementArea");
            lstCanArrangementPosition.Clear();
            mngSelect.ClearAllTiles();
        }

        public void ArrangementUnits(int min, int max, int direction, string cls, string method)
        {
            mode = ProcessMode.ArrangementUnits;
            arrangementUnitNum = 0;
            arrangementUnitMinNum = min;
            arrangementUnitMaxNum = max;
            arrangementUnitDirection = (DirectionType)direction;
            if (string.IsNullOrWhiteSpace(cls))
            { nextCallClass = mngScript.NowClass; }
            else
            { nextCallClass = cls; }
            nextCallMethod = method;
            foreach (var pos in lstCanArrangementPosition)
            {
                var cpos = Get3dPosition(pos);
                var tile = datCommonSettings.ArrangementArea(dictMapTiles[cpos].CursorIndex);
                if (tile == null)
                { continue; }
                mngSelect.SetTile(cpos, tile);
            }
        }

        public void ViewHilightTiles()
        {
            Debug.Log($"ViewHilightTiles : {lstHighlightTilePosition.Count}");
            foreach (var pos in lstHighlightTilePosition)
            {
                if (mode == ProcessMode.ArrangementUnits && lstCanArrangementPosition.Contains(pos))
                { continue; }
                var cpos = Get3dPosition(pos);
                var tile = datCommonSettings.HighlightArea(dictMapTiles[cpos].CursorIndex);
                if (tile == null)
                { continue; }
                mngSelect.SetTile(cpos, tile);
            }
        }

        public void SetHighlightArea(Vector2Int pos, Vector2Int area)
        {
            Debug.Log($"SetHighlightArea : ({pos.x}, {pos.y}), ({area.x}, {area.y})");
            for (var x = pos.x - area.x; x <= pos.x + area.x; x++)
            {
                for (var y = pos.y - area.y; y <= pos.y + area.y; y++)
                {
                    var target = new Vector2Int(x, y);
                    var tile = GetTile(target);
                    if (tile == null || tile.VoidTile)
                    { continue; }
                    lstHighlightTilePosition.Add(target);
                }
            }
            ViewHilightTiles();
        }

        public void ClearHighlightArea()
        {
            Debug.Log("ClearHighlightArea");
            lstHighlightTilePosition.Clear();
            mngSelect.ClearAllTiles();
        }

        public void LoadSystemSettings()
        {
            music.volume = DataUtil.SystemSettingsData.BgmVolume;
            sound.volume = DataUtil.SystemSettingsData.SoundVolume;
        }

        public List<ConditionData> VictoryConditionList
        {
            get => lstVictoryCondition;
        }

        public List<ConditionData> DefeatConditionList
        {
            get => lstDefeatCondition;
        }

        public List<ConditionData> EventConditionList
        {
            get => lstEventCondition;
        }

        public Vector2Int CursorPosition
        {
            get => posCursor;
        }

        public Vector2Int SelectUnitPosition
        {
            get => posSelectUnit;
            set => posSelectUnit = value;
        }

        public Vector2Int TargetUnitPosition
        {
            get => posTargetUnit;
        }

        public ProcessMode ProcessMode
        {
            get => mode;
            set => mode = value;
        }

        public bool IsCancel
        {
            get => action.UI.Cancel.triggered || action.UI.RightClick.WasPressedThisFrame();
        }

        public ScriptManager ScriptManager
        {
            get => mngScript;
        }

        public SlgWindowManager SlgWindowManager
        {
            get => mngSlgWindow;
        }

        public BasicAction Action
        {
            get => action;
        }

        public BattleInfoData BattleInfo
        {
            get => datBattleInfo;
        }

        public BattleResultData BattleResult
        {
            get => datBattleResult;
        }

        public TileMapManager StageMap
        {
            get => mngStage;
        }

        public string LoadingData
        {
            set => loadingData = value;
        }

        private string NowMapId
        {
            get => map == null ? string.Empty : map.MapId;
        }

        public List<Vector2Int> CanArrangementPositionList
        {
            get => lstCanArrangementPosition;
        }

        public int ArrangmentUnitNum
        {
            get => arrangementUnitNum;
        }

        public int ArrangmentUnitMinNum
        {
            get => arrangementUnitMinNum;
        }

        public int ArrangmentUnitMaxNum
        {
            get => arrangementUnitMaxNum;
        }
    }
}
