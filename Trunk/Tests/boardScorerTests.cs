﻿using System.Collections.Generic;
using doktorChess;
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

            List<square> us = new List<square>
                                  {
                                      new queenSquare(new squarePos(1, 1), pieceColour.white)
                                  };

            List<square> them = new List<square>
                                    {
                                        new pawnSquare(new squarePos(3, 4), pieceColour.black),
                                        new pawnSquare(new squarePos(3, 5), pieceColour.black)
                                    };

            BoardScorer myscorer = new BoardScorer(us, them);

            // TODO: change getScore to getMaterialAdvantage or the like for this test.
            Assert.AreEqual(8 - 2, myscorer.getScore());
        }

        [TestMethod]
        public void testFinishedGameScoreNoPieces()
        {
            // Generate a board which is lost via the 'no pieces remain' rule, and verify
            // we get the correct score.
            Board ourboard = new Board(gameType.queenAndPawns);
            ourboard.addPiece(1,1,pieceType.pawn, pieceColour.black);

            // position is lost for white..
            BoardScorer whiteScorer = new BoardScorer(ourboard, pieceColour.white);
            Assert.AreEqual(BoardScorer.lowest, whiteScorer.getScore());

            // and won for black.
            BoardScorer blackScorer = new BoardScorer(ourboard, pieceColour.black);
            Assert.AreEqual(BoardScorer.highest, blackScorer.getScore());
        }

        [TestMethod]
        public void testFinishedGameScorePawnToOtherEnd()
        {
            // We make two different boards here to test two different scenarios - if a black
            // pawn is at rank 0 and a white at rank 7.
            Board pawnAt0 = new Board(gameType.queenAndPawns);
            pawnAt0.addPiece(1, 0, pieceType.pawn, pieceColour.black);

            // position is lost for white..
            BoardScorer whiteScorer = new BoardScorer(pawnAt0, pieceColour.white);
            Assert.AreEqual(BoardScorer.lowest, whiteScorer.getScore());

            // and won for black.
            BoardScorer blackScorer = new BoardScorer(pawnAt0, pieceColour.black);
            Assert.AreEqual(BoardScorer.highest, blackScorer.getScore());

            // Now the white pawn at rank 7.
            Board pawnAt7 = new Board(gameType.queenAndPawns);
            pawnAt7.addPiece(1, 7, pieceType.pawn, pieceColour.white);

            whiteScorer = new BoardScorer(pawnAt7, pieceColour.white);
            Assert.AreEqual(BoardScorer.highest, whiteScorer.getScore());
            blackScorer = new BoardScorer(pawnAt7, pieceColour.black);
            Assert.AreEqual(BoardScorer.lowest, blackScorer.getScore());
        }

        [TestMethod]
        public void testFinishedGameScoreStalemate()
        {
            // Generate a board two pawns, deadlocked in front of each other. This should
            // be a draw via stalemate. Add a third pawn to ensure that stalemate is causing
            // the '0' board score.
            Board ourboard = new Board(gameType.queenAndPawns);
            ourboard.addPiece(1, 1, pieceType.pawn, pieceColour.white);
            ourboard.addPiece(1, 2, pieceType.pawn, pieceColour.white);
            ourboard.addPiece(1, 3, pieceType.pawn, pieceColour.black);

            Assert.IsTrue(ourboard.getGameStatus(pieceColour.white) == gameStatus.drawn);
            Assert.IsTrue(ourboard.getGameStatus(pieceColour.black) == gameStatus.drawn);

            BoardScorer whiteScorer = new BoardScorer(ourboard, pieceColour.white);
            Assert.AreEqual(0, whiteScorer.getScore());
            BoardScorer blackScorer = new BoardScorer(ourboard, pieceColour.black);
            Assert.AreEqual(0, blackScorer.getScore());
        }
    }
}