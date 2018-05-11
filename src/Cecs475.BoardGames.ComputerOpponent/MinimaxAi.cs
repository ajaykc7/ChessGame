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
            return FindBestMove(b, depthLeft, b.CurrentPlayer==1, alpha, beta);
		}

        private static MinimaxBestMove FindBestMove(IGameBoard b, int depthLeft, bool isMaximizing, int alpha, int beta)
        {
            if (depthLeft == 0 || b.IsFinished)
            {
                return new MinimaxBestMove()
                {
                    Weight = b.BoardWeight,
                    Move = null
                };
            }

            long bestWeight = isMaximizing ? long.MinValue : long.MaxValue;
            IGameMove bestMove = null;

            foreach (IGameMove move in b.GetPossibleMoves())
            {
                b.ApplyMove(move);
                MinimaxBestMove w = FindBestMove(b, depthLeft - 1, !isMaximizing, alpha, beta);
                b.UndoLastMove();
                if(isMaximizing && w.Weight > bestWeight)
                {
                    bestWeight = w.Weight;
                    bestMove = move;
                    alpha = (int) w.Weight;
                    
                }else if(!isMaximizing && w.Weight < bestWeight)
                {
                    bestWeight = w.Weight;
                    bestMove = move;
                    beta = (int)w.Weight;
                }
                
                if(!(alpha < beta))
                {
                    if(isMaximizing)
                    {
                        return new MinimaxBestMove()
                        {
                            Weight = beta,
                            Move = bestMove
                        };
                    }
                    else
                    {
                        return new MinimaxBestMove()
                        {
                            Weight = alpha,
                            Move = bestMove
                        };
                    }
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
