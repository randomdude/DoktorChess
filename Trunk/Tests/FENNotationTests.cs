﻿using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class FENNotationTests
    {
        [TestMethod]
        public void testFENInitialPosition()
        {
            string testFENString = @"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

            Board ourBoard = Board.makeNormalFromFEN(testFENString, new boardSearchConfig());

            string expectedBoardString = Board.makeNormalStartPosition(new boardSearchConfig()).ToString(); ;

            Assert.AreEqual(expectedBoardString, ourBoard.ToString());
            Assert.AreEqual(pieceColour.white, ourBoard.colToMove);
        }

        [TestMethod]
        public void testFENInitialPositionAfterE4()
        {
            string testFENString = @"rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";

            Board ourBoard = Board.makeNormalFromFEN(testFENString, new boardSearchConfig());

            Board expectedBoard = Board.makeNormalStartPosition(new boardSearchConfig());
            expectedBoard.doMove(new move( expectedBoard[4, 1], expectedBoard[4, 3] ));
            string expectedBoardString = expectedBoard.ToString(); ;

            Assert.AreEqual(expectedBoardString, ourBoard.ToString());
            Assert.AreEqual(pieceColour.black, ourBoard.colToMove);
        }
    }
}