using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WMPLib;

namespace EnglishTrainPro.UIComponent
{
    class SentencePlayer
    {
        /// <summary>句子Google發音</summary>
        private List<WindowsMediaPlayer> SentencePlayers = new List<WindowsMediaPlayer>();
        public SentencePlayer(List<string> sentencePaths)
        {
            foreach (var sentencePath in sentencePaths)
            {
                SentencePlayers.Add(new WindowsMediaPlayer { URL = sentencePath });
                SentencePlayers[SentencePlayers.Count - 1].controls.stop();
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
            foreach (var SentencePlayer in SentencePlayers)
            {
                SentencePlayer.controls.pause();
            }
            SentencePlayers[(int)b.Tag].controls.currentPosition = 0;
            SentencePlayers[(int)b.Tag].controls.play();
        }
    }
}
