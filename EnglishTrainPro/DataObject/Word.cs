using System;
using System.Collections.Generic;

namespace EnglishTrainPro.DataObject
{
    [Serializable]
    abstract class Word
    {
        protected Word(string word,
            Dictionary<string, Dictionary<string, List<Sentence>>> sentences)
        {
            this.word = word.ToLower();
            Sentences = sentences;
            Remark = string.Empty;
            Weight = 3;
        }
        protected readonly string word;
        /// <summary>Key=詞性，Value=(Key=中文意思，Value=句子)</summary>
        public readonly Dictionary<string, Dictionary<string, List<Sentence>>> Sentences;
        /// <summary>單字權重起始值，數字越大越不熟，0=非常熟，完全不會出現在單字練習。</summary>
        protected int Weight { get; set; }
        /// <summary>備註</summary>
        protected string Remark { get; set; }
        public override string ToString()
        {
            return word;
        }
    }
}
