using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaliseWords;
using AnaliseWords.Stemmer;

namespace testAnalise
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var pRus = new Porter();
                var pEng = new PorterStemmer();

                //путь к папкам входных и выходных данных
                var dataPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString());

                //чтение входных данных
                var readerWriter = new AnaliseWords.ReadWrite();
                readerWriter.read(String.Format(dataPath.ToString() + "\\InputFolder\\sample.csv"));

                //разбиение файлов на группы словоформ
                var analizerEng = new WordAnalizer(readerWriter.Englist, pEng, 0.9);
                var analizerRus = new WordAnalizer(readerWriter.Ruslist, pRus, 0.9);
                var resultDicEng = analizerEng.analiseWords();
                var resultDicRus = analizerRus.analiseWords();

                //запись в html
                readerWriter.writeHTML(new Dictionary<string, string[]>[] { resultDicEng, resultDicRus }, "Results.html", dataPath.ToString());
            }
            catch(Exception ex)
            {

            }
            

            
            
        }

        

    }
    
}
