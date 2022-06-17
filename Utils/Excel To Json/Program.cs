using System;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excel_To_Json
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "Data.xlsx");

            Excel.Application app = new Excel.Application();
            Excel.Workbook workBook = app.Workbooks.Open(path);

            try
            {

                foreach (Excel.Worksheet sheet in workBook.Worksheets)
                {
                    string name = sheet.Name;
                    string filename = name + ".json";
                    string contents = "";
                    Excel.Range range = sheet.UsedRange;

                    switch (name.ToLower())
                    {
                        case "item":
                            contents = string.Format(JsonFormat.jsonFormat, ParseItem(range));
                            break;
                    }

                    File.WriteAllText(Path.Combine(Environment.CurrentDirectory, filename), contents);
                }

                workBook.Close(true);
                app.Quit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                ReleaseObject(app);
                ReleaseObject(workBook);
            }

        }

        private static string ParseItem(Excel.Range range)
        {
            string contents = "";

            // row 1: type's name
            // 
            // column - 6 = upgrade dats's number
            // 하나의 row는 하나의 데이터를 나타내고 있음.
            // 첫번째 row는 데이터의 타입을 나타내고 있음.
            for (int row = 2; row <= range.Rows.Count; row++)
            {

                string datas = "";
                string keys = "";
                string values = "";

                for (int column = 1; column <= range.Columns.Count; column++)
                {
                    object o = (range.Cells[row, column] as Excel.Range).Value2;

                    if (o != null)
                    {
                        string type = (range.Cells[1, column] as Excel.Range).Value2;

                        if (column <= 5)
                        {
                            datas += ParseValue(type, o) + ",\n";
                        }
                        else
                        {
                            if (keys != "")
                            {
                                keys += ",\n";
                                values += ",\n";
                            }
                            
                            keys += (column - 6).ToString();
                            values +=ParseUpgradeData(o.ToString());
                        }
                    }
                        /*
                        string s = o.ToString();
                        if (s.ToLower() != "true" && s.ToUpper() != "false" && && row > 1)
                        {
                            Tuple<float, float, float, int> t = ParseUpgradeData(s);
                            Console.Write(t.Item1.ToString() + "," + t.Item2.ToString() + "," + t.Item3.ToString() + "," + t.Item4.ToString() + " ");

                        }
                        else Console.Write(s + " ");
                    }
                    else Console.Write(" " + " ");*/
                }
                if (datas != "")
                {
                    if (contents != "") contents += ",\n";
                    contents += string.Format(JsonFormat.itemContentsFormat, datas, keys, values);
                }
                Console.WriteLine();
            }
            return contents;
        }

        private static string ParseValue(string type, object value)
        {
            string s = value.ToString();
            int i;
            float f;
            bool b;
            // 타입에 대한 체크
            if (int.TryParse(s, out i))
            {
                s = i.ToString();
            }
            else if (float.TryParse(s, out f))
            {
                s = f.ToString();
            }
            // bool
            else if (bool.TryParse(s, out b))
            {
                s = b.ToString().ToLower();
            }
            // string
            else
            {
                s = "\"" + s + "\"";
            }
            return string.Format(JsonFormat.valueFormat, type, s);
        }

        private static string ParseUpgradeData(string str)
        {
            string upgradeContents = "";
            float[] values = new float[3];
            int upgradeCost;

            int si = 0;
            int ei = 0;
            int vi = 0;

            for (int i = 0; i < str.Length; i++)
            {
                ei = i;
                if (str[i] == ';')
                {
                    float.TryParse(str.Substring(si, ei - si), out values[vi++]);
                    si = i + 1;
                }
            }

            if (si == 0)
            {
                upgradeContents = "null";
            }
            else
            {
                int.TryParse(str.Substring(si, ei - si), out upgradeCost);

                upgradeContents =
                    ParseValue("defaultValue", values[0]) + ",\n" +
                    ParseValue("currentValue", values[0]) + ",\n" +
                    ParseValue("upgradeValue", values[1]) + ",\n" +
                    ParseValue("maxValue", values[2]) + ",\n" +
                    ParseValue("cost", upgradeCost) + "\n";
                upgradeContents = string.Format(JsonFormat.basicContentsFormat, upgradeContents);
            }
            return upgradeContents;
        }

        private static void ReleaseObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);  // 액셀 객체 해제
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();   // 가비지 수집
            }
        }
    }
}
