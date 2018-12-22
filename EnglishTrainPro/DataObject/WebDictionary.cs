using System;
using System.Collections.Generic;

namespace EnglishTrainPro.DataObject
{
    [Serializable]
    abstract class WebDictionary
    {
        public enum DictionaryType
        {
            Yahoo,
            Cambridge
        }
        public DictionaryType Type { get; set; }
        protected WebDictionary(string word, Dictionary<string, Dictionary<string, List<Sentence>>> sentences)
        {
            this.word = word.ToLower();
            Sentences = sentences;
        }
        protected readonly string word;
        /// <summary>Key=詞性，Value=(Key=中文意思，Value=句子)</summary>
        public readonly Dictionary<string, Dictionary<string, List<Sentence>>> Sentences;
        public override string ToString()
        {
            return word;
        }
    }
}
