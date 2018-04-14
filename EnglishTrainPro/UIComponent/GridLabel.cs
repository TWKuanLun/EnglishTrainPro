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
    public enum GridDefinitions
    {
        Column, Row
    }
    class GridLabel
    {
        public GridLabel(string content, double fontSize, Brush foreground, Grid grid, GridDefinitions gridDefinitions, GridLength? gridLength, int index)
        {
            var label = new Label();
            label.Content = content;
            label.FontSize = fontSize;
            if (foreground != null)
                label.Foreground = foreground;
            switch (gridDefinitions)
            {
                case GridDefinitions.Column:
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    if(gridLength != null)
                        grid.ColumnDefinitions[index].Width = (GridLength)gridLength;
                    Grid.SetColumn(label, index);
                    break;
                case GridDefinitions.Row:
                    grid.RowDefinitions.Add(new RowDefinition());
                    if (gridLength != null)
                        grid.RowDefinitions[index].Height = (GridLength)gridLength;
                    Grid.SetRow(label, index);
                    break;
            }
            grid.Children.Add(label);
        }
        public GridLabel(ICollection<string> contents, double fontSize, Brush foreground, Grid grid, GridDefinitions gridDefinitions, GridLength? gridLength, int index)
        {
            foreach (string content in contents)
            {
                var label = new Label();
                label.Content = content;
                label.FontSize = fontSize;
                if (foreground != null)
                    label.Foreground = foreground;
                switch (gridDefinitions)
                {
                    case GridDefinitions.Column:
                        grid.ColumnDefinitions.Add(new ColumnDefinition());
                        if (gridLength != null)
                            grid.ColumnDefinitions[index].Width = (GridLength)gridLength;
                        Grid.SetColumn(label, index);
                        break;
                    case GridDefinitions.Row:
                        grid.RowDefinitions.Add(new RowDefinition());
                        if (gridLength != null)
                            grid.RowDefinitions[index].Height = (GridLength)gridLength;
                        Grid.SetRow(label, index);
                        break;
                }
                grid.Children.Add(label);
                break;
            }
            //var label = new Label();
            //label.Content = content;
            //label.FontSize = fontSize;
            //if (foreground != null)
            //    label.Foreground = foreground;
            //switch (gridDefinitions)
            //{
            //    case GridDefinitions.Column:
            //        grid.ColumnDefinitions.Add(new ColumnDefinition());
            //        if (gridLength != null)
            //            grid.ColumnDefinitions[index].Width = (GridLength)gridLength;
            //        Grid.SetColumn(label, index);
            //        break;
            //    case GridDefinitions.Row:
            //        grid.RowDefinitions.Add(new RowDefinition());
            //        if (gridLength != null)
            //            grid.RowDefinitions[index].Height = (GridLength)gridLength;
            //        Grid.SetRow(label, index);
            //        break;
            //}
            //grid.Children.Add(label);
        }
    }
}
