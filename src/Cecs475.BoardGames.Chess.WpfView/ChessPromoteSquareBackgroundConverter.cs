using Cecs475.BoardGames.Chess.Model;
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
    class ChessPromoteSquareBackgroundConverter : IValueConverter
    {
        private static SolidColorBrush HOVERED_BRUSH = Brushes.Green;
        private static SolidColorBrush DEFAULT_BRUSH = Brushes.BurlyWood;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isHighlighted = (bool)value;
            return isHighlighted ? HOVERED_BRUSH : DEFAULT_BRUSH;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
