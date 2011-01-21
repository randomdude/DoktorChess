using System.Collections.Generic;
using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class queensideCastlingMovementTests
    {
        [TestMethod]
        public void testQueensideCastlingMoveIsFound()
        {
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(4, 0, pieceType.king, pieceColour.white);
            ourBoard.addPiece(0, 0, pieceType.rook, pieceColour.white);
            ourBoard.addPiece(7, 7, pieceType.king, pieceColour.black);

            List<move> possibleMoves = ourKing.getPossibleMoves(ourBoard);

            // One of these moves should be a non-capturing move of the king to (2,0).
            List<move> castlingMoveList = possibleMoves.FindAll(a => !a.isCapture && a.dstPos.isSameSquareAs(new squarePos(2, 0)));

            Assert.AreNotEqual(0, castlingMoveList.Count, "Castling move was not found");
            Assert.AreEqual(1, castlingMoveList.Count, "Multiple castling moves were found");

            // Verify some other stuff on the move.
            move castlingMove = castlingMoveList[0];
            if (!castlingMove.srcPos.isSameSquareAs(ourKing.position))
                throw new AssertFailedException("Castling move has incorrect source square");
        }

        [TestMethod]
        public void testQueensideCastlingMoveIsFoundThroughCheck()
        {
            // It is legal to castle queenside if the space next to the rook is covered, since the king
            // does not move through it.
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(4, 0, pieceType.king, pieceColour.white);
            ourBoard.addPiece(0, 0, pieceType.rook, pieceColour.white);
            ourBoard.addPiece(7, 7, pieceType.king, pieceColour.black);

            // Throw in a rook which covers (1,0)
            ourBoard.addPiece(1, 7, pieceType.rook, pieceColour.black);

            List<move> possibleMoves = ourKing.getPossibleMoves(ourBoard);

            // One of these moves should be a non-capturing move of the king to (2,0).
            List<move> castlingMoveList = possibleMoves.FindAll(a => !a.isCapture && a.dstPos.isSameSquareAs(new squarePos(2, 0)));

            Assert.AreNotEqual(0, castlingMoveList.Count, "Castling move was not found");
            Assert.AreEqual(1, castlingMoveList.Count, "Multiple castling moves were found");

            // Verify some other stuff on the move.
            move castlingMove = castlingMoveList[0];
            if (!castlingMove.srcPos.isSameSquareAs(ourKing.position))
                throw new AssertFailedException("Castling move has incorrect source square");
        }

        [TestMethod]
        public void testQueensideCastlingMoveIsNotFoundThroughCheck()
        {
            // And we cannot castle through check apart from as noted above.
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(4, 0, pieceType.king, pieceColour.white);
            ourBoard.addPiece(0, 0, pieceType.rook, pieceColour.white);
            ourBoard.addPiece(7, 7, pieceType.king, pieceColour.black);
            ourBoard.addPiece(3, 7, pieceType.rook, pieceColour.black);

            List<move> possibleMoves = ourKing.getPossibleMoves(ourBoard);

            // None of these moves should end up at (2, 0).
            if (possibleMoves.Find(a => a.dstPos.isSameSquareAs(new squarePos(2, 0))) != null)
                throw new AssertFailedException("Castling found through check");
        }

        [TestMethod]
        public void testQueensideCastlingMoveIsNotFoundThroughAPiece()
        {
            // Place an enemy pawn in the way which will prevent us from castling.
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(4, 0, pieceType.king, pieceColour.white);
            ourBoard.addPiece(0, 0, pieceType.rook, pieceColour.white);
            ourBoard.addPiece(1, 0, pieceType.pawn, pieceColour.black);
            ourBoard.addPiece(7, 7, pieceType.king, pieceColour.black);

            List<move> possibleMoves = ourKing.getPossibleMoves(ourBoard);

            // None of these moves should end up at (2, 0).
            if (possibleMoves.Find(a => a.dstPos.isSameSquareAs(new squarePos(2, 0))) != null)
                throw new AssertFailedException("Castling found through an enemy piece");
        }
    }
}