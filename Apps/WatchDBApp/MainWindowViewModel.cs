using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDBApp
{
    public class MainWindowViewModel
    {
        const string jsonfile = @"D:\Github\WathDB\Docs\ETAs\OK.json";
        public List<CalibresItem> CalibresAll
        {   get;
            set;
        }

        public List<CalibresItem> Calibres
        {
            get;
            set;
        }
        public MainWindowViewModel()
        {
            CalibresAll = JsonConvert.DeserializeObject<List<CalibresItem>>(File.ReadAllText(jsonfile));

            Calibres = CalibresAll.Take(10).ToList();
        }
    }
}
