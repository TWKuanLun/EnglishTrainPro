using EnglishTrainPro.DataFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishTrainPro.DataObject
{
    class Word
    {
        private readonly string word;
        /// <summary>單字權重起始值，數字越大越不熟，0=非常熟，完全不會出現在單字練習。</summary>
        public int Weight { get; set; }
        public string Remark { get; set; }
        public List<WebDictionary> dictionary { get; set; }
        public Word(string word)
        {
            this.word = word;
            Remark = string.Empty;
            Weight = 3;
            dictionary = new List<WebDictionary>();
        }
        public override string ToString()
        {
            return word;
        }
    }
}
