using EnglishTrainPro.DataObject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static EnglishTrainPro.IO.Serialization;
using static EnglishTrainPro.IO.Download;
using System.Windows;

namespace EnglishTrainPro.DataFactory
{
    public enum AddResult
    {
        Success, SearchFail, HaveWord
    }
    class WordBuilder
    {
        private static WordBuilder singleton;
        protected WordBuilder()
        {
        }
        public static WordBuilder Instance()
        {
            if (singleton == null)
                singleton = new WordBuilder();
            return singleton;
        }
        private WordHelper helper = new WordHelper();

        private string PublishPath = Directory.GetCurrentDirectory();
        public event EventHandler ProgressChanged;
        public event EventHandler LocalDataChanged;
        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnProgressChanged(new EventArgs());
            }
        }
        protected virtual void OnProgressChanged(EventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }
        protected virtual void OnLocalDataChanged(EventArgs e)
        {
            LocalDataChanged?.Invoke(this, e);
        }
        public (string Name, string URL)[] WordMediaURL(string wordStr)
        {
            return new (string Name, string URL)[]
            {
                ("Google", $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&q={wordStr}"),
                ("VoiceTube", $"https://tw.voicetube.com/player/{wordStr}.mp3"),
                ("YahooNormal", $"https://s.yimg.com/bg/dict/dreye/live/f/{wordStr}.mp3"),
                ("YahooNormal2", $"https://s.yimg.com/bg/dict/dreye/live/f/{wordStr}@2.mp3"),
                ("YahooNormal3", $"https://s.yimg.com/bg/dict/dreye/live/f/{wordStr}@3.mp3"),
                ("YahooUS1", $"https://s.yimg.com/bg/dict/ox/mp3/v1/{wordStr}@_us_1.mp3"),
                ("YahooUS2", $"https://s.yimg.com/bg/dict/ox/mp3/v1/{wordStr}@_us_2.mp3"),
                ("YahooUS3", $"https://s.yimg.com/bg/dict/ox/mp3/v1/{wordStr}@_us_3.mp3"),
                ("YahooGB1", $"https://s.yimg.com/bg/dict/ox/mp3/v1/{wordStr}@_gb_1.mp3"),
                ("YahooGB2", $"https://s.yimg.com/bg/dict/ox/mp3/v1/{wordStr}@_gb_2.mp3"),
                ("YahooGB3", $"https://s.yimg.com/bg/dict/ox/mp3/v1/{wordStr}@_gb_3.mp3")
            };
        }
        public (string Name, string URL)[] LocalWordMediaPath(string wordStr)
        {
            var wordDirectory = new DirectoryInfo($@"{PublishPath}\WordData\{wordStr}");
            return wordDirectory.EnumerateFiles("*.mp3").Where(x => !x.Name.Contains("Sentence"))
                .Select(x => (Name: x.Name.Replace(".mp3",""), URL: x.FullName)).ToArray();
        }
        public List<string> SentencesMediaURL(WebDictionary word)
        {
            var list = new List<string>();
            foreach (var sentencesByPos in word.Sentences)
            {
                foreach (var sentencesByMeaning in sentencesByPos.Value)
                {
                    foreach (var sentence in sentencesByMeaning.Value)
                    {
                        var sentenceURL = $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&q={sentence.GetEnglishSentence()}";
                        list.Add(sentenceURL);
                    }
                }
            }
            return list;
        }
        public List<string> LocalSentencesMediaPath(WebDictionary word)
        {
            var list = new List<string>();
            var count = 0;
            foreach (var sentencesByPos in word.Sentences)
            {
                foreach (var sentencesByMeaning in sentencesByPos.Value)
                {
                    foreach (var sentence in sentencesByMeaning.Value)
                    {
                        //var sentenceURL = $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&q={sentence.GetEnglishSentence()}";
                        list.Add($@"{PublishPath}\WordData\{word.ToString()}\{word.Type.ToString()}Sentence{count.ToString()}.mp3");
                        count++;
                    }
                }
            }
            return list;
        }
        public void DownloadWordMp3(string wordStr)
        {
            var medias = WordMediaURL(wordStr);
            Task[] tasks = new Task[medias.Length];

            for(int i= 0; i < tasks.Length; i++)
            {
                //https://stackoverflow.com/questions/25173722/indexoutofrangeexception-exception-when-using-tasks-in-for-loop-in-c-sharp
                var currentIndex = i;
                tasks[i] = Task.Factory.StartNew(() => WebDownloadFile(medias[currentIndex].URL, $@"{PublishPath}\WordData\{wordStr}\{medias[currentIndex].Name}.mp3"));
            }
            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException e)
            {
                MessageBox.Show($"Error: {e.ToString()}");
            }
        }
        protected void CreateSentencesMp3(WebDictionary word)
        {
            var webURLs = SentencesMediaURL(word);
            for(int i = 0; i < webURLs.Count; i++)
            {
                var index = i;
                Task.Run(() => WebDownloadFile(webURLs[index], $@"{PublishPath}\WordData\{word.ToString()}\{word.Type.ToString()}Sentence{index}.mp3"));
            }
        }
        public void RemoveWord(string wordStr)
        {
            Directory.Delete($@"{PublishPath}\WordData\{wordStr}", true);
            OnLocalDataChanged(new EventArgs());
        }

        private bool CreateWordData(string wordStr, DirectoryInfo path)
        {
            var word = GetWordByWeb(wordStr);
            if (word == null)
                return false;
            foreach(var dictionary in word.dictionary)
            {
                CreateSentencesMp3(dictionary);
            }
            SaveObject($@"{path.FullName}\Word.txt", word);
            DownloadWordMp3(wordStr);
            return true;
        }
        private Word GetWordByWeb(string wordStr)
        {
            var yahooFactory = new YahooDictionaryFactory();
            var yahooWord = yahooFactory.GetDictionaryByHtml(wordStr);
            var cambridgeFactory = new CambridgeDictionaryFactory();
            var cambridgeWord = cambridgeFactory.GetDictionaryByHtml(wordStr);
            if (yahooWord == null && cambridgeWord == null)
                return null;
            Word word = new Word(wordStr);
            if (yahooWord != null)
            {
                word.dictionary.Add(yahooWord);
            }
            if (cambridgeWord != null)
            {
                word.dictionary.Add(cambridgeWord);
            }
            return word;
        }
        public Word GetWord(string wordStr)
        {
            wordStr = wordStr.ToLower();
            var word = LoadObject<Word>($@"{PublishPath}\WordData\{wordStr}\Word.txt");
            if (word == null)
                word = GetWordByWeb(wordStr);
            return word;
        }
        public AddResult CreateWord(string wordStr)
        {
            wordStr = helper.getVerbRoot(wordStr);
            wordStr = helper.getSingularNoun(wordStr);

            wordStr = wordStr.ToLower();
            DirectoryInfo rootDirectory = new DirectoryInfo($"{PublishPath}\\WordData");
            rootDirectory.Create();//目錄已存在不作用
            DirectoryInfo[] subDirectories = rootDirectory.GetDirectories();
            DirectoryInfo wordDirectory = new DirectoryInfo($@"{PublishPath}\WordData\{wordStr}");
            var findWord = subDirectories.Where(x => x.Name == wordStr).SingleOrDefault();
            if (findWord != null)//已有此單字
                return AddResult.HaveWord;
            wordDirectory.Create();
            var succeed = CreateWordData(wordStr, wordDirectory);
            if (!succeed)
                return AddResult.SearchFail;
            OnLocalDataChanged(new EventArgs());
            return AddResult.Success;
        }
        public AddResult[] CreateWords(string[] wordStrs)
        {
            var result = new AddResult[wordStrs.Length];
            Progress = 0;
            for (int i = 0; i < wordStrs.Length; i++)
            {
                result[i] = CreateWord(wordStrs[i]);
                Progress = 100 * (i + 1) / wordStrs.Length;
                //防止被檔IP
                Task.Delay(new Random(new Guid().GetHashCode()).Next(5000));
            }
            OnLocalDataChanged(new EventArgs());
            return result;
        }
        
    }
}
