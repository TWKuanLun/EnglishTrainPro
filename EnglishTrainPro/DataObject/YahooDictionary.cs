using System;
using System.Collections.Generic;

namespace EnglishTrainPro.DataObject
{
    [Serializable]
    class YahooDictionary : WebDictionary
    {
        public YahooDictionary(string word, Dictionary<string, Dictionary<string, List<Sentence>>> sentences,
            string phoneticSymbol) : base(word, sentences)
        {
            PhoneticSymbol = phoneticSymbol;
            Type = DictionaryType.Yahoo;
        }
        /// <summary>音標</summary>
        public readonly string PhoneticSymbol;
    }
}
