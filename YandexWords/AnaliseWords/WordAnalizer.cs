﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnaliseWords
{
    public class WordAnalizer
    {
        public static int LongestCommonSubstringLength(string str1, string str2)
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
        
     
     

        public class MainPart 
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
