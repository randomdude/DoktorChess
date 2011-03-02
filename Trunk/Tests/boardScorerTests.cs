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

            // position is lost for white..
            BoardScorer whiteScorer = new BoardScorer(pawnAt0, pieceColour.white, new scoreModifiers());
            Assert.AreEqual(BoardScorer.lowest, whiteScorer.getScore());

            // and won for black.
            BoardScorer blackScorer = new BoardScorer(pawnAt0, pieceColour.black, new scoreModifiers());
            Assert.AreEqual(BoardScorer.highest, blackScorer.getScore());

            // Now the white pawn at rank 7.
            Board pawnAt7 = new Board(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());
            pawnAt7.addPiece(pieceType.pawn, pieceColour.white, 1, 7);

            whiteScorer = new BoardScorer(pawnAt7, pieceColour.white, new scoreModifiers());
            Assert.AreEqual(BoardScorer.highest, whiteScorer.getScore());
            blackScorer = new BoardScorer(pawnAt7, pieceColour.black, new scoreModifiers());
            Assert.AreEqual(BoardScorer.lowest, blackScorer.getScore());
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
            Assert.IsTrue(ourboard.getGameStatus(pieceColour.black) != gameStatus.drawn);

            BoardScorer whiteScorer = new BoardScorer(ourboard, pieceColour.white, new scoreModifiers());
            Assert.AreEqual(0, whiteScorer.getScore());
            BoardScorer blackScorer = new BoardScorer(ourboard, pieceColour.black, new scoreModifiers());
            Assert.AreNotEqual(0, blackScorer.getScore());
        }
    }
}