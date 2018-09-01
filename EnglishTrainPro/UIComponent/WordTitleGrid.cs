using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EnglishTrainPro.UIComponent
{
    class WordTitleGrid
    {
        public WordTitleGrid(string word, double wordFontSize, double partOfSpeechAndWordPlayerFontSize, List<string> wordPaths, Grid mainGrid, int index)
        {
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions[index].Height = new GridLength(0, GridUnitType.Auto);
            Grid titleGrid = new Grid();
            Grid.SetRow(titleGrid, index);
            mainGrid.Children.Add(titleGrid);
            var wordLabel = new GridLabel(
                word,
                wordFontSize,
                Brushes.SkyBlue,
                titleGrid,
                GridDefinitions.Column,
                new GridLength(0, GridUnitType.Auto),
                0);
            WordPlayer wordPlayer = new WordPlayer(wordPaths, partOfSpeechAndWordPlayerFontSize, titleGrid, 1);
        }
    }
}
