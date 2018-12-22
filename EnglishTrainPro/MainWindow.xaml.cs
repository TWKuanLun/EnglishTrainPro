using EnglishTrainPro.DataFactory;
using EnglishTrainPro.Display;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace EnglishTrainPro
{
    public partial class MainWindow : Window
    {
        private string DebugOrReleasePath = Directory.GetCurrentDirectory();
        private string Local_OldWord;
        public MainWindow()
        {
            InitializeComponent();
            DirectoryInfo rootDirectory = new DirectoryInfo($"{DebugOrReleasePath}\\WordData");
            rootDirectory.Create();//目錄已存在不作用
            updataList();
            Local_OldWord = string.Empty;

            WordBuilder.Instance().LocalDataChanged += MainWindow_LocalDataChanged;
            Local_WordListBox.SelectionChanged += Local_WordListBox_SelectionChanged;
        }
        #region Window event
        private void MainWindow_LocalDataChanged(object sender, EventArgs e)
        {
            updataList();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D1:
                    tabControl.SelectedIndex = 0;
                    break;
                case Key.D2:
                    tabControl.SelectedIndex = 1;
                    break;
                case Key.D3:
                    tabControl.SelectedIndex = 2;
                    break;
                case Key.D4:
                    tabControl.SelectedIndex = 3;
                    break;
                case Key.D5:
                    tabControl.SelectedIndex = 4;
                    break;
                case Key.F5:
                    //更新
                    break;
                case Key.NumPad1:
                    //右邊數字鍵
                    break;
                case Key.NumPad2:
                    break;
                case Key.NumPad3:
                    break;
            }
        }
        #endregion

        #region Window Function
        private void updataList()//更新listbox
        {
            Local_WordListBox.Items.Clear();
            DirectoryInfo dir = new DirectoryInfo($@"{DebugOrReleasePath}\WordData");
            var searchItem = dir.GetDirectories().Select(x => x.Name).Where(x => x.Contains(Local_SearchTextBox.Text)).OrderByDescending(x => x);
            foreach (string word in searchItem)
            {
                Local_WordListBox.Items.Add(word);
            }
        }
        #endregion

        #region 單字庫
        private void Local_WordListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Local_WordListBox.SelectedValue != null)
            {
                checkWordRemark(Local_OldWord);
                string word = Local_WordListBox.SelectedValue.ToString();
                Local_OldWord = word;
                var gridBuilder = new DictionarySwitchTabGrid();
                gridBuilder.SetDictionarySwitchTabControl(word, Local_WordGrid);
                //remarkTB.Text = Words[word].remark;
                //remarkTB.IsEnabled = true;
            }
            else
            {
                Local_OldWord = string.Empty;
                Local_WordGrid.Children.Clear();
                //remarkTB.Text = "";
                //remarkTB.IsEnabled = false;
            }
        }

        private void Local_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            updataList();
        }
        private void Local_RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                var builder = WordBuilder.Instance();
                builder.RemoveWord(Local_WordListBox.SelectedValue.ToString());
                updataList();
                Local_WordListBox.SelectedIndex = 0;
                Local_OldWord = string.Empty;
            }
            catch (Exception e2)
            {

                MessageBox.Show("錯誤，無選取單字", e2.Message);
            }
        }

        private void checkWordRemark(string word)
        {
            //if (!Local_OldWord.Equals(string.Empty) && !Words[word].remark.Equals(remarkTB.Text))
            //    if (MessageBox.Show("保存筆記/註解/備忘錄?", "您有做了些修改，是否需要保存?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            //    {
            //        seveWordRemark(word, remarkTB.Text);
            //    }
        }
        #endregion

        #region 新增單字
        private async void Download_Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.Content = "Downloading, please wait!";
            button.IsEnabled = false;

            var builder = WordBuilder.Instance();
            builder.ProgressChanged += Builder_ProgressChanged;
            string[] words = Download_TextBox.Text.Split(new char[] { '\n' }); //TextBox文字以\n劃分各單字
            words = words.Select(x => x.Replace("\r","")).Where(x => x != string.Empty).ToArray();
            var message = new StringBuilder();
            var newText = new StringBuilder(); //讓成功新增的單字在Textbox上除去
            var addResult = await Task.Factory.StartNew(() => builder.CreateWords(words));
            for (int i = 0; i < words.Length; i++)
            {
                switch (addResult[i])
                {
                    case AddResult.HaveWord:
                        message.Append($"{words[i]}失敗，已有此單字資料\n");
                        newText.Append($"{words[i]}\r\n");
                        break;
                    case AddResult.SearchFail:
                        if (!words[i].Equals(String.Empty))
                        {
                            message.Append($"{words[i]}失敗，查無此單字\n");
                            newText.Append($"{words[i]}\r\n");
                        }
                        break;
                    default:
                        break;
                }
            }
            if (!message.ToString().Equals(String.Empty))
                MessageBox.Show(message.ToString(), "新增失敗");
            //if (!newText.ToString().Equals(String.Empty))
            Download_TextBox.Text = newText.ToString();

            button.Content = "_Download";
            button.IsEnabled = true;
            updataList();
        }
        private void Builder_ProgressChanged(object sender, EventArgs e)
        {
            var builder = sender as WordBuilder;
            pbStatus.Dispatcher.Invoke(() => pbStatus.Value = builder.Progress, DispatcherPriority.Background);
        }
        #endregion

        #region 單字查詢
        private void Search_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var word = Search_TextBox.Text;
                word = Regex.Replace(word, "[.,']", "", RegexOptions.IgnoreCase).ToLower();
                var helper = new WordHelper();
                word = helper.getVerbRoot(word);
                word = helper.getSingularNoun(word);
                var gridBuilder = new DictionarySwitchTabGrid();
                gridBuilder.SetDictionarySwitchTabControl(word, SearchGrid);
            }
        }

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            var word = Search_TextBox.Text;
            word = Regex.Replace(word, "[.,']", "", RegexOptions.IgnoreCase).ToLower();
            var helper = new WordHelper();
            word = helper.getVerbRoot(word);
            word = helper.getSingularNoun(word);
            var gridBuilder = new DictionarySwitchTabGrid();
            gridBuilder.SetDictionarySwitchTabControl(word, SearchGrid);
        }
        #endregion
    }
}
