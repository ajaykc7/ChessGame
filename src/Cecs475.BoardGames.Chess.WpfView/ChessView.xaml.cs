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
                if (vm.PossibleMoves.Contains(new ChessMove(selectedSquare.Position, square.Position)))
                {
                    square.IsHighlighted = true;
                }
            }
            
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            square.IsHighlighted = false;
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
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
                //if the square that is not a possible move is selected, deselect the previous square
                if(selectedSquare != null)
                {
                    selectedSquare.IsSelected = false;
                    if (square.IsHighlighted)
                    {
                        vm.StartBoardPosition = selectedSquare.Position;
                        vm.ApplyMove(square.Position);
                    }
                    selectedSquare = null;
                }
            }
        }

        
    }
}
