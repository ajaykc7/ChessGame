using Cecs475.BoardGames.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Cecs475.BoardGames.Chess.WpfView
{
    class ChessSquareBackgroundConverter : IMultiValueConverter
    {
        private static SolidColorBrush CHECKERED_BRUSH_1 = Brushes.Brown;
        private static SolidColorBrush CHECKERED_BRUSH_2 = Brushes.BurlyWood;
        private static SolidColorBrush SELECTED_SQUARE = Brushes.Red;
        private static SolidColorBrush HOVERED_SQUARE = Brushes.Green;
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // This converter will receive two properties: the Position of the square, and whether it
            // is being hovered.
            BoardPosition pos = (BoardPosition)values[0];

            bool isSelected = (bool)values[1];
            bool isHighlighted = (bool)values[2];

            //Generate background colors that alternate
            
            if (isSelected)
            {
                return SELECTED_SQUARE;
            }

            
            if (pos.Row % 2 == 0)
            {
                return (pos.Col % 2 == 0) ? CHECKERED_BRUSH_1 : CHECKERED_BRUSH_2;
            }
            else
            {
                return (pos.Col % 2 == 0) ? CHECKERED_BRUSH_2 : CHECKERED_BRUSH_1;
            }
            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
