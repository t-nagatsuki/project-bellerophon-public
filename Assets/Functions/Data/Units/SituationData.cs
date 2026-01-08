using System;

namespace Functions.Data.Units
{
    [Serializable]
    public class SituationData
    {
        public string[] Message;
        public int HitUnder;
        public int HitOver;
        public int HpUnder;
        public int HpOver;
        
        public SituationData(Json.SituationJson _json)
        {
            Message = _json.message;
            HitUnder = _json.hit_under;
            HitOver = _json.hit_over;
            HpUnder = _json.hp_under;
            HpOver = _json.hp_over;
        }
    }
}
