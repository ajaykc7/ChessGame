using Cecs475.BoardGames.Chess.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Cecs475.BoardGames.Chess.WpfView
{
    public class ChessSquarePlayerConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            String fileLocation = "D:/GitHub Repo/project2-anotherrandomteamname/src/Cecs475.BoardGames.Chess.WpfView/Resources/";
            int player = (int)values[0];
            ChessPieceType piece = (ChessPieceType)values[1];

            if (player == 1)
            {
                switch (piece)
                {
                    case ChessPieceType.Bishop:
                        fileLocation += "WhiteBishop.png";
                        break;
                    case ChessPieceType.King:
                        fileLocation += "WhiteKing.png";
                        break;
                    case ChessPieceType.Knight:
                        fileLocation += "WhiteKnight.png";
                        break;
                    case ChessPieceType.Pawn:
                        fileLocation += "WhitePawn.png";
                        break;
                    case ChessPieceType.Queen:
                        fileLocation += "WhiteQueen.png";
                        break;
                    case ChessPieceType.Rook:
                        fileLocation += "WhiteRook.png";
                        break;
                }
            }else if(player == 2)
            {
                switch (piece)
                {
                    case ChessPieceType.Bishop:
                        fileLocation += "BlackBishop.png";
                        break;
                    case ChessPieceType.King:
                        fileLocation += "BlackKing.png";
                        break;
                    case ChessPieceType.Knight:
                        fileLocation += "BlackKnight.png";
                        break;
                    case ChessPieceType.Pawn:
                        fileLocation += "BlackPawn.png";
                        break;
                    case ChessPieceType.Queen:
                        fileLocation += "BlackQueen.png";
                        break;
                    case ChessPieceType.Rook:
                        fileLocation += "BlackRook.png";
                        break;
                }
            }
            else
            {
                return null;
            }
            //return new BitmapImage(new Uri("/Cecs475.BoardGames.Chess.WpfView/Resources/BlackKing.png", UriKind.Relative));
           
            Uri mUri = new Uri(fileLocation, UriKind.RelativeOrAbsolute);
            PngBitmapDecoder decoder = new PngBitmapDecoder(mUri, BitmapCreateOptions.None, BitmapCacheOption.Default);
            BitmapSource bitmapSource = decoder.Frames[0];

            Image token = new Image();
            token.Source = bitmapSource;
            return token;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
