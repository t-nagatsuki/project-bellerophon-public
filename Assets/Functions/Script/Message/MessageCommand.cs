using System;
using Functions.Attributes;
using Functions.Manager;
namespace Functions.Script.Message
{
    [Serializable]
    [Command("message")]
    public class MessageCommand : ICommand
    {
        private string characterId;
        private string chara;
        private string name;
        private string[] text;
        private string position;

        public MessageCommand(Json.ProcessJson prc)
        {
            characterId = prc.chara_id;
            chara = prc.chara;
            name = prc.name;
            text = prc.text;
            if (text == null) text = Array.Empty<string>();
            if (!String.IsNullOrWhiteSpace(prc.position))
            { position = prc.position; }
            else
            { position = "down"; }
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SlgWindowManager.SetMessage(
                mng, 
                mng.ScriptManager.AnalysisEmbeddedvariable(characterId), 
                mng.ScriptManager.AnalysisEmbeddedvariable(chara), 
                mng.ScriptManager.AnalysisEmbeddedvariable(name), 
                mng.ScriptManager.AnalysisEmbeddedvariable(text), 
                position
            );
            return true;
        }
    }
}

