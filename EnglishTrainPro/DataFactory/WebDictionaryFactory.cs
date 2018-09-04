using EnglishTrainPro.DataObject;
using NSoup;
using NSoup.Nodes;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace EnglishTrainPro.DataFactory
{
    public enum DictionaryType
    {
        Yahoo,
        Cambridge
    }
    abstract class WebDictionaryFactory
    {
        private string DebugOrReleasePath = Directory.GetCurrentDirectory();
        public WebDictionary GetWord(string wordStr)
        {
            var word = GetWordByLocal(wordStr);
            if (word != null)
                return word;
            var htmlStr = GetHtml(GetWordURL(wordStr));
            Document htmlDoc = NSoupClient.Parse(htmlStr);
            word = GetWordByHtml(htmlDoc, wordStr);
            return word;
        }
        public bool CreateWordData(string wordStr, DirectoryInfo path)
        {
            var htmlStr = GetHtml(GetWordURL(wordStr));
            NSoup.Nodes.Document htmlDoc = NSoup.NSoupClient.Parse(htmlStr);
            var word = GetWordByHtml(htmlDoc, wordStr);
            if (word == null)
                return false;
            path.Create();
            SaveData($@"{path.FullName}\{Type}Word.txt", word);
            //SaveData($@"{DebugOrReleasePath}\WordData\{wordStr}\{Type}Word.txt", word);
            return true;
        }
        public DictionaryType Type { get; set; }
        protected void SaveData(string path, WebDictionary stuff)
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
        protected WebDictionary GetWordByLocal(string wordStr)
        {
            wordStr = wordStr.ToLower();
            WebDictionary word = null;
            try
            {
                //將檔案還原成原來的物件
                using (FileStream oFileStream = new FileStream($@"{DebugOrReleasePath}\WordData\{wordStr}\{Type.ToString()}Word.txt", FileMode.Open))
                {
                    BinaryFormatter myBinaryFormatter = new BinaryFormatter();
                    switch (Type)
                    {
                        case DictionaryType.Yahoo:
                            word = (WebDictionary)myBinaryFormatter.Deserialize(oFileStream);
                            break;
                        case DictionaryType.Cambridge:
                            word = (CambridgeDictionary)myBinaryFormatter.Deserialize(oFileStream);
                            break;
                        default:
                            word = (WebDictionary)myBinaryFormatter.Deserialize(oFileStream);
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
        protected abstract WebDictionary GetWordByHtml(Document htmlDoc, string wordStr);
    }
}
