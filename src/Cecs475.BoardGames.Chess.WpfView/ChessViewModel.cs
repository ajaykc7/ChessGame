using Cecs475.BoardGames.Chess.Model;
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

        public ChessPieceType ChessPieceType
        {
            get; set;
        }

        private bool mIsSelected;

        /// <summary>
        /// Whether the square is selected by the user
        /// </summary>
        public bool IsSelected
        {
            get { return mIsSelected; }
            set
            {
                if(value != mIsSelected)
                {
                    mIsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
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
    public class ChessViewModel : INotifyPropertyChanged, IGameViewModel
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
                    Player = mBoard.GetPlayerAtPosition(pos),
                    ChessPieceType = mBoard.GetPieceAtPosition(pos).PieceType
                })
            );
            //PossibleMoves = new HashSet<ChessMove>(
            //    from ChessMove m in mBoard.GetPossibleMoves()
            //    select m
            //);

            PossibleStartPositions = new HashSet<BoardPosition>(
                from ChessMove m in mBoard.GetPossibleMoves()
                select m.StartPosition
            );

            PossibleEndPositions = new HashSet<BoardPosition>(
                from ChessMove m in mBoard.GetPossibleMoves()
                select m.EndPosition
            );

            PossibleMoves = new HashSet<ChessMove>(
            from ChessMove m in mBoard.GetPossibleMoves()
            select m
            );
            // PossibleMoves = new HashSet<BoardPosition>(
            //   mBoard.GetPossibleMoves()
            // .Select(m => m.EndPosition));
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
            //PossibleMoves = new HashSet<ChessMove>(
            //  mBoard.GetPossibleMoves()
            //.Select(m => m.EndPosition));
            // PossibleMoves = new HashSet<ChessMove>(
            //    from ChessMove m in mBoard.GetPossibleMoves()
            //   select m
            //);
            PossibleStartPositions = new HashSet<BoardPosition>(
               from ChessMove m in mBoard.GetPossibleMoves()
               select m.StartPosition
            );

            PossibleEndPositions = new HashSet<BoardPosition>(
                from ChessMove m in mBoard.GetPossibleMoves()
                select m.EndPosition
            );

            PossibleMoves = new HashSet<ChessMove>(
            from ChessMove m in mBoard.GetPossibleMoves()
            select m
            );

            //Update the collection of square by examining the new board state
            var newSquares = BoardPosition.GetRectangularPositions(8, 8);
            int i = 0;
            foreach(var pos in newSquares)
            {
                mSquares[i].Player = mBoard.GetPlayerAtPosition(pos);
                i++;
            }
            OnPropertyChanged(nameof(BoardAdvantage));
            OnPropertyChanged(nameof(CurrentPlayer));
            OnPropertyChanged(nameof(CanUndo));
        }

        /// <summary>
		/// A collection of 64 ChessSquare objects representing the state of the 
		/// game board.
		/// </summary>
		public ObservableCollection<ChessSquare> Squares
        {
            get { return mSquares; }
        }

        /// <summary>
		/// A set of chess moves available for players
		/// </summary>
		public HashSet<ChessMove> PossibleMoves
        {
            get; private set;
        }

        
        /// <summary>
		/// A set of board positions where the current player can move.
		/// </summary>
		public HashSet<BoardPosition> PossibleStartPositions
        {
            get; private set;
        }

        /// <summary>
		/// A set of board positions where the current player can move the piece from.
		/// </summary>
		public HashSet<BoardPosition> PossibleEndPositions
        {
            get; private set;
        }

        /// <summary>
		/// The player whose turn it currently is.
		/// </summary>
		public int CurrentPlayer
        {
            get { return mBoard.CurrentPlayer; }
        }

        /// <summary>
		/// The value of the othello board.
		/// </summary>
        public GameAdvantage BoardAdvantage => mBoard.CurrentAdvantage;

        public bool CanUndo => mBoard.MoveHistory.Any();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void UndoMove()
        {
            if (CanUndo)
            {
                mBoard.UndoLastMove();
                RebindState();
            }
        }
    }
    
    
}
