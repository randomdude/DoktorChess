using System.Collections.Generic;
using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class KingsideCastlingMovementTests
    {
        [TestMethod]
        public void testKingsideCastlingMoveIsFound()
        {
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            ourBoard.addPiece(pieceType.rook, pieceColour.white, 7, 0);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 0, 0);

            List<move> possibleMoves = ourKing.getPossibleMoves(ourBoard);

            // One of these moves should be a non-capturing move of the king to (6,0).
            List<move> castlingMoveList = possibleMoves.FindAll(a => !a.isCapture && a.dstPos.isSameSquareAs(new squarePos(6, 0)));

            Assert.AreNotEqual(0, castlingMoveList.Count, "Castling move was not found");
            Assert.AreEqual(1, castlingMoveList.Count, "Multiple castling moves were found");

            // Verify some other stuff on the move.
            move castlingMove = castlingMoveList[0];
            if (!castlingMove.srcPos.isSameSquareAs(ourKing.position))
                throw new AssertFailedException("Castling move has incorrect source square");
        }

        [TestMethod]
        public void testKingsideCastlingMoveIsNotFoundThroughCheck()
        {
            // Place an enemy rook which will prevent us from castling.
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            ourBoard.addPiece(pieceType.rook, pieceColour.white, 7, 0);
            ourBoard.addPiece(pieceType.rook, pieceColour.black, 5, 7);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 0, 0);

            List<move> possibleMoves = ourKing.getPossibleMoves(ourBoard);

            // None of these moves should end up at (6, 0).
            if (possibleMoves.Find(a => a.dstPos.isSameSquareAs(new squarePos(6, 0))) != null)
                throw new AssertFailedException("Castling found through check");
        }

        [TestMethod]
        public void testKingsideCastlingMoveIsNotFoundThroughAPiece()
        {
            // Place an enemy pawn in the way which will prevent us from castling.
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            ourBoard.addPiece(pieceType.rook, pieceColour.white, 7, 0);
            ourBoard.addPiece(pieceType.pawn, pieceColour.black, 6, 0);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 0, 0);

            List<move> possibleMoves = ourKing.getPossibleMoves(ourBoard);

            // None of these moves should end up at (6, 0).
            if (possibleMoves.Find(a => a.dstPos.isSameSquareAs(new squarePos(6, 0))) != null)
                throw new AssertFailedException("Castling found through an enemy piece");
        }

        [TestMethod]
        public void testKingsideCastlingMoveIsNotAfterKingHasMoved()
        {
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 3, 0);
            ourBoard.addPiece(pieceType.rook, pieceColour.white, 7, 0);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 0, 0);

            // Move the king to 4,0.
            ourBoard.doMove(new move(ourKing, ourBoard[4,0]));

            // Now make sure we cannot castle.
            List<move> possibleMoves = ourKing.getPossibleMoves(ourBoard);

            // None of these moves should end up at (6,0).
            if (possibleMoves.Find(a => a.dstPos.isSameSquareAs(new squarePos(6,0))) != null)
                throw new AssertFailedException("Castling found after king has moved");
        }

        [TestMethod]
        public void testKingsideCastlingMoveIsNotAfterRookHasMoved()
        {
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            square ourRook = ourBoard.addPiece(pieceType.rook, pieceColour.white, 6, 0);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 0, 0);

            // Move the rook to 7,0.
            ourBoard.doMove(new move(ourRook, ourBoard[7,0]));

            // Now make sure we cannot castle.
            List<move> possibleMoves = ourKing.getPossibleMoves(ourBoard);

            // None of these moves should end up at (6,0).
            if (possibleMoves.Find(a => a.dstPos.isSameSquareAs(new squarePos(6, 0))) != null)
                throw new AssertFailedException("Castling found after rook has moved");
        }

        [TestMethod]
        public void testKingsideCastlingMoveIsExecutedCorrectly()
        {
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            square ourRook = ourBoard.addPiece(pieceType.rook, pieceColour.white, 7, 0);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 0, 0);

            // Make our castling move..
            move castlingMove = new move(ourKing, ourBoard[6, 0]) ;
            ourBoard.doMove(castlingMove);

            // Verify that the rook and king have both moved to their correct squares.
            Assert.IsTrue(ourBoard[6, 0] == ourKing);
            Assert.IsTrue(ourBoard[5, 0] == ourRook);
        }

        [TestMethod]
        public void testKingsideCastlingMoveIsUnExecutedCorrectly()
        {
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            square ourRook = ourBoard.addPiece(pieceType.rook, pieceColour.white, 7, 0);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 0, 0);

            string origBoard = ourBoard.ToString();

            // Make out castling move
            move castlingMove = new move(ourKing, ourBoard[6, 0]);
            ourBoard.doMove(castlingMove);

            Assert.AreNotEqual(origBoard, ourBoard.ToString(), "Castling did not affect the board");

            // Now undo our castling and verify that we get back to the original position.
            ourBoard.undoMove(castlingMove);

            Assert.AreEqual(origBoard, ourBoard.ToString(), "Castling and then un-castling did not return the original board");        
        }
    }
}