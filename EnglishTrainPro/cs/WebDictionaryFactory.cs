using NSoup;
using NSoup.Nodes;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace EnglishTrainPro.cs
{
    public enum DictionaryType
    {
        Yahoo,
        Cambridge
    }
    abstract class WebDictionaryFactory
    {
        private string DebugOrReleasePath = Directory.GetCurrentDirectory();
        public Word GetWord(string wordStr)
        {
            var word = GetWordByLocal(wordStr);
            if (word != null)
                return word;
            var htmlStr = GetHtml(GetWordURL(wordStr));
            Document htmlDoc = NSoupClient.Parse(htmlStr);
            word = GetWordByHtml(htmlDoc, wordStr);
            return word;
        }
        public bool CreateWordData(string wordStr)
        {
            var htmlStr = GetHtml(GetWordURL(wordStr));
            NSoup.Nodes.Document htmlDoc = NSoup.NSoupClient.Parse(htmlStr);
            var word = GetWordByHtml(htmlDoc, wordStr);
            if (word == null)
                return false;
            SaveData($@"{DebugOrReleasePath}\WordData\{wordStr}\{Type}Word.txt", word);
            return true;
        }
        public DictionaryType Type { get; set; }
        protected void SaveData(string path, Word stuff)
        {
            using (FileStream oFileStream = new FileStream(path, FileMode.Create))
            {
                //建立二進位格式化物件
                BinaryFormatter myBinaryFormatter = new BinaryFormatter();
                //將物件進行二進位格式序列化，並且將之儲存成檔案
                myBinaryFormatter.Serialize(oFileStream, stuff);
                oFileStream.Flush();
                oFileStream.Close();
                oFileStream.Dispose();
            }
        }
        protected Word GetWordByLocal(string wordStr)
        {
            wordStr = wordStr.ToLower();
            Word word = null;
            try
            {
                //將檔案還原成原來的物件
                using (FileStream oFileStream = new FileStream($@"{DebugOrReleasePath}\WordData\{wordStr}\{Type.ToString()}Word.txt", FileMode.Open))
                {
                    BinaryFormatter myBinaryFormatter = new BinaryFormatter();
                    switch (Type)
                    {
                        case DictionaryType.Yahoo:
                            word = (YahooWord)myBinaryFormatter.Deserialize(oFileStream);
                            break;
                        case DictionaryType.Cambridge:
                            word = (CambridgeWord)myBinaryFormatter.Deserialize(oFileStream);
                            break;
                        default:
                            word = (Word)myBinaryFormatter.Deserialize(oFileStream);
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
            return word;
        }
        protected string GetHtml(string URL)
        {
            WebRequest myRequest = WebRequest.Create(URL);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream());
            string htmlSourceCode = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();
            return htmlSourceCode;
        }
        protected abstract string GetWordURL(string wordStr);
        protected abstract Word GetWordByHtml(Document htmlDoc, string wordStr);
    }
}
