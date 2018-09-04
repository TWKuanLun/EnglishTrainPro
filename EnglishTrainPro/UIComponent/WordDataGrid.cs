using EnglishTrainPro.DataObject;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EnglishTrainPro.UIComponent
{
    class WordDataGrid
    {
        public WordDataGrid(WebDictionary word, double partOfSpeechAndWordPlayerFontSize, double sentenceFontSize, Grid mainGrid, int index, SentencePlayer sentencePlayer)
        {
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions[index].Height = new GridLength(1, GridUnitType.Star);
            ScrollViewer scrollViewer = new ScrollViewer();
            Grid dataGrid = new Grid();
            dataGrid.VerticalAlignment = VerticalAlignment.Top;
            scrollViewer.Content = dataGrid;
            Grid.SetRow(scrollViewer, index);
            mainGrid.Children.Add(scrollViewer);
            int gridRowIndex = 0;
            int sentenceCount = 0;
            foreach (var partOfSpeechAndMeaningPair in word.Sentences)
            {
                #region 詞性設定
                var partOfSpeechLabel = new GridLabel(
                    partOfSpeechAndMeaningPair.Key,
                    partOfSpeechAndWordPlayerFontSize,
                    Brushes.Lavender,
                    dataGrid,
                    GridDefinitions.Row,
                    null,
                    gridRowIndex);
                gridRowIndex++;
                #endregion
                foreach (var meaningAndSentencePair in partOfSpeechAndMeaningPair.Value)
                {
                    #region 中文意思設定
                    var chineseMeaningLabel = new GridLabel(
                        $"　{meaningAndSentencePair.Key}",
                        partOfSpeechAndWordPlayerFontSize,
                        Brushes.PapayaWhip,
                        dataGrid,
                        GridDefinitions.Row,
                        null,
                        gridRowIndex);
                    gridRowIndex++;
                    #endregion

                    for (int i = 0; i < meaningAndSentencePair.Value.Count; i++)
                    {
                        #region 英文例句設定
                        dataGrid.RowDefinitions.Add(new RowDefinition());
                        var sentenceGrid = new Grid();
                        string[] sentanceWords = meaningAndSentencePair.Value[i].GetEnglishSentence().Split(new char[] { ' ' });
                        sentenceGrid.HorizontalAlignment = HorizontalAlignment.Left;
                        #region 加上前面的空白間隔
                        var emptyLabel = new GridLabel($"　　　", sentenceFontSize, null, sentenceGrid, GridDefinitions.Column, GridLength.Auto, 0);
                        #endregion
                        for (int j = 0; j < sentanceWords.Length; j++)
                        {
                            var wordButton = new WordButton(sentanceWords[j] + " ", sentenceFontSize, sentenceGrid, j + 1);
                        }
                        #region 句子發音
                        sentencePlayer.SetSentenceVoiceButton(sentenceCount, sentenceFontSize, sentenceGrid, sentanceWords.Length + 1);
                        #endregion
                        Grid.SetRow(sentenceGrid, gridRowIndex);
                        dataGrid.Children.Add(sentenceGrid);
                        gridRowIndex++;
                        #endregion
                        #region 中文例句設定
                        var chineseSentenceLabel = new GridLabel(
                            $"　　　 {meaningAndSentencePair.Value[i].GetChineseSentence()}",
                            sentenceFontSize,
                            Brushes.Gainsboro,
                            dataGrid,
                            GridDefinitions.Row,
                            null,
                            gridRowIndex);
                        gridRowIndex++;
                        #endregion
                        sentenceCount++;
                    }
                }
            }
        }
    }
}
