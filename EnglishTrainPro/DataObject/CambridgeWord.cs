using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishTrainPro.DataObject
{
    [Serializable]
    class CambridgeWord : Word
    {
        public CambridgeWord(string word,
            Dictionary<string, Dictionary<string, List<Sentence>>> sentences,
            HashSet<string> phoneticSymbol) : base(word, sentences)
        {
            PhoneticSymbol = phoneticSymbol;
        }
        /// <summary>音標</summary>
        public readonly HashSet<string> PhoneticSymbol;
    }
}
