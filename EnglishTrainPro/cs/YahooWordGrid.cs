using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WMPLib;

namespace EnglishTrainPro.cs
{
    class YahooWordGrid : WordGrid
    {
        private string DebugOrReleasePath = Directory.GetCurrentDirectory();
        /// <summary>單字發音</summary>
        List<WindowsMediaPlayer> WordPlayers = new List<WindowsMediaPlayer>();
        /// <summary>句子Google發音</summary>
        List<WindowsMediaPlayer> SentencePlayers = new List<WindowsMediaPlayer>();
        List<string> sentencePaths = new List<string>();
        List<string> wordPaths = new List<string>();
        string wordStr;
        public YahooWordGrid(string wordStr, Grid mainGrid)
        {
            wordStr = wordStr.ToLower();
            double wordFontSize = 60;
            double partOfSpeechAndWordPlayerFontSize = 35;
            double sentenceFontSize = 27;

            DirectoryInfo dir = new DirectoryInfo($@"{DebugOrReleasePath}\WordData\{wordStr}");
            foreach (var file in dir.GetFiles())
            {
                if (file.Name.Contains("Sentence"))
                {
                    if(file.Name.Contains("Yahoo"))
                        sentencePaths.Add(file.FullName);
                    continue;
                }
                if (file.Name.Substring(file.Name.Length - 4) != ".mp3")
                    continue;
                wordPaths.Add(file.FullName);
            }
            this.wordStr = wordStr;
            #region 播放按鈕設定
            foreach (var sentencePath in sentencePaths)
            {
                SentencePlayers.Add(new WindowsMediaPlayer { URL = sentencePath });
                SentencePlayers[SentencePlayers.Count - 1].controls.stop();
            }
            foreach (var wordPath in wordPaths)
            {
                WordPlayers.Add(new WindowsMediaPlayer { URL = wordPath });
                WordPlayers[WordPlayers.Count - 1].controls.stop();
            }
            #endregion

            WebDictionaryFactory yahooFactory = new YahooDictionaryFactory();
            YahooWord word = (YahooWord)yahooFactory.GetWord(wordStr);
            
            mainGrid.Children.Clear();
            mainGrid.RowDefinitions.Clear();
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Auto);
            mainGrid.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Auto);
            mainGrid.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
            Grid titleGrid = new Grid();
            #region 音標Label Name:phoneticLabel
            Label phoneticLabel = new Label();
            phoneticLabel.Content = word.PhoneticSymbol;
            phoneticLabel.Foreground = Brushes.SpringGreen;
            phoneticLabel.FontSize = 35;
            #endregion
            ScrollViewer scrollViewer = new ScrollViewer();
            Grid dataGrid = new Grid();
            dataGrid.VerticalAlignment = VerticalAlignment.Top;
            scrollViewer.Content = dataGrid;
            Grid.SetRow(titleGrid, 0);
            Grid.SetRow(phoneticLabel, 1);
            Grid.SetRow(scrollViewer, 2);
            mainGrid.Children.Add(titleGrid);
            mainGrid.Children.Add(phoneticLabel);
            mainGrid.Children.Add(scrollViewer);

            #region titleGrid設定 含wordLabel、googleButton、voiceTubeButton、yahooButton
            Label wordLabel = new Label();
            wordLabel.Content = word;
            wordLabel.Foreground = Brushes.SkyBlue;
            wordLabel.FontSize = wordFontSize;
            titleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            titleGrid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Auto);
            Grid.SetColumn(wordLabel, 0);
            titleGrid.Children.Add(wordLabel);
            for (int i = 0; i < WordPlayers.Count; i++)
            {
                titleGrid.ColumnDefinitions.Add(new ColumnDefinition());
                titleGrid.ColumnDefinitions[i + 1].Width = new GridLength(0, GridUnitType.Auto);
                Button button = new Button();
                var index = wordPaths[i].LastIndexOf("\\");
                var fileName = wordPaths[i].Substring(index + 1);
                StringBuilder sb = new StringBuilder(fileName);
                sb[0] = fileName.ToUpper()[0];
                var name = sb.ToString(0, fileName.Length - 4);
                button.Tag = i;
                button.Content = name;
                button.Background = Brushes.Black;
                button.Foreground = Brushes.White;
                button.FontSize = partOfSpeechAndWordPlayerFontSize;
                button.Click += WordVoiceButton_Click;
                Grid.SetColumn(button, i + 1);
                titleGrid.Children.Add(button);
            }
            #endregion
            #region dataGrid設定 含詞性、中文意思、例句

            int gridRowIndex = 0;
            int sentenceCount = 0;
            foreach (var partOfSpeechAndMeaningPair in word.Sentences)
            {
                #region 詞性設定
                dataGrid.RowDefinitions.Add(new RowDefinition());
                var partOfSpeechlabel = new Label();
                partOfSpeechlabel.Content = partOfSpeechAndMeaningPair.Key;
                partOfSpeechlabel.Foreground = Brushes.Lavender;
                partOfSpeechlabel.FontSize = partOfSpeechAndWordPlayerFontSize;
                Grid.SetRow(partOfSpeechlabel, gridRowIndex);
                dataGrid.Children.Add(partOfSpeechlabel);
                gridRowIndex++;
                #endregion
                foreach(var meaningAndSentencePair in partOfSpeechAndMeaningPair.Value)
                {
                    #region 中文意思設定
                    dataGrid.RowDefinitions.Add(new RowDefinition());
                    var chiMeaninglabel = new Label();
                    chiMeaninglabel.Content = $"　{meaningAndSentencePair.Key}";
                    chiMeaninglabel.Foreground = Brushes.PapayaWhip;
                    chiMeaninglabel.FontSize = partOfSpeechAndWordPlayerFontSize;
                    Grid.SetRow(chiMeaninglabel, gridRowIndex);
                    dataGrid.Children.Add(chiMeaninglabel);
                    gridRowIndex++;
                    #endregion

                    for(int i= 0; i < meaningAndSentencePair.Value.Count; i++)
                    {
                        #region 英文例句設定
                        dataGrid.RowDefinitions.Add(new RowDefinition());
                        var sentenceGrid = new Grid();
                        string[] sentanceWords = meaningAndSentencePair.Value[i].GetEnglishSentence().Split(new char[] { ' ' });
                        sentenceGrid.HorizontalAlignment = HorizontalAlignment.Left;
                        #region 加上前面的空白間隔
                        sentenceGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        sentenceGrid.ColumnDefinitions[0].Width = GridLength.Auto;
                        var Emptylabel = new Label();
                        Emptylabel.Content = $"　　　";
                        Emptylabel.FontSize = sentenceFontSize;
                        Grid.SetRow(Emptylabel, 0);
                        sentenceGrid.Children.Add(Emptylabel);
                        #endregion
                        for (int j = 0; j < sentanceWords.Length; j++)
                        {
                            sentenceGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            sentenceGrid.ColumnDefinitions[j + 1].Width = GridLength.Auto;//給予grid寬度自動，防止按鈕部分遮蓋
                            var button = new Button();
                            button.Content = sentanceWords[j] + " ";
                            button.BorderThickness = new Thickness(0);//按鈕框線粗細，0=看不到框線
                            button.Background = Brushes.Black;
                            button.Foreground = Brushes.Pink;
                            button.FontSize = sentenceFontSize;
                            button.Click += Button_Click;
                            Grid.SetColumn(button, j + 1);
                            sentenceGrid.Children.Add(button);
                        }
                        #region 句子發音
                        //SentencePlayer.Add(new WMPLib.WindowsMediaPlayer { URL = $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&q={s.Eng}" });
                        //SentencePlayer[sentenceCount].controls.stop();
                        sentenceGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        sentenceGrid.ColumnDefinitions[sentanceWords.Length + 1].Width = GridLength.Auto;
                        var SentenceVoiceButton = new Button();
                        SentenceVoiceButton.Tag = sentenceCount;
                        SentenceVoiceButton.Content = $"_{sentenceCount}Play";
                        SentenceVoiceButton.Background = Brushes.Black;
                        SentenceVoiceButton.Foreground = Brushes.White;
                        SentenceVoiceButton.FontSize = sentenceFontSize;
                        SentenceVoiceButton.Click += SentenceVoiceButton_Click;
                        Grid.SetColumn(SentenceVoiceButton, sentanceWords.Length + 1);
                        sentenceGrid.Children.Add(SentenceVoiceButton);
                        #endregion
                        Grid.SetRow(sentenceGrid, gridRowIndex);
                        dataGrid.Children.Add(sentenceGrid);
                        gridRowIndex++;
                        #endregion
                        #region 中文例句設定
                        dataGrid.RowDefinitions.Add(new RowDefinition());
                        var clabel = new Label();
                        clabel.Content = $"　　　 {meaningAndSentencePair.Value[i].GetChineseSentence()}";
                        clabel.Foreground = Brushes.Gainsboro;
                        clabel.FontSize = sentenceFontSize;
                        Grid.SetRow(clabel, gridRowIndex);
                        dataGrid.Children.Add(clabel);
                        gridRowIndex++;
                        #endregion
                        sentenceCount++;
                    }
                }
                for (int i = 0; i < partOfSpeechAndMeaningPair.Value.Count; i++)
                {
                    
                }
            }
            #endregion
        }

        private void SentenceVoiceButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            foreach (var SentencePlayer in SentencePlayers)
            {
                SentencePlayer.controls.pause();
            }
            SentencePlayers[(int)b.Tag].controls.currentPosition = 0;
            SentencePlayers[(int)b.Tag].controls.play();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string word = b.Content.ToString().Substring(0, b.Content.ToString().Length - 1);//去除空白
            //wordExplanationWindow wordWindow = new wordExplanationWindow(word);
            //wordWindow.Show();
        }

        private void WordVoiceButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            foreach (var WordPlayer in WordPlayers)
            {
                WordPlayer.controls.pause();
            }
            WordPlayers[(int)b.Tag].controls.currentPosition = 0;
            WordPlayers[(int)b.Tag].controls.play();
        }
    }
}
