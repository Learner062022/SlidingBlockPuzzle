using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace DylanDeSouzaSlidingBlockExercise
{
    public partial class MainPage : ContentPage
    {
        MainPageViewModel _mainPageViewModel;
        public MainPage()
        {
            InitializeComponent();
            var images = imagesGrid.Children.OfType<Image>().ToList();
            Debug.WriteLine($"Found {images.Count} images in grid.");
            _mainPageViewModel = new MainPageViewModel(images);
            BindingContext = _mainPageViewModel;
        }

        void OnSwipe(object sender, SwipedEventArgs e)
        {
            Image image = (Image)sender;
            int row = Grid.GetRow(image);
            int col = Grid.GetColumn(image);
            Debug.WriteLine($"Swipe detected on image at row {row} and column {col} with direction {e.Direction}");
            _mainPageViewModel.SlidePieceManually(row, col);
        }

        void ButtonClicked(object sender, EventArgs e)
        {
            _mainPageViewModel.ResetBoard();
        }
    }
}