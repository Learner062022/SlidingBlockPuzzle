using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DylanDeSouzaSlidingBlockExercise
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            // RandomiseBoard
        }

        public int GetColumnEmptyBlock()
        {
            int columnNum = 0;
            return columnNum;
        }

        public int GetRowEmptyBlock()
        {
            int RowNum = 0;
            return RowNum;
        }

        private void OnSwipe(object sender, SwipedEventArgs e)
        {
            // use sender to find the column and row of the image being swiped 
            Game.CheckWon();
            Game.SlidePiece(/* column and row of the image being swiped */);
        }

        private void RandomiseBoard()
        {

        }

        private void ResetAndRandomiseBoard(object sender, EventArgs e)
        {
            // RandomiseBoard
            Game.won = false;
        }
    }
}
