using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace DylanDeSouzaSlidingBlockExercise
{
    public partial class MainPage : ContentPage
    {
        MainPageModel mainPageModel;
        public MainPage()
        {
            InitializeComponent();
            mainPageModel = new MainPageModel(imagesGrid);
        }

        void OnSwipe(object sender, SwipedEventArgs e)
        {
            Image image = (Image)sender;
            int row = Grid.GetRow(image);
            int col = Grid.GetColumn(image);
            Debug.WriteLine($"Swipe detected on image at row {row} and column {col} with direction {e.Direction}");

            mainPageModel.SlidePieceManually(row, col);

        }

        void ButtonClicked(object sender, EventArgs e)
        {
            
        }
    }
}
