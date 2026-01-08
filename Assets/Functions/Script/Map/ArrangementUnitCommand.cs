using System;
using Functions.Manager;
using Functions.Util;
namespace Functions.Script.Map
{
    [Serializable]
    public class ArrangementUnitCommand : ICommand
    {
        private int minNum;
        private int maxNum;
        private int direction;
        private string name;
        private string method;
        
        public ArrangementUnitCommand(Json.ProcessJson prc)
        {
            minNum = prc.min_num;
            maxNum = prc.max_num;
            direction = prc.direction;
            name = prc.name;
            method = prc.method;
            
            if (minNum < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "minNum"));
            if (maxNum < 1) throw new Exception(LocaleUtil.GetMessage("E_S0004", "maxNum", "1"));
            if (direction < 0 || direction > 3) throw new Exception(LocaleUtil.GetMessage("E_S0005", "direction", 0, 3));
            if (string.IsNullOrWhiteSpace(method)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "method"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.ArrangementUnits(minNum, maxNum, direction, name, method);
            return true;
        }
    }
}

