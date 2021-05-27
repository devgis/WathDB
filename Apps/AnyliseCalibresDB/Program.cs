using AngleSharp.Html.Parser;
using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace AnyliseCalibresDB
{
    class Program
    {
        const string file = @"D:\Github\WathDB\Docs\ETAs\OK.txt";
        const string outfile = @"D:\Github\WathDB\Docs\ETAs\OK.json";
        static void Main(string[] args)
        {
            Console.WriteLine($"Started[{DateTime.Now.ToString()}]--------------------------------------------");
            Console.WriteLine();
            string json = File.ReadAllText(file);
            List<TempItem> list = JsonConvert.DeserializeObject<List<TempItem>>(json);
            var parser = new HtmlParser();
            List<CalibresItem> Calibres = new List<CalibresItem>();
            foreach (var item in list)
            {
                Console.Write(".");
                //var table = ParsingWeb(item.Html, "/html/body/table/tbody/tr/td/table");
                var document = parser.ParseDocument(item.Html);
                var maintable = document.QuerySelector("html>body>table");

                CalibresItem calibresItem = new CalibresItem() { Descriptions=new List<string>(), ModelName=item.Name, Parts=new List<CalibresPart>() };
                var rows= maintable.QuerySelectorAll("tr");
                foreach (var element in rows)
                {
                    string rowhtml = element.OuterHtml;
                    if (rowhtml.ToLower().Contains("table"))
                    {
                        //html>body>table>tbody>tr>td>table
                        var subrows = element.QuerySelectorAll("tr");
                        foreach (var subrow in subrows)
                        {
                            var subprops = subrow.QuerySelectorAll("td");
                            if (subprops != null && subprops.Length > 0)
                            {
                                CalibresPart cpart = new CalibresPart();
                                cpart.Description = subprops[0].TextContent;
                                cpart.Alternate = subprops[1].TextContent;
                                try
                                {
                                    cpart.Price = Convert.ToDouble(subprops[2].TextContent);
                                }
                                catch
                                { }
                                calibresItem.Parts.Add(cpart);
                            }

                        }
                        break;
                    }
                    else
                    {
                        string s = element.TextContent;
                        if (!s.Trim().Equals("*"))
                        {
                            calibresItem.Descriptions.Add(s);
                        }
                    }
                    
                }
                Calibres.Add(calibresItem);
            }
            File.WriteAllText(outfile, JsonConvert.SerializeObject(Calibres));
            Console.WriteLine();
            Console.WriteLine($"Done! Saved:{Calibres.Count}");
            Console.Read();
        }

        ////HtmlString 获取的html页面的字符串
        ////XmlPath 解析元素在html中的位置,像:XmlPath = "/html/body/div[3]/div[3]/div[1]/table"
        //public static DataTable ParsingWeb(string HtmlString, string XmlPath)
        //{
        //    try
        //    {
        //        //HtmlWeb web = new HtmlWeb();
        //        //HtmlDocument doc = web.Load(WebUrl);
        //        var doc = new HtmlDocument();
        //        doc.LoadHtml(HtmlString);
        //        DataTable htTable = new DataTable();
        //        var tablehtml = doc.DocumentNode.SelectSingleNode(XmlPath);

        //        if (tablehtml == null)
        //        {
        //            return null;
        //        }
        //        var TrSelected = tablehtml.SelectNodes(".//tr");
        //        foreach (HtmlNode row in TrSelected)
        //        {
        //            var Index = TrSelected.IndexOf(row);
        //            if (TrSelected.IndexOf(row) == 0)
        //            {
        //                foreach (HtmlNode cell in row.SelectNodes("th|td"))  //有些table 表头是写在 td中的
        //                {

        //                    htTable.Columns.Add(cell.InnerText, typeof(string));
        //                }
        //            }
        //            else
        //            {
        //                DataRow TempRow = htTable.NewRow();
        //                foreach (HtmlNode cell in row.SelectNodes("th|td"))
        //                {

        //                    var position = row.SelectNodes("th|td").IndexOf(cell);
        //                    TempRow[htTable.Columns[position].ColumnName] = cell.InnerText;
        //                }
        //                htTable.Rows.Add(TempRow);
        //            }
        //        }
        //        return htTable;
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}
    }
}
