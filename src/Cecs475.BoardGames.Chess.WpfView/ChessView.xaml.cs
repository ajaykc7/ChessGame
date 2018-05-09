using Cecs475.BoardGames.Chess.Model;
using Cecs475.BoardGames.WpfView;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cecs475.BoardGames.Chess.WpfView
{
    /// <summary>
    /// Interaction logic for ChessView.xaml
    /// </summary>
    public partial class ChessView : UserControl, IWpfGameView
    {
        public ChessSquare selectedSquare;
        public ChessView()
        {
            InitializeComponent();
        }

        public ChessViewModel ChessViewModel => FindResource("vm") as ChessViewModel;

        public Control ViewControl => this;

        public IGameViewModel ViewModel => ChessViewModel;

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            var vm = FindResource("vm") as ChessViewModel;

            //if no square is selected
            if (selectedSquare == null)
            {
                //if hovering over a piece that has a possible move, square is highlighted
                square.IsHighlighted = vm.PossibleStartPositions.Contains(square.Position) ? true : false;
            }
            else
            {
                //only highlight squares that are possible endPosition for selected piece
                ChessPieceType pieceType = vm.GetPieceAtPosition(selectedSquare.Position).PieceType;

                switch(pieceType){
                    case (ChessPieceType.Pawn):
                        ChessMove promote1 = new ChessMove(selectedSquare.Position, square.Position, ChessMoveType.PawnPromote);
                        promote1.PromoteTo = ChessPieceType.Queen;
                        if ((vm.PossibleMoves.Contains(new ChessMove(selectedSquare.Position, square.Position,ChessMoveType.Normal)))
                            || (vm.PossibleMoves.Contains(new ChessMove(selectedSquare.Position, square.Position, ChessMoveType.EnPassant)))
                            || (vm.PossibleMoves.Contains(promote1)))
                        {
                            square.IsHighlighted = true;
                        }
                        break;
                    case (ChessPieceType.King):
                        if ((vm.PossibleMoves.Contains(new ChessMove(selectedSquare.Position, square.Position, ChessMoveType.Normal)))
                            || (vm.PossibleMoves.Contains(new ChessMove(selectedSquare.Position, square.Position, ChessMoveType.CastleKingSide)))
                            || (vm.PossibleMoves.Contains(new ChessMove(selectedSquare.Position, square.Position, ChessMoveType.CastleQueenSide))))
                        {
                            square.IsHighlighted = true;
                        }
                        break;
                    default:
                        if (vm.PossibleMoves.Contains(new ChessMove(selectedSquare.Position, square.Position)))
                        {
                            square.IsHighlighted = true;
                        }
                        break;
                }
                /*if (vm.PossibleMoves.Contains(new ChessMove(selectedSquare.Position, square.Position)))
                {
                    square.IsHighlighted = true;
                }*/
            }
            
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            square.IsHighlighted = false;
        }

        private async void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            var vm = FindResource("vm") as ChessViewModel;

            //only select squares that are possible start positions from all possible moves
            if (vm.PossibleStartPositions.Contains(square.Position))
            {
                //if no square has been selected currently, select the clicked square
                if(selectedSquare == null)
                {
                    square.IsSelected = true;
                    selectedSquare = square;

                }
                //if a square is already select, determine if the same square or a different square is clicked
                else
                {
                    //if the same square is clicked, deselect the square
                    if (selectedSquare == square)
                    {
                        square.IsSelected = false;
                        selectedSquare = null;
                    }
                    //if a different square is clicked, delselect the previous square and select the new one
                    else
                    {
                        //Fix Applied: Need to deselect before changing selectedSquare
                        selectedSquare.IsSelected = false;
                        //if (square.IsHighlighted)
                        //{
                        //    vm.ApplyMove(square.Position);
                        //    selectedSquare = null;
                        //}
                        //else
                        //{
                            square.IsSelected = true;
                            selectedSquare = square;
                        //}
                    }
                }
                //square.IsSelected = (square.IsSelected) ? false:true;
            }
            else
            {
                //if the square that is not a starting position of possible move is selected, deselect the previous square
                if(selectedSquare != null)
                {
                    selectedSquare.IsSelected = false;
                    //True if square is ending position of possible move
                    if (square.IsHighlighted)
                    {
                        vm.StartBoardPosition = selectedSquare.Position;
                        //Check if move is pawn promotion
                        if (vm.GetPieceAtPosition(vm.StartBoardPosition).PieceType == ChessPieceType.Pawn
                            && ((vm.CurrentPlayer == 1 && vm.StartBoardPosition.Row == 1) ||
                            (vm.CurrentPlayer == 2 && vm.StartBoardPosition.Row == 6)))
                        {
                            PromotionWindow promoteWin = new PromotionWindow(vm, vm.StartBoardPosition, square.Position);
                            promoteWin.ShowDialog();
                            //ChessPieceType checker = vm.PromotedPiece;
                        }
                        //else
                        //{

                        this.IsEnabled = false;
                        //Apply Move needs to have an extra param to take PieceType for the pawn to promote to
                        await vm.ApplyMove(square.Position);
                        this.IsEnabled = true;
                        //}
                    }
                    selectedSquare = null;
                }
            }
        }

        
    }
}
