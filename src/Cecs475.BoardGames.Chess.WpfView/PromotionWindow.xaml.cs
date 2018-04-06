using Cecs475.BoardGames.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cecs475.BoardGames.Chess.WpfView
{
    /// <summary>
    /// Interaction logic for PromotionWindow.xaml
    /// </summary>
    /// Not sure if this needs to implement IWpfGameView
    public partial class PromotionWindow : Window
    {
        public ChessViewModel ChessViewModel;
        public BoardPosition StartPos;
        public BoardPosition EndPos;
        public PromotionWindow(ChessViewModel vm, BoardPosition start, BoardPosition end)
        {
            ChessViewModel = vm;
            StartPos = start;
            EndPos = end;
            InitializeComponent();
            DataContext = ChessViewModel;
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            square.IsHighlighted = true;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            square.IsHighlighted = false;
        }

        private void Border_MouseUp(object sender, MouseEventArgs e)
        {
            //Close window & return ChessPiece
        }
    }
    
}
