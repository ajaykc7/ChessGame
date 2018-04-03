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
    public class ChessSquarePlayerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ChessPiece piece = (ChessPiece)value;
            int player = piece.Player;
            ChessPieceType pieceType = piece.PieceType;
            string uriString = "/Cecs475.BoardGames.Chess.WpfView;component/Resources/";

            if (player == 1)
            {
                switch (pieceType)
                {
                    case ChessPieceType.Bishop:
                        uriString += "WhiteBishop.png";
                        break;
                    case ChessPieceType.King:
                        uriString += "WhiteKing.png";
                        break;
                    case ChessPieceType.Knight:
                        uriString += "WhiteKnight.png";
                        break;
                    case ChessPieceType.Pawn:
                        uriString += "WhitePawn.png";
                        break;
                    case ChessPieceType.Queen:
                        uriString += "WhiteQueen.png";
                        break;
                    case ChessPieceType.Rook:
                        uriString += "WhiteRook.png";
                        break;
                }
            }
            else if (player == 2)
            {
                switch (pieceType)
                {
                    case ChessPieceType.Bishop:
                        uriString += "BlackBishop.png";
                        break;
                    case ChessPieceType.King:
                        uriString += "BlackKing.png";
                        break;
                    case ChessPieceType.Knight:
                        uriString += "BlackKnight.png";
                        break;
                    case ChessPieceType.Pawn:
                        uriString += "BlackPawn.png";
                        break;
                    case ChessPieceType.Queen:
                        uriString += "BlackQueen.png";
                        break;
                    case ChessPieceType.Rook:
                        uriString += "BlackRook.png";
                        break;
                }
            }
            else
            {
                return null;
            }
            /* String fileLocation = "D:/GitHub Repo/project2-anotherrandomteamname/src/Cecs475.BoardGames.Chess.WpfView/Resources/";
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



             Uri mUri = new Uri(fileLocation, UriKind.RelativeOrAbsolute);
             PngBitmapDecoder decoder = new PngBitmapDecoder(mUri, BitmapCreateOptions.None, BitmapCacheOption.Default);
             BitmapSource bitmapSource = decoder.Frames[0];

             Image token = new Image();
             token.Source = bitmapSource;
             return token;
             */
            return new Image()
            {
                Source = new BitmapImage(new Uri(uriString, UriKind.Relative))
            };
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
