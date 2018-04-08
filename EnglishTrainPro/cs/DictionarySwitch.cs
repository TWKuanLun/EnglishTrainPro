using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace EnglishTrainPro.cs
{
    class DictionarySwitch
    {
        public void SetDictionarySwitchTabControl(Grid grid)
        {
            var tabControl = new TabControl();
            tabControl.Background = Brushes.Black;

            var yahooTabItem = new TabItem();
            yahooTabItem.Header = "Yahoo";
            yahooTabItem.Content = new Grid();
            yahooTabItem.IsSelected = true;

            var CambridgeTabItem = new TabItem();
            CambridgeTabItem.Header = "Cambridge";
            CambridgeTabItem.Content = new Grid();

            tabControl.Items.Add(yahooTabItem);
            tabControl.Items.Add(CambridgeTabItem);

            grid.Children.Add(tabControl);
        }
    }
}
