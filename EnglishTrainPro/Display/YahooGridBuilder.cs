﻿using EnglishTrainPro.DataFactory;
using EnglishTrainPro.DataObject;
using EnglishTrainPro.UIComponent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WMPLib;

namespace EnglishTrainPro.Display
{
    class YahooGridBuilder : IWordGrid
    {
        private Grid mainGrid;
        private YahooDictionary word;
        private string DebugOrReleasePath = Directory.GetCurrentDirectory();
        private List<string> sentencePaths;
        private (string Source, string URL)[] wordPaths;
        public YahooGridBuilder(WebDictionary word, Grid mainGrid, List<string> sentencePaths, (string Source, string URL)[] wordPaths)
        {
            this.word = (YahooDictionary)word;
            this.mainGrid = mainGrid;
            this.sentencePaths = sentencePaths;
            this.wordPaths = wordPaths;
        }
        public void SetupGrid()
        {
            double wordFontSize = 60;
            double partOfSpeechAndWordPlayerFontSize = 35;
            double sentenceFontSize = 27;
            
            #region 句子播放按鈕設定
            var sentencePlayer = new SentencePlayer(sentencePaths);
            #endregion

            mainGrid.Children.Clear();
            mainGrid.RowDefinitions.Clear();

            #region titleGrid設定 含wordLabel、googleButton、voiceTubeButton、yahooButton
            var wordTitleGrid = new WordTitleGrid(word.ToString(), wordFontSize, partOfSpeechAndWordPlayerFontSize, wordPaths, mainGrid, 0);
            #endregion
            #region 音標Label Name:phoneticLabel
            var phoneticLabel = new GridLabel(
                word.PhoneticSymbol,
                partOfSpeechAndWordPlayerFontSize,
                Brushes.SpringGreen,
                mainGrid,
                GridDefinitions.Row,
                new GridLength(0, GridUnitType.Auto),
                1);
            #endregion
            #region dataGrid設定 含詞性、中文意思、例句
            var wordDataGrid = new WordDataGrid(word, partOfSpeechAndWordPlayerFontSize, sentenceFontSize, mainGrid, 2, sentencePlayer);
            #endregion
        }
        
    }
}
