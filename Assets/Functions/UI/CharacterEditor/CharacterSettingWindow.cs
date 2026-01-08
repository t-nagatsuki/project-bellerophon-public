using Functions.Data.Units;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
using UnityEngine.UIElements;

namespace Functions.UI.CharacterEditor
{
    public class CharacterSettingWindow : MonoBehaviour
    {
        private UIDocument document;
        private Button btnAccept;
        private Button btnCancel;
        private TextField txtFileName;
        private TextField txtCharacterId;

        private bool isNewCharacters;
        private bool isAddCharacter;

        private CharacterData character;
        private MessageData message;
        
        private void Awake()
        {
            document = GetComponent<UIDocument>();
            document.rootVisualElement.style.display = DisplayStyle.None;
            btnAccept = document.rootVisualElement.Q<Button>("BtnAccept");
            btnCancel = document.rootVisualElement.Q<Button>("BtnCancel");
            txtFileName = document.rootVisualElement.Q<TextField>("TxtFileName");
            txtCharacterId = document.rootVisualElement.Q<TextField>("TxtCharacterId");
        }

        public void SetManager(CharacterEditorManager mng)
        {
            btnAccept.clicked += () =>
            {
                if (string.IsNullOrWhiteSpace(txtFileName.value))
                {
                    mng.EditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_S0001", LocaleUtil.GetEntry("lbl_name_file")));
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtCharacterId.value))
                {
                    mng.EditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_S0001", LocaleUtil.GetEntry("lbl_id_character")));
                    return;
                }
                mng.FileName = txtFileName.value;
                if (isNewCharacters)
                {
                    mng.InitializeCharacter(new CharacterData(txtCharacterId.value), new MessageData(txtCharacterId.value));
                }
                else
                {
                    if (isAddCharacter)
                    {
                        if (mng.Characters.ContainsKey(character.CharacterId))
                        {
                            mng.EditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_C0002", LocaleUtil.GetEntry("lbl_id_character")));
                            return;
                        }
                        character = new CharacterData(txtCharacterId.value);
                        message = new MessageData(txtCharacterId.value);
                    }
                    else
                    {
                        if (mng.Character.CharacterId != character.CharacterId && mng.Characters.ContainsKey(character.CharacterId))
                        {
                            mng.EditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_C0002", LocaleUtil.GetEntry("lbl_id_character")));
                            return;
                        }
                        mng.RemoveCharacter(mng.Character.CharacterId);
                    }
                    mng.EditCharacter(character, message);
                }
                mng.WaitNavigate = 0;
                document.rootVisualElement.style.display = DisplayStyle.None;
            };
            btnCancel.clicked += () =>
            {
                mng.WaitNavigate = 0;
                document.rootVisualElement.style.display = DisplayStyle.None;
            };
        }

        public void InitializeDisplay(CharacterEditorManager mng)
        {
            isNewCharacters = true;
            isAddCharacter = false;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            txtFileName.value = "default_characters";
            txtCharacterId.value = "default_character";
        }

        public void AddDisplay(CharacterEditorManager mng)
        {
            isNewCharacters = false;
            isAddCharacter = true;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            txtFileName.value = mng.FileName;
            txtCharacterId.value = "default_character";
        }

        public void EditDisplay(CharacterEditorManager mng)
        {
            isNewCharacters = false;
            isAddCharacter = false;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            txtFileName.value = mng.FileName;
            character = mng.Character;
            message = mng.Message;
            txtCharacterId.value = character.CharacterId;
        }

        public bool IsDisplay()
        {
            return document.rootVisualElement.style.display == DisplayStyle.Flex;
        }
    }
}
