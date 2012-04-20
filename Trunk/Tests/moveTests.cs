using doktorChess;
using doktorChessGameEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class moveTests
    {
        [TestMethod]
        public void verifyMoveConstructorWithCapture()
        {
            // Make two occupied squares and construct a move from one to the other. Test
            // that the capture is correctly propagated to the move.
            square src = new pawnSquare(new squarePos(1, 1), pieceColour.black);
            square dst = new pawnSquare(new squarePos(2, 1), pieceColour.white);

            move captureMove = new move(src, dst);

            // First, check that the move has the correct squares associated with it..
            Assert.IsTrue(captureMove.srcPos.isSameSquareAs(src.position));
            Assert.IsTrue(captureMove.dstPos.isSameSquareAs(dst.position));

            Assert.IsTrue(captureMove.isCapture);
            Assert.IsTrue(captureMove.capturedSquare == dst);
        }

        [TestMethod]
        public void verifyLegalMoveIsLegal()
        {
            Board ourBoard = Board.makeQueenAndPawnsStartPosition(boardSearchConfig.getDebugConfig());
            move legalMove = new move(ourBoard[1, 1], ourBoard[1, 3]);
            Assert.IsTrue(legalMove.isLegal(ourBoard));
        }

        [TestMethod]
        public void verifyIllegalMoveIsIllegal()
        {
            Board ourBoard = Board.makeQueenAndPawnsStartPosition(boardSearchConfig.getDebugConfig());
            move illegalMove = new move(ourBoard[1, 1], ourBoard[1, 6]);
            Assert.IsFalse(illegalMove.isLegal(ourBoard));
        }

        [TestMethod]
        public void verifyMoveOfEmptySpaceIsIllegal()
        {
            Board ourBoard = Board.makeQueenAndPawnsStartPosition(boardSearchConfig.getDebugConfig());
            move illegalMove = new move(ourBoard[3, 3], ourBoard[4, 4]);
            Assert.IsFalse(illegalMove.isLegal(ourBoard));
        }
    }
}