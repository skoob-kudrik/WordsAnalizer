using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaliseWords.Stemmer.RU;
using AnaliseWords.Stemmer.Eng;

namespace testAnalise
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Porter pRus = new Porter();
            PorterStemmer pEng = new PorterStemmer();
            var resultDicEng = new Dictionary<string, string[]>();
            var resultDicRus = new Dictionary<string, string[]>();
            AnaliseWords.ReadWrite forRead = new AnaliseWords.ReadWrite();
            forRead.read("ddd");//добавить передачу сюда файла
            getWords(forRead.Englist, resultDicEng,"ENGWords",  pEng);
            getWords(forRead.Ruslist, resultDicRus,"RUSWords",null, pRus);
            

            
            
        }

        public static void getWords( List<string> input ,Dictionary<string, string[]> output,string fileName, PorterStemmer pEng, Porter pRus = null)
        {

            var output2 = new List<string>();
            var resultDic = new Dictionary<string, string[]>();
            var mainpartDic = new Dictionary<string, AnaliseWords.WordAnalizer.MainPart>();
            output2 = input.OrderBy(pr => pr.Length).ToList();
            var maxlength = output2[output2.Count - 1].Length;
            for (int i = 1; i < maxlength + 1; i++)
            {
                var curLenghtSubList = output2.Where(x => x.Length == i).ToList();
                var dic = AnaliseWords.ReadWrite.findSameWords(curLenghtSubList);
                foreach (var key in dic.Keys)
                {
                    resultDic.Add(key, dic[key]);
                }
            }
            output2 = resultDic.Keys.ToList();
            foreach (var item in output2)
            {
                if (item.Length > 2)
                {
                    string newWord;
                    if (pEng != null)
                        newWord = pEng.StemWord(item.ToLower());
                    else
                        newWord = pRus.StemWord(item.ToLower());
                    if (newWord.Length < 3)
                        mainpartDic.Add(item, new AnaliseWords.WordAnalizer.MainPart(item.ToLower()));
                    else
                        mainpartDic.Add(item, new AnaliseWords.WordAnalizer.MainPart(newWord));
                }
            }
            mainpartDic = mainpartDic.OrderBy(x => x.Value.Length).ToDictionary(x => x.Key, x => x.Value);
            AnaliseWords.ReadWrite.findSameMainParts(mainpartDic, resultDic, 0.8);

            mainpartDic = mainpartDic.OrderBy(x => -x.Value.Length).ToDictionary(x => x.Key, x => x.Value);

            AnaliseWords.ReadWrite.checkMains(mainpartDic, resultDic, 0.9);
            AnaliseWords.ReadWrite.writeHTML(resultDic, fileName);
            output = resultDic;
        }

    }
    
}
