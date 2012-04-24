using System;
using System.Collections.Generic;
using doktorChess;
using doktorChessGameEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class boardScorerTests
    {
        [TestMethod]
        public void testMaterialAdvantage()
        {
            // Generate a boardScorer and present it with a queen (ours), and two pawns
            // (enemy). Verify the resultant score as 8-2 = 6.

            Board ourboard = new Board(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());

            ourboard.addPiece(pieceType.queen, pieceColour.white, 1, 1);
            ourboard.addPiece(pieceType.pawn, pieceColour.black, 3, 4);
            ourboard.addPiece(pieceType.pawn, pieceColour.black, 3, 5);

            BoardScorer myscorer = new BoardScorer(ourboard, pieceColour.white, new scoreModifiers());

            // Check only material advantage
            myscorer.modifiers.danglingModifier = 0;
            myscorer.modifiers.materialModifier = 1;
            Assert.AreEqual(8 - 2, myscorer.getScore());
        }

        [TestMethod]
        public void testScoreWithDangling()
        {
            Board ourboard = new Board(gameType.normal, boardSearchConfig.getDebugConfig());
            ourboard.addPiece(pieceType.pawn, pieceColour.black, 3, 3);
            ourboard.addPiece(pieceType.queen, pieceColour.white, 2, 2);

            ourboard.addPiece(pieceType.king, pieceColour.black, 7, 7);
            ourboard.addPiece(pieceType.king, pieceColour.white, 5, 5);

            BoardScorer whiteScorer = new BoardScorer(ourboard, pieceColour.white, new scoreModifiers());

            // White's queen is dangling, as is blacks pawn.
            int expected = whiteScorer.modifiers.materialModifier * (8 - 1);
            expected -= whiteScorer.modifiers.danglingModifier * 8;
            expected += whiteScorer.modifiers.danglingModifier * 1;

            Assert.AreEqual(expected, whiteScorer.getScore());
        }

        [TestMethod]
        public void testFinishedGameScoreNoPieces()
        {
            // Generate a board which is lost via the 'no pieces remain' rule, and verify
            // we get the correct score.
            Board ourboard = new Board(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());
            ourboard.addPiece(pieceType.pawn, pieceColour.black, 1, 1);

            // position is lost for white..
            BoardScorer whiteScorer = new BoardScorer(ourboard, pieceColour.white, new scoreModifiers());
            Assert.AreEqual(BoardScorer.lowest, whiteScorer.getScore());

            // and won for black.
            BoardScorer blackScorer = new BoardScorer(ourboard, pieceColour.black, new scoreModifiers());
            Assert.AreEqual(BoardScorer.highest, blackScorer.getScore());
        }

        [TestMethod]
        public void testFinishedGameScorePawnToOtherEnd()
        {
            // We make two different boards here to test two different scenarios - if a black
            // pawn is at rank 0 and a white at rank 7.
            Board pawnAt0 = new Board(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());
            pawnAt0.addPiece(pieceType.pawn, pieceColour.black, 1, 0);

            // Should be a black win.
            verifyWonForWhite(pawnAt0, pieceColour.black);

            // Now the white pawn at rank 7.
            Board pawnAt7 = new Board(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());
            pawnAt7.addPiece(pieceType.pawn, pieceColour.white, 1, 7);

            // Should be a white win.
            verifyWonForWhite(pawnAt7, pieceColour.white);
        }

        [TestMethod]
        public void testFinishedGameScoreStalemate()
        {
            // Generate a board two pawns, deadlocked in front of each other. This should
            // be a draw via stalemate. Add a third pawn to ensure that stalemate is causing
            // the '0' board score, not a materian mismatch.
            Board ourboard = new Board(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());
            // Two deadlocked pawns
            ourboard.addPiece(pieceType.pawn, pieceColour.white, 1, 2);
            ourboard.addPiece(pieceType.pawn, pieceColour.black, 1, 3);
            // an outlier pawn
            ourboard.addPiece(pieceType.pawn, pieceColour.black, 4, 4);

            Assert.IsTrue(ourboard.getGameStatus(pieceColour.white) == gameStatus.drawn);
            Assert.IsTrue(ourboard.getGameStatus(pieceColour.black) == gameStatus.drawn);

            BoardScorer whiteScorer = new BoardScorer(ourboard, pieceColour.white, new scoreModifiers());
            Assert.AreEqual(0, whiteScorer.getScore());
            BoardScorer blackScorer = new BoardScorer(ourboard, pieceColour.black, new scoreModifiers());
            Assert.AreEqual(0, blackScorer.getScore());
        }

        [TestMethod]
        public void testFinishedGameScoreStalemate_example()
        {
            // Specific situation that was broken. It's black-to-play stalemate.
            Board ourboard = Board.makeNormalFromFEN("5B2/6P1/8/1p6/1N6/kP6/2K5/8 b - - 0 0",
                                                     boardSearchConfig.getDebugConfig());

            Assert.IsTrue(ourboard.getGameStatus(pieceColour.white) == gameStatus.drawn);
            Assert.IsTrue(ourboard.getGameStatus(pieceColour.black) == gameStatus.drawn);

            BoardScorer whiteScorer = new BoardScorer(ourboard, pieceColour.white, new scoreModifiers());
            Assert.AreEqual(0, whiteScorer.getScore());
            BoardScorer blackScorer = new BoardScorer(ourboard, pieceColour.black, new scoreModifiers());
            Assert.AreEqual(0, blackScorer.getScore());
        }

        [TestMethod] 
        public void verifyFiftyMoveRule()
        {
            Board ourBoard = Board.makeNormalStartPosition();
            verifyFiftyMoveRule(ourBoard, 0);
        }

        [TestMethod]
        public void verifyFiftyMoveRule_afterMoves()
        {
            Board ourBoard = Board.makeNormalStartPosition();

            // Play Nf3
            ourBoard.doMove(new move(ourBoard[6, 0], ourBoard[5, 2]));
            // Play Nf6
            ourBoard.doMove(new move(ourBoard[6, 7], ourBoard[5, 5]));

            verifyFiftyMoveRule(ourBoard, 2);
        }

        [TestMethod]
        public void verifyFiftyMoveRule_afterPawnMoves()
        {
            Board ourBoard = Board.makeNormalStartPosition();

            // Play Nf3
            ourBoard.doMove(new move(ourBoard[6, 0], ourBoard[5, 2]));
            // Play Nf6
            ourBoard.doMove(new move(ourBoard[6, 7], ourBoard[5, 5]));
            // A4
            ourBoard.doMove(new move(ourBoard[0, 1], ourBoard[0, 2]));
            // A6
            ourBoard.doMove(new move(ourBoard[0, 6], ourBoard[0, 5]));

            verifyFiftyMoveRule(ourBoard, 0);
        }

        public void verifyFiftyMoveRule(Board ourBoard, int moveOffset)
        {

            for (int n = 0; n < 100 - moveOffset; n++)
            {
                Assert.AreEqual(gameStatus.inProgress, ourBoard.getGameStatus(pieceColour.white), "Game declared drawn at move " + n.ToString());

                switch (n % 4)
                {
                    case 0:
                        // Play Nc3
                        ourBoard.doMove(new move(ourBoard[1, 0], ourBoard[2, 2]));
                        break;
                    case 1:
                        // Play Nc6
                        ourBoard.doMove(new move(ourBoard[1, 7], ourBoard[2, 5]));
                        break;
                    case 2:
                        // And move knights back again.
                        // Nb1
                        ourBoard.doMove(new move(ourBoard[2, 2], ourBoard[1, 0]));
                        break;
                    case 3:
                        // nb8
                        ourBoard.doMove(new move(ourBoard[2, 5], ourBoard[1, 7]));
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            // 50 moves have elapsed! It's a draw!
            Assert.AreEqual(gameStatus.drawn, ourBoard.getGameStatus(pieceColour.white));
        }

        [TestMethod]
        public void testFinishedGameScore_example1()
        {
            // This specific situation was being mis-detected as not a win, so here's a unit test to sort it out.
            testFinishedGameScore_example(@"8/8/8/8/8/8/4KPk1/R6Q b - - 0 1");
        }

        [TestMethod]
        public void testFinishedGameScore_example2()
        {
            // This specific situation was also being mis-detected as not a win, so here's a unit test to sort it out.
            testFinishedGameScore_example(@"R6k/6rp/5B2/8/8/8/7P/7K b - - 0 1");
        }
        
        public void testFinishedGameScore_example(string toTestFEN)
        {
            // Verify a situation is a win for white from a FEN.
            Board ourBoard = Board.makeNormalFromFEN(toTestFEN, new boardSearchConfig());

            // Game should be won for white and lost for black.
            verifyWonForWhite(ourBoard, pieceColour.white);
        }

        private void verifyWonForWhite(Board ourBoard, pieceColour wonCol)
        {
            pieceColour lostCol = Board.getOtherSide(wonCol);

            // The position should be won/lost for white/black, respectively
            Assert.IsTrue(ourBoard.getGameStatus(wonCol) == gameStatus.won);
            Assert.IsTrue(ourBoard.getGameStatus(lostCol) == gameStatus.lost);

            // and this should be reflected in the scores
            BoardScorer whiteScorer = new BoardScorer(ourBoard, wonCol, new scoreModifiers());
            Assert.AreEqual(BoardScorer.highest, whiteScorer.getScore());

            // and won for black.
            BoardScorer blackScorer = new BoardScorer(ourBoard, lostCol, new scoreModifiers());
            Assert.AreEqual(BoardScorer.lowest, blackScorer.getScore());            
        }
    }
}