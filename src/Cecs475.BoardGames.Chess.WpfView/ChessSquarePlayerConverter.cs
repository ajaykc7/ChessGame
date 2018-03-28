using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Cecs475.BoardGames.Chess.WpfView
{
    public class ChessSquarePlayerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int player = (int)value;
            if(player == 0)
            {
                return null;
            }
            return new BitmapImage(new Uri("/Cecs475.BoardGames.Chess.WpfView;component/Resources/BlackKing.png", UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
