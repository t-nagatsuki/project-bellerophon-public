using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Csv;
using Cysharp.IO;
using Cysharp.Threading.Tasks;
using Functions.Attributes;
using Functions.Csv;
using Functions.Data;
using Functions.Data.Maps;
using Functions.Data.Scripts;
using Functions.Data.Units;
using Functions.Enum;
using Functions.Json;
using Functions.Script;
using Functions.Skills;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using VYaml.Serialization;
using Object = UnityEngine.Object;
using TileData = Functions.Data.Maps.TileData;

namespace Functions.Util
{
    public static class DataUtil
    {
        private static readonly Dictionary<string, Texture2D> ImageLoader = new Dictionary<string, Texture2D>();
        public static SystemSettingsData SystemSettingsData;
        public static LocalData LocalData;
        public static string ErrorMessage;

        private static Dictionary<string, Type> _commandMap;
        private static Dictionary<string, Type> _skillMap;

        public static void InitCommandMap()
        {
            if (_commandMap != null) return;
            _commandMap = new Dictionary<string, Type>();
            var commandInterface = typeof(ICommand);
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes()
                .Where(t => commandInterface.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<CommandAttribute>();
                if (attr != null)
                {
                    _commandMap[attr.Name] = type;
                }
                else
                {
                    // 属性がない場合はクラス名から推測 (例: ChgMapCommand -> chg_map)
                    var name = type.Name;
                    if (name.EndsWith("Command"))
                    {
                        name = name.Substring(0, name.Length - "Command".Length);
                    }
                    var snakeName = ToSnakeCase(name);
                    if (!_commandMap.ContainsKey(snakeName))
                    {
                        _commandMap[snakeName] = type;
                    }
                }
            }
        }

        public static void InitSkillMap()
        {
            if (_skillMap != null) return;
            _skillMap = new Dictionary<string, Type>();
            var skillInterface = typeof(ISkill);
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes()
                .Where(t => skillInterface.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<SkillAttribute>();
                if (attr != null)
                {
                    _skillMap[attr.Name] = type;
                }
                else
                {
                    // 属性がない場合はクラス名から推測 (例: FlavorSkill -> flavor)
                    var name = type.Name;
                    if (name.EndsWith("Skill"))
                    {
                        name = name.Substring(0, name.Length - "Skill".Length);
                    }
                    var snakeName = ToSnakeCase(name);
                    if (!_skillMap.ContainsKey(snakeName))
                    {
                        _skillMap[snakeName] = type;
                    }
                }
            }
        }

        private static string ToSnakeCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var result = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (i > 0 && char.IsUpper(c))
                {
                    result.Append('_');
                }
                result.Append(char.ToLower(c));
            }
            return result.ToString();
        }

        public static string PathBase
        {
            get
            {
#if UNITY_EDITOR
                return Directory.GetCurrentDirectory();
#else
                return new DirectoryInfo(Application.dataPath).Parent.FullName;
#endif
            }
        }

        public static void SaveData<T>(string path, T dat)
        {
            var json = JsonUtility.ToJson(dat);
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException("保存先に指定されたパス文字列からディレクトリ情報を取得出来ませんでした。"));
            using var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            fs.SetLength(0);
            using var wr = new StreamWriter(fs, Encoding.UTF8);
            wr.WriteLine(json);
            wr.Close();
            fs.Close();
        }

        public static async UniTask<T> LoadData<T>(string path)
        {
            return await GetFromJson<T>(path);
        }

        public static List<SerializableKeyValuePair<TKey, TValue>> SerializeDictionary<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            var list = new List<SerializableKeyValuePair<TKey, TValue>>();
            foreach (var pair in dict)
            {
                var serializablePair = new SerializableKeyValuePair<TKey, TValue>(pair.Key, pair.Value);
                list.Add(serializablePair);
            }
            return list;
        }

        public static Dictionary<TKey, TValue> DeserializeDictionary<TKey, TValue>(
            List<SerializableKeyValuePair<TKey, TValue>> lst)
        {
            var deserializedDict = new Dictionary<TKey, TValue>();
            FillDictionary(deserializedDict, lst);
            return deserializedDict;
        }

        public static SortedDictionary<TKey, TValue> DeserializeSortedDictionary<TKey, TValue>(
            List<SerializableKeyValuePair<TKey, TValue>> lst)
        {
            var deserializedDict = new SortedDictionary<TKey, TValue>();
            FillDictionary(deserializedDict, lst);
            return deserializedDict;
        }

        private static void FillDictionary<TKey, TValue>(
            IDictionary<TKey, TValue> dictionary,
            List<SerializableKeyValuePair<TKey, TValue>> pairs)
        {
            foreach (var pair in pairs)
            {
                dictionary.Add(pair.Key, pair.Value);
            }
        }

        private static async UniTask<T> GetFromJson<T>(string path)
        {
            return !File.Exists(path) ? default(T) : JsonUtility.FromJson<T>(await ReadText(path));
        }

        public static async UniTask<string> ReadText(string path)
        {
            await using var fs = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            await using var sr = new Utf8StreamReader(fs).AsTextReader();
            var text = await sr.ReadToEndAsync();
            fs.Close();
            return text;
        }

        public static async UniTask<T> GetYamlString<T>(string path)
        {
            if (!File.Exists(path))
            {
                return default(T);
            }
            return YamlSerializer.Deserialize<T>(await ReadBytes(path));
        }

        private static async UniTask<byte[]> ReadBytes(string path)
        {
            using var sr = new Utf8StreamReader(path);
            return await sr.ReadToEndAsync();
        }

        public static string[] GetImages(string resource, string path)
        {
            var pathResourceBase = Path.Combine(PathBase, "resource", "image", resource);
            return Directory.GetFiles(pathResourceBase, path, SearchOption.AllDirectories);
        }

        public static Texture2D GetImage(string path)
        {
            if (ImageLoader.TryGetValue(path, out var image))
            {
                return image;
            }
            if (!File.Exists(path))
            {
                return null;
            }
            var texture = new Texture2D(1, 1);
            texture.LoadImage(File.ReadAllBytes(path));
            ImageLoader[path] = texture;
            texture.filterMode = FilterMode.Point;
            return texture;
        }

        public static void RemoveImage(string path)
        {
            if (!ImageLoader.TryGetValue(path, out var value))
            {
                return;
            }
            Object.Destroy(value);
            ImageLoader.Remove(path);
        }

        public static async UniTask LoadSystemSettings(string pathBase)
        {
            SystemSettingsData = await DataUtil.LoadData<SystemSettingsData>(Path.Combine(pathBase, "system.json"));
        }
        
        private static async UniTask<SystemSettingsJson> LoadSystemSetting(string pathBase)
        {
            var pathSettings = Path.Combine(pathBase, "system.json");
            if (File.Exists(pathSettings))
            {
                return await GetFromJson<SystemSettingsJson>(pathSettings);
            }
            pathSettings = Path.Combine(pathBase, "system.yml");
            if (File.Exists(pathSettings))
            {
                return await GetYamlString<SystemSettingsJson>(pathSettings);
            }
            return null;
        }

        public static async UniTask<CommonSettingsData> LoadCommonSettings(string pathBase)
        {
            var json = await LoadCommonSetting(pathBase);
            if (json == null)
            {
                return null;
            }
            var cursor = json.cursor;
            var result = new CommonSettingsData(
                cursor.id,
                json.map_cursor,
                json.highlight_area,
                json.arrangement_area,
                json.move_area,
                json.attack_area,
                json.group_marker,
                json.self_marker,
                json.target_marker
            );
            result.DebugMode = json.debug_mode;
            result.ToolMode = json.tool_mode;
            foreach (var chip in cursor.tile)
            {
                result.AddCursor(LoadLayers(pathBase, null, cursor, chip, "system"));
            }
            foreach (var dat in json.system_menu)
            {
                result.AddMenu(dat);
            }
            return result;
        }

        private static async UniTask<CommonSettingsJson> LoadCommonSetting(string pathBase)
        {
            var pathSettings = Path.Combine(pathBase, "data", "common", "settings.json");
            if (File.Exists(pathSettings))
            {
                return await GetFromJson<CommonSettingsJson>(pathSettings);
            }
            pathSettings = Path.Combine(pathBase, "data", "common", "settings.yml");
            if (File.Exists(pathSettings))
            {
                return await GetYamlString<CommonSettingsJson>(pathSettings);
            }
            return null;
        }

        public static string[] GetTiles(string path)
        {
            var pathResourceBase = Path.Combine(PathBase, "data", "tiles");
            return Directory.GetFiles(pathResourceBase, path, SearchOption.AllDirectories);
        }

        public static async UniTask<Dictionary<string, TilesData>> LoadTiles(string pathBase)
        {
            return await LoadTiles(pathBase, Path.Combine(pathBase, "data", "tiles"), "*");
        }

        public static async UniTask<Dictionary<string, TilesData>> LoadTiles(string pathBase, string path, string fileName)
        {
            Debug.Log($"LoadTiles : {path}, {fileName}");
            var dictTiles = new Dictionary<string, TilesData>();
            foreach (var f in Directory.GetFiles(path, $"{fileName}.json", SearchOption.AllDirectories))
            {
                dictTiles = await LoadTileJson(pathBase, f, dictTiles);
            }
            foreach (var f in Directory.GetFiles(path, $"{fileName}.yml", SearchOption.AllDirectories))
            {
                dictTiles = await LoadTileYaml(pathBase, f, dictTiles);
            }
            return dictTiles;
        }

        private static async UniTask<Dictionary<string, TilesData>> LoadTileJson(string pathBase, string path, Dictionary<string, TilesData> dict)
        {
            if (!File.Exists(path))
            {
                return dict;
            }
            var json = await GetFromJson<TilesJson>(path);
            return LoadTile(pathBase, dict, json);
        }

        private static async UniTask<Dictionary<string, TilesData>> LoadTileYaml(string pathBase, string path, Dictionary<string, TilesData> dict)
        {
            if (!File.Exists(path))
            {
                return dict;
            }
            var json = await GetYamlString<TilesJson>(path);
            return LoadTile(pathBase, dict, json);
        }

        private static Dictionary<string, TilesData> LoadTile(string pathBase, Dictionary<string, TilesData> dict, TilesJson json)
        {
            foreach (var dat in json.tiles)
            {
                if (!dict.ContainsKey(dat.id))
                {
                    dict[dat.id] = new TilesData(dat.id, dat.pixels);
                }
                foreach (var chip in dat.tile)
                {
                    dict[dat.id].Tiles[chip.name] = LoadLayers(pathBase, dict, dat, chip);
                }
            }
            return dict;
        }

        private static TileData LoadLayers(string pathBase, Dictionary<string, TilesData> dict, TileJson tile, TileChipJson chip, string resource = "map")
        {
            var dat = new TileData(tile.id, chip.name)
            {
                TileType = chip.tile_type,
                BaseLayer = LoadLayer(pathBase, dict, 0, chip.base_layer, $"base_{chip.name}", chip.collision, tile.pixels,
                    resource),
                OverlayBackLayer = LoadLayer(pathBase, dict, 1, chip.overlay_back, $"back_{chip.name}", chip.collision,
                    tile.pixels, resource),
                OverlayFrontLayer = LoadLayer(pathBase, dict, 2, chip.overlay_front, $"front_{chip.name}", chip.collision,
                    tile.pixels, resource),
                CursorIndex = chip.cursor_index,
                Collision = chip.collision,
                VoidTile = chip.void_tile,
                ToolHidden = chip.tool_hidden
            };
            if (chip.suitable != null)
            {
                foreach (var suitable in chip.suitable)
                {
                    var moveType = (suitable.move_type ?? string.Empty.ToLower()) switch
                    {
                        "space" => MoveType.Space,
                        "air" => MoveType.Air,
                        "ground" => MoveType.Ground,
                        "underwater" => MoveType.Underwater,
                        _ => MoveType.Space
                    };
                    dat.MoveCost[moveType] = new MoveCostData(moveType, suitable.cost);
                }
            }

            return dat;
        }

        private static LayerData LoadLayer(string pathBase, Dictionary<string, TilesData> dict, int layer, TileChipLayerJson json, string name, bool collision, int pixels, string resource = "map")
        {
            if (json == null)
            {
                return null;
            }
            var sprite = GetSprite(pathBase, resource, json.resource, pixels, json.pos);
            if (sprite.Length == 0)
            {
                return null;
            }
            var collider = Tile.ColliderType.None;
            if (collision)
            {
                collider = Tile.ColliderType.Grid;
            }

            if (json.override_rule != null &&
                dict != null &&
                !string.IsNullOrWhiteSpace(json.override_rule.tileset) &&
                !string.IsNullOrWhiteSpace(json.override_rule.tile) &&
                dict.ContainsKey(json.override_rule.tileset) &&
                dict[json.override_rule.tileset].Tiles.ContainsKey(json.override_rule.tile))
            {
                var baseTile = dict[json.override_rule.tileset].Tiles[json.override_rule.tile].BaseLayer.Tile;
                switch (layer)
                {
                    case 1:
                        baseTile = dict[json.override_rule.tileset].Tiles[json.override_rule.tile].OverlayBackLayer.Tile;
                        break;
                    case 2:
                        baseTile = dict[json.override_rule.tileset].Tiles[json.override_rule.tile].OverlayFrontLayer.Tile;
                        break;
                }

                return new LayerData(
                    resource,
                    json.color,
                    CreateTile(
                        name,
                        baseTile,
                        sprite,
                        json.override_rule.original_sprite,
                        json.override_rule.original_sprite_index,
                        GetSprite(pathBase, resource, json.override_rule.override_resource, pixels,
                            json.override_rule.override_pos)),
                    json.flip_x,
                    json.flip_y);
            }
            else if (json.rules != null && json.rules.Length > 0)
            {
                return new LayerData(
                    resource,
                    json.color,
                    CreateTile(pathBase, resource, pixels, name, sprite[0], collider, json.rules),
                    json.flip_x,
                    json.flip_y);
            }
            else
            {
                if (sprite.Length == 1)
                {
                    return new LayerData(
                        resource,
                        json.color,
                        CreateTile(name, sprite[0], json.color, collider),
                        json.flip_x,
                        json.flip_y);
                }
                return new LayerData(
                    resource,
                    json.color,
                    CreateTile($"{name}", sprite, json.anim_speed, collider),
                    json.flip_x,
                    json.flip_y);
            }
        }

        private static Sprite[] GetSprite(string pathBase, string resourceType, string resource, int pixels, Vector2Int[] positions)
        {
            var sprite = new List<Sprite>();
            if (String.IsNullOrWhiteSpace(resource))
            {
                return sprite.ToArray();
            }
            var pathResourceBase = Path.Combine(pathBase, "resource", "image", resourceType);
            var pathResource = String.Empty;
            foreach (var f in Directory.GetFiles(pathResourceBase, resource, SearchOption.AllDirectories))
            {
                pathResource = f;
                break;
            }
            if (String.IsNullOrWhiteSpace(pathResource))
            {
                return sprite.ToArray();
            }
            var texture = GetImage(pathResource);
            if (texture == null)
            {
                return sprite.ToArray();
            }
            foreach (var pos in positions)
            {
                var rect = new Rect(pos.x, pos.y, pixels * 2, pixels * 3);
                sprite.Add(Sprite.Create(texture, rect, new Vector2(0.5f, 0), pixels * 2));
            }
            return sprite.ToArray();
        }

        public static Tile CreateTile(string name, Sprite sprite, Color32 color, Tile.ColliderType collider)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.name = name;
            tile.sprite = sprite;
            tile.color = color;
            tile.colliderType = collider;
            return tile;
        }

        public static AnimatedTile CreateTile(string name, Sprite[] sprite, float speed, Tile.ColliderType collider)
        {
            var tile = ScriptableObject.CreateInstance<AnimatedTile>();
            tile.name = name;
            tile.m_AnimatedSprites = sprite;
            tile.m_MinSpeed = speed;
            tile.m_MaxSpeed = speed;
            tile.m_TileColliderType = collider;
            return tile;
        }

        public static RuleOverrideTile CreateTile(string name, TileBase rule, Sprite[] sprite, int[] original, int[] originalIdx, Sprite[] over)
        {
            var tile = ScriptableObject.CreateInstance<RuleOverrideTile>();
            tile.name = name;
            tile.m_Tile = (RuleTile)rule;
            tile.m_Sprites.Add(new RuleOverrideTile.TileSpritePair()
            {
                m_OriginalSprite = tile.m_Tile.m_DefaultSprite,
                m_OverrideSprite = sprite[0]
            });
            for (var i = 0; i < original.Length; i++)
            {
                tile.m_Sprites.Add(new RuleOverrideTile.TileSpritePair()
                {
                    m_OriginalSprite = tile.m_Tile.m_TilingRules[original[i]].m_Sprites[originalIdx[i]],
                    m_OverrideSprite = over[i]
                });
            }
            return tile;
        }

        public static IsometricRuleTile CreateTile(string pathBase, string resource, int pixels, string name, Sprite sprite, Tile.ColliderType collider, TileRuleJson[] rules)
        {
            var tile = ScriptableObject.CreateInstance<IsometricRuleTile>();
            tile.name = name;
            tile.m_DefaultSprite = sprite;
            foreach (var json in rules)
            {
                var sprites = GetSprite(pathBase, resource, json.resource, pixels, json.pos);
                var dict = new Dictionary<Vector3Int, int>();
                if (json.neighbors != null)
                {
                    foreach (var neighbor in json.neighbors)
                    {
                        dict[new Vector3Int(neighbor.pos.x, neighbor.pos.y, 0)] = neighbor.flg;
                    }
                }
                switch (json.output)
                {
                    case "animation":
                        tile.m_TilingRules.Add(CreateAnimationTilingRule(sprites, json.min_speed, json.max_speed, collider, dict));
                        break;
                    case "random":
                        tile.m_TilingRules.Add(CreateRandomTilingRule(sprites, json.perlin, collider, dict));
                        break;
                    default:
                        tile.m_TilingRules.Add(CreateSingleTilingRule(sprites, collider, dict));
                        break;
                }
            }
            return tile;
        }

        private static RuleTile.TilingRule CreateSingleTilingRule(Sprite[] sprite, Tile.ColliderType collider, Dictionary<Vector3Int, int> dict)
        {
            var rule = new RuleTile.TilingRule()
            {
                m_Output = RuleTile.TilingRuleOutput.OutputSprite.Single,
                m_Sprites = sprite,
                m_ColliderType = collider
            };
            rule.ApplyNeighbors(dict);
            return rule;
        }

        private static RuleTile.TilingRule CreateRandomTilingRule(Sprite[] sprite, float perlin, Tile.ColliderType collider, Dictionary<Vector3Int, int> dict)
        {
            var rule = new RuleTile.TilingRule()
            {
                m_Output = RuleTile.TilingRuleOutput.OutputSprite.Random,
                m_Sprites = sprite,
                m_PerlinScale = perlin,
                m_RuleTransform = RuleTile.TilingRuleOutput.Transform.Fixed,
                m_ColliderType = collider
            };
            rule.ApplyNeighbors(dict);
            return rule;
        }

        private static RuleTile.TilingRule CreateAnimationTilingRule(Sprite[] sprite, float minSpeed, float maxSpeed, Tile.ColliderType collider, Dictionary<Vector3Int, int> dict)
        {
            var rule = new RuleTile.TilingRule()
            {
                m_Output = RuleTile.TilingRuleOutput.OutputSprite.Animation,
                m_Sprites = sprite,
                m_MaxAnimationSpeed = minSpeed,
                m_MinAnimationSpeed = maxSpeed,
                m_ColliderType = collider
            };
            rule.ApplyNeighbors(dict);
            return rule;
        }

        public static string[] GetMaps(string path)
        {
            var pathResourceBase = Path.Combine(PathBase, "data", "map");
            return Directory.GetFiles(pathResourceBase, path, SearchOption.AllDirectories);
        }

        public static async UniTask<Dictionary<string, MapData>> LoadMaps(string pathBase)
        {
            var dictMaps = new Dictionary<string, MapData>();
            foreach (var f in GetMaps("*.json"))
            {
                Debug.Log($"Loading : {f}");
                dictMaps = await LoadMapJson(f, dictMaps);
            }
            foreach (var f in GetMaps("*.yml"))
            {
                Debug.Log($"Loading : {f}");
                dictMaps = await LoadMapYaml(f, dictMaps);
            }
            return dictMaps;
        }

        public static async UniTask<Dictionary<string, MapData>> LoadMapJson(string path, Dictionary<string, MapData> dict)
        {
            if (!File.Exists(path))
            {
                return dict;
            }
            var json = await GetFromJson<MapsJson>(path);
            return LoadMap(dict, json);
        }

        public static async UniTask<Dictionary<string, MapData>> LoadMapYaml(string path, Dictionary<string, MapData> dict)
        {
            if (!File.Exists(path))
            {
                return dict;
            }
            var json = await GetYamlString<MapsJson>(path);
            return LoadMap(dict, json);
        }

        private static Dictionary<string, MapData> LoadMap(Dictionary<string, MapData> dict, MapsJson json)
        {
            foreach (var dat in json.maps)
            {
                dict[dat.id] = new MapData(dat);
            }
            return dict;
        }

        public static string[] GetCharacters(string path)
        {
            var pathResourceBase = Path.Combine(PathBase, "data", "character");
            return Directory.GetFiles(pathResourceBase, path, SearchOption.AllDirectories);
        }

        public static async UniTask<Dictionary<string, CharacterData>> LoadCharacters(string pathBase)
        {
            var dictCharacters = new Dictionary<string, CharacterData>();
            foreach (var f in GetCharacters( "*.json"))
            {
                Debug.Log($"Loading : {f}");
                dictCharacters = await LoadCharacterJson(pathBase, f, dictCharacters);
            }
            foreach (var f in GetCharacters( "*.yml"))
            {
                Debug.Log($"Loading : {f}");
                dictCharacters = await LoadCharacterYaml(pathBase, f, dictCharacters);
            }
            return dictCharacters;
        }

        public static async UniTask<Dictionary<string, CharacterData>> LoadCharacterJson(string pathBase, string path, Dictionary<string, CharacterData> dict)
        {
            Debug.Log($"LoadCharacterJson : {path}");
            if (!File.Exists(path))
            {
                Debug.Log($"NotExists");
                return dict;
            }
            var json = await GetFromJson<CharactersJson>(path);
            return LoadCharacter(pathBase, dict, json);
        }

        public static async UniTask<Dictionary<string, CharacterData>> LoadCharacterYaml(string pathBase, string path, Dictionary<string, CharacterData> dict)
        {
            Debug.Log($"LoadCharacterYaml : {path}");
            if (!File.Exists(path))
            {
                Debug.Log($"NotExists");
                return dict;
            }
            var json = await GetFromJson<CharactersJson>(path);
            return LoadCharacter(pathBase, dict, json);
        }

        private static Dictionary<string, CharacterData> LoadCharacter(string pathBase, Dictionary<string, CharacterData> dict, CharactersJson json)
        {
            foreach (var dat in json.character)
            {
                var pathResource = Path.Combine(pathBase, "resource", "image", "character", dat.image);
                var texture = GetImage(pathResource);
                // TODO : キャラクターエディタでまだテクスチャが選択出来ないのでコメントアウト
                // if (texture == null)
                // {
                //     return null;
                // }
                var chara = new CharacterData(dat);
                chara.CharacterImage = texture;
                dict[dat.id] = chara;
            }
            return dict;
        }

        public static async UniTask<Dictionary<string, MessageData>> LoadMessages(string pathBase)
        {
            var dictMessages = new Dictionary<string, MessageData>();
            var pathMap = Path.Combine(pathBase, "data", "message");
            foreach (var f in Directory.GetFiles(pathMap, "*.json", SearchOption.AllDirectories))
            {
                Debug.Log($"Loading : {f}");
                dictMessages = await LoadMessageJson(pathBase, f, dictMessages);
            }
            foreach (var f in Directory.GetFiles(pathMap, "*.yml", SearchOption.AllDirectories))
            {
                Debug.Log($"Loading : {f}");
                dictMessages = await LoadMessageYaml(pathBase, f, dictMessages);
            }
            return dictMessages;
        }

        public static async UniTask<Dictionary<string, MessageData>> LoadMessageJson(string pathBase, string path, Dictionary<string, MessageData> dict)
        {
            if (!File.Exists(path))
            {
                return dict;
            }
            var json = await GetFromJson<MessagesJson>(path);
            return LoadMessage(pathBase, dict, json);
        }

        public static async UniTask<Dictionary<string, MessageData>> LoadMessageYaml(string pathBase, string path, Dictionary<string, MessageData> dict)
        {
            if (!File.Exists(path))
            {
                return dict;
            }
            var json = await GetFromJson<MessagesJson>(path);
            return LoadMessage(pathBase, dict, json);
        }

        private static Dictionary<string, MessageData> LoadMessage(string pathBase, Dictionary<string, MessageData> dict, MessagesJson json)
        {
            foreach (var dat in json.messages)
            {
                var chara = new MessageData(dat);
                dict[dat.id] = chara;
            }
            return dict;
        }

        public static string[] GetUnits(string path)
        {
            var pathResourceBase = Path.Combine(PathBase, "data", "unit");
            return Directory.GetFiles(pathResourceBase, path, SearchOption.AllDirectories);
        }

        public static async UniTask<Dictionary<string, UnitData>> LoadUnits(string pathBase)
        {
            var dictUnits = new Dictionary<string, UnitData>();
            foreach (var f in GetUnits("*.json"))
            {
                Debug.Log($"Loading : {f}");
                dictUnits = await LoadUnitJson(pathBase, f, dictUnits);
            }
            foreach (var f in GetUnits("*.yml"))
            {
                Debug.Log($"Loading : {f}");
                dictUnits = await LoadUnitYaml(pathBase, f, dictUnits);
            }
            return dictUnits;
        }

        public static async UniTask<Dictionary<string, UnitData>> LoadUnitJson(string pathBase, string path, Dictionary<string, UnitData> dict)
        {
            if (!File.Exists(path))
            {
                return dict;
            }
            var json = await GetFromJson<UnitsJson>(path);
            return LoadUnit(pathBase, dict, json);
        }

        public static async UniTask<Dictionary<string, UnitData>> LoadUnitYaml(string pathBase, string path, Dictionary<string, UnitData> dict)
        {
            if (!File.Exists(path))
            {
                return dict;
            }
            var json = await GetYamlString<UnitsJson>(path);
            return LoadUnit(pathBase, dict, json);
        }

        private static Dictionary<string, UnitData> LoadUnit(string pathBase, Dictionary<string, UnitData> dict, UnitsJson json)
        {
            foreach (var dat in json.unit)
            {
                var texDisplay = GetUnitTexture(pathBase, dat.display);
                var texBaseResource = GetUnitTexture(pathBase, dat.base_resource);
                var texFrontResource = GetUnitTexture(pathBase, dat.front_resource);
                if (texDisplay == null && texBaseResource == null)
                {
                    return null;
                }
                if (texFrontResource == null)
                {
                    texFrontResource = texBaseResource;
                }
                var unit = new UnitData(dat);
                if (texDisplay)
                {
                    unit.UnitImage = texDisplay;
                }
                else
                {
                    unit.UnitImage = new Texture2D(dat.pixels * 2, dat.pixels * 2);
                    unit.UnitImage.SetPixels(texBaseResource.GetPixels(0, dat.pixels, dat.pixels * 2, dat.pixels * 2));
                    unit.UnitImage.Apply();
                }
                unit.UnitBaseTile = GetUnitTiles(dat, dat.base_image, texBaseResource);
                unit.UnitFrontTile = GetUnitTiles(dat, dat.front_image, texFrontResource);
                dict[dat.id] = unit;
            }
            return dict;
        }

        private static Texture2D GetUnitTexture(string pathBase, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            Debug.Log($"{path}");
            var pathResource = Path.Combine(pathBase, "resource", "image", "unit", path);
            return GetImage(pathResource);
        }

        private static TileBase[] GetUnitTiles(UnitJson dat, TileChipLayerJson[] chips, Texture2D texResource)
        {
            var tiles = new List<TileBase>();
            if (chips != null)
            {
                foreach (var chip in chips)
                {
                    var sprite = new List<Sprite>();
                    foreach (var pos in chip.pos)
                    {
                        var rect = new Rect(pos.x, pos.y, dat.pixels * 2, dat.pixels * 3);
                        sprite.Add(Sprite.Create(texResource, rect, new Vector2(0.5f, 0), dat.pixels * 2));
                    }

                    var collider = Tile.ColliderType.Grid;
                    if (sprite.Count == 1)
                    {
                        var tile = ScriptableObject.CreateInstance<Tile>();
                        tile.name = dat.name;
                        tile.sprite = sprite[0];
                        tile.color = chip.color;
                        tile.colliderType = collider;
                        tiles.Add(tile);
                    }
                    else
                    {
                        var tile = ScriptableObject.CreateInstance<AnimatedTile>();
                        tile.name = dat.name;
                        tile.m_AnimatedSprites = sprite.ToArray();
                        tile.m_MinSpeed = chip.anim_speed;
                        tile.m_MaxSpeed = chip.anim_speed;
                        tile.m_TileColliderType = collider;
                        tiles.Add(tile);
                    }
                }
            }
            return tiles.ToArray();
        }

        public static async UniTask<Dictionary<string, ScriptsData>> LoadScripts(string pathBase)
        {
            var dictScripts = new Dictionary<string, ScriptsData>();
            var pathScript = Path.Combine(pathBase, "script");
            foreach (var f in Directory.GetFiles(pathScript, "*.json", SearchOption.AllDirectories))
            {
                Debug.Log($"Loading : {f}");
                dictScripts = await LoadScriptJson(f, dictScripts);
            }
            foreach (var f in Directory.GetFiles(pathScript, "*.yml", SearchOption.AllDirectories))
            {
                Debug.Log($"Loading : {f}");
                dictScripts = await LoadScriptYaml(f, dictScripts);
            }
            return dictScripts;
        }

        private static async UniTask<Dictionary<string, ScriptsData>> LoadScriptJson(string path, Dictionary<string, ScriptsData> dict)
        {
            if (!File.Exists(path))
            {
                return dict;
            }
            var json = await GetFromJson<ScriptsJson>(path);
            return LoadScript(dict, json);
        }

        private static async UniTask<Dictionary<string, ScriptsData>> LoadScriptYaml(string path, Dictionary<string, ScriptsData> dict)
        {
            if (!File.Exists(path))
            {
                return dict;
            }
            var json = await GetYamlString<ScriptsJson>(path);
            return LoadScript(dict, json);
        }

        private static Dictionary<string, ScriptsData> LoadScript(Dictionary<string, ScriptsData> dict, ScriptsJson json)
        {
            var scripts = new ScriptsData(json.id);

            foreach (var methodJson in json.methods)
            {
                var methodData = new MethodData(methodJson.name);
                
                for (var i = 0; i < methodJson.process.Length; i++)
                {
                    var process = methodJson.process[i];
                    try
                    {
                        methodData.Commands.Add(CreateCommand(process));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{ex.Message}{Environment.NewLine}" +
                                            $"Class : {json.id} Method : {methodJson.name}{Environment.NewLine}" +
                                            $"Command : {process.cmd} Rows : {i + 1}");
                    }
                }
                scripts.Methods[methodJson.name] = methodData;
            }

            dict[json.id] = scripts;
            return dict;
        }

        private static ICommand CreateCommand(ProcessJson prc)
        {
            if (_commandMap.TryGetValue(prc.cmd, out var type))
            {
                return (ICommand)Activator.CreateInstance(type, prc);
            }

            throw new Exception("undefined command: " + prc.cmd);
        }

        public static ISkill GetSkill(string name)
        {
            if (_skillMap.TryGetValue(name, out var type))
            {
                return (ISkill)Activator.CreateInstance(type);
            }

            return new FlavorSkill();
        }

        public static string[] GetAudios(string path)
        {
            var pathResourceBase = Path.Combine(PathBase, "resource", "music");
            return Directory.GetFiles(pathResourceBase, path, SearchOption.AllDirectories);
        }

        public static UniTask<AudioClip> GetAudio(string path)
        {
            var pathResourceBase = Path.Combine(PathBase, "resource", "music");
            return GetAudioClip(pathResourceBase, path);
        }

        public static UniTask<AudioClip> GetSound(string path)
        {
            var pathResourceBase = Path.Combine(PathBase, "resource", "sound");
            return GetAudioClip(pathResourceBase, path);
        }

        private static async UniTask<AudioClip> GetAudioClip(string basePath, string path)
        {
            var pathResource = String.Empty;
            foreach (var f in Directory.GetFiles(basePath, path, SearchOption.AllDirectories))
            {
                pathResource = f;
                break;
            }
            if (!File.Exists(pathResource))
            {
                return null;
            }
            Debug.Log(pathResource);
            var request = UnityWebRequestMultimedia.GetAudioClip(pathResource, GetAudioType(pathResource));
            ((DownloadHandlerAudioClip)request.downloadHandler).streamAudio = true;
            await request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                return null;
            }
            return DownloadHandlerAudioClip.GetContent(request);
        }

        private static AudioType GetAudioType(string path)
        {
            switch (Path.GetExtension(path).ToLower())
            {
                case ".ogg":
                    return AudioType.OGGVORBIS;
                case ".mp3":
                    return AudioType.MPEG;
                default:
                    return AudioType.UNKNOWN;
            }
        }

        public static async UniTask LoadLocales(string pathBase)
        {
            LocalData = new LocalData();
            var pathLocal = Path.Combine(pathBase, "data", "local");
            if (!Directory.Exists(pathLocal)) return;
            foreach (var f in Directory.GetFiles(pathLocal, $"*.csv", SearchOption.AllDirectories))
            {
                LocalData.Load(await LoadLocale(f));
            }
        }
        
        public static async UniTask<LocalCsv[]> LoadLocale(string path)
        {
            if (!File.Exists(path)) return Array.Empty<LocalCsv>();
            return CsvSerializer.Deserialize<LocalCsv>(await ReadText(path), new CsvOptions()
            {
                HasHeader = true,
                AllowComments = true,
                NewLine = NewLineType.CRLF,
                Separator = SeparatorType.Comma,
                QuoteMode = QuoteMode.Minimal
            });
        }
        
        public static async UniTask<CopyrightBodyJson[]> LoadCopyright(string pathBase)
        {
            var pathCopyright = Path.Combine(pathBase, "copyright.json");
            if (File.Exists(pathCopyright))
            {
                return (await GetFromJson<CopyrightJson>(pathCopyright)).copyright;
            }
            pathCopyright = Path.Combine(pathBase, "copyright.yml");
            if (File.Exists(pathCopyright))
            {
                return (await GetYamlString<CopyrightJson>(pathCopyright)).copyright;
            }
            return Array.Empty<CopyrightBodyJson>();
        }
    }
}
