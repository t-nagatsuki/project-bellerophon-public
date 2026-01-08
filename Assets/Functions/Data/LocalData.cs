using System.Collections.Generic;
using Functions.Csv;
namespace Functions.Data
{
    public class LocalData
    {
        private readonly Dictionary<string, LocalCsv> localData = new Dictionary<string, LocalCsv>();

        public void Load(LocalCsv[] data)
        {
            foreach (var datum in data)
            {
                localData[datum.Key] = datum;
            }
        }

        public string GetEntry(int local, string key)
        {
            if (!localData.ContainsKey(key)) return null;
            switch (local)
            {
                case 0:
                    return localData[key].Japanese;
                case 1:
                    return localData[key].English;
                default:
                    return string.Empty;
            }
        }
    }
}
