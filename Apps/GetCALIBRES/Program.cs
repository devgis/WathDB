using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading;
using Common;

namespace GetCALIBRES
{
    class Program
    {
        const string baseURL = "http://cgi.julesborel.com/";
        const string baseHtmlPath = @"D:\Github\WathDB\Docs\ETAs";
        static void Main(string[] args)
        {
            Console.WriteLine($"Started[{DateTime.Now.ToString()}]--------------------------------------------");
            try
            {
                var htmlFiles = new DirectoryInfo(baseHtmlPath).GetFiles("*.html");
                if (htmlFiles == null || htmlFiles.Length <= 0)
                {
                    Console.WriteLine($"No html file found [{DateTime.Now.ToString()}]【Base Path:{baseHtmlPath}】--------------------------------------------");
                }
                else
                {
                    List<TempItem> results = new List<TempItem>();
                    foreach (var htmlfile in htmlFiles)
                    {
                        Console.WriteLine($"Begin anylise [{DateTime.Now.ToString()}]【{htmlfile.FullName}】--------------------------------------------");
                        string html = File.ReadAllText(htmlfile.FullName);
                        var calibreInfos = GetInfo(html);
                        Console.WriteLine($"Count 【{calibreInfos.Count}】");
                        var client = new WebClient();
                        foreach (var calibre in calibreInfos)
                        {
                            Console.Write(".");
                            var tempitem = new TempItem();
                            tempitem.Name = calibre.ModelName;
                            tempitem.Html = client.DownloadString(calibre.URL);
                            results.Add(tempitem);
                            //Thread.Sleep(200);
                        }
                    }
                    File.WriteAllText(Path.Combine(baseHtmlPath, "RS.txt"), JsonConvert.SerializeObject(results));
                    Console.WriteLine($"Successed[{DateTime.Now.ToString()}]--------------------------------------------");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unhandled Error[{DateTime.Now.ToString()}]【{ex.Message}】--------------------------------------------");
            }
            Console.Read();
        }

        public static List<CalibreInfo> GetInfo(string html)
        {
            List<CalibreInfo> list = new List<CalibreInfo>();
            string reg = "<a(\\s+(href=\"(?<url>([^\"])*)\"|'([^'])*'|\\w+=\"(([^\"])*)\"|'([^'])*'))+>(?<text>(.*?))</a>";
            var matchs = Regex.Matches(html, reg, RegexOptions.IgnoreCase);
            if (matchs != null && matchs.Count > 0)
            {
                foreach (Match m in matchs)
                {
                    CalibreInfo info = new CalibreInfo()
                    {
                        URL = m.Groups["url"].Value,
                        ModelName = m.Groups["text"].Value,
                        Desc = m.Groups["text"].Value
                    };
                    list.Add(info);
                }
            }
            return list;
        }
    }
}
