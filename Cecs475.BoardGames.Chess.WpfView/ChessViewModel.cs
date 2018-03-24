﻿using Cecs475.BoardGames.Chess.Model;
using Cecs475.BoardGames.Model;
using Cecs475.BoardGames.WpfView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cecs475.BoardGames.Chess.WpfView
{
    /// <summary>
    /// Represents a square on the Chess board grid
    /// </summary>
    public class ChessSquare : INotifyPropertyChanged
    {
        private int mPlayer;

        /// <summary>
        /// The player that has a piece in the given sqare, or 0 if empty.
        /// </summary>
        public int Player
        {
            get { return mPlayer; }
            set
            {
                if(value != mPlayer)
                {
                    mPlayer = value;
                    OnPropertyChanged(nameof(Player));
                }
            }
        }

        /// <summary>
        /// The position of the square
        /// </summary>
        public BoardPosition Position
        {
            get; set;
        }

        private bool mIsHighlighted;
        /// <summary>
        /// Whether the square should be highlighted because of a user action.
        /// </summary>
        public bool IsHighlighted
        {
            get { return mIsHighlighted; }
            set
            {
                if (value != mIsHighlighted)
                {
                    mIsHighlighted = value;
                    OnPropertyChanged(nameof(IsHighlighted));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Represents the game state of a single chess game
    /// </summary>
    class ChessViewModel : INotifyPropertyChanged, IGameViewModel
    {
        private ChessBoard mBoard;
        private ObservableCollection<ChessSquare> mSquares;
        public event EventHandler GameFinished;

        public ChessViewModel()
        {
            mBoard = new ChessBoard();

            //Initialize the square objects based on the board's initial state
            mSquares = new ObservableCollection<ChessSquare>(
                BoardPosition.GetRectangularPositions(8, 8)
                .Select(pos => new ChessSquare()
                {
                    Position = pos,
                    Player = mBoard.GetPlayerAtPosition(pos)
                })
            );

            PossibleMoves = new HashSet<BoardPosition>(
                mBoard.GetPossibleMoves()
                .Select(m => m.EndPosition));
        }

        /// <summary>
        /// Applies a move for the current player at the given position
        /// </summary>
        /// <param name="position"> The position the piece is moved to</param>
        public void ApplyMove(BoardPosition position)
        {
            var possMoves = mBoard.GetPossibleMoves() as IEnumerable<ChessMove>;
            //Make sure the move is valid
            foreach(var move in possMoves)
            {
                if (move.EndPosition.Equals(position))
                {
                    mBoard.ApplyMove(move);
                    break;
                }
            }

            RebindState();

            if (mBoard.IsFinished)
            {
                GameFinished?.Invoke(this, new EventArgs());
            }
        }

        private void RebindState()
        {
            //Rebind the possible moves, now that the board has changed.
            PossibleMoves = new HashSet<BoardPosition>(
                mBoard.GetPossibleMoves()
                .Select(m => m.EndPosition));

            //Update the collection of square by examining the new board state
            var newSquares = BoardPosition.GetRectangularPositions(8, 8);
            int i = 0;
            foreach(var pos in newSquares)
            {
                mSquares[i].Player = mBoard.GetPlayerAtPosition(pos);
                i++;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
    {
    }
}