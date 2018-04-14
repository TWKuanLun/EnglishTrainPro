using EnglishTrainPro.DataFactory;
using EnglishTrainPro.Display;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace EnglishTrainPro
{
    /// <summary>
    /// wordExplanationWindow.xaml 的互動邏輯
    /// </summary>
    public partial class wordExplanationWindow : Window
    {
        private string DebugOrReleasePath = Directory.GetCurrentDirectory();
        private string wordStr;
        private bool save = false;
        public wordExplanationWindow(string wordStr)
        {
            InitializeComponent();
            WordHelper helper = new WordHelper();
            //去除'和.和,
            wordStr = Regex.Replace(wordStr, "[.,']", "", RegexOptions.IgnoreCase);
            wordStr = wordStr.ToLower();
            //獲得原型動詞與單數
            wordStr = helper.getVerbRoot(wordStr);
            wordStr = helper.getSingularNoun(wordStr);
            this.wordStr = wordStr;

            DirectoryInfo dir = new DirectoryInfo($@"{DebugOrReleasePath}\WordData");
            bool haveWord = false;
            foreach (var subDir in dir.GetDirectories())
            {
                if (subDir.Name == wordStr)
                    haveWord = true;
            }
            if (haveWord)
            {
                var DictionarySwitchTabGrid = new DictionarySwitchTabGrid();
                DictionarySwitchTabGrid.SetDictionarySwitchTabControl(wordStr, mainGrid);
            }
            else
            {
                WordBuilder builder = new WordBuilder();
                AddResult status = builder.CreateWord(wordStr);
                if (status == AddResult.SearchFail)
                {
                    MessageBox.Show($"Yahoo查無此單字：{wordStr}\n");
                    Close();
                }
                else
                {
                    var DictionarySwitchTabGrid = new DictionarySwitchTabGrid();
                    DictionarySwitchTabGrid.SetDictionarySwitchTabControl(wordStr, mainGrid);
                }
            }
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            save = true;
            Close();
        }
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            WordBuilder builder = new WordBuilder();
            builder.RemoveWord(wordStr);
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!save)
            {
                WordBuilder builder = new WordBuilder();
                builder.RemoveWord(wordStr);
            }
        }
    }
}
