using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnaliseWords.Stemmer
{
    public interface iStemmer
    {
        String StemWord(String Word);
    }
}
