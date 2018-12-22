using EnglishTrainPro.DataFactory;
using EnglishTrainPro.DataObject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EnglishTrainPro.Display
{
    class DictionarySwitchTabGrid
    {
        private string PublishPath = Directory.GetCurrentDirectory();
        public bool SetDictionarySwitchTabControl(string wordStr, Grid grid)
        {
            wordStr = wordStr.ToLower();
            var builder = WordBuilder.Instance();
            var word = builder.GetWord(wordStr);
            if(word == null)
            {
                MessageBox.Show("查無此單字");
                return false;
            }

            var yahooGrid = new Grid();
            var cambridgeGrid = new Grid();
            var helper = new WordHelper();

            try
            {
                (string Name, string URL)[] wordPaths = null;
                var directoryExists = Directory.Exists($@"{PublishPath}\WordData\{word.ToString()}");
                if (directoryExists)
                    wordPaths = builder.LocalWordMediaPath(word.ToString());
                else
                    wordPaths = builder.WordMediaURL(word.ToString()).Where(x =>
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(x.URL);
                        request.Method = "HEAD";
                        try
                        {
                            using (var response = (HttpWebResponse)request.GetResponse())
                            {
                                // Code here 
                            }
                            request.GetResponse();
                        }
                        catch (WebException we)
                        {
                            HttpWebResponse errorResponse = we.Response as HttpWebResponse;
                            if (errorResponse.StatusCode == HttpStatusCode.Forbidden ||
                                errorResponse.StatusCode == HttpStatusCode.NotFound)
                            {
                                return false;
                            }
                        }
                        return true;
                    }).ToArray();
                foreach (var dictionary in word.dictionary)
                {
                    var sentencePaths = builder.SentencesMediaURL(dictionary);

                    if (directoryExists)
                        sentencePaths = builder.LocalSentencesMediaPath(dictionary);

                    IWordGrid wordGrid = null;
                    switch (dictionary.Type)
                    {
                        case WebDictionary.DictionaryType.Cambridge:
                            wordGrid = new CambridgeGridBuilder(dictionary, cambridgeGrid, sentencePaths, wordPaths);
                            break;
                        case WebDictionary.DictionaryType.Yahoo:
                            wordGrid = new YahooGridBuilder(dictionary, yahooGrid, sentencePaths, wordPaths);
                            break;
                    }
                    wordGrid.SetupGrid();
                }

                var tabControl = new TabControl();
                tabControl.Background = Brushes.Black;

                var yahooTabItem = new TabItem();
                yahooTabItem.Header = "_Yahoo";
                yahooTabItem.Content = yahooGrid;
                yahooTabItem.IsSelected = true;

                var CambridgeTabItem = new TabItem();
                CambridgeTabItem.Header = "_Cambridge";
                CambridgeTabItem.Content = cambridgeGrid;

                tabControl.Items.Add(yahooTabItem);
                tabControl.Items.Add(CambridgeTabItem);

                grid.Children.Add(tabControl);

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show($"發生未知的錯誤，請聯絡作者:jacky19918@gmail.com，並將此訊息截圖給作者：\n{e.ToString()}");
                return false;
            }
        }
    }
}
