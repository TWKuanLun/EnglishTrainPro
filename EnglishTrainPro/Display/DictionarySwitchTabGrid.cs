using EnglishTrainPro.DataFactory;
using EnglishTrainPro.DataObject;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace EnglishTrainPro.Display
{
    class DictionarySwitchTabGrid
    {
        public void SetDictionarySwitchTabControl(string wordStr, Grid grid)
        {
            wordStr = wordStr.ToLower();
            var yahooGrid = new Grid();
            var cambridgeGrid = new Grid();
            var helper = new WordHelper();
            var yahooSentencePaths = new List<string>();
            var cambridgeSentencePaths = new List<string>();
            var wordPaths = new List<string>();
            helper.getMp3Path(wordStr, ref yahooSentencePaths, ref cambridgeSentencePaths, wordPaths);

            WebDictionaryFactory dictionaryFactory = new YahooDictionaryFactory();
            Word yahooWord = dictionaryFactory.GetWord(wordStr);
            var yahooBuiler = new YahooGridBuilder(yahooWord, yahooGrid, yahooSentencePaths, wordPaths);

            dictionaryFactory = new CambridgeDictionaryFactory();
            Word cambridgeWord = dictionaryFactory.GetWord(wordStr);
            var cambridgeBuiler = new CambridgeGridBuilder(cambridgeWord, cambridgeGrid, cambridgeSentencePaths, wordPaths);

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
