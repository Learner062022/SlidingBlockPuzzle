using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace DylanDeSouzaSlidingBlockExercise
{
    public class MainPageModel
    {
        Grid _grid;
        ImageInfo[,] _imagesGrid = new ImageInfo[gridSize, gridSize]; // Contain images' correct position
        public List<ImageInfo> _imageInfos = new List<ImageInfo>(); // Contain images' position throughout gameplay
        const int gridSize = 3;
        (int, int) emptyCell = (0, 0);

        public MainPageModel(Grid grid)
        {
            _grid = grid;
            InitializeImages();
            RandomizeBoard();
            Debug.WriteLine($"Game won {CheckWon()}");
        }

        void InitializeImages()
        {
            int imageIndex = 0;
            for (int index = 0; index < gridSize * gridSize; index++)
            {
                int row = index / gridSize;
                int col = index % gridSize;

                Image image;
                if (emptyCell == (row, col))
                {
                    _imagesGrid[row, col] = null; 
                    continue;
                }

                if (imageIndex < _grid.Children.OfType<Image>().Count())
                {
                    image = _grid.Children.OfType<Image>().ElementAt(imageIndex++);
                }
                else
                {
                    continue;  
                }

                var imageInfo = new ImageInfo(image) { Row = row, Column = col };
                _imagesGrid[row, col] = imageInfo;
                _imageInfos.Add(imageInfo);
                image.BindingContext = imageInfo;
            }
        }

        void SwapCoordinates(int row, int col)
        {
            ImageInfo currentImageInfo = _imageInfos.FirstOrDefault(img => img.Row == row && img.Column == col);

            int indexCurrentImageInfo = _imageInfos.IndexOf(currentImageInfo);

            int oldRow = currentImageInfo.Row;
            int oldCol = currentImageInfo.Column;

            // Update the position of the current image to where the empty cell is.
            currentImageInfo.Row = emptyCell.Item1;
            currentImageInfo.Column = emptyCell.Item2;

            // Update the position of the empty cell to the old position of the current image.
            emptyCell = (oldRow, oldCol);

            Debug.WriteLine($"Swapped ({oldRow}, {oldCol}) with empty cell ({emptyCell.Item1}, {emptyCell.Item2}).");
        }

        bool CheckWon()
        {
            bool hasIncorrectPosition = _imageInfos.Any(imageInfo => !imageInfo.PositionIsCorrect);
            return !hasIncorrectPosition;
        }

        void AddOrUpdateSwipeGestureRecognizer(Image image, SwipeDirection swipeDirection)
        {
            var swipeGesture = image.GestureRecognizers.OfType<SwipeGestureRecognizer>().FirstOrDefault();

            if (swipeGesture == null)
            {
                swipeGesture = new SwipeGestureRecognizer();
                image.GestureRecognizers.Add(swipeGesture);
            }

            swipeGesture.Direction = swipeDirection;
        }

        SwipeDirection? DetermineSwipeDirection(int row, int col)
        {
            if (emptyCell.Item1 == row)
            {
                return emptyCell.Item2 > col ? SwipeDirection.Left : SwipeDirection.Right;
            }
            else if (emptyCell.Item2 == col)
            {
                return emptyCell.Item1 > row ? SwipeDirection.Up : SwipeDirection.Down;
            }
            return null;
        }

        void SlidePieceAutonomously(List<ImageInfo> movableImagesInfo)
        {
            Random random = new Random();
            ImageInfo selectedImageInfo = movableImagesInfo[random.Next(movableImagesInfo.Count)];
            AttemptSlidePiece(selectedImageInfo.Row, selectedImageInfo.Column);
        }

        void AttemptSlidePiece(int row, int col)
        {
            Debug.WriteLine($"Attempting to slide piece at {row}, {col}");
            if (row < 0 || col < 0 || row >= gridSize || col >= gridSize)
            {
                Debug.WriteLine("Invalid move attempt due to out-of-bounds.");
                return;
            }

            var targetImageInfo = _imagesGrid[row, col];
            if (targetImageInfo == null)
            {
                Debug.WriteLine("Attempted to slide an empty space or uninitialized cell.");
                return;
            }

            // Check if the target cell can swap with the empty cell.
            if (targetImageInfo.CanSwapWith(emptyCell.Item1, emptyCell.Item2))
            {
                SwapCoordinates(row, col);
                var swipeDirection = DetermineSwipeDirection(row, col);
                Debug.WriteLine($"Swipe direction determined as {swipeDirection}.");

                if (swipeDirection.HasValue)
                {
                    AddOrUpdateSwipeGestureRecognizer(targetImageInfo.Image, swipeDirection.Value);
                    ValidateImagePositions();
                }
            }
            else
            {
                Debug.WriteLine("Move not allowed: target piece is not adjacent to the empty space.");
            }
        }

        public void SlidePieceManually(int row, int col)
        {
            if (_imagesGrid[row, col] == null || CheckWon())
            {
                Debug.WriteLine("Move not allowed or game already won.");
                return;
            }
            AttemptSlidePiece(row, col);
        }

        List<ImageInfo> FindMovableImagesInfo()
        {
            List<ImageInfo> movableImageInfos = new List<ImageInfo>();
            AddIfValid(movableImageInfos, emptyCell.Item1, emptyCell.Item2 - 1); // Left
            AddIfValid(movableImageInfos, emptyCell.Item1, emptyCell.Item2 + 1); // Right
            AddIfValid(movableImageInfos, emptyCell.Item1 - 1, emptyCell.Item2); // Top
            AddIfValid(movableImageInfos, emptyCell.Item1 + 1, emptyCell.Item2); // Bottom
            return movableImageInfos;
        }

        void AddIfValid(List<ImageInfo> list, int row, int col)
        {
            if (row >= 0 && row < gridSize && col >= 0 && col < gridSize)
            {
                var imageInfo = _imageInfos.FirstOrDefault(info => info.Row == row && info.Column == col);
                list.Add(imageInfo);
            }
        }

        void ValidateImagePositions()
        {
            foreach (Image image in _grid.Children.OfType<Image>())
            {
                int row = Grid.GetRow(image);
                int col = Grid.GetColumn(image);
                if (_imagesGrid[row, col] != null)
                {
                    ImageInfo currentImageInfo = _imageInfos.FirstOrDefault(img => img.Row == row && img.Column == col);
                    if (currentImageInfo != null)
                    {
                        currentImageInfo.PositionIsCorrect = (_imagesGrid[row, col].Image == currentImageInfo.Image);
                        Debug.WriteLine($"Position ({row}, {col}) of ({currentImageInfo.Image.Source}) is correct: {currentImageInfo.PositionIsCorrect}");
                    }
                }
            }
        }

        void RandomizeBoard()
        {
            for (int i = 0; i < 100; i++)
            {
                SlidePieceAutonomously(FindMovableImagesInfo());
            }
        }
    }
}
