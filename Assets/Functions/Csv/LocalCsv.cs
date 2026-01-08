using Csv.Annotations;
namespace Functions.Csv
{
    [CsvObject]
    public partial class LocalCsv
    {
        [Column(0)]
        public string Key { get; set; }

        [Column(1)]
        public string Japanese { get; set; }

        [Column(2)]
        public string English { get; set; }
    }
}
