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
        public ReadWrite()
        { }

        public List<string> Ruslist;
        public List<string> Englist;
        public string  read(string filePath)
        {
            string Line;
            List<string> list = new List<string>();
            Ruslist = new List<string>();
            Englist = new List<string>();
            FileStream fs = new FileStream("C:\\Users\\kudrik\\Documents\\visual studio 2013\\Projects\\YandexWords\\CSVFolder\\sample.csv", FileMode.Open);
            //Если текст в русской кодировке, необходимо это указать
            using (StreamReader sr = new StreamReader(fs))
            {
                while (sr.EndOfStream != true)
                {
                    Line = sr.ReadLine();
                    Line = Line.Replace("\"", "");

                    if (IsStringLatin(Line))
                        Englist.Add(Line);
                    else
                        Ruslist.Add(Line);
                    list.Add(Line);
                }

                sr.Close();

            }

          
            return list[1700];
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

        public static void checkMains(Dictionary<string,AnaliseWords.WordAnalizer.MainPart> mainDic,Dictionary<string, string[]> wordsDic , double percent) 
        {
            
            var result = new Dictionary<string,AnaliseWords.WordAnalizer.MainPart>();
            var Keys = mainDic.Keys.ToList();
            var Values = mainDic.Values.ToList();
            for(int i=0;i< mainDic.Keys.Count; i++)
            {
                var secondString = Values[i];
                var isIncluded = false;
                var checkList = new List<AnaliseWords.WordAnalizer.MainPart>();
                for(int j=i;j<mainDic.Keys.Count;j++)
                {
                    
                    if(i!=j)
                    {

                        var firstString = Values[j];
                        
                        int length = AnaliseWords.WordAnalizer.LongestCommonSubstringLength(firstString.Word, secondString.Word);
                        if (length >= percent * firstString.Length) 
                        {
                            if (needtoAdd(checkList, firstString, percent))
                            {
                                isIncluded = true;
                                if (Values[i].InUse)
                                {
                                    var wordList1 = wordsDic[Keys[j]].ToList();
                                    wordList1.Add(Keys[i]);
                                    wordsDic[Keys[j]] = wordList1.ToArray();
                                }
                                else
                                {
                                    var wordList1 = wordsDic[Keys[j]].ToList();
                                    var wordList2 = wordsDic[Keys[i]].ToList();
                                    var rs = wordList1.Concat(wordList2).ToList();
                                    wordsDic[Keys[j]] = rs.ToArray();
                                    
                                }
                                Values[j].InUse = true;
                                checkList.Add(firstString);
                            }
                            else 
                            {

                            }
                        }


                    }
                }
                if (!Values[i].InUse && isIncluded)
                {
                    wordsDic.Remove(Keys[i]);
                }


            }
             
        }

        public static bool needtoAdd(List<AnaliseWords.WordAnalizer.MainPart> list, AnaliseWords.WordAnalizer.MainPart candidate, double percent) 
        {
            bool result = true;
            foreach (var item in list)
            {
                int length = AnaliseWords.WordAnalizer.LongestCommonSubstringLength(candidate.Word, item.Word);
                if (length >= percent * candidate.Length)
                {
                    result = false;
                    break;
                }
                else
                    result = true;
            }
            return result;

        }

        public static void findSameMainParts(Dictionary<string, AnaliseWords.WordAnalizer.MainPart> mainDic, Dictionary<string, string[]> wordsDic, double percent)
        {
            
            var maxlength = mainDic.Values.ToList()[mainDic.Values.Count - 1].Length;
            for (int i = 1; i < maxlength + 1; i++)
            {
                var curLenghtSubList = mainDic.Where(x => x.Value.Length == i).ToDictionary(x => x.Key, x => x.Value);
                var Keys = curLenghtSubList.Keys.ToList();
                var Values = curLenghtSubList.Values.ToList();
                for (int k = 0; k < curLenghtSubList.Count; k++)
                {
                    for (int j = i; j < curLenghtSubList.Count; j++)
                    {
                        if (k != j)
                        {

                            if (Values[k].Word.Equals(Values[j].Word) )//&& (Keys[j].Length - Keys[k].Length) / Keys[k].Length < 1- percent)
                            {
                                var list1 = wordsDic[Keys[k]].ToList();
                                var list2 = wordsDic[Keys[j]].ToList();
                                list1.AddRange(list2);
                                wordsDic[Keys[k]] = list1.ToArray();
                                wordsDic.Remove(Keys[j]);
                                mainDic.Remove(Keys[j]);

                                curLenghtSubList.Remove(Keys[j]);
                                Keys.Remove(Keys[j]);
                                Values.Remove(Values[j]);
                                j = j - 1;
                            }
                        }
                    }

                }

            }
        }



        public static Dictionary<string, string[]> findSameWords(List<string> list1)
        {
            var result = new Dictionary<string, string[]>();
            for (int i = 0; i < list1.Count; i++) 
            {
                result.Add(list1[i], new string[] { list1[i] });
                for (int j = i; j < list1.Count; j++) 
                {
                    if (i != j) 
                    {
                        if (list1[i].ToLower().Equals(list1[j].ToLower()))
                        {
                            var l = result[list1[i]].ToList();
                            l.Add(list1[j]);
                            result[list1[i]] = l.ToArray();
                            //удаление совпадающего элемента, чтобы его большене проверять
                            list1.Remove(list1[j]);
                            j = j - 1;
                        }

                    }
                }
            }
            return result;
        }

        public static void writeHTML(Dictionary<string, string[]> wordsDic, string htmlName) 
        {
            StreamWriter str = new StreamWriter(String.Format(@"C:\Users\kudrik\Documents\visual studio 2013\Projects\YandexWords\CSVFolder\{0}.htm",htmlName));
            str.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            foreach (var key in wordsDic.Keys)
            {
                var val = wordsDic[key].ToList();
                str.WriteLine(String.Format("<table border=\"1px\" cellpadding=\"0\" cellspacing=\"0\"><caption >{0}</caption>", key));
                foreach (var item in val)
                {
                    str.WriteLine(String.Format("<tr><td>{0}</td></tr>", item));
                }
                str.WriteLine("</table>");
            }
           

        }
        
    }
}
