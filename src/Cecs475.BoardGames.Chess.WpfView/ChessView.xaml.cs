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
            if (vm.PossibleEndPositions.Contains(square.Position))
            {
                square.IsHighlighted = true;
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
            if (vm.PossibleStartPositions.Contains(square.Position))
            {
                if(selectedSquare == null)
                {
                    square.IsSelected = true;
                    selectedSquare = square;

                }
                else
                {
                    if (selectedSquare == square)
                    {
                        square.IsSelected = false;
                        selectedSquare = null;
                    }
                    else
                    {
                        selectedSquare.IsSelected = false;
                        square.IsSelected = true;
                        selectedSquare = square;
                    }
                }
                
                
                //square.IsSelected = (square.IsSelected) ? false:true;
            }
        }

        
    }
}
