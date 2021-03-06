using System;
using System.Collections.Generic;
using System.Text;

namespace WatchDBApp
{
    public class CalibresItem
    {
        public string ModelName
        {
            get;
            set;
        }
        public List<string> Descriptions { get; set; }

        public List<CalibresPart> Parts { get; set; }
    }
}
