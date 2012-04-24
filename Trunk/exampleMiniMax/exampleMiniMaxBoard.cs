using System;
using System.Linq;
using System.Linq.Expressions;
using doktorChessGameEngine;

namespace exampleMiniMax
{
    [chessAIAttribute]
    public class exampleMiniMaxBoard : baseBoard
    {
        const int searchDepth = 2;

        public exampleMiniMaxBoard(gameType newType) : base(newType)
        {
            // This engine supports only 'normal' chess.
            if (newType != gameType.normal)
                throw new NotImplementedException();
        }

        public static exampleMiniMaxBoard makeNormalStartPosition()
        {
            exampleMiniMaxBoard newBoard = new exampleMiniMaxBoard(gameType.normal);

            newBoard.makeStartPosition();

            return newBoard;
        }
        
        public override lineAndScore findBestMove()
        {
            return findBestMove(true, searchDepth);
        }

        private lineAndScore findBestMove(bool usToPlay, int depthLeft)
        {
            lineAndScore bestLineSoFar;

            if (usToPlay)
                bestLineSoFar = new lineAndScore(new move[searchDepth + 1], int.MinValue, null);
            else
                bestLineSoFar = new lineAndScore(new move[searchDepth + 1], int.MaxValue, null);

            // Find a list of possible moves..
            sizableArray<move> movesToConsider = getMoves(colToMove);
            foreach (move consideredMove in movesToConsider)
            {
                pieceColour movingCol = colToMove;
                doMove(consideredMove);

                // If this move would leave us in check, we can ignore it
                // TODO: optimise this.
                if (playerIsInCheck(movingCol))
                {
                    undoMove(consideredMove);
                    continue;
                }

                if (depthLeft == 0)
                {
                    boardScorer scorer = new boardScorer(this, movingCol);
                    int score = scorer.getScore();

                    if ((usToPlay && (score > bestLineSoFar.finalScore)) ||
                        (!usToPlay && (score < bestLineSoFar.finalScore)))
                    {
                        bestLineSoFar.finalScore = score;
                        bestLineSoFar.line[searchDepth] = consideredMove;
                        bestLineSoFar._scorer = scorer;
                    }
                }
                else
                {
                    lineAndScore thisMove = findBestMove(!usToPlay, depthLeft - 1);

                    if ((usToPlay && (thisMove.finalScore > bestLineSoFar.finalScore)) ||
                        (!usToPlay && (thisMove.finalScore < bestLineSoFar.finalScore)))
                    {
                        bestLineSoFar.finalScore = thisMove.finalScore;
                        bestLineSoFar._scorer = thisMove._scorer;
                        bestLineSoFar.line[searchDepth - depthLeft] = consideredMove;
                        for (int index = 0; index < thisMove.line.Length; index++)
                        {
                            if (thisMove.line[index] != null)
                                bestLineSoFar.line[index] = thisMove.line[index];
                        }
                    }
                }

                undoMove(consideredMove);
            }

            return bestLineSoFar;
        }
    }
}