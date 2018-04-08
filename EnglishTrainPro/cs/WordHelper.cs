using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishTrainPro.cs
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
    }
}
