using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace DylanDeSouzaSlidingBlockExercise
{
    public class MainPageViewModel 
    {
        public MainPageModel MainPageModel { get; }
       
        List<Image> _images;

        public MainPageViewModel(List<Image> images)
        {
            _images = images;
            MainPageModel = new MainPageModel(images);
            InitializeImagesWithGestures(images);
            UpdateUI();
        }

        void UpdateUI()
        {
            Debug.WriteLine("Updating UI...");
            int imageCount = 0;
            foreach (var (image, currentPosition) in MainPageModel.GetCoordinatesWithImages(_images))
            {
                if (image != null)
                {
                    Grid.SetRow(image, currentPosition.Row);
                    Grid.SetColumn(image, currentPosition.Column);
                    Debug.WriteLine($"{image.Source} set to position ({currentPosition.Row}, {currentPosition.Column}).");
                    imageCount++;
                }
                else
                {
                    Debug.WriteLine($"Image not found for position ({currentPosition.Row}, {currentPosition.Column}).");
                }
            }
            Debug.WriteLine($"Total images positioned: {imageCount}");
            Debug.WriteLine("UI updated.");
        }

        public void SlidePieceManually(int row, int col)
        {
            if (!MainPageModel.CanSwapWith(row, col) || MainPageModel.CheckWon)
            {
                Debug.WriteLine($"Move not allowed from ({row}, {col}) to empty cell ({MainPageModel.CoordinatesEmptyCell.Row}, {MainPageModel.CoordinatesEmptyCell.Column})");
                return;
            }

            MainPageModel.AttemptSwapCoordinates(row, col);
            UpdateUI();
        }

        public void ResetBoard()
        {
            Debug.WriteLine("Resetting board...");
            MainPageModel.RandomizeBoard();
            UpdateUI();
        }

        public SwipeDirection? DetermineSwipeDirection(int row, int col)
        {
            var (emptyRow, emptyCol) = MainPageModel.CoordinatesEmptyCell;
            if (emptyRow == row)
            {
                return emptyCol > col ? SwipeDirection.Left : SwipeDirection.Right;
            }
            if (emptyCol == col)
            {
                return emptyRow > row ? SwipeDirection.Up : SwipeDirection.Down;
            }
            return null;
        }

        void InitializeImagesWithGestures(IEnumerable<Image> images)
        {
            Debug.WriteLine("Initializing images with gestures...");
            foreach (var image in images)
            {
                var swipeDirection = DetermineSwipeDirection(Grid.GetRow(image), Grid.GetColumn(image));
                if (swipeDirection.HasValue)
                {
                    AddOrUpdateSwipeGestureRecognizer(image, swipeDirection.Value);
                }
                Debug.WriteLine($"{image.Source} contains {swipeDirection}");
            }
            Debug.WriteLine("Images initialized with gestures.");
        }

        public void AddOrUpdateSwipeGestureRecognizer(Image image, SwipeDirection swipeDirection)
        {
            var swipeGesture = image.GestureRecognizers.OfType<SwipeGestureRecognizer>().FirstOrDefault() ?? new SwipeGestureRecognizer();
            swipeGesture.Direction = swipeDirection;
            if (!image.GestureRecognizers.Contains(swipeGesture))
            {
                image.GestureRecognizers.Add(swipeGesture);
                Debug.WriteLine($"{swipeGesture.Direction} added to {image.Source}.");
            }
        }
    }
}