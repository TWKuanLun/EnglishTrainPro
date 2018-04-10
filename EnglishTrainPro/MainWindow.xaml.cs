using EnglishTrainPro.cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace EnglishTrainPro
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            var yahooGrid = new Grid();
            YahooWordGrid test = new YahooWordGrid("architecture", yahooGrid);
            DictionarySwitch ds = new DictionarySwitch();
            ds.SetDictionarySwitchTabControl(SearchGrid, yahooGrid, new Grid());
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

        private void Local_Search_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void Local_RemoveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Download_Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.Content = "Downloading, please wait!";
            button.IsEnabled = false;

            var builder = new WordBuilder();
            builder.ProgressChanged += Builder_ProgressChanged;
            string[] words = Download_TextBox.Text.Split(new char[] { '\n' }); //TextBox文字以\n劃分各單字
            words = words.Select(x => x.Replace("\r","")).Where(x => x != string.Empty).ToArray();
            var message = new StringBuilder();
            var newText = new StringBuilder(); //讓成功新增的單字在Textbox上除去
            var addResult = await builder.TaskCreateWords(words);
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
        }

        private void Builder_ProgressChanged(object sender, EventArgs e)
        {
            var builder = sender as WordBuilder;
            pbStatus.Dispatcher.Invoke(() => pbStatus.Value = builder.Progress, DispatcherPriority.Background);
        }

        private void Search_TextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
