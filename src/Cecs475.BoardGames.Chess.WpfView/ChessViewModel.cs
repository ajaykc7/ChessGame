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
using Cecs475.BoardGames.ComputerOpponent;
using System.Diagnostics;
using System.Windows;

namespace Cecs475.BoardGames.Chess.WpfView
{
    /// <summary>
    /// Represents a square on the Chess board grid
    /// </summary>
    public class ChessSquare : INotifyPropertyChanged
    {
        private int mPlayer;
        private ChessPiece mChessPiece;

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

        /// <summary>
        /// The chess piece at a given square
        /// </summary>
        public ChessPiece ChessPiece
        {
            get
            {
                return mChessPiece;
            }
            set
            {
                if(!value.Equals(mChessPiece))
                {
                    mChessPiece = value;
                    OnPropertyChanged(nameof(ChessPiece));
                }
            }
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

        private bool mIsKingInCheck;
        public bool IsKingInCheck
        {
            get
            {
                return mIsKingInCheck;
            }
            set
            {
                if(value != mIsKingInCheck)
                {
                    mIsKingInCheck = value;
                    OnPropertyChanged(nameof(IsKingInCheck));
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
        private ObservableCollection<ChessSquare> promotionSquares;
        private ChessPieceType mPromotedPiece = ChessPieceType.Empty;
        public event EventHandler GameFinished;
        private const int MAX_AI_DEPTH = 4;
        private IGameAi mGameAi = new MinimaxAi(MAX_AI_DEPTH);

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
                    ChessPiece = mBoard.GetPieceAtPosition(pos)
                })
            );

            //Initialize squares to use for PromotionWindow
            promotionSquares = new ObservableCollection<ChessSquare>();
            var pieceType = new ObservableCollection<ChessPieceType>();
            pieceType.Add(ChessPieceType.Rook);
            pieceType.Add(ChessPieceType.Knight);
            pieceType.Add(ChessPieceType.Bishop);
            pieceType.Add(ChessPieceType.Queen);
            for (int i = 0; i < 4; i++)
            {
                promotionSquares.Add(new ChessSquare()
                {
                    Position = new BoardPosition(0, i),
                    Player = CurrentPlayer,
                    ChessPiece = new ChessPiece(pieceType[i], CurrentPlayer),
                });
            }

            //mPromotedPiece = ;

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

        public ChessPiece GetPieceAtPosition(BoardPosition pos)
        {
            return mBoard.GetPieceAtPosition(pos);
        }

        /// <summary>
        /// Applies a move for the current player at the given position
        /// </summary>
        /// <param name="position"> The position the piece is moved to</param>
        public async Task ApplyMove(BoardPosition position)
        {
            var possMoves = mBoard.GetPossibleMoves() as IEnumerable<ChessMove>;
            //Make sure the move is valid
            foreach(var move in possMoves)
            {
                if ((move.StartPosition.Equals(StartBoardPosition))&&(move.EndPosition.Equals(position)))
                {
                    //if ((move.EndPosition.Row==0)||(move.EndPosition.Row==7)
                    //    &&(GetPieceAtPosition(move.StartPosition).PieceType==ChessPieceType.Pawn))
                    //{
                    if (!PromotedPiece.Equals(ChessPieceType.Empty))
                    {
                        if(move.PromoteTo == PromotedPiece)
                        {
                            mBoard.ApplyMove(move);
                            break;
                        }
                    }
                    //}
                    else
                    {
                        mBoard.ApplyMove(move);
                        break;
                    }
                    
                }
            }

            RebindState();

            if (Players == NumberOfPlayers.One && !mBoard.IsFinished)
            {
                var bestMoveTask = Task.Run(() =>mGameAi.FindBestMove(mBoard));

                var bestMove = await bestMoveTask;

                if (bestMove != null)
                {
                    mBoard.ApplyMove(bestMove as ChessMove);
                }
                RebindState();
            }


            MessageBox.Show(mBoard.BoardWeight.ToString());

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
                mSquares[i].ChessPiece = mBoard.GetPieceAtPosition(pos);
                if ((mSquares[i].ChessPiece.PieceType == ChessPieceType.King)&&(mBoard.IsCheck)&& (mSquares[i].ChessPiece.Player == CurrentPlayer))
                {
                    //if (mSquares[i].ChessPiece.Player == CurrentPlayer)
                    //{
                        mSquares[i].IsKingInCheck = true;
                    //}
                    
                    
                }
                else
                {
                    mSquares[i].IsKingInCheck = false;
                }
                i++;
            }
            //Update promotionSquares to the Current Player
            for(int j = 0; j < promotionSquares.Count; j++)
            {
                promotionSquares[j].Player = CurrentPlayer;
                promotionSquares[j].ChessPiece = new ChessPiece(promotionSquares[j].ChessPiece.PieceType, CurrentPlayer);
            }

            PromotedPiece = ChessPieceType.Empty;
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

        public ObservableCollection<ChessSquare> PromoteSquares
        {
            get { return promotionSquares; }
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

        public BoardPosition StartBoardPosition
        {
            get; set;
        }

        public ChessPieceType PromotedPiece
        {
            get
            {
               return mPromotedPiece;
            }
            set
            {
                if(value!= mPromotedPiece)
                {
                    mPromotedPiece = value;
                    OnPropertyChanged(nameof(PromotedPiece));
                }
            }
            
        }

        /// <summary>
		/// The value of the chess board.
		/// </summary>
        public GameAdvantage BoardAdvantage => mBoard.CurrentAdvantage;

        public bool CanUndo => mBoard.MoveHistory.Any();

        public NumberOfPlayers Players { get; set; }

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
                // In one-player mode, Undo has to remove an additional move to return to the
                // human player's turn.
                if (Players == NumberOfPlayers.One && CanUndo)
                {
                    mBoard.UndoLastMove();
                }
                RebindState();
            }
        }
    }
    
    
}
