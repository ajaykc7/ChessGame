using System;
using System.Collections.Generic;
using System.Text;
using Cecs475.BoardGames.Model;
using System.Linq;

namespace Cecs475.BoardGames.Chess.Model
{
    /// <summary>
    /// Represents the board state of a game of chess. Tracks which squares of the 8x8 board are occupied
    /// by which player's pieces.
    /// </summary>
    public class ChessBoard : IGameBoard
    {
        #region Member fields.
        // The history of moves applied to the board.
        public List<ChessMove> mMoveHistory = new List<ChessMove>();

        public int checker;
        public const int BoardSize = 8;

        //black pieces represented in bitboard
        private ulong blackRooks = 0b10000001_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
        private ulong blackKnights = 0b01000010_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
        private ulong blackBishops = 0b00100100_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
        private ulong blackKing = 0b00001000_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
        private ulong blackQueen = 0b00010000_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
        private ulong blackPawns = 0b00000000_11111111_00000000_00000000_00000000_00000000_00000000_00000000;

        //white pieces represented in bitboard
        private ulong whiteRooks = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_10000001;
        private ulong whiteKnights = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_01000010;
        private ulong whiteBishops = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00100100;
        private ulong whiteKing = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00001000;
        private ulong whiteQueen = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00010000;
        private ulong whitePawns = 0b00000000_00000000_00000000_00000000_00000000_00000000_11111111_00000000;


        // TODO: Add a means of tracking miscellaneous board state, like captured pieces and the 50-move rule.

        private bool advantageCalculateFlag = false;
        //List of captured pieces
        public List<ChessPiece> mCapturedPieces = new List<ChessPiece>();

        //int to count the number of non-capture or non-pawn moves.
        private int mDrawCounter = 0;
        public List<int> DrawHistory = new List<int>();

        // TODO: add a field for tracking the current player and the board advantage.		
        private int mCurrentPlayer = 1;

        //Represents the current advantage based on pieces on the board for a player
        private GameAdvantage mAdvantage = new GameAdvantage(0, 0);

        //Store initial positions of pawns
        public IEnumerable<BoardPosition> initialWhitePawn = new List<BoardPosition>();
        public IEnumerable<BoardPosition> initialBlackPawn = new List<BoardPosition>();

        //Check if these pieces moved, used to check for castling
        //For now, index 0 is left rook and index 1 is right rook
        public List<bool> hasWhiteRookMoved = new List<bool>();
        public List<bool> hasBlackRookMoved = new List<bool>();
        private const int left = 0;
        private const int right = 1;
        public bool hasWhiteKingMoved = false;
        public bool hasBlackKingMoved = false;

        //Testing Purposes: Breakpoints
        bool _cool = false;
        public bool cool
        {
            get { return _cool; }
            set { _cool = value; }
        }

        //Index refers to the index in mMoveHistory
        //For now, index 0 is left rook and index 1 is right rook
        private List<int> whenWhiteRookMoved = new List<int>();
        private List<int> whenBlackRookMoved = new List<int>();
        public int whenWhiteKingMoved = -1;
        private int whenBlackKingMoved = -1;

        //Test
        public bool gone = false;
        public bool gone2 = false;
        public BoardPosition gone3;

        #endregion

        #region Properties.
        // TODO: implement these properties.
        // You can choose to use auto properties, computed properties, or normal properties 
        // using a private field to back the property.

        // You can add set bodies if you think that is appropriate, as long as you justify
        // the access level (public, private).

        public bool IsFinished { get { return (IsCheckmate || IsStalemate || IsDraw); } }

        public int CurrentPlayer
        {
            get
            {
                return mCurrentPlayer;
            }
            private set
            {
                mCurrentPlayer = value;
            }
        }

        public GameAdvantage CurrentAdvantage
        {
            get
            {
                if (mAdvantage.Advantage == 0) return new GameAdvantage(0, 0);
                else return mAdvantage;
            }
            private set { mAdvantage = value; }
        }

        public IReadOnlyList<ChessMove> MoveHistory => mMoveHistory;

        // TODO: implement IsCheck, IsCheckmate, IsStalemate
        public bool IsCheck
        {
            get
            {
                int oppositionPlayer = (CurrentPlayer == 1) ? 2 : 1;

                //King Position of the current player
                var KingPosition = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).First();

                //Possible Moves the King can move
                //int KingPossibleMove = GetPossibleMoves().Where(m => m.Player == CurrentPlayer && m.StartPosition == KingPosition).Count();
                int KingPossibleMove = GetPossibleMoves().Where(m => m.Player == CurrentPlayer).Count();

                if (PositionIsThreatened(KingPosition, oppositionPlayer))
                {
                    return (KingPossibleMove != 0);
                }
                return false;
            }
        }

        public bool IsCheckmate
        {
            get
            {
                int oppositionPlayer = (CurrentPlayer == 1) ? 2 : 1;

                //King Position of the current player
                var KingPosition = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).First();

                //Possible Moves the King can move
                //int KingPossibleMove = GetPossibleMoves().Where(m => m.Player == CurrentPlayer && m.StartPosition == KingPosition).Count();

                if (PositionIsThreatened(KingPosition, oppositionPlayer))
                {
                    return (GetPossibleMoves().Count() == 0);
                }
                return false;
            }
        }

        public bool IsStalemate
        {
            get
            {
                int oppositionPlayer = (CurrentPlayer == 1) ? 2 : 1;

                //King Position of the current player
                var KingPosition = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).First();

                //Possible Moves the King can move
                //int KingPossibleMove = GetPossibleMoves().Where(m => m.Player == CurrentPlayer && m.StartPosition == KingPosition).Count();

                if (!PositionIsThreatened(KingPosition, oppositionPlayer))
                {
                    //var kingMoves = GetPossibleMoves().Where(m=> m.StartPosition==KingPosition);
                    var kingMoves = GetPossibleMoves();
                    int isCheckedMove = 0;
                    foreach (ChessMove move in kingMoves)
                    {
                        if (PositionIsThreatened(move.EndPosition, oppositionPlayer))
                        {
                            isCheckedMove++;
                        }
                    }

                    return (isCheckedMove == kingMoves.Count());


                }
                return false;
            }
        }

        public bool IsDraw
        {
            get { return (DrawCounter == 100); }
        }

        /// <summary>
        /// Tracks the current draw counter, which goes up by 1 for each non-capturing, non-pawn move, and resets to 0
        /// for other moves. If the counter reaches 100 (50 full turns), the game is a draw.
        /// </summary>
        public int DrawCounter
        {
            get { return DrawHistory.Last(); }
            private set
            {
                if (value == 1)
                {
                    mDrawCounter += value;
                }
                else
                {
                    mDrawCounter = 0;
                }
            }
        }

        /// <summary>
        /// Property to signfy the weight of the current board state to determine AI move
        /// </summary>
        public long BoardWeight
        {
            get
            {
                int pawnMovementPointDifference = PawnMovementPoint(1) - PawnMovementPoint(2);
                long threatenedPointDifference = ThreatenPoint(1) - ThreatenPoint(2);

                return pawnMovementPointDifference;
            }
        }
        #endregion


        #region Public methods.
        public IEnumerable<ChessMove> GetPossibleMoves()
        {
            List<ChessMove> moves = new List<ChessMove>();
            List<ChessMove> movesToCheck = new List<ChessMove>();
            //List<ChessMove> whiteMovesCheck = new List<ChessMove>();
            //List<ChessMove> blackMovesCheck = new List<ChessMove>();

            //Nonempty positions
            //List<BoardPosition> checkPos = BoardPosition.GetRectangularPositions(BoardSize, BoardSize)
            //.Where(m => !PositionIsEmpty(m)).ToList();

            //All current player's positions
            List<BoardPosition> checkPos = BoardPosition.GetRectangularPositions(BoardSize, BoardSize)
                .Where(m => GetPlayerAtPosition(m) == CurrentPlayer).ToList();


            //Find possible moves
            foreach (BoardPosition pos in checkPos)
            {
                //Only add moves of the current player
                //if (GetPlayerAtPosition(pos) == CurrentPlayer)
                //{

                List<ChessMove> pawn = GetPossiblePawnMoves(pos).ToList();
                if (pawn.Count > 0)
                {
                    movesToCheck.AddRange(pawn);
                }
                //Check for castling
                List<ChessMove> straight = GetPossibleStraightMoves(pos, true).ToList();
                if (straight.Count > 0)
                {
                    movesToCheck.AddRange(straight);
                }
                List<ChessMove> knight = GetPossibleKnightMoves(pos).ToList();
                if (knight.Count > 0)
                {
                    movesToCheck.AddRange(knight);
                }

                //}
            }


            /*

            //Seprate Moves by player
            /*
            foreach(ChessMove cMove in movesToCheck)
            {
                if(cMove.Player == 1)
                {
                    whiteMovesCheck.Add(cMove);
                }
                else
                {
                    blackMovesCheck.Add(cMove);
                }
            }*/
            //White

            //flag the game advantage to not increment/decrement 
            advantageCalculateFlag = true;

            if (CurrentPlayer == 1)
            {

                foreach (ChessMove wMove in movesToCheck)
                {

                    //ApplyMove(wMove);
                    //UndoLastMove();
                    //moves.Add(wMove);

                    //Get the position of king
                    var kingPos = GetPositionsOfPiece(ChessPieceType.King, 1).First();

                    //Check if king is currently in check: allow castling or not
                    bool valid = true;

                    //Check if any black piece is threatening the white king
                    if (PositionIsThreatened(kingPos, 2))
                    {
                        if (wMove.MoveType == ChessMoveType.CastleKingSide || wMove.MoveType == ChessMoveType.CastleQueenSide)
                        {
                            //Castling is not allowed while in check
                            valid = false;
                        }
                    }

                    if (valid)
                    {
                        ApplyMove(wMove);

                        //Update the position of king
                        kingPos = GetPositionsOfPiece(ChessPieceType.King, 1).First();
                        //bool threatened = false;

                        //If no black piece is threatening the king, white move is valid
                        //if(!threatened)
                        if (!PositionIsThreatened(kingPos, 2))
                        {
                            moves.Add(wMove);
                        }

                        UndoLastMove();

                    }


                }


            }
            else
            {

                //Black
                foreach (ChessMove bMove in movesToCheck)
                {

                    //ApplyMove(bMove);
                    //UndoLastMove();
                    //moves.Add(bMove);

                    //Get the position of king
                    var kingPos = GetPositionsOfPiece(ChessPieceType.King, 2).First();

                    //Check if king is currently in check
                    bool valid = true;

                    //Check if any white piece is threatening the black king

                    if (PositionIsThreatened(kingPos, 1))
                    {
                        if (bMove.MoveType == ChessMoveType.CastleKingSide || bMove.MoveType == ChessMoveType.CastleQueenSide)
                        {
                            //Castling is not allowed while in check
                            valid = false;
                        }
                    }
                    if (valid)
                    {
                        ApplyMove(bMove);

                        //Update the position of king
                        kingPos = GetPositionsOfPiece(ChessPieceType.King, 2).First();


                        //Check if any black piece is threatening the white king

                        //If no black piece is threatening the king, white move is valid
                        if (!PositionIsThreatened(kingPos, 1))
                        {
                            moves.Add(bMove);
                        }
                        UndoLastMove();
                    }

                }


            }


            //Code to check if the moves do not result in the king is in check
            //Done by using ApplyMove() & check to see if king is in check & undo it.
            //Can't really call isCheck() since you will need to call GetPossibleMoves()
            //in that method, will cause infinite, so copy logic of the method. Neal said something
            //like this in class.
            /*
            foreach(ChessMove cMove in movesToCheck)
            {
                //Only one king
                int enemyPlayer;
                int player = cMove.Player;
                if (player == 1) { enemyPlayer = 2; }
                else { enemyPlayer = 1; }

                ApplyMove(cMove);//Test move

                //Only one king
                //BoardPosition kingPos = GetPositionsOfPiece(ChessPieceType.King, cMove.Player).ToList()[0];
                var kingPos = GetPositionsOfPiece(ChessPieceType.King, player).First();
                if (!PositionIsThreatened(kingPos, enemyPlayer))
                {
                    //Move is valid if king is no longer threatened either by check or checkmate
                    moves.Add(cMove);
                }

                UndoLastMove();//Revert to orignal board state
                
            }
            */
            //return moves;

            advantageCalculateFlag = false;
            return moves;
        }

        public void ApplyMove(ChessMove m)
        {
            checker = 9;
            switch (m.MoveType)
            {
                //when the move type is normal
                case (ChessMoveType.Normal):
                    checker = 0;
                    //check if the ending position of the piece has a enemy's piece
                    if (PositionIsEnemy(m.EndPosition, CurrentPlayer))
                    {

                        //Update the advantage

                        UpdateAdvantage(GetAdvantagePoint(GetPieceAtPosition(m.EndPosition).PieceType), CurrentPlayer);

                        mCapturedPieces.Add(GetPieceAtPosition(m.EndPosition));
                        //Move to remove the piece out of the bitboard
                        SetPieceAtPosition(m.EndPosition, GetPieceAtPosition(m.EndPosition));

                        //Reset the drawCounter to zero after capture
                        DrawCounter = 0;
                    }
                    else
                    {
                        //Increase the drawCounter if non-pawn was moved or reset to zero if pawn was moved
                        DrawCounter = (GetPieceAtPosition(m.StartPosition).PieceType != ChessPieceType.Pawn)
                            ? 1 : 0;
                        //Add empty to mCapturedHistory
                        mCapturedPieces.Add(new ChessPiece(ChessPieceType.Empty, 0));
                    }

                    //Move the piece to the ending position in bitboard
                    SetPieceAtPosition(m.EndPosition, GetPieceAtPosition(m.StartPosition));

                    //Remove the piece from the starting position in bitboard
                    SetPieceAtPosition(m.StartPosition, GetPieceAtPosition(m.StartPosition));

                    //Check if the king and rook was moved
                    ChessPiece movingPiece = GetPieceAtPosition(m.StartPosition);

                    if (m.Player == 1)
                    {
                        /*Check if the king has been moved
                         * Yes - No need to check it again
                         * No  - Check if the move start positon is the original position of the king
                         */
                        if (!hasWhiteKingMoved)
                        {
                            if (m.StartPosition == new BoardPosition(7, 4))
                            {
                                hasWhiteKingMoved = true;
                                whenWhiteKingMoved = mMoveHistory.Count + 1;
                                //whenWhiteKingMoved = mMoveHistory.Count;
                            }
                        }

                        /*Check if the left white rook has been moved
                         * Yes - No need to check it again
                         * No  - Check if the move start position is the orignal position of the left rook
                         */
                        if (!hasWhiteRookMoved[0])
                        {
                            if (m.StartPosition == new BoardPosition(7, 0))
                            {
                                hasWhiteRookMoved[0] = true;
                                whenWhiteRookMoved[0] = mMoveHistory.Count + 1;
                                //whenWhiteRookMoved[0] = mMoveHistory.Count;
                            }
                        }

                        /*Check if the right white rook has been moved
                         * Yes - No need to check it again
                         * No  - Check if the move start position is the orignal position of the right rook
                         */
                        if (!hasWhiteRookMoved[1])
                        {
                            if (m.StartPosition == new BoardPosition(7, 7))
                            {
                                hasWhiteRookMoved[1] = true;
                                whenWhiteRookMoved[1] = mMoveHistory.Count + 1;
                                //whenWhiteRookMoved[1] = mMoveHistory.Count;
                            }
                        }
                    }
                    else if (m.Player == 2)
                    {
                        /*Check if the king has been moved
                         * Yes - No need to check it again
                         * No  - Check if the move start positon is the original position of the king
                         */
                        if (!hasBlackKingMoved)
                        {
                            if (m.StartPosition == new BoardPosition(0, 4))
                            {
                                hasBlackKingMoved = true;
                                whenBlackKingMoved = mMoveHistory.Count + 1;
                                //whenBlackKingMoved = mMoveHistory.Count;
                            }
                        }

                        /*Check if the left black rook has been moved
                         * Yes - No need to check it again
                         * No  - Check if the move start position is the orignal position of the left rook
                         */
                        if (!hasBlackRookMoved[0])
                        {
                            if (m.StartPosition == new BoardPosition(0, 0))
                            {
                                hasBlackRookMoved[0] = true;
                                whenBlackRookMoved[0] = mMoveHistory.Count + 1;
                                //whenBlackRookMoved[0] = mMoveHistory.Count;
                            }
                        }

                        /*Check if the right black rook has been moved
                         * Yes - No need to check it again
                         * No  - Check if the move start position is the orignal position of the right rook
                         */
                        if (!hasBlackRookMoved[1])
                        {
                            if (m.StartPosition == new BoardPosition(0, 7))
                            {
                                hasBlackRookMoved[1] = true;
                                whenBlackRookMoved[1] = mMoveHistory.Count + 1;
                                //whenBlackRookMoved[1] = mMoveHistory.Count;
                            }
                        }
                    }
                    break;

                case (ChessMoveType.CastleKingSide):
                    checker = 1;
                    //Move the king to ending position in bitboard
                    SetPieceAtPosition(m.EndPosition, GetPieceAtPosition(m.StartPosition));
                    //Remove the king from the starting position in bitboard
                    SetPieceAtPosition(m.StartPosition, GetPieceAtPosition(m.StartPosition));

                    //Move the rook to ending position in bitboard
                    SetPieceAtPosition(m.StartPosition.Translate(0, 1), new ChessPiece(ChessPieceType.Rook, CurrentPlayer));
                    //Remove the rook from its original position in bitboard
                    SetPieceAtPosition(m.StartPosition.Translate(0, 3), ChessPiece.Empty);

                    //Mark King and right rook as moved
                    if (CurrentPlayer == 1)
                    {
                        hasWhiteKingMoved = true;
                        hasWhiteRookMoved[1] = true;
                        whenWhiteKingMoved = mMoveHistory.Count + 1;
                        whenWhiteRookMoved[1] = mMoveHistory.Count + 1;
                        //whenWhiteKingMoved = mMoveHistory.Count;
                        //whenWhiteRookMoved[1] = mMoveHistory.Count;
                    }
                    else
                    {
                        hasBlackKingMoved = true;
                        hasBlackRookMoved[1] = true;
                        whenBlackKingMoved = mMoveHistory.Count + 1;
                        whenBlackRookMoved[1] = mMoveHistory.Count + 1;
                        //whenBlackKingMoved = mMoveHistory.Count;
                        //whenBlackRookMoved[1] = mMoveHistory.Count;
                    }

                    //Increase the drawCounter 
                    DrawCounter = 1;

                    //Add empty to mCapturedHistory
                    mCapturedPieces.Add(new ChessPiece(ChessPieceType.Empty, 0));

                    break;

                case (ChessMoveType.CastleQueenSide):
                    checker = 3;
                    //Move the king to ending position in bitboard
                    SetPieceAtPosition(m.EndPosition, GetPieceAtPosition(m.StartPosition));
                    //Remove the king from the starting position in bitboard
                    SetPieceAtPosition(m.StartPosition, GetPieceAtPosition(m.EndPosition));

                    //Move the rook to ending position in bitboard
                    SetPieceAtPosition(m.StartPosition.Translate(0, -1), new ChessPiece(ChessPieceType.Rook, CurrentPlayer));
                    //Remove the rook from its original position in bitboard
                    SetPieceAtPosition(m.StartPosition.Translate(0, -4), ChessPiece.Empty);

                    //Mark King and left rook as moved
                    if (CurrentPlayer == 1)
                    {
                        hasWhiteKingMoved = true;
                        hasWhiteRookMoved[0] = true;
                        whenWhiteKingMoved = mMoveHistory.Count + 1;
                        whenWhiteRookMoved[0] = mMoveHistory.Count + 1;
                        //whenWhiteKingMoved = mMoveHistory.Count;
                        //whenWhiteRookMoved[0] = mMoveHistory.Count;
                    }
                    else
                    {
                        hasBlackKingMoved = true;
                        hasBlackRookMoved[0] = true;
                        whenBlackKingMoved = mMoveHistory.Count + 1;
                        whenBlackRookMoved[0] = mMoveHistory.Count + 1;
                        //whenBlackKingMoved = mMoveHistory.Count;
                        //whenBlackRookMoved[0] = mMoveHistory.Count;
                    }

                    //Increase the drawCounter 
                    DrawCounter = 1;

                    //Add empty to mCapturedHistory
                    mCapturedPieces.Add(new ChessPiece(ChessPieceType.Empty, 0));

                    break;

                case (ChessMoveType.EnPassant):
                    checker = 4;
                    int enemyPlayer = (CurrentPlayer == 1) ? 2 : 1;

                    //Move the friendly pawn to the diagonal position in bitboard
                    SetPieceAtPosition(m.EndPosition, GetPieceAtPosition(m.StartPosition));
                    //Remove the friendly pawn from its original position in bitboard
                    SetPieceAtPosition(m.StartPosition, GetPieceAtPosition(m.EndPosition));



                    //Remove the enemy's pawn from the bitboard
                    if (enemyPlayer == 1)
                    {
                        SetPieceAtPosition(m.EndPosition.Translate(-1, 0), GetPieceAtPosition(m.EndPosition.Translate(-1, 0)));
                        mCapturedPieces.Add(GetPieceAtPosition(m.EndPosition.Translate(-1, 0)));
                    }
                    else
                    {
                        SetPieceAtPosition(m.EndPosition.Translate(1, 0), GetPieceAtPosition(m.EndPosition.Translate(1, 0)));
                        mCapturedPieces.Add(GetPieceAtPosition(m.EndPosition.Translate(1, 0)));
                    }


                    //Update the advantage after enPassant
                    UpdateAdvantage(GetAdvantagePoint(ChessPieceType.Pawn), CurrentPlayer);

                    //Reset the drawCounter
                    DrawCounter = 0;
                    break;

                case (ChessMoveType.PawnPromote):
                    checker = 5;
                    //check if the ending position of the piece has a enemy's piece
                    if (PositionIsEnemy(m.EndPosition, CurrentPlayer))
                    {

                        //Update the advantage
                        UpdateAdvantage(GetAdvantagePoint(GetPieceAtPosition(m.EndPosition).PieceType), CurrentPlayer);

                        mCapturedPieces.Add(GetPieceAtPosition(m.EndPosition));

                        //Move to remove the piece out of the bitboard
                        SetPieceAtPosition(m.EndPosition, GetPieceAtPosition(m.EndPosition));
                    }
                    else { mCapturedPieces.Add(new ChessPiece(ChessPieceType.Empty, 0)); }

                    //Remove the pawn from the ending position from bitboard
                    //SetPieceAtPosition(m.EndPosition, GetPieceAtPosition(m.StartPosition));
                    //SetPieceAtPosition(m.EndPosition, GetPieceAtPosition(m.EndPosition));
                    SetPieceAtPosition(m.StartPosition, GetPieceAtPosition(m.StartPosition));

                    //Introduce the new piece available via promotion in the bitboard
                    SetPieceAtPosition(m.EndPosition, new ChessPiece(m.PromoteTo, CurrentPlayer));

                    //Update the advantage by adding the piece value and substracting one for the pawn 
                    UpdateAdvantage(GetAdvantagePoint(m.PromoteTo) - 1, CurrentPlayer);

                    //Increase the drawCounter
                    DrawCounter = 0;

                    //Add empty to mCapturedHistory


                    break;
            }

            //add the move to the movehistory list
            mMoveHistory.Add(m);

            //save Draw Counter
            DrawHistory.Add(mDrawCounter);

            //Change the currentPlayer to opposite player after the move
            CurrentPlayer = (CurrentPlayer == 1) ? 2 : 1;

            //Update the boardWeight    
            //BoardWeight = 
        }


        public void UndoLastMove()
        {

            ChessMove prev = mMoveHistory.Last();
            int pointReceiver = (prev.Player == 1) ? 2 : 1;
            if (prev.MoveType == ChessMoveType.Normal)
            {
                //ApplyMove(new ChessMove(prev.EndPosition, prev.StartPosition));
                //mMoveHistory.RemoveAt(mMoveHistory.Count - 1);
                //mCapturedPieces.RemoveAt(mCapturedPieces.Count - 1);
                //DrawHistory.RemoveAt(DrawHistory.Count - 1);

                //set the piece at the end position to the start position
                SetPieceAtPosition(prev.StartPosition, GetPieceAtPosition(prev.EndPosition));
                //Remove piece at the end position 
                SetPieceAtPosition(prev.EndPosition, ChessPiece.Empty);

                //CapturedPiece and mMoveHistory should be the same size, so the index of CapturedPiece
                //correspond to mMoveHistory.
                if (mCapturedPieces.Count > 0 && mCapturedPieces.Last().PieceType != ChessPieceType.Empty)//A capture has been made
                {
                    SetPieceAtPosition(prev.EndPosition, mCapturedPieces.Last());//Set new captured piece
                    UpdateAdvantage(GetAdvantagePoint(mCapturedPieces.Last().PieceType), pointReceiver);
                    //mCapturedPieces.RemoveAt(mCapturedPieces.Count - 1);

                }

            }
            else if (prev.MoveType == ChessMoveType.PawnPromote)
            {
                //Add the pawn back to start position
                SetPieceAtPosition(prev.StartPosition, new ChessPiece(ChessPieceType.Pawn, prev.Player));
                //Remove promoted piece
                SetPieceAtPosition(prev.EndPosition, ChessPiece.Empty);

                //set the piece at the end position to the start position
                //SetPieceAtPosition(prev.StartPosition, new ChessPiece(ChessPieceType.Pawn, rewindMove.Player));
                //set the affected(removed) piece at the end position 
                //SetPieceAtPosition(rewindMove.EndPosition, rewindMove.AffectedPiece);

                //ApplyMove(new ChessMove(prev.EndPosition, prev.StartPosition));
                //mMoveHistory.RemoveAt(mMoveHistory.Count - 1);
                //mCapturedPieces.RemoveAt(mCapturedPieces.Count - 1);
                //DrawHistory.RemoveAt(DrawHistory.Count - 1);
                //SetPieceAtPosition(prev.EndPosition, new ChessPiece(ChessPieceType.Pawn, prev.Player));
                //SetPieceAtPosition(prev.EndPosition, new ChessPiece(prev.PromoteTo, prev.Player));
                //SetPieceAtPosition(prev.StartPosition, GetPieceAtPosition(prev.StartPosition));
                //


                //pawn comes back to them
                UpdateAdvantage(1, prev.Player);

                UpdateAdvantage(GetAdvantagePoint(prev.PromoteTo), pointReceiver);
                if (mCapturedPieces.Count > 0 && mCapturedPieces.Last().PieceType != ChessPieceType.Empty)//A capture has been made
                {
                    SetPieceAtPosition(prev.EndPosition, mCapturedPieces.Last());//Set new captured piece
                    UpdateAdvantage(GetAdvantagePoint(mCapturedPieces.Last().PieceType), pointReceiver);
                    //mCapturedPieces.RemoveAt(mCapturedPieces.Count - 1);
                }

            }
            else if (prev.MoveType == ChessMoveType.EnPassant)
            {
                //ApplyMove(new ChessMove(prev.EndPosition, prev.StartPosition));
                //mMoveHistory.RemoveAt(mMoveHistory.Count - 1);
                //mCapturedPieces.RemoveAt(mCapturedPieces.Count - 1);
                //DrawHistory.RemoveAt(DrawHistory.Count - 1);

                //set the pawn back
                SetPieceAtPosition(prev.StartPosition, new ChessPiece(ChessPieceType.Pawn, prev.Player));
                //remove pawn piece at the end position
                SetPieceAtPosition(prev.EndPosition, ChessPiece.Empty);

                if (prev.Player == 1)
                {
                    //Captured black piece is below the white's end position
                    SetPieceAtPosition(prev.EndPosition.Translate(1, 0), new ChessPiece(ChessPieceType.Pawn, 2));
                    UpdateAdvantage(GetAdvantagePoint(ChessPieceType.Pawn), pointReceiver);
                    //mCapturedPieces.RemoveAt(mCapturedPieces.Count - 1);
                }
                else
                {
                    //Captured white piece is above the black's end position
                    SetPieceAtPosition(prev.EndPosition.Translate(-1, 0), new ChessPiece(ChessPieceType.Pawn, 1));
                    UpdateAdvantage(GetAdvantagePoint(ChessPieceType.Pawn), pointReceiver);
                    // mCapturedPieces.RemoveAt(mCapturedPieces.Count - 1);
                }
            }
            //This is highly dependent on how you implement applyMove for castling
            //Should keep castling as one move to make CapturedPiece correspond to mMoveHistory
            else if (prev.MoveType == ChessMoveType.CastleKingSide)
            {
                //ApplyMove(new ChessMove(prev.EndPosition, prev.StartPosition, ChessMoveType.Normal));
                //mMoveHistory.RemoveAt(mMoveHistory.Count - 1);
                //mCapturedPieces.RemoveAt(mCapturedPieces.Count - 1);
                //DrawHistory.RemoveAt(DrawHistory.Count - 1);
                //Move the king to original position in bitboard
                //SetPieceAtPosition(prev.StartPosition, new ChessPiece(ChessPieceType.King, prev.Player));
                //Remove the king from the ending position in bitboard
                //SetPieceAtPosition(prev.StartPosition, new ChessPiece(ChessPieceType.King, prev.Player));

                //set the King back to start position
                SetPieceAtPosition(prev.StartPosition, new ChessPiece(ChessPieceType.King, prev.Player));
                //Remove the king at end position
                SetPieceAtPosition(prev.EndPosition, ChessPiece.Empty);

                //Move the rook to ending position in bitboard
                SetPieceAtPosition(prev.StartPosition.Translate(0, 3), new ChessPiece(ChessPieceType.Rook, prev.Player));
                //Remove the rook from its original position in bitboard
                SetPieceAtPosition(prev.StartPosition.Translate(0, 1), ChessPiece.Empty);

            }
            else if (prev.MoveType == ChessMoveType.CastleQueenSide)
            {
                //ApplyMove(new ChessMove(prev.EndPosition, prev.StartPosition, ChessMoveType.Normal));
                //mMoveHistory.RemoveAt(mMoveHistory.Count - 1);
                //mCapturedPieces.RemoveAt(mCapturedPieces.Count - 1);
                //DrawHistory.RemoveAt(DrawHistory.Count - 1);

                //set the King back to start position
                SetPieceAtPosition(prev.StartPosition, new ChessPiece(ChessPieceType.King, prev.Player));
                //Remove the king at end position
                SetPieceAtPosition(prev.EndPosition, ChessPiece.Empty);

                //Move the rook to ending position
                SetPieceAtPosition(prev.StartPosition.Translate(0, -4), new ChessPiece(ChessPieceType.Rook, prev.Player));
                //Remove the rook from its starting position
                SetPieceAtPosition(prev.StartPosition.Translate(0, -1), new ChessPiece(ChessPieceType.Rook, prev.Player));

            }//Change the currentPlayer to opposite player after the move
            CurrentPlayer = (CurrentPlayer == 1) ? 2 : 1;
            //If the move that is being undone is the first time the king or rooks move, 
            //reset the status of those pieces
            if (CurrentPlayer == 1)
            {
                if (hasWhiteKingMoved && whenWhiteKingMoved == mMoveHistory.Count)
                {
                    hasWhiteKingMoved = false;
                    whenWhiteKingMoved = -1;
                }
                if (hasWhiteRookMoved[0] && whenWhiteRookMoved[0] == mMoveHistory.Count)
                {
                    hasWhiteRookMoved[0] = false;
                    whenWhiteRookMoved[0] = -1;
                }
                if (hasWhiteRookMoved[1] && whenWhiteRookMoved[1] == mMoveHistory.Count)
                {
                    hasWhiteRookMoved[1] = false;
                    whenWhiteRookMoved[1] = -1;
                }
            }
            else
            {
                if (hasBlackKingMoved && whenBlackKingMoved == mMoveHistory.Count)
                {
                    hasBlackKingMoved = false;
                    whenBlackKingMoved = -1;
                }
                if (hasBlackRookMoved[0] && whenBlackRookMoved[0] == mMoveHistory.Count)
                {
                    hasBlackRookMoved[0] = false;
                    whenBlackRookMoved[0] = -1;
                }
                if (hasBlackRookMoved[1] && whenBlackRookMoved[1] == mMoveHistory.Count)
                {
                    hasBlackRookMoved[1] = false;
                    whenBlackRookMoved[1] = -1;
                }
            }
            //mMoveHistory.RemoveAt(mMoveHistory.Count - 1);
            mMoveHistory.RemoveAt(mMoveHistory.Count - 1);

            DrawHistory.RemoveAt(DrawHistory.Count - 1);
            mDrawCounter = DrawHistory.Last();

            mCapturedPieces.RemoveAt(mCapturedPieces.Count - 1);


        }

        /// <summary>
        /// Returns whatever chess piece is occupying the given position.
        /// </summary>
        public ChessPiece GetPieceAtPosition(BoardPosition position)
        {

            //Get the corresponding bit position for the corresponding board position
            int index = GetBitIndexForPosition(position);

            //bitmask with a 1 at the index position
            ulong bitMask = 1UL << index;

            //check which piece is at the position and return the chesspiece object
            if ((bitMask & blackPawns) != 0)
            {
                return new ChessPiece(ChessPieceType.Pawn, 2);
            }
            else if ((bitMask & whitePawns) != 0)
            {
                return new ChessPiece(ChessPieceType.Pawn, 1);
            }
            else if ((bitMask & whiteRooks) != 0)
            {
                return new ChessPiece(ChessPieceType.Rook, 1);
            }
            else if ((bitMask & blackRooks) != 0)
            {
                return new ChessPiece(ChessPieceType.Rook, 2);
            }
            else if ((bitMask & whiteBishops) != 0)
            {
                return new ChessPiece(ChessPieceType.Bishop, 1);
            }
            else if ((bitMask & blackBishops) != 0)
            {
                return new ChessPiece(ChessPieceType.Bishop, 2);
            }
            else if ((bitMask & whiteKnights) != 0)
            {
                return new ChessPiece(ChessPieceType.Knight, 1);
            }
            else if ((bitMask & blackKnights) != 0)
            {
                return new ChessPiece(ChessPieceType.Knight, 2);
            }
            else if ((bitMask & whiteQueen) != 0)
            {
                return new ChessPiece(ChessPieceType.Queen, 1);
            }
            else if ((bitMask & blackQueen) != 0)
            {
                return new ChessPiece(ChessPieceType.Queen, 2);
            }
            else if ((bitMask & whiteKing) != 0)
            {
                return new ChessPiece(ChessPieceType.King, 1);
            }
            else if ((bitMask & blackKing) != 0)
            {
                return new ChessPiece(ChessPieceType.King, 2);
            }

            return ChessPiece.Empty;
        }

        /// <summary>
        /// Returns whatever player is occupying the given position.
        /// </summary>
        public int GetPlayerAtPosition(BoardPosition pos)
        {
            // As a hint, you should call GetPieceAtPosition.
            return GetPieceAtPosition(pos).Player;
        }

        /// <summary>
        /// Returns true if the given position on the board is empty.
        /// </summary>
        /// <remarks>returns false if the position is not in bounds</remarks>
        public bool PositionIsEmpty(BoardPosition pos)
        {
            //Note: This is right if the player is set to 0 when empty,
            //      which I think it does from the ChessPiece class.
            return (GetPlayerAtPosition(pos) == 0);
        }

        /// <summary>
        /// Returns true if the given position contains a piece that is the enemy of the given player.
        /// </summary>
        /// <remarks>returns false if the position is not in bounds</remarks>
        public bool PositionIsEnemy(BoardPosition pos, int player)
        {
            if (PositionInBounds(pos))
            {
                return ((GetPlayerAtPosition(pos) != player) &&
                        (GetPlayerAtPosition(pos) != 0));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the given position is in the bounds of the board.
        /// </summary>
        public static bool PositionInBounds(BoardPosition pos)
        {
            return ((pos.Row >= 0) && (pos.Row < BoardSize) &&
                    (pos.Col >= 0) && (pos.Col < BoardSize));
        }

        /// <summary>
        /// Returns all board positions where the given piece can be found.
        /// </summary>
        public IEnumerable<BoardPosition> GetPositionsOfPiece(ChessPieceType piece, int player)
        {

            List<BoardPosition> piecePositions = new List<BoardPosition>();
            //piecePositions.Add(new BoardPosition(0,0));
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    BoardPosition pos = new BoardPosition(i, j);
                    if (GetPieceAtPosition(pos).PieceType == piece)
                    {
                        if (!PositionIsEnemy(pos, player))
                        {
                            piecePositions.Add(pos);
                        }
                    }
                }
            }
            return piecePositions;
        }

        /// <summary>
        /// Returns true if the given player's pieces are attacking the given position.
        /// </summary>
        public bool PositionIsThreatened(BoardPosition position, int byPlayer)
        {
            return GetAttackedPositions(byPlayer).Contains(position);
        }

        /// <summary>
        /// Returns a set of all BoardPositions that are attacked by the given player.
        /// </summary>
        public ISet<BoardPosition> GetAttackedPositions(int byPlayer)
        {
            int enemy = (byPlayer == 1) ? 2 : 1;
            //IEnumerable<ChessMove> moves = GetPossibleMoves().Where(m => m.Player == byPlayer);
            List<ChessMove> move = new List<ChessMove>();

            //Enemy Positions
            List<BoardPosition> checkPos = BoardPosition.GetRectangularPositions(BoardSize, BoardSize)
                .Where(m => GetPlayerAtPosition(m) == byPlayer).ToList();

            //Get all possible moves from enemy
            foreach (BoardPosition pos in checkPos)
            {
                //if (GetPlayerAtPosition(pos) == CurrentPlayer)
                //{
                List<ChessMove> pawn = GetPossiblePawnMoves(pos).ToList();
                if (pawn.Count > 0)
                {
                    move.AddRange(pawn);
                }
                //Don't check for castling, so it doesn't call PositionIsThreatened 
                //since castling can't capture & not cause infinite loop
                List<ChessMove> straight = GetPossibleStraightMoves(pos, false).ToList();
                if (straight.Count > 0)
                {
                    move.AddRange(straight);
                }
                List<ChessMove> knight = GetPossibleKnightMoves(pos).ToList();
                if (knight.Count > 0)
                {
                    move.AddRange(knight);
                }
                //}
            }

            List<BoardPosition> attacked = move.Where(m => GetPlayerAtPosition(m.EndPosition) == enemy)
                .Select(m => m.EndPosition).ToList();
            //List<BoardPosition> attacked = move.Where(m => PositionIsEnemy(m.EndPosition, byPlayer))
            //.Select(m => m.EndPosition).ToList();

            //List<BoardPosition> attacked = move.Select(m => m.EndPosition).ToList();
            return new HashSet<BoardPosition>(attacked); //No repeat elements
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Mutates the board state so that the given piece is at the given position.
        /// </summary>
        private void SetPieceAtPosition(BoardPosition position, ChessPiece piece)
        {
            int index = GetBitIndexForPosition(position);
            ulong mask = 1UL << index;

            if (piece.Player == 1)
            {
                switch (piece.PieceType)
                {
                    case ChessPieceType.Pawn:
                        whitePawns ^= mask;
                        break;
                    case ChessPieceType.Rook:
                        whiteRooks ^= mask;
                        break;
                    case ChessPieceType.Knight:
                        whiteKnights ^= mask;
                        break;
                    case ChessPieceType.Bishop:
                        whiteBishops ^= mask;
                        break;
                    case ChessPieceType.Queen:
                        whiteQueen ^= mask;
                        break;
                    case ChessPieceType.King:
                        whiteKing ^= mask;
                        break;
                }
            }
            else if (piece.Player == 2)
            {
                switch (piece.PieceType)
                {
                    case ChessPieceType.Pawn:
                        blackPawns ^= mask;
                        break;
                    case ChessPieceType.Rook:
                        blackRooks ^= mask;
                        break;
                    case ChessPieceType.Knight:
                        blackKnights ^= mask;
                        break;
                    case ChessPieceType.Bishop:
                        blackBishops ^= mask;
                        break;
                    case ChessPieceType.Queen:
                        blackQueen ^= mask;
                        break;
                    case ChessPieceType.King:
                        blackKing ^= mask;
                        break;
                }
            }
            else if (piece.PieceType == ChessPieceType.Empty)
            {
                if (GetPlayerAtPosition(position) == 1)
                {
                    switch (GetPieceAtPosition(position).PieceType)
                    {
                        case ChessPieceType.Pawn:
                            whitePawns ^= mask;
                            break;
                        case ChessPieceType.Rook:
                            whiteRooks ^= mask;
                            break;
                        case ChessPieceType.Knight:
                            whiteKnights ^= mask;
                            break;
                        case ChessPieceType.Bishop:
                            whiteBishops ^= mask;
                            break;
                        case ChessPieceType.Queen:
                            whiteQueen ^= mask;
                            break;
                        case ChessPieceType.King:
                            whiteKing ^= mask;
                            break;
                    }
                }
                else if (GetPlayerAtPosition(position) == 2)
                {
                    switch (GetPieceAtPosition(position).PieceType)
                    {
                        case ChessPieceType.Pawn:
                            blackPawns ^= mask;
                            break;
                        case ChessPieceType.Rook:
                            blackRooks ^= mask;
                            break;
                        case ChessPieceType.Knight:
                            blackKnights ^= mask;
                            break;
                        case ChessPieceType.Bishop:
                            blackBishops ^= mask;
                            break;
                        case ChessPieceType.Queen:
                            blackQueen ^= mask;
                            break;
                        case ChessPieceType.King:
                            blackKing ^= mask;
                            break;
                    }
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static int GetBitIndexForPosition(BoardPosition position) =>
            63 - (position.Row * 8 + position.Col);

        /// <summary>
        /// Get all possible moves from a specified pawn, Return null if wrong piece.
        /// </summary>
        public IEnumerable<ChessMove> GetPossiblePawnMoves(BoardPosition position)
        {
            ChessPiece pawn = GetPieceAtPosition(position);
            int player = GetPlayerAtPosition(position);
            int vert = -1;//Used to determine direction depending on Player
            List<ChessMove> moves = new List<ChessMove>();

            if (pawn.PieceType != ChessPieceType.Pawn)
            {
                return moves;
            }
            else
            {
                if (player == 2)
                {
                    vert = 1;//Flip direction if black
                }


                List<BoardPosition> endPosition = new List<BoardPosition>();
                ChessMove nMove;
                //All possible moves for pawn
                endPosition.Add(position.Translate(vert, 0));
                endPosition.Add(position.Translate(vert * 2, 0));
                endPosition.Add(position.Translate(vert, -1));
                endPosition.Add(position.Translate(vert, 1));

                bool blocked = false;
                for (int i = 0; i < endPosition.Count; i++)
                {
                    if (PositionInBounds(endPosition[i]))
                    {
                        //Forward Moves
                        if (i < 2)
                        {

                            if (PositionIsEmpty(endPosition[i]))
                            {
                                if (i == 0)
                                {
                                    //Promotion
                                    if (((player == 1) && (position.Row == 1)) || ((player == 2) && (position.Row == 6)))
                                    {
                                        ChessMove promote = new ChessMove(position, endPosition[i], ChessMoveType.PawnPromote);
                                        promote.Player = player;
                                        promote.PromoteTo = ChessPieceType.Bishop;
                                        moves.Add(promote);

                                        promote = new ChessMove(position, endPosition[i], ChessMoveType.PawnPromote);
                                        promote.Player = player;
                                        promote.PromoteTo = ChessPieceType.Rook;
                                        moves.Add(promote);

                                        promote = new ChessMove(position, endPosition[i], ChessMoveType.PawnPromote);
                                        promote.Player = player;
                                        promote.PromoteTo = ChessPieceType.Knight;
                                        moves.Add(promote);

                                        promote = new ChessMove(position, endPosition[i], ChessMoveType.PawnPromote);
                                        promote.Player = player;
                                        promote.PromoteTo = ChessPieceType.Queen;
                                        moves.Add(promote);
                                    }
                                    else
                                    {
                                        nMove = new ChessMove(position, endPosition[i]);
                                        nMove.Player = player;
                                        moves.Add(nMove);
                                    }
                                }
                                else if ((i == 1) && !hasMoved(position) && !blocked)
                                {
                                    nMove = new ChessMove(position, endPosition[i]);
                                    nMove.Player = player;
                                    moves.Add(nMove);
                                }
                            }
                            else
                            {
                                blocked = true;
                            }
                        }
                        //here
                        //Diagonal Capture Moves
                        else
                        {

                            if (PositionIsEnemy(endPosition[i], player))

                            {
                                //Promotion
                                if (((player == 1) && (position.Row == 1)) || ((player == 2) && (position.Row == 6)))
                                {
                                    /*
                                    ChessMove promote = new ChessMove(position, endPosition[i], ChessMoveType.PawnPromote);
                                    promote.Player = player;
                                    promote.PromoteTo = ChessPieceType.Bishop;
                                    moves.Add(promote);
                                    promote.PromoteTo = ChessPieceType.Rook;
                                    moves.Add(promote);
                                    promote.PromoteTo = ChessPieceType.Knight;
                                    moves.Add(promote);
                                    promote.PromoteTo = ChessPieceType.Queen;
                                    moves.Add(promote);
                                    */
                                    ChessMove promote = new ChessMove(position, endPosition[i], ChessMoveType.PawnPromote);
                                    promote.Player = player;
                                    promote.PromoteTo = ChessPieceType.Bishop;
                                    moves.Add(promote);

                                    promote = new ChessMove(position, endPosition[i], ChessMoveType.PawnPromote);
                                    promote.Player = player;
                                    promote.PromoteTo = ChessPieceType.Rook;
                                    moves.Add(promote);

                                    promote = new ChessMove(position, endPosition[i], ChessMoveType.PawnPromote);
                                    promote.Player = player;
                                    promote.PromoteTo = ChessPieceType.Knight;
                                    moves.Add(promote);

                                    promote = new ChessMove(position, endPosition[i], ChessMoveType.PawnPromote);
                                    promote.Player = player;
                                    promote.PromoteTo = ChessPieceType.Queen;
                                    moves.Add(promote);
                                }
                                else
                                {
                                    nMove = new ChessMove(position, endPosition[i]);
                                    nMove.Player = player;
                                    moves.Add(nMove);
                                }

                            }
                        }
                    }
                }

                //En passant move
                int count = mMoveHistory.Count();



                if (count != 0)
                {
                    ChessMove prev = mMoveHistory.Last();
                    //Check if the previous move is a double space pawn move
                    if ((prev.MoveType == ChessMoveType.Normal)
                        && (GetPieceAtPosition(prev.EndPosition).PieceType == ChessPieceType.Pawn)
                        && (Math.Abs(prev.EndPosition.Row - prev.StartPosition.Row) == 2)
                        && (prev.Player != CurrentPlayer))

                    {
                        //Testing
                        if (position == new BoardPosition(6, 1) && count == 4)// && 
                                                                              //prev == new ChessMove(new BoardPosition(1,4),new BoardPosition(3,4)))
                        {
                            cool = true;
                            cool = false;
                        }
                        //Check if there is a pawn at the left or right of enemy's pawn that did double space move
                        for (int horiz = -1; horiz < 2; horiz += 2)//-1 for left & 1 for right
                        {
                            BoardPosition possibleAlly = new BoardPosition(prev.EndPosition.Row, prev.EndPosition.Col + horiz);
                            if (PositionInBounds(possibleAlly))
                            {
                                int enemy = (prev.Player == 1) ? 1 : 2;
                                if ((GetPieceAtPosition(possibleAlly).PieceType == ChessPieceType.Pawn)
                                    //&& (PositionIsEnemy(possibleAlly, enemy)))
                                    && possibleAlly == position)
                                {
                                    nMove = new ChessMove(position, position.Translate(vert, -1 * horiz), ChessMoveType.EnPassant);
                                    nMove.Player = player;
                                    moves.Add(nMove);

                                }
                            }
                        }

                    }
                }

                return moves;
            }


        }
        /// <summary>
        /// Get all possible moves from a specified rook, bishop, queen, or king; Return null if wrong piece.
        /// </summary>
        public IEnumerable<ChessMove> GetPossibleStraightMoves(BoardPosition position, bool castleCheck)
        {
            ChessPiece piece = GetPieceAtPosition(position);
            int player = GetPlayerAtPosition(position);
            List<BoardDirection> dir = BoardDirection.CardinalDirections.ToList();
            List<ChessMove> moves = new List<ChessMove>();
            bool isKing = false;
            if (piece.PieceType == ChessPieceType.Rook)
            {
                //Remove the diagonals
                dir.Remove(new BoardDirection(-1, -1));
                dir.Remove(new BoardDirection(-1, 1));
                dir.Remove(new BoardDirection(1, -1));
                dir.Remove(new BoardDirection(1, 1));
            }
            else if (piece.PieceType == ChessPieceType.Bishop)
            {
                //Remove the non-diagonals
                dir.Remove(new BoardDirection(-1, 0));
                dir.Remove(new BoardDirection(0, -1));
                dir.Remove(new BoardDirection(1, 0));
                dir.Remove(new BoardDirection(0, 1));
            }
            else if (piece.PieceType == ChessPieceType.King)
            {
                isKing = true;
            }
            else if ((piece.PieceType != ChessPieceType.Queen))
            {
                return moves;
            }


            for (int i = 0; i < dir.Count; i++)
            {
                int numSpaces = 1;//Determine how far the piece goes
                bool blocked = false;
                ChessMove nMove;
                do
                {
                    //Calculate position by direction and number of spaces
                    BoardPosition endPosition = position.Translate
                                                (dir[i].RowDelta * numSpaces, dir[i].ColDelta * numSpaces);
                    if (PositionInBounds(endPosition))
                    {
                        if (PositionIsEmpty(endPosition))
                        {
                            nMove = new ChessMove(position, endPosition);
                            nMove.Player = player;
                            moves.Add(nMove);
                        }
                        else if (PositionIsEnemy(endPosition, player))
                        {
                            nMove = new ChessMove(position, endPosition);
                            nMove.Player = player;
                            moves.Add(nMove);
                            blocked = true;
                        }
                        else
                        {
                            blocked = true;
                        }
                    }
                    else
                    {
                        blocked = true;
                    }
                    numSpaces++;
                } while (!blocked && !isKing);//King can only go 1 space, so only go through loop once
            }

            //King Castling
            if (isKing && castleCheck)
            {
                /*
                int enemyPlayer = -1;
                if (player == 1) { enemyPlayer = 2; }
                else { enemyPlayer = 1; }
                */
                moves.AddRange(KingCastling(position));
            }

            return moves;
        }

        public int personals(BoardPosition position)
        {
            bool isKing = true;
            int player = GetPlayerAtPosition(position);
            List<BoardDirection> dir = BoardDirection.CardinalDirections.ToList();
            int count = 0;
            for (int i = 0; i < dir.Count; i++)
            {
                int numSpaces = 1;//Determine how far the piece goes
                bool blocked = false;
                ChessMove nMove;

                do
                {
                    //Calculate position by direction and number of spaces
                    BoardPosition endPosition = position.Translate
                                                (dir[i].RowDelta * numSpaces, dir[i].ColDelta * numSpaces);
                    if (PositionInBounds(endPosition))
                    {
                        if (PositionIsEmpty(endPosition))
                        {
                            nMove = new ChessMove(position, endPosition);
                            nMove.Player = player;
                            //moves.Add(nMove);
                            count++;
                        }
                        //else if (PositionIsEnemy(endPosition, player))
                        //{
                        //   nMove = new ChessMove(position, endPosition);
                        // nMove.Player = player;
                        //moves.Add(nMove);
                        //blocked = true;
                        //}
                        else
                        {
                            blocked = true;
                        }
                    }
                    else
                    {
                        blocked = true;
                    }
                    numSpaces++;
                } while (!blocked && !isKing);//King can only go 1 space, so only go through loop once


            }
            return count;
        }
        /// <summary>
        /// Get castle queen side or king side moves if valid given a specified King position
        /// enemyPlayer is used for PositionIsThreatened in checkEmptySpaces
        /// </summary>
        public IEnumerable<ChessMove> KingCastling(BoardPosition position)
        {
            List<ChessMove> castle = new List<ChessMove>();
            int enemyPlayer = (CurrentPlayer == 1) ? 2 : 1;
            //if (enemyPlayer == 1) { player = 2; }
            //else { player = 1; }
            for (int i = 0; i < hasWhiteRookMoved.Count; i++)
            {
                ChessMove nMove;
                if (CurrentPlayer == 1)
                {
                    if (!hasWhiteKingMoved && !hasWhiteRookMoved[i])
                    {
                        //Note to self: When you apply these moves, make sure to set 2 pieces' positions.
                        if (i == 0 && checkEmptySpaces(position, -1, enemyPlayer) == 3)
                        {
                            nMove = new ChessMove(position, position.Translate(0, -2), ChessMoveType.CastleQueenSide);
                            nMove.Player = CurrentPlayer;
                            castle.Add(nMove);
                            //castle.Add(new ChessMove(position, position.Translate(0, -2), ChessMoveType.CastleQueenSide));
                        }
                        if (i == 1 & checkEmptySpaces(position, 1, enemyPlayer) == 2)
                        {
                            nMove = new ChessMove(position, position.Translate(0, 2), ChessMoveType.CastleKingSide);
                            nMove.Player = CurrentPlayer;
                            castle.Add(nMove);
                            //castle.Add(new ChessMove(position, position.Translate(0, 2), ChessMoveType.CastleKingSide));
                        }
                    }
                }
                else
                {
                    if (!hasBlackKingMoved && !hasBlackRookMoved[i])
                    {
                        //Note to self: When you apply these moves, make sure to set 2 pieces' positions.
                        if (i == 0 && checkEmptySpaces(position, -1, enemyPlayer) == 3)
                        {
                            nMove = new ChessMove(position, position.Translate(0, -2), ChessMoveType.CastleQueenSide);
                            nMove.Player = CurrentPlayer;
                            castle.Add(nMove);
                            //castle.Add(new ChessMove(position, position.Translate(0, -2), ChessMoveType.CastleQueenSide));
                        }
                        if (i == 1 && checkEmptySpaces(position, 1, enemyPlayer) == 2)
                        {
                            nMove = new ChessMove(position, position.Translate(0, 2), ChessMoveType.CastleKingSide);
                            nMove.Player = CurrentPlayer;
                            castle.Add(nMove);
                            //castle.Add(new ChessMove(position, position.Translate(0, 2), ChessMoveType.CastleKingSide));
                        }
                    }
                }

            }
            return castle;
        }
        public String personals()
        {
            int countOne = 0;
            int countTwo = 0;
            foreach (ChessMove m in mMoveHistory)
            {
                if (m.Player == 1)
                {
                    countOne++;
                }
                else
                {
                    countTwo++;
                }
            }
            return countOne + " " + countTwo;
        }
        /// <summary>
        /// Find the number of empty & safe spaces between rook and King with a specified direction;
        /// -1 for left and 1 for right;
        /// enemyPlayer is used for PositionIsThreatened;
        /// Used to check castling.
        /// </summary>
        public int checkEmptySpaces(BoardPosition position, int dir, int enemyPlayer)
        {
            if (dir != -1 && dir != 1)
            {
                return -1;
            }
            int player = 0;
            if (enemyPlayer == 1) { player = 2; }
            else { player = 1; }

            int numEmptySpaces = 0;
            bool blocked = false;
            //Check if the spaces between rook and king is empty
            do
            {
                BoardPosition possiblyEmpty = position.Translate(0, dir * (numEmptySpaces + 1));
                if (PositionInBounds(possiblyEmpty))
                {
                    if (PositionIsEmpty(possiblyEmpty))
                    {
                        //Need to check if the King go through these spaces, the King will not be threatened in those spaces.
                        //Otherwise, castling is no longer valid.

                        //Move King Piece to check if threatened
                        SetPieceAtPosition(possiblyEmpty, new ChessPiece(ChessPieceType.King, player));
                        SetPieceAtPosition(position, ChessPiece.Empty);

                        if (!PositionIsThreatened(possiblyEmpty, enemyPlayer) || numEmptySpaces == 2)
                        {
                            numEmptySpaces++;
                        }
                        else
                        {
                            blocked = true;
                        }

                        //Move King Back
                        SetPieceAtPosition(position, new ChessPiece(ChessPieceType.King, player));
                        SetPieceAtPosition(possiblyEmpty, ChessPiece.Empty);
                    }
                    else
                    {
                        blocked = true;
                    }
                }
                else
                {
                    blocked = true;
                }
            } while (!blocked);
            return numEmptySpaces;
        }

        /// <summary>
        /// Get all possible moves from a specified knight; Return null if wrong piece.
        /// </summary>
        public IEnumerable<ChessMove> GetPossibleKnightMoves(BoardPosition position)
        {
            ChessPiece knight = GetPieceAtPosition(position);
            List<ChessMove> moves = new List<ChessMove>();
            int player = GetPlayerAtPosition(position);
            if (knight.PieceType != ChessPieceType.Knight)
            {
                return moves;
            }
            else
            {

                List<BoardPosition> endPosition = new List<BoardPosition>();
                ChessMove nMove;
                //All Possible Knight Moves
                endPosition.Add(position.Translate(2, 1));
                endPosition.Add(position.Translate(2, -1));
                endPosition.Add(position.Translate(-2, 1));
                endPosition.Add(position.Translate(-2, -1));
                endPosition.Add(position.Translate(1, 2));
                endPosition.Add(position.Translate(1, -2));
                endPosition.Add(position.Translate(-1, 2));
                endPosition.Add(position.Translate(-1, -2));

                for (int i = 0; i < endPosition.Count; i++)
                {
                    if (PositionInBounds(endPosition[i]))
                    {
                        //Valid if empty or enemy piece, but not your own piece
                        if ((PositionIsEnemy(endPosition[i], player)) || (PositionIsEmpty(endPosition[i])))
                        {
                            nMove = new ChessMove(position, endPosition[i]);
                            nMove.Player = player;
                            moves.Add(nMove);
                            //moves.Add(new ChessMove(position, endPosition[i]));
                        }
                    }
                }
                return moves;
            }
        }

        /// <summary>
        /// Returns the advantage point received when capturing the given piece
        /// </summary>
        /// <param name="pieceType"> The piece being captured</param>
        /// <returns></returns>
        public int GetAdvantagePoint(ChessPieceType pieceType)
        {
            if (ChessPieceType.Pawn == pieceType)
            {
                return 1;
            }
            else if ((ChessPieceType.Knight == pieceType) || (ChessPieceType.Bishop == pieceType))
            {
                return 3;
            }
            else if (ChessPieceType.Rook == pieceType)
            {
                return 5;
            }
            else if (ChessPieceType.Queen == pieceType)
            {
                return 9;
            }
            else
            {
                return -1;
            }

        }

        /// <summary>
        /// Update the advantage based on chess move
        /// </summary>
        /// <param name="point"> Point difference due to the chess move</param>
        private void UpdateAdvantage(int point, int player)
        {
            if (!advantageCalculateFlag)
            {
                int enemyPlayer = (player == 1) ? 2 : 1;

                //int piecePoint = (int)GetPieceAtPosition(m.EndPosition).PieceType;

                if (CurrentAdvantage.Player == 0)
                {
                    CurrentAdvantage = new GameAdvantage(player, point);
                }
                else if (CurrentAdvantage.Player == player)
                {
                    int currentPoints = CurrentAdvantage.Advantage;
                    CurrentAdvantage = new GameAdvantage(player, currentPoints + point);
                }
                else
                {
                    int currentPoints = CurrentAdvantage.Advantage;
                    int difference = currentPoints - point;
                    if (difference < 0)
                    {
                        CurrentAdvantage = new GameAdvantage(player, Math.Abs(difference));
                    }
                    else if (difference == 0)
                    {
                        CurrentAdvantage = new GameAdvantage(0, 0);
                    }
                    else
                    {
                        CurrentAdvantage = new GameAdvantage(enemyPlayer, difference);
                    }
                }
            }
        }

        /// <summary>
        /// Check if a pawn has moved.
        /// </summary>
        /// Can't use the same logic since rooks and Kings can go back to their initial positions
        private Boolean hasMoved(BoardPosition pos)
        {
            ChessPiece piece = GetPieceAtPosition(pos);
            int player = GetPlayerAtPosition(pos);
            if (piece.PieceType == ChessPieceType.Pawn)
            {
                bool isMoved = true;
                if (player == 1)
                {
                    foreach (BoardPosition p in initialWhitePawn)
                    {
                        if (pos.Equals(p))
                        {
                            isMoved = false;
                            break;
                        }
                    }
                }
                else if (player == 2)
                {
                    foreach (BoardPosition p in initialBlackPawn)
                    {
                        if (pos.Equals(p))
                        {
                            isMoved = false;
                            break;
                        }
                    }
                }

                return isMoved;
            }
            return false;
        }

        #endregion

        #region Explicit IGameBoard implementations.
        IEnumerable<IGameMove> IGameBoard.GetPossibleMoves()
        {
            return GetPossibleMoves();
        }
        void IGameBoard.ApplyMove(IGameMove m)
        {
            ApplyMove(m as ChessMove);
        }
        IReadOnlyList<IGameMove> IGameBoard.MoveHistory => mMoveHistory;

        
        /// <summary>
        /// One of the method to calculate the board weight. Calculates the point 
        /// based on the "player" pawn movement. 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private int PawnMovementPoint(int player)
        {
            int point = 0;
            int startingRow = player == 1 ? 6 : 1;
            IEnumerable<BoardPosition> pawnPositions = GetPositionsOfPiece(ChessPieceType.Pawn, player);
            foreach(BoardPosition pos in pawnPositions)
            {
                point += Math.Abs(pos.Row - startingRow);
            }

            return point;
        }

        private long ThreatenPoint(int player)
        {
            long point = 0;
            var threatenedPositions = GetAttackedPositions(player);
            foreach(BoardPosition pos in threatenedPositions)
            {
                ChessPieceType piece = GetPieceAtPosition(pos).PieceType;
                if((piece == ChessPieceType.Knight)||(piece == ChessPieceType.Bishop))
                {
                    point += 1;
                }else if (piece == ChessPieceType.Rook)
                {
                    point += 2;
                }else if(piece == ChessPieceType.Rook)
                {
                    point += 5;
                }else if(piece == ChessPieceType.King)
                {
                    point += 4;
                }
            }
            return point;
        }

         
        #endregion

        // You may or may not need to add code to this constructor.
        public ChessBoard()
        {
            initialWhitePawn = GetPositionsOfPiece(ChessPieceType.Pawn, 1);
            initialBlackPawn = GetPositionsOfPiece(ChessPieceType.Pawn, 2);
            hasWhiteRookMoved.Add(false);
            hasWhiteRookMoved.Add(false);
            hasBlackRookMoved.Add(false);
            hasBlackRookMoved.Add(false);

            whenWhiteRookMoved.Add(-1);
            whenWhiteRookMoved.Add(-1);
            whenBlackRookMoved.Add(-1);
            whenBlackRookMoved.Add(-1);

            DrawHistory.Add(0);

        }

        public ChessBoard(IEnumerable<Tuple<BoardPosition, ChessPiece>> startingPositions)
            : this()
        {


            var king1 = startingPositions.Where(t => t.Item2.Player == 1 && t.Item2.PieceType == ChessPieceType.King);
            var king2 = startingPositions.Where(t => t.Item2.Player == 2 && t.Item2.PieceType == ChessPieceType.King);
            if (king1.Count() != 1 || king2.Count() != 1)
            {
                throw new ArgumentException("A chess board must have a single king for each player");
            }



            initialWhitePawn = GetPositionsOfPiece(ChessPieceType.Pawn, 1);
            initialBlackPawn = GetPositionsOfPiece(ChessPieceType.Pawn, 2);

            foreach (var position in BoardPosition.GetRectangularPositions(8, 8))
            {
                SetPieceAtPosition(position, ChessPiece.Empty);
            }
            /*//Remove All Black Pieces
            for (int i =0; i < 2; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    BoardPosition pos = new BoardPosition(i, j);
                    SetPieceAtPosition(pos, GetPieceAtPosition(pos));
                }
            }

            //Remove All White Pieces
            for (int i = 6; i < 7; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    BoardPosition pos = new BoardPosition(i, j);
                    SetPieceAtPosition(pos, GetPieceAtPosition(pos));
                }
            }*/


            //hasWhiteRookMoved.Add(true);
            //hasWhiteRookMoved.Add(true);
            //hasBlackRookMoved.Add(true);
            //hasBlackRookMoved.Add(true);

            hasWhiteRookMoved[0] = true;
            hasWhiteRookMoved[1] = true;
            hasBlackRookMoved[0] = true;
            hasBlackRookMoved[1] = true;

            whenWhiteRookMoved.Add(-1);
            whenWhiteRookMoved.Add(-1);
            whenBlackRookMoved.Add(-1);
            whenBlackRookMoved.Add(-1);

            hasWhiteKingMoved = true;
            hasBlackKingMoved = true;

            DrawHistory.Add(0);

            int[] values = { 0, 0 };
            foreach (var pos in startingPositions)
            {
                SetPieceAtPosition(pos.Item1, pos.Item2);
                UpdateAdvantage(GetAdvantagePoint(pos.Item2.PieceType), pos.Item2.Player);
                // TODO: you must calculate the overall advantage for this board, in terms of the pieces
                // that the board has started with. "pos.Item2" will give you the chess piece being placed
                // on this particular position.
            }

            //Check if rook and Kings have moved
            if (GetPieceAtPosition(new BoardPosition(7, 0)).PieceType == ChessPieceType.Rook)
            {
                if (GetPieceAtPosition((new BoardPosition(7, 0))).Player == 1)
                {
                    hasWhiteRookMoved[0] = false;
                }
            }
            if (GetPieceAtPosition(new BoardPosition(7, 7)).PieceType == ChessPieceType.Rook)
            {
                if (GetPieceAtPosition((new BoardPosition(7, 7))).Player == 1)
                {
                    hasWhiteRookMoved[1] = false;
                }
            }
            if (GetPieceAtPosition(new BoardPosition(0, 0)).PieceType == ChessPieceType.Rook)
            {
                if (GetPieceAtPosition((new BoardPosition(0, 0))).Player == 2)
                {
                    hasBlackRookMoved[0] = false;
                }
            }
            if (GetPieceAtPosition(new BoardPosition(0, 7)).PieceType == ChessPieceType.Rook)
            {
                if (GetPieceAtPosition((new BoardPosition(0, 7))).Player == 2)
                {
                    hasBlackRookMoved[1] = false;
                }
            }
            if (GetPieceAtPosition(new BoardPosition(7, 4)).PieceType == ChessPieceType.King)
            {
                if (GetPieceAtPosition((new BoardPosition(7, 4))).Player == 1)
                {
                    hasWhiteKingMoved = false;
                }
            }
            if (GetPieceAtPosition(new BoardPosition(0, 4)).PieceType == ChessPieceType.King)
            {
                if (GetPieceAtPosition((new BoardPosition(0, 4))).Player == 2)
                {
                    hasBlackKingMoved = false;
                }
            }
        }
        /*
        public ChessBoard(List<Tuple<BoardPosition,ChessPiece>> pieces)
        {
            initialWhitePawn = GetPositionsOfPiece(ChessPieceType.Pawn, 1);
            initialBlackPawn = GetPositionsOfPiece(ChessPieceType.Pawn, 2);

            hasWhiteRookMoved.Add(true);
            hasWhiteRookMoved.Add(true);
            hasBlackRookMoved.Add(true);
            hasBlackRookMoved.Add(true);

            whenWhiteRookMoved.Add(-1);
            whenWhiteRookMoved.Add(-1);
            whenBlackRookMoved.Add(-1);
            whenBlackRookMoved.Add(-1);

            hasWhiteKingMoved = true;
            hasBlackKingMoved = true;

            foreach(Tuple<BoardPosition,ChessPiece> t in pieces)
            {
                //UpdateAdvantage((int)t.Item2.PieceType, GetPlayerAtPosition(t.Item1));
                SetPieceAtPosition(t.Item1, GetPieceAtPosition(t.Item1));//Remove the piece, if any, at that position
                SetPieceAtPosition(t.Item1, t.Item2);//Add the piece at that position
            }
            
            
            int missingWhitePawnPoints = (8 - GetPositionsOfPiece(ChessPieceType.Pawn, 1).Count())*(int)ChessPieceType.Pawn;
            int missingWhiteRookPoints = (2 - GetPositionsOfPiece(ChessPieceType.Rook, 1).Count()) * (int)ChessPieceType.Rook;
            int missingWhiteKnightPoints = (2 - GetPositionsOfPiece(ChessPieceType.Knight, 1).Count()) * (int)ChessPieceType.Knight;
            int missingWhiteBishopPoints = (2 - GetPositionsOfPiece(ChessPieceType.Bishop, 1).Count()) * (int)ChessPieceType.Bishop;
            int missingWhiteQueenPoints = (1 - GetPositionsOfPiece(ChessPieceType.Queen, 1).Count()) * (int)ChessPieceType.Queen;

            int whiteMissingPieceTotal = missingWhitePawnPoints + missingWhiteRookPoints + missingWhiteKnightPoints + missingWhiteBishopPoints + missingWhiteQueenPoints;

            int missingBlackPawnPoints = (8 - GetPositionsOfPiece(ChessPieceType.Pawn, 2).Count()) * (int)ChessPieceType.Pawn;
            int missingBlackRookPoints = (2 - GetPositionsOfPiece(ChessPieceType.Rook, 2).Count()) * (int)ChessPieceType.Rook;
            int missingBlackKnightPoints = (2 - GetPositionsOfPiece(ChessPieceType.Knight, 2).Count()) * (int)ChessPieceType.Knight;
            int missingBlackBishopPoints = (2 - GetPositionsOfPiece(ChessPieceType.Bishop, 2).Count()) * (int)ChessPieceType.Bishop;
            int missingBlackQueenPoints = (1 - GetPositionsOfPiece(ChessPieceType.Queen, 2).Count()) * (int)ChessPieceType.Queen;

            int blackMissingPieceTotal = missingBlackPawnPoints + missingBlackRookPoints + missingBlackKnightPoints + missingBlackBishopPoints + missingBlackQueenPoints;

            int difference = whiteMissingPieceTotal - blackMissingPieceTotal;

            if(difference == 0) 
            {
                CurrentAdvantage = new GameAdvantage(0, 0);
            }
            else if(difference>0)
            {
                CurrentAdvantage = new GameAdvantage(2, difference);
            }
            else
            {
                CurrentAdvantage = new GameAdvantage(1, Math.Abs(difference));
            }
        }*/
    }
}
