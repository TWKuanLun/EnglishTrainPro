using System;
using System.Collections.Generic;

namespace EnglishTrainPro.DataObject
{
    [Serializable]
    class YahooWord : Word
    {
        public YahooWord(string word, Dictionary<string, Dictionary<string, List<Sentence>>> sentences,
            string phoneticSymbol) : base(word, sentences)
        {
            PhoneticSymbol = phoneticSymbol;
        }
        /// <summary>音標</summary>
        public readonly string PhoneticSymbol;
    }
}
