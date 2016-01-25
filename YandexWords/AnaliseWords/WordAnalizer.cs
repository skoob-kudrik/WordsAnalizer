using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnaliseWords
{
    public class WordAnalizer
    {
        private List<string> inputList;
        private Stemmer.iStemmer stemmer;
        private double accuracy;

        public double Accuracy
        {
            get { return accuracy; }
            set { accuracy = value; }
        }

        public Stemmer.iStemmer Stemmer
        {
            get { return stemmer; }
            set { stemmer = value; }
        }

        public List<string> InputList
        {
            get { return inputList; }
            set { inputList = value; }
        }
        /// <summary>
        /// Класс для проведения разбиения слов на группы
        /// </summary>
        /// <param name="listOfWords">Перечень анализируемых слов</param>
        /// <param name="stemmerToUse">Используемый стеммер(в зависимости от языка)</param>
        /// <param name="detectAcuracy">Точность соотвествия между выделенными основами</param>
        public WordAnalizer(List<string> listOfWords, Stemmer.iStemmer stemmerToUse, double detectAcuracy) 
        {
            InputList = listOfWords;
            Stemmer = stemmerToUse;
            Accuracy = detectAcuracy;
        }
        /// <summary>
        /// Основной метод для выполнения анализа
        /// </summary>
        /// <returns>Возвращает словарь со словами, разбитыми на группы. Ключь в словаре - слово, по которому формировалась группа значений</returns>
        public  Dictionary<string, string[]> analiseWords()
        {

            
            var resultDic = new Dictionary<string, string[]>();
            var mainpartDic = new Dictionary<string, MainPart>();
            var words = inputList.OrderBy(pr => pr.Length).ToList();
            var maxlength = words[words.Count - 1].Length;
            for (int i = 1; i < maxlength + 1; i++)
            {
                var curLenghtSubList = words.Where(x => x.Length == i).ToList();
                var dic = findSameWords(curLenghtSubList);
                foreach (var key in dic.Keys)
                {
                    resultDic.Add(key, dic[key]);
                }
            }
            words = resultDic.Keys.ToList();
            foreach (var item in words)
            {
                string newWord;
                if (item.Length > 2)
                {
                    newWord = stemmer.StemWord(item.ToLower());
                    if (newWord.Length < 3)
                        mainpartDic.Add(item, new MainPart(item.ToLower()));
                    else
                        mainpartDic.Add(item, new MainPart(newWord));
                }
            }
            mainpartDic = mainpartDic.OrderBy(x => x.Value.Length).ToDictionary(x => x.Key, x => x.Value);
            findSameMainParts(mainpartDic, resultDic);

            mainpartDic = mainpartDic.OrderBy(x => -x.Value.Length).ToDictionary(x => x.Key, x => x.Value);

            checkMains(mainpartDic, resultDic, accuracy);
            return resultDic;
        }
        
        /// <summary>
        /// Поиск слов со схожими основами
        /// </summary>
        /// <param name="mainDic"></param>
        /// <param name="wordsDic"></param>
        /// <param name="percent"></param>
        private  void checkMains(Dictionary<string, MainPart> mainDic, Dictionary<string, string[]> wordsDic, double percent)
        {

            var result = new Dictionary<string, MainPart>();
            var Keys = mainDic.Keys.ToList();
            var Values = mainDic.Values.ToList();
            for (int i = 0; i < mainDic.Keys.Count; i++)
            {
                var secondString = Values[i];
                var isIncluded = false;
                var checkList = new List<MainPart>();
                for (int j = i; j < mainDic.Keys.Count; j++)
                {

                    if (i != j)
                    {

                        var firstString = Values[j];

                        int length = LongestCommonSubstringLength(firstString.Word, secondString.Word);
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

        private  bool needtoAdd(List<MainPart> list, MainPart candidate, double percent)
        {
            bool result = true;
            foreach (var item in list)
            {
                int length = LongestCommonSubstringLength(candidate.Word, item.Word);
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
        /// <summary>
        /// Поиск слов с совпадающей основой
        /// </summary>
        /// <param name="mainDic"></param>
        /// <param name="wordsDic"></param>
        private  void findSameMainParts(Dictionary<string,MainPart> mainDic, Dictionary<string, string[]> wordsDic)
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

                            if (Values[k].Word.Equals(Values[j].Word))
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


        /// <summary>
        /// Функция поиска одинаковых слов среди слов одинаковой длины
        /// </summary>
        /// <param name="list1">Лист со словами одинаковой длины</param>
        /// <returns>Словарь одинаковых слов с ключем - совпадающим словом</returns>
        private  Dictionary<string, string[]> findSameWords(List<string> list1)
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
                            list1.Remove(list1[j]);
                            j = j - 1;
                        }

                    }
                }
            }
            return result;
        }

        public  int LongestCommonSubstringLength(string str1, string str2)
        {
            if (String.IsNullOrEmpty(str1) || String.IsNullOrEmpty(str2))
                return 0;

            List<int[]> num = new List<int[]>();
            int maxlen = 0;
            for (int i = 0; i < str1.Length; i++)
            {
                num.Add(new int[str2.Length]);
                for (int j = 0; j < str2.Length; j++)
                {
                    if (str1[i] != str2[j])
                        num[i][j] = 0;
                    else
                    {
                        if ((i == 0) || (j == 0))
                            num[i][j] = 1;
                        else
                            num[i][j] = 1 + num[i - 1][j - 1];
                        if (num[i][j] > maxlen)
                            maxlen = num[i][j];
                    }
                    if (i >= 2)
                        num[i - 2] = null;
                }
            }
            return maxlen;
        }

        private class MainPart 
        {
            private string word;

            public string Word
            {
                get { return word; }
                set { word = value; }
            }
            private bool inUse = false;

            public bool InUse
            {
                get { return inUse; }
                set { inUse = value; }
            }

            public int Length
            {
                get { return word.Length; }
            }
            public MainPart(string MainPart)
            {
                Word = MainPart;
            }
            public override string ToString()
            {
                return word;
            }

        }
    }
}
