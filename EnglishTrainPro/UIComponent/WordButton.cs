using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EnglishTrainPro.UIComponent
{
    class WordButton
    {
        public WordButton(string content, double fontSize, Grid grid, int index)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions[index].Width = GridLength.Auto;//給予grid寬度自動，防止按鈕部分遮蓋
            var button = new Button();
            button.Content = content;
            button.BorderThickness = new Thickness(0);//按鈕框線粗細，0=看不到框線
            button.Background = Brushes.Black;
            button.Foreground = Brushes.Pink;
            button.FontSize = fontSize;
            button.Click += Button_Click;
            Grid.SetColumn(button, index);
            grid.Children.Add(button);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string word = b.Content.ToString().Substring(0, b.Content.ToString().Length - 1);//去除空白
            wordExplanationWindow wordWindow = new wordExplanationWindow(word);
            wordWindow.Show();
        }
    }
}
