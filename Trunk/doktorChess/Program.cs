using System;
using System.Collections.Generic;

namespace doktorChess
{
    class Program
    {
        static void Main(string[] args)
        {
            Board myBoard = Board.makeQueenAndPawnsStartPosition();
            //Board myBoard = Board.makeNormalStartPosition();
            myBoard.searchDepth = 4;

            pieceColour toPlay = pieceColour.white;

            List<move> moves = new List<move>();

            while (true)
            {
                Console.WriteLine(myBoard.ToString());

                // Verify alpha-beta against minimax searches
                myBoard.alphabeta = true;
                //lineAndScore bestMoveAB = myBoard.findBestMove(toPlay);
                //myBoard.alphabeta = false;
                lineAndScore bestMove = myBoard.findBestMove(toPlay);

                //if (bestMoveAB.finalScore != bestMove.finalScore)
                //    throw new Exception("Final score of alpha-beta result differs from that of minimax result");

                //for (int index = 0; index < bestMoveAB.line.Length; index++)
                //{
                //    move moveAB = bestMoveAB.line[index];
                //    move move = bestMove.line[index];
                //    if (!moveAB.srcPos.isSameSquareAs(move.srcPos) ||
                //        !moveAB.dstPos.isSameSquareAs(move.dstPos))
                //    {
                //        throw new Exception("Alpha-beta result differs from minimax result");
                //    }
                //}

                Console.WriteLine(string.Format("Best line for {0}: {1}", toPlay, bestMove.ToString(moveStringStyle.chessNotation)));
                Console.WriteLine("{0} boards scored in {1} ms, {2}/sec. {3} ms in board scoring.", myBoard.stats.boardsScored, myBoard.stats.totalSearchTime, myBoard.stats.scoredPerSecond, myBoard.stats.boardScoreTime );
                move bestFirstMove = bestMove.line[0];
                myBoard.doMove(bestFirstMove);

                moves.Add(bestFirstMove);

                // Check if the game is over
                gameStatus status = myBoard.getGameStatus(pieceColour.white);
                if (status != gameStatus.inProgress)
                {
                    Console.WriteLine("Game over: white has " + status);
                    Console.WriteLine(myBoard.ToString());

                    foreach (move thisMove in moves)
                        Console.WriteLine(thisMove.ToString(moveStringStyle.chessNotation));
                    break;
                }

                // Change player
                toPlay = Board.getOtherSide(toPlay);
            }
        }
    }
}
