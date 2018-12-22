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
    abstract class WebDictionaryFactory
    {
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
        protected abstract string GetDictionaryURL(string wordStr);
        public abstract WebDictionary GetDictionaryByHtml(string wordStr);
    }
}
