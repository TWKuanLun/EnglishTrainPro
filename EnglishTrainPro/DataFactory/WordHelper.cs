using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EnglishTrainPro.DataFactory
{
    class WordHelper
    {
        private string CurrentPath = Directory.GetCurrentDirectory();
        public WordHelper()
        {
            getVerbLemmas();
        }
        private Dictionary<string, string> verb_lemmas = new Dictionary<string, string>();//動詞型態字典
        private void getVerbLemmas()//獲得動詞型態字典
        {
            #region getData
            string[] data;
            var verb_tenses = new Dictionary<string, string[]>();

            using (var sr = new StreamReader(CurrentPath + "\\verb.txt"))
            {
                string line = sr.ReadToEnd();
                data = line.Split(new char[] { '\n' });
            }
            for (int i = 0; i < data.Length; i++)
            {
                string[] a = data[i].Split(new char[] { ',' });
                verb_tenses[a[0]] = a;
            }
            foreach (var infinitive in verb_tenses)
            {
                foreach (string tense in verb_tenses[infinitive.Key])
                {
                    if (!tense.Equals(""))
                    {
                        verb_lemmas[tense] = infinitive.Key;
                    }
                }
            }
            #endregion
        }
        /// <summary>獲得原形動詞</summary>
        public string getVerbRoot(string v)
        {
            try
            {
                return verb_lemmas[v];
            }
            catch (Exception)
            {
                return v;
            }
        }
        /// <summary>獲得單數名詞</summary>
        public string getSingularNoun(string n)
        {
            return System.Data.Entity.Design.PluralizationServices.PluralizationService.CreateService(System.Globalization.CultureInfo.GetCultureInfo("en-us")).Singularize(n);
        }
        public void getMp3Path(string wordStr, ref List<string> yahooSentencePaths, ref List<string> cambridgeSentencePaths, List<string> wordPaths)
        {
            DirectoryInfo dir = new DirectoryInfo($@"{CurrentPath}\WordData\{wordStr}");
            foreach (var file in dir.GetFiles())
            {
                if (file.Name.Substring(file.Name.Length - 4) != ".mp3")
                    continue;
                if (file.Name.Contains("Sentence"))
                {
                    if (file.Name.Contains("Yahoo"))
                        yahooSentencePaths.Add(file.FullName);
                    else if (file.Name.Contains("Cambridge"))
                        cambridgeSentencePaths.Add(file.FullName);
                    continue;
                }
                wordPaths.Add(file.FullName);
            }

            //將1,11,12,2,3,4,5,6,7,8,9,10順序變成1,2,3,4,5,6,7,8,9,10,11,12
            //以下需要ref
            var cambridgeStrLength = $@"{CurrentPath}\WordData\{wordStr}\CambridgeSentence".Length;
            cambridgeSentencePaths = cambridgeSentencePaths.OrderBy(x => Convert.ToInt32(x.Substring(cambridgeStrLength, x.LastIndexOf(".mp3") - cambridgeStrLength))).ToList();

            var yahooStrLength = $@"{CurrentPath}\WordData\{wordStr}\YahooSentence".Length;
            yahooSentencePaths = yahooSentencePaths.OrderBy(x => Convert.ToInt32(x.Substring(yahooStrLength, x.LastIndexOf(".mp3") - yahooStrLength))).ToList();
        }
    }
}
