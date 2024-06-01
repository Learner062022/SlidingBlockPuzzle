using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Xamarin.Forms;

namespace DylanDeSouzaSlidingBlockExercise
{
    public class MainPageModel
    {
        public const int GridSize = 3;
        (int Row, int Column)?[,] _currentCoordinates;
        (int Row, int Column)?[,] _correctCoordinates;
        public static int _timesShuffle = 100;
        int _numberImagesPlacedCorrectly = 0;

        public (int Row, int Column) CoordinatesEmptyCell { get; private set; }

        public static int TimesShuffled 
        {
            get
            {
                return Math.Abs(100 - _timesShuffle);
            }
        }

        public int NumberImagesPlacedCorrectly
        {
            get
            {
                _numberImagesPlacedCorrectly = 0;
                for (int row = 0; row < GridSize; row++)
                {
                    for (int col = 0; col < GridSize; col++)
                    {
                        if (_currentCoordinates[row, col] == _correctCoordinates[row, col])
                        {
                            _numberImagesPlacedCorrectly++;
                        }
                        else
                        {
                            _numberImagesPlacedCorrectly--;
                        }
                    }
                }
                return _numberImagesPlacedCorrectly;
            }
        }
        
        public MainPageModel(List<Image> images)
        {
            _currentCoordinates = new (int, int)?[GridSize, GridSize];
            _correctCoordinates = new (int, int)?[GridSize, GridSize];
            CoordinatesEmptyCell = (0, 0);
            InitializeImages(images);
            RandomizeBoard();
        }

        public bool CanSwapWith(int targetRow, int targetCol)
        {
            var (emptyRow, emptyCol) = CoordinatesEmptyCell;
            return Math.Abs(targetRow - emptyRow) + Math.Abs(targetCol - emptyCol) == 1;
        }

        public void InitializeImages(List<Image> images)
        {
            int imageIndex = 0;
            Debug.WriteLine("Initializing images...");

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (CoordinatesEmptyCell == (row, col))
                    {
                        _correctCoordinates[row, col] = null;
                        _currentCoordinates[row, col] = null;
                        Debug.WriteLine($"Empty cell set at ({row}, {col}).");
                        continue;
                    }
                    if (imageIndex < images.Count)
                    {  
                        _correctCoordinates[row, col] = (row, col);
                        _currentCoordinates[row, col] = (row, col);
                        Debug.WriteLine($"Image {images[imageIndex].Source} set at ({row}, {col}).");
                        imageIndex++;
                    }
                    else
                    {
                        Debug.WriteLine($"No image found for position ({row}, {col})");
                    }
                }
            }
            Debug.WriteLine("Images initialized.");
        }

        public void SwapCoordinates(int row, int col)
        {
            if (!CanSwapWith(row, col))
            {
                Debug.WriteLine($"{TimesShuffled}: Invalid move: ({row}, {col}) cannot swap with empty cell ({CoordinatesEmptyCell.Row}, {CoordinatesEmptyCell.Column})");
                return;
            }

            var emptyCell = CoordinatesEmptyCell;
            var current = _currentCoordinates[row, col];
            _currentCoordinates[row, col] = null;
            _currentCoordinates[CoordinatesEmptyCell.Row, CoordinatesEmptyCell.Column] = current;
            CoordinatesEmptyCell = (row, col);
            Debug.WriteLine($"{TimesShuffled}: Swapped empty cell from ({emptyCell.Row}, {emptyCell.Column}) to ({row}, {col})");
        }

        public bool CheckWon
        {
            get
            {
                return NumberImagesPlacedCorrectly == (GridSize * GridSize) - 1;
            }
        }

        public void AttemptSwapCoordinates(int row, int col)
        {
            if (CanSwapWith(row, col))
            {
                SwapCoordinates(row, col);
            }
        }

        public List<(int, int)> FindMovableImagesCoordinates()
        {
            var (emptyRow, emptyCol) = CoordinatesEmptyCell;
            return new List<(int, int)>
            {
                (emptyRow, emptyCol - 1),
                (emptyRow, emptyCol + 1),
                (emptyRow - 1, emptyCol),
                (emptyRow + 1, emptyCol)
            }.Where(IsWithinBounds).ToList();
        }

        bool IsWithinBounds((int Row, int Column) cell) => cell.Row >= 0 && cell.Row < GridSize && cell.Column >= 0 && cell.Column < GridSize;

        void PrintCurrentCoordinates()
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    var coord = _currentCoordinates[row, col];
                    if (coord.HasValue)
                    {
                        Debug.Write($"({coord.Value.Row}, {coord.Value.Column}) ");
                    }
                    else
                    {
                        Debug.Write("(empty) ");
                    }
                }
                Debug.WriteLine("");
            }
        }

        public void RandomizeBoard()
        {
            Debug.WriteLine("Randomizing board...");
            var random = new Random();
            while (_timesShuffle > 0)
            {
                var movableImagesInfo = FindMovableImagesCoordinates();
                if (movableImagesInfo.Any())
                {
                    var selectedImageInfo = movableImagesInfo[random.Next(movableImagesInfo.Count)];
                    SwapCoordinates(selectedImageInfo.Item1, selectedImageInfo.Item2);
                }
                _timesShuffle -= 1;
                Debug.WriteLine($"Board randomized. Current state:");
                PrintCurrentCoordinates();
            }
        }

        public IEnumerable<(Image OriginalPosition, (int Row, int Column) CurrentPosition)> GetCoordinatesWithImages(List<Image> images)
        {
            var coordinatesWithImages = new List<(Image, (int Row, int Column))>();

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    var currentPosition = _currentCoordinates[row, col];
                    if (currentPosition.HasValue)
                    {
                        var image = images.FirstOrDefault(img =>
                        {
                            var imgRow = Grid.GetRow(img);
                            var imgCol = Grid.GetColumn(img);
                            return (imgRow, imgCol) == currentPosition.Value;
                        });
                        if (image != null)
                        {
                            coordinatesWithImages.Add((image, (row, col)));
                        }
                        else
                        {
                            Debug.WriteLine($"Image not found for current position ({currentPosition.Value.Row}, {currentPosition.Value.Column})");
                        }
                    }
                }
            }
            return coordinatesWithImages;
        }
    }
}