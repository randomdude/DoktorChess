using System;
using System.Collections.Generic;
using System.Diagnostics;
using doktorChessGameEngine;

namespace doktorChess
{
    public class chessGame
    {
        private ChessPlayer p1;
        private ChessPlayer p2;

        pieceColour toPlay;
        List<move> moves;

        public chessGame(ChessPlayer newp1, ChessPlayer newp2)
        {
            p1 = newp1;
            p2 = newp2;
        }

        public void play()
        {
            toPlay = pieceColour.white;
            moves = new List<move>();

            Debug.WriteLine("--- game start ---");

            ChessPlayer playerToPlay = p1;

            do
            {
                doMove(playerToPlay.board);

                toPlay = baseBoard.getOtherSide(toPlay);
                playerToPlay = playerToPlay == p1 ? p2 : p1;

            } while (playerToPlay.board.getGameStatus(toPlay) == gameStatus.inProgress);

            // See who won and allocate points
            gameStatus status = p1.board.getGameStatus(toPlay);
            switch (status)
            {
                case gameStatus.won:
                    p1.score++;
                    break;
                case gameStatus.drawn:
                    p1.score += 0.5f;
                    p2.score += 0.5f;
                    break;
                case gameStatus.lost:
                    p2.score++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // print some output
            Console.WriteLine("Game over. Final position:");
            Console.WriteLine(p1.ToString());
            Console.WriteLine("Game transcript:");
            foreach (move thisMove in moves)
                Console.WriteLine(thisMove.ToString(moveStringStyle.chessNotation));

        }

        private void doMove(baseBoard boardToMove)
        {
            lineAndScore bestMove = boardToMove.findBestMove();

            Console.WriteLine(string.Format("Best line for {0}: {1}", toPlay, bestMove.ToString(moveStringStyle.chessNotation)));
            Console.WriteLine(bestMove.ToString());
            //Console.WriteLine("{0} boards scored in {1} ms, {2}/sec. {3} ms in board scoring.", myBoard.stats.boardsScored, myBoard.stats.totalSearchTime, myBoard.stats.scoredPerSecond, myBoard.stats.boardScoreTime );

            //Console.Write(String.Format("{0},", myBoard.stats.boardsScored));
            //Console.Write(boardToMove.coverLevel.ToString());

            move bestFirstMove = bestMove.line[0];
            p1.board.doMove(bestFirstMove);
            p2.board.doMove(bestFirstMove);

            Console.WriteLine(boardToMove.ToString());

            moves.Add(bestFirstMove);
        }
    }
}