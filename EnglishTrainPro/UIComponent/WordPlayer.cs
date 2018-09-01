using EnglishTrainPro.DataFactory;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WMPLib;

namespace EnglishTrainPro.UIComponent
{
    class WordPlayer
    {
        /// <summary>單字發音</summary>
        private List<MediaPlayerHelper> WordPlayers = new List<MediaPlayerHelper>();
        public WordPlayer(List<string> wordPaths, double fontSize, Grid grid, int startInGridColumnNum)
        {
            foreach (var wordPath in wordPaths)
            {
                WordPlayers.Add(new MediaPlayerHelper(wordPath));
            }
            for (int i = 0; i < WordPlayers.Count; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions[i + startInGridColumnNum].Width = new GridLength(0, GridUnitType.Auto);
                Button button = new Button();
                var index = wordPaths[i].LastIndexOf("\\");
                var fileName = wordPaths[i].Substring(index + 1);
                StringBuilder sb = new StringBuilder(fileName);
                sb[0] = fileName.ToUpper()[0];
                var name = sb.ToString(0, fileName.Length - 4);
                if (name.Substring(0, 5) != "Yahoo")
                    name = "_" + name;
                button.Tag = i;
                button.Content = name;
                button.Background = Brushes.Black;
                button.Foreground = Brushes.White;
                button.FontSize = fontSize;
                button.Click += WordVoiceButton_Click;
                Grid.SetColumn(button, i + startInGridColumnNum);
                grid.Children.Add(button);
            }
        }

        private void WordVoiceButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            foreach (var WordPlayer in WordPlayers)
            {
                WordPlayer.Pause();
            }
            WordPlayers[(int)b.Tag].PlayFromStart();
        }
    }
}
