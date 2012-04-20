using System;
using doktorChessGameEngine;

namespace doktorChess
{
    class Program
    {
        static void Main(string[] args)
        {
            //Board myBoard = Board.makeQueenAndPawnsStartPosition();
            boardSearchConfig config = new boardSearchConfig();
            config.searchDepth = 4;
            //config.checkThreatMapLots = config.checkLots = true;
            Board myBoard = Board.makeNormalStartPosition(config);

            config = new boardSearchConfig();
            config.searchDepth = 1;
            //config.checkThreatMapLots = config.checkLots = true;
            Board sillyBoard = Board.makeNormalStartPosition(config);

            ChessPlayer player1 = new ChessPlayer(myBoard);
            ChessPlayer player2 = new ChessPlayer(sillyBoard);

            chessGame game = new chessGame(player1, player2);

            game.play();

            Console.WriteLine("Scores: ");
            Console.WriteLine("Player 1: " + player1.score);
            Console.WriteLine("Player 2: " + player1.score);
        }
    }
}
