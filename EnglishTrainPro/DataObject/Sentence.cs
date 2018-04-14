using System;

namespace EnglishTrainPro.DataObject
{
    [Serializable]
    class Sentence
    {
        public Sentence(string chi, string eng)
        {
            chinese = chi;
            english = eng;
        }
        private readonly string chinese;
        private readonly string english;
        public string GetChineseSentence() => chinese;
        public string GetEnglishSentence() => english;
    }
}
