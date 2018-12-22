using EnglishTrainPro.DataFactory;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WMPLib;

namespace EnglishTrainPro.UIComponent
{
    class SentencePlayer
    {
        /// <summary>句子Google發音</summary>
        private List<MediaPlayerHelper> SentencePlayers = new List<MediaPlayerHelper>();
        public SentencePlayer(List<string> sentencePaths)
        {
            foreach (var sentencePath in sentencePaths)
            {
                SentencePlayers.Add(new MediaPlayerHelper(sentencePath));
            }
        }
        public void SetSentenceVoiceButton(int sentenceCount, double fontSize, Grid grid, int index)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions[index].Width = GridLength.Auto;
            var SentenceVoiceButton = new Button();
            SentenceVoiceButton.Tag = sentenceCount;
            SentenceVoiceButton.Content = $"_{sentenceCount}Play";
            SentenceVoiceButton.Background = Brushes.Black;
            SentenceVoiceButton.Foreground = Brushes.White;
            SentenceVoiceButton.FontSize = fontSize;
            SentenceVoiceButton.Click += SentenceVoiceButton_Click;
            Grid.SetColumn(SentenceVoiceButton, index);
            grid.Children.Add(SentenceVoiceButton);
        }

        private void SentenceVoiceButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            int clickIndex = (int)b.Tag;
            for (int i = 0; i < SentencePlayers.Count; i++)
            {
                if (i == clickIndex)
                {
                    SentencePlayers[i].PlayFromStart();
                }
                else
                {
                    SentencePlayers[i].Pause();
                }
            }
        }
    }
}
