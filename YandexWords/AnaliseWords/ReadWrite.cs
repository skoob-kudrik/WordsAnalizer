using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace AnaliseWords
{
    public class ReadWrite
    {
        /// <summary>
        /// Класс чтения исходных данных и записи выходных данных
        /// </summary>
        public ReadWrite()
        { }

        private List<string> ruslist;

        public List<string> Ruslist
        {
            get { return ruslist; }
            set { ruslist = value; }
        }
        private List<string> englist;

        public List<string> Englist
        {
            get { return englist; }
            set { englist = value; }
        }
        /// <summary>
        /// Метод извлечения исходных данных из csv файла
        /// </summary>
        /// <param name="filePath">Путь к файлу с учетом названия файла</param>
        public void  read(string filePath)
        {
            string Line;
            List<string> list = new List<string>();
            ruslist = new List<string>();
            englist = new List<string>();
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open);
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (sr.EndOfStream != true)
                    {
                        Line = sr.ReadLine();
                        Line = Line.Replace("\"", "");

                        if (IsStringLatin(Line))
                            englist.Add(Line);
                        else
                            ruslist.Add(Line);
                        list.Add(Line);
                    }

                    sr.Close();

                }
            }
            catch(Exception ex)
            {
                
            }

        }

        private  bool IsStringLatin(string content)
        {
            bool result = true;

            char[] letters = content.ToCharArray();

            for (int i = 0; i < letters.Length; i++)
            {
                int charValue = System.Convert.ToInt32(letters[i]);

                if (charValue > 128)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        
        /// <summary>
        /// Пишет получившеееся разбиение в html файл
        /// </summary>
        /// <param name="wordsDics">Разбиение в виде массива словарей. Количество словарей - количество языков</param>
        /// <param name="htmlName">Название html файла</param>
        /// <param name="htmlPath">Путь к html файлу</param>
        public  void writeHTML(Dictionary<string, string[]>[] wordsDics, string htmlName, string htmlPath) 
        {
            try
            {
                using (StreamWriter str = new StreamWriter(String.Format(htmlPath + "\\OutputFolder\\" + htmlName)))
                {

                    str.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
                    foreach (var dictionary in wordsDics) 
                    {
                        str.WriteLine( printDic(dictionary));
                    }

                    
                }
            }
            catch (Exception ex) 
            {
            } 
        }
        
        private string printDic(IDictionary<string, string[]> dic) 
        {
            string result = null;
            foreach (var key in dic.Keys)
            {
                var val = dic[key].ToList();
                result = result+ String.Format("<table border=\"1px\" cellpadding=\"0\" cellspacing=\"0\"><caption >{0}</caption>", key);
                foreach (var item in val)
                {
                    result = result+ String.Format("<tr><td>{0}</td></tr>", item);
                }
                result = result+"</table>";
            }
            return result;
        }
        
    }
}
