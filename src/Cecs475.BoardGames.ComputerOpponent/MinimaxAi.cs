using Cecs475.BoardGames.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cecs475.BoardGames.ComputerOpponent {
	internal struct MinimaxBestMove {
		public long Weight { get; set; }
		public IGameMove Move { get; set; }
	}

	public class MinimaxAi : IGameAi {
		private int mMaxDepth;
		public MinimaxAi(int maxDepth) {
			mMaxDepth = maxDepth;
		}

		public IGameMove FindBestMove(IGameBoard b) {
			return FindBestMove(b,
				true ? int.MinValue : int.MaxValue,
				true ? int.MaxValue : int.MinValue,
				mMaxDepth).Move;
		}

		private static MinimaxBestMove FindBestMove(IGameBoard b, int alpha, int beta, int depthLeft) {
            //return new MinimaxBestMove() {
            //	Move = null
            //};
            return FindBestMove(b, depthLeft, true);
		}

        private static MinimaxBestMove FindBestMove(IGameBoard b, int depthLeft, bool isMaximizing)
        {

            if ((depthLeft == 0)||(b.IsFinished))
            {
                return new MinimaxBestMove()
                {
                    Weight = b.BoardWeight,
                    Move = null
                };
            }

            long bestWeight = isMaximizing == true ? long.MinValue : long.MaxValue;
            IGameMove bestMove = null;

            foreach (IGameMove move in b.GetPossibleMoves())
            {
                b.ApplyMove(move);
                MinimaxBestMove w = FindBestMove(b, depthLeft - 1, !isMaximizing);
                b.UndoLastMove();
                if((isMaximizing) && (w.Weight> bestWeight))
                {
                    bestWeight = w.Weight;
                    bestMove = move;
                }else if((!isMaximizing) && (w.Weight < bestWeight))
                {
                    bestWeight = w.Weight;
                    bestMove = move;
                }
            }

            return new MinimaxBestMove()
            {
                Weight = bestWeight,
                Move = bestMove
            };
        }

	}
}
