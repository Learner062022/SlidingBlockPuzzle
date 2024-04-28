using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace DylanDeSouzaSlidingBlockExercise
{
    public class ImageInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int row;
        private int column;
        public Image Image { get; set; }
        public int Row
        {
            get => row;
            set
            {
                if (row != value)
                {
                    row = value;
                    OnPropertyChanged(nameof(Row));
                }
            }
        }
        public int Column
        {
            get => column;
            set
            {
                if (column != value)
                {
                    column = value;
                    OnPropertyChanged(nameof(Column));
                }
            }
        }
        public bool PositionIsCorrect { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ImageInfo(Image image)
        {
            Image = image;
            Binding rowBinding = new Binding(nameof(Row)) { Source = this };
            Binding colBinding = new Binding(nameof(Column)) { Source = this };
            image.SetBinding(Grid.RowProperty, rowBinding);
            image.SetBinding(Grid.ColumnProperty, colBinding);
        }

        public bool CanSwapWith(int targetRow, int targetCol) => Math.Abs(row - targetRow) == 1 && column == targetCol ||
                   Math.Abs(column - targetCol) == 1 && row == targetRow; 
    }
}
