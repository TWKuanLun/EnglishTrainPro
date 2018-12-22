using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishTrainPro.DataObject
{
    [Serializable]
    class CambridgeDictionary : WebDictionary
    {
        public CambridgeDictionary(string word,
            Dictionary<string, Dictionary<string, List<Sentence>>> sentences,
            HashSet<string> phoneticSymbol) : base(word, sentences)
        {
            PhoneticSymbol = phoneticSymbol;
            Type = DictionaryType.Cambridge;
        }
        /// <summary>音標</summary>
        public readonly HashSet<string> PhoneticSymbol;
    }
}
