using System.Collections.Generic;
using System.IO;
using System.Linq;
using Functions.Data.Units;
using Functions.Json;
using Functions.Util;
using UnityEngine;

namespace Functions.Manager
{
    public class CharacterEditorManager : MonoBehaviour
    {
        [SerializeField] private CharacterEditorWindowManager mngWindow;
        [SerializeField] private AudioSource music;

        private string nameFile = "default_characters";
        private Dictionary<string, CharacterData> dictCharacters = new();
        private Dictionary<string, MessageData> dictMessages = new();
        /// <summary>キャラクター情報</summary>
        private CharacterData character;
        private MessageData message;
        
        private BasicAction action;
        /// <summary>カーソル移動入力待機時間</summary>
        private float waitNavigate = 999;
        /// <summary>カーソル移動入力制限時間</summary>
        private const float WaitNavigateTime = 0.2f;
        private bool isPlayMusic;

        private void Awake()
        {
            action = new BasicAction();
            action.Enable();
            mngWindow.Initialize(this);
            character = null;
            InitializeCharacter(new CharacterData("default_character"), new MessageData("default_character"));
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

        public async void LoadCharacters(string path)
        {
            // キャラクター定義読込
            nameFile = path;
            dictCharacters.Clear();
            dictMessages.Clear();
            mngWindow.EditorToolBar.CharacterDropdown.choices.Clear();
            var pathBase = Path.Combine(DataUtil.PathBase, "data", "character");
            if (File.Exists(Path.Combine(pathBase, $"{path}.json"))) dictCharacters = await DataUtil.LoadCharacterJson(DataUtil.PathBase, Path.Combine(pathBase, $"{path}.json"), dictCharacters);
            else dictCharacters = await DataUtil.LoadCharacterYaml(DataUtil.PathBase, Path.Combine(pathBase, $"{path}.yml"), dictCharacters);
            pathBase = Path.Combine(DataUtil.PathBase, "data", "message");
            if (File.Exists(Path.Combine(pathBase, $"{path}.json"))) dictMessages = await DataUtil.LoadMessageJson(DataUtil.PathBase, Path.Combine(pathBase, $"{path}.json"), dictMessages);
            else dictMessages = await DataUtil.LoadMessageYaml(DataUtil.PathBase, Path.Combine(pathBase, $"{path}.yml"), dictMessages);
            character = null;
            message = null;
            foreach (var key in dictCharacters.Keys)
            { mngWindow.EditorToolBar.CharacterDropdown.choices.Add(key); }
            if (dictCharacters.Count > 0)
            { mngWindow.EditorToolBar.CharacterDropdown.value = dictCharacters.Keys.First(); }
        }

        public void InitializeCharacter(CharacterData dat, MessageData msg)
        {
            dictCharacters.Clear();
            dictMessages.Clear();
            mngWindow.EditorToolBar.CharacterDropdown.choices.Clear();
            EditCharacter(dat, msg);
        }

        public void EditCharacter(CharacterData dat, MessageData msg)
        {
            dictCharacters[dat.CharacterId] = dat;
            dictMessages[msg.CharacterId] = msg;
            if (!mngWindow.EditorToolBar.CharacterDropdown.choices.Contains(dat.CharacterId))
            { mngWindow.EditorToolBar.CharacterDropdown.choices.Add(dat.CharacterId); }
            mngWindow.EditorToolBar.CharacterDropdown.value = dat.CharacterId;
        }

        public void RemoveCharacter(string characterId)
        {
            dictCharacters.Remove(characterId);
            dictMessages.Remove(characterId);
            mngWindow.EditorToolBar.CharacterDropdown.choices.Remove(characterId);
        }

        public void SetCharacter(string newCharacter)
        {
            if (!dictCharacters.TryGetValue(newCharacter, out var datCharacter))
            {
                return;
            }
            dictMessages.TryGetValue(newCharacter, out var datMessage);
            mngWindow.EditorToolBar.UpdateCharacter(character, message);
            character = datCharacter;
            message = datMessage;
            mngWindow.EditorToolBar.SetCharacter(datCharacter);
        }

        public void SaveCharacters()
        {
            mngWindow.EditorToolBar.UpdateCharacter(character, message);
            var jcharacters = new List<CharacterJson>();
            foreach (var key in dictCharacters.Keys)
            {
                var data = dictCharacters[key];
                var growth = data.GetGrowthSuitability();
                List<GrowthSuitabilityJson> suitability = new();
                foreach (var lv in growth.Keys)
                {
                    var g = new GrowthSuitabilityJson()
                    {
                        lv = lv
                    };
                    if (growth[lv].EnabledSpace) g.space = growth[lv].Space.ToString();
                    if (growth[lv].EnabledAir) g.air = growth[lv].Air.ToString();
                    if (growth[lv].EnabledGround) g.ground = growth[lv].Ground.ToString();
                    if (growth[lv].EnabledUnderwater) g.underwater = growth[lv].Underwater.ToString();
                    suitability.Add(g);
                }
                jcharacters.Add(new CharacterJson()
                {
                    id = key,
                    image = data.CharacterImagePath,
                    name = data.CharacterName,
                    music = data.Music,
                    suitability = new SuitabilityJson()
                    {
                        space = data.Space.suitable.ToString(),
                        air = data.Air.suitable.ToString(),
                        ground = data.Ground.suitable.ToString(),
                        underwater = data.Underwater.suitable.ToString()
                    },
                    status = new CharacterStatusJson()
                    {
                        concentration = data.Concentration.status,
                        ability = data.Ability.status,
                        reaction = data.Reaction.status,
                        perception = data.Perception.status,
                        intention = data.Intention.status,
                        endurance = data.Endurance.status,
                        expertise = data.Expertise.status,
                        sp = data.SP.status
                    },
                    growth = new GrowthJson()
                    {
                        concentration = data.GrowthConcentration,
                        ability = data.GrowthAbility,
                        reaction = data.GrowthReaction,
                        perception = data.GrowthPerception,
                        intention = data.GrowthIntention,
                        endurance = data.GrowthEndurance,
                        expertise = data.GrowthExpertise,
                        sp = data.GrowthSP,
                        suitability = suitability.ToArray()
                    }
                });
            }
            var characters = new CharactersJson()
            {
                character = jcharacters.ToArray()
            };
            var jmessages = new List<MessageJson>();
            foreach (var key in dictMessages.Keys)
            {
                var data = dictMessages[key];
                jmessages.Add(new MessageJson()
                {
                    id = key,
                });
            }
            var messages = new MessagesJson()
            {
                messages = jmessages.ToArray()
            };
            DataUtil.SaveData<CharactersJson>(Path.Combine(DataUtil.PathBase, "data", "character", $"{nameFile}.json"), characters);
            DataUtil.SaveData<MessagesJson>(Path.Combine(DataUtil.PathBase, "data", "message", $"{nameFile}.json"), messages);
        }

        public void DeleteCharacter()
        {
            if (dictCharacters.Count < 2)
            { return; }
            dictCharacters.Remove(character.CharacterId);
            dictMessages.Remove(character.CharacterId);
            mngWindow.EditorToolBar.CharacterDropdown.choices.Remove(character.CharacterId);
            mngWindow.EditorToolBar.CharacterDropdown.value = dictCharacters.Keys.ToArray()[0];
        }

        public void SetPlayMusic(string nameMusic)
        {
            isPlayMusic = !isPlayMusic;
            if (isPlayMusic)
            {
                SetMusic(nameMusic, true);
            }
            else
            {
                StopMusic();
            }
        }

        private async void SetMusic(string nameMusic, bool loop)
        {
            Debug.Log($"start_music {nameMusic}");
            if (string.IsNullOrWhiteSpace(nameMusic))
            {
                music.Stop();
                return;
            }
            music.clip = await DataUtil.GetAudio(nameMusic);
            music.loop = loop;
            music.Play();
        }

        private void StopMusic()
        {
            Debug.Log("stop_music");
            music.Stop();
        }

        public BasicAction Action => action;
        public CharacterEditorWindowManager EditorWindowManager => mngWindow;
        public Dictionary<string, CharacterData> Characters => dictCharacters;
        public Dictionary<string, MessageData> Messages => dictMessages;
        public CharacterData Character => character;
        public MessageData Message => message;

        
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