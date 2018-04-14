using EnglishTrainPro.DataObject;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EnglishTrainPro.DataFactory
{
    public enum AddResult
    {
        Success, SearchFail, HaveWord
    }
    class WordBuilder
    {
        WordHelper helper = new WordHelper();

        private string DebugOrReleasePath = Directory.GetCurrentDirectory();
        public event EventHandler ProgressChanged;
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
        public AddResult CreateWord(string wordStr)
        {
            wordStr = helper.getVerbRoot(wordStr);
            wordStr = helper.getSingularNoun(wordStr);
            
            wordStr = wordStr.ToLower();
            DirectoryInfo rootDirectory = new DirectoryInfo($"{DebugOrReleasePath}\\WordData");
            rootDirectory.Create();//目錄已存在不作用
            DirectoryInfo[] subDirectories = rootDirectory.GetDirectories();
            DirectoryInfo wordDirectory = new DirectoryInfo($@"{DebugOrReleasePath}\WordData\{wordStr}");
            var findWord = subDirectories.Where(x => x.Name == wordStr).SingleOrDefault();
            if (findWord != null)//已有此單字
                return AddResult.HaveWord;
            var cambridgeFactory = new CambridgeDictionaryFactory();
            var yahooFactory = new YahooDictionaryFactory();
            var tCambridge = Task.Run(() => cambridgeFactory.CreateWordData(wordStr, wordDirectory));
            var tYahoo = Task.Run(() => yahooFactory.CreateWordData(wordStr, wordDirectory));
            tCambridge.Wait();
            tYahoo.Wait();
            if (!tCambridge.Result && !tYahoo.Result)
                return AddResult.SearchFail;
            DownloadWordMp3(wordStr);
            if (tYahoo.Result)
            {
                var yahooWord = yahooFactory.GetWord(wordStr);
                CreateSentencesMp3(yahooWord, yahooFactory.Type.ToString());
            }
            if (tCambridge.Result)
            {
                var cambridgeWord = cambridgeFactory.GetWord(wordStr);
                CreateSentencesMp3(cambridgeWord, cambridgeFactory.Type.ToString());
            }
            return AddResult.Success;
        }
        private AddResult[] CreateWords(string[] wordStrs)
        {
            var result = new AddResult[wordStrs.Length];
            Progress = 0;
            for(int i = 0; i < wordStrs.Length; i++)
            {
                result[i] = CreateWord(wordStrs[i]);
                Progress = 100 * (i + 1) / wordStrs.Length;
            }
            //防止被檔IP
            Task.Delay(new Random(new Guid().GetHashCode()).Next(5000));
            return result;
        }
        public Task<AddResult[]> TaskCreateWords(string[] wordStrs)
        {
            return Task.Factory.StartNew(() => CreateWords(wordStrs));
        }
        private bool WebDownloadFile(string source, string destination)
        {
            bool success = true;
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent: Other");
                try
                {
                    client.DownloadFile(new Uri(source), destination);
                }
                catch (Exception)
                {
                    success = false;
                }
            }
            return success;
        }
        private void DownloadWordMp3(string wordStr)
        {
            var googleURL = $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&q={wordStr}";
            var voiceTubeURL = $"https://tw.voicetube.com/player/{wordStr}.mp3";
            var yahooURL = new Tuple<string, string>[]
            {
                new Tuple<string, string>("yahooNormal",$"https://s.yimg.com/tn/dict/dreye/live/f/{wordStr}.mp3"),
                new Tuple<string, string>("yahooNormal2",$"https://s.yimg.com/tn/dict/dreye/live/f/{wordStr}@2.mp3"),
                new Tuple<string, string>("yahooNormal3",$"https://s.yimg.com/tn/dict/dreye/live/f/{wordStr}@3.mp3"),
                new Tuple<string, string>("yahooUS1",$"https://s.yimg.com/tn/dict/ox/mp3/v1/{wordStr}@_us_1.mp3"),
                new Tuple<string, string>("yahooUS2",$"https://s.yimg.com/tn/dict/ox/mp3/v1/{wordStr}@_us_2.mp3"),
                new Tuple<string, string>("yahooUS3",$"https://s.yimg.com/tn/dict/ox/mp3/v1/{wordStr}@_us_3.mp3"),
                new Tuple<string, string>("yahooGB1",$"https://s.yimg.com/tn/dict/ox/mp3/v1/{wordStr}@_gb_1.mp3"),
                new Tuple<string, string>("yahooGB2",$"https://s.yimg.com/tn/dict/ox/mp3/v1/{wordStr}@_gb_2.mp3"),
                new Tuple<string, string>("yahooGB3",$"https://s.yimg.com/tn/dict/ox/mp3/v1/{wordStr}@_gb_3.mp3")
            };
            Task.Run(() => WebDownloadFile(googleURL, $@"{DebugOrReleasePath}\WordData\{wordStr}\google.mp3"));
            Task.Run(() => WebDownloadFile(voiceTubeURL, $@"{DebugOrReleasePath}\WordData\{wordStr}\voiceTube.mp3"));
            
            foreach (var Url in yahooURL)
            {
                Task.Run(() => WebDownloadFile(Url.Item2, $@"{DebugOrReleasePath}\WordData\{wordStr}\{Url.Item1}.mp3"));
            }
        }
        protected void CreateSentencesMp3(Word word, string source)
        {
            int count = 0;
            foreach (var sentencesByPos in word.Sentences)
            {
                foreach (var sentencesByMeaning in sentencesByPos.Value)
                {
                    foreach (var sentence in sentencesByMeaning.Value)
                    {
                        var sentenceURL = $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&q={sentence.GetEnglishSentence()}";
                        if (WebDownloadFile(sentenceURL, DebugOrReleasePath + "\\WordData\\" + word.ToString() + "\\" + source + "Sentence" + count + ".mp3"))
                        {
                            count++;
                        }
                    }
                }
            }
        }
        public void RemoveWord(string wordStr)
        {
            Directory.Delete($@"{DebugOrReleasePath}\WordData\{wordStr}", true);
        }
    }
}
