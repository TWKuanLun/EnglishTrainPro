using EnglishTrainPro.DataFactory;
using EnglishTrainPro.Display;
using System;
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
        private string PublishPath = Directory.GetCurrentDirectory();
        private string wordStr;
        bool shown;
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

            var dictionarySwitchTabGrid = new DictionarySwitchTabGrid();
            shown = dictionarySwitchTabGrid.SetDictionarySwitchTabControl(wordStr, mainGrid);
        }

        protected override void OnContentRendered(System.EventArgs e)
        {
            if (!shown)
                Close();
            base.OnContentRendered(e);
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            var builder = WordBuilder.Instance();
            AddResult status = builder.CreateWord(wordStr);
            if (status == AddResult.SearchFail)
            {
                MessageBox.Show($"{wordStr}新增失敗，請再試一次。");
            }
            else
            {
                Close();
            }
        }
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Windows_Closed(object sender, System.EventArgs e)
        {
        }

        private void Content_Rendered(object sender, System.EventArgs e)
        {

        }
    }
}
