using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Functions.Data.Units
{
    [Serializable]
    public class MessageData
    {
        public string CharacterId { get; set; }
        public List<SituationData> Attack { get; set; } = new();
        public List<SituationData> Guard { get; set; } = new();
        public List<SituationData> Avoid { get; set; } = new();
        public List<SituationData> Damage { get; set; } = new();
        public List<SituationData> Destroy { get; set; } = new();

        public MessageData(string id)
        {
            CharacterId = id;
        }

        public MessageData(Json.MessageJson _json)
        {
            CharacterId = _json.id;

            if (_json.attack != null)
            {
                foreach (var situation in _json.attack)
                {
                    Attack.Add(new SituationData(situation));
                }
            }
            if (_json.guard != null)
            {
                foreach (var situation in _json.guard)
                {
                    Guard.Add(new SituationData(situation));
                }
            }
            if (_json.avoid != null)
            {
                foreach (var situation in _json.avoid)
                {
                    Avoid.Add(new SituationData(situation));
                }
            }
            if (_json.damage != null)
            {
                foreach (var situation in _json.damage)
                {
                    Damage.Add(new SituationData(situation));
                }
            }
            if (_json.destroy != null)
            {
                foreach (var situation in _json.destroy)
                {
                    Destroy.Add(new SituationData(situation));
                }
            }
        }

        public bool HaveAttack(float _hit, float _hp)
        {
            return Get(Attack, _hit, _hp).Any();
        }

        public string[] GetAttack(float _hit, float _hp)
        {
            var messages = Get(Attack, _hit, _hp).ToArray();
            return messages[Random.Range(0, messages.Length)].Message;
        }

        public bool HaveGuard(float _hit, float _hp)
        {
            return Get(Guard, _hit, _hp).Any();
        }

        public string[] GetGuard(float _hit, float _hp)
        {
            var messages = Get(Guard, _hit, _hp).ToArray();
            return messages[Random.Range(0, messages.Length)].Message;
        }

        public bool HaveAvoid(float _hit, float _hp)
        {
            return Get(Avoid, _hit, _hp).Any();
        }

        public string[] GetAvoid(float _hit, float _hp)
        {
            var messages = Get(Avoid, _hit, _hp).ToArray();
            return messages[Random.Range(0, messages.Length)].Message;
        }

        public bool HaveDamage(float _hit, float _hp)
        {
            return Get(Damage, _hit, _hp).Any();
        }

        public string[] GetDamage(float _hit, float _hp)
        {
            var messages = Get(Damage, _hit, _hp).ToArray();
            return messages[Random.Range(0, messages.Length)].Message;
        }

        public bool HaveDestroy(float _hit, float _hp)
        {
            return Get(Destroy, _hit, _hp).Any();
        }

        public string[] GetDestroy(float _hit, float _hp)
        {
            var messages = Get(Destroy, _hit, _hp).ToArray();
            return messages[Random.Range(0, messages.Length)].Message;
        }

        private IEnumerable<SituationData> Get(List<SituationData> _lst, float _hit, float _hp)
        {
            return _lst.Where(v => (v.HitOver == 0 || v.HitOver <= _hit) &&
                                   (v.HitUnder == 0 || v.HitUnder >= _hit) &&
                                   (v.HpOver == 0 || v.HpOver <= _hp) &&
                                   (v.HpUnder == 0 || v.HpUnder >= _hp));
        }
    }
}
