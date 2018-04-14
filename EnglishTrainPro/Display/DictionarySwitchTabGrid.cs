using EnglishTrainPro.DataFactory;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace EnglishTrainPro.Display
{
    class DictionarySwitchTabGrid
    {
        public void SetDictionarySwitchTabControl(string wordStr, Grid grid)
        {
            var yahooGrid = new Grid();
            var cambridgeGrid = new Grid();
            var helper = new WordHelper();
            var yahooSentencePaths = new List<string>();
            var cambridgeSentencePaths = new List<string>();
            var wordPaths = new List<string>();
            helper.getMp3Path(wordStr, ref yahooSentencePaths, ref cambridgeSentencePaths, wordPaths);
            var yahooBuiler = new YahooGridBuilder(wordStr, yahooGrid, yahooSentencePaths, wordPaths);
            var cambridgeBuiler = new CambridgeGridBuilder(wordStr, cambridgeGrid, cambridgeSentencePaths, wordPaths);
            yahooBuiler.SetupGrid();
            cambridgeBuiler.SetupGrid();
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
        }
    }
}
