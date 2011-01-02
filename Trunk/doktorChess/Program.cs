using System;

namespace doktorChess
{
    class Program
    {
        static void Main(string[] args)
        {
            Board myBoard = Board.makeQueenAndPawnsStartPosition();

            pieceColour toPlay = pieceColour.white;

            while (true)
            {
                Console.WriteLine(myBoard.ToString());

                lineAndScore bestMove = myBoard.findBestMove(toPlay);
                Console.WriteLine(string.Format("Best line for {0}: {1}", toPlay, bestMove.ToString(moveStringStyle.chessNotation)));
                Console.WriteLine("{0} boards scored in {1} ms, {2}/sec. {3} ms in board scoring.", myBoard.stats.boardsScored, myBoard.stats.totalSearchTime, myBoard.stats.scoredPerSecond, myBoard.stats.boardScoreTime );
                move bestFirstMove = bestMove.line[0];
                myBoard.doMove(bestFirstMove);

                // Check if the game is over
                gameStatus status = myBoard.getGameStatus(pieceColour.white);
                if (status != gameStatus.inProgress)
                {
                    Console.WriteLine("Game over: white has " + status);
                    Console.WriteLine(myBoard.ToString());
                    break;
                }

                // Change player
                toPlay = Board.getOtherSide(toPlay);
            }
        }
    }
}
