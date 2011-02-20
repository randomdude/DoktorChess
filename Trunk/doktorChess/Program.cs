using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace doktorChess
{
    class Program
    {
        static void Main(string[] args)
        {
            //Board myBoard = Board.makeQueenAndPawnsStartPosition();
            boardSearchConfig config = new boardSearchConfig();
            config.searchDepth = 4;
            Board myBoard = Board.makeNormalStartPosition(config);

            pieceColour toPlay = pieceColour.white;

            List<move> moves = new List<move>();

            Debug.WriteLine("--- game start ---");

            killerMoveStore otherSideKS = null;
            killerMoveStore tmp = null;

            while (true)
            {
                Console.WriteLine(myBoard.ToString());
                //Console.WriteLine(myBoard.coverLevel.ToString());

                lineAndScore bestMove = myBoard.findBestMove(toPlay);
                myBoard.advanceKillerTables();

                //Console.WriteLine(string.Format("Best line for {0}: {1}", toPlay, bestMove.ToString(moveStringStyle.chessNotation)));
                //Console.WriteLine("{0} boards scored in {1} ms, {2}/sec. {3} ms in board scoring.", myBoard.stats.boardsScored, myBoard.stats.totalSearchTime, myBoard.stats.scoredPerSecond, myBoard.stats.boardScoreTime );

                //Console.Write(String.Format("{0},", myBoard.stats.boardsScored));

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

                // Store the killer store
                tmp = myBoard.killerStore;
                myBoard.killerStore = otherSideKS;
                otherSideKS = tmp;

                // Change player
                toPlay = Board.getOtherSide(toPlay);
            }
        }
    }
}
