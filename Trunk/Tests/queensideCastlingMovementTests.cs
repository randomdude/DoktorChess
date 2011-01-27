using System.Collections.Generic;
using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class moveNotationTests
    {
        [TestMethod]
        public void testPieceCaptureNotation()
        {
            square bish = new bishopSquare(new squarePos(0, 0), pieceColour.black);
            square rook = new rookSquare(new squarePos(0, 1), pieceColour.white);

            move theMove = new move(bish, rook);

            Assert.AreEqual("BxR", theMove.ToString(moveStringStyle.chessNotation));
        }

        [TestMethod]
        public void testPieceMoveNotation()
        {
            square bish = new bishopSquare(new squarePos(0, 0), pieceColour.black);
            square targetSpace = new square( new squarePos(0, 1) );

            move theMove = new move(bish, targetSpace);

            Assert.AreEqual("Ba2", theMove.ToString(moveStringStyle.chessNotation));
        }

        [TestMethod]
        public void testKingsideCastlingNotation()
        {
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            ourBoard.addPiece(pieceType.rook, pieceColour.white, 7, 0);

            move theMove = new move(ourKing, new square(6, 0) );

            Assert.AreEqual("O-O", theMove.ToString(moveStringStyle.chessNotation));
        }

        [TestMethod]
        public void testQueensideCastlingNotation()
        {
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            ourBoard.addPiece(pieceType.rook, pieceColour.white, 0, 0);

            move theMove = new move(ourKing, new square(2, 0));

            Assert.AreEqual("O-O-O", theMove.ToString(moveStringStyle.chessNotation));
        }
    }

    [TestClass]
    public class queensideCastlingMovementTests
    {
        [TestMethod]
        public void testQueensideCastlingMoveIsFound()
        {
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            ourBoard.addPiece(pieceType.rook, pieceColour.white, 0, 0);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 7, 7);

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
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            ourBoard.addPiece(pieceType.rook, pieceColour.white, 0, 0);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 7, 7);

            // Throw in a rook which covers (1,0)
            ourBoard.addPiece(pieceType.rook, pieceColour.black, 1, 7);

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
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            ourBoard.addPiece(pieceType.rook, pieceColour.white, 0, 0);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 7, 7);
            ourBoard.addPiece(pieceType.rook, pieceColour.black, 3, 7);

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
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            ourBoard.addPiece(pieceType.rook, pieceColour.white, 0, 0);
            ourBoard.addPiece(pieceType.pawn, pieceColour.black, 1, 0);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 7, 7);

            List<move> possibleMoves = ourKing.getPossibleMoves(ourBoard);

            // None of these moves should end up at (2, 0).
            if (possibleMoves.Find(a => a.dstPos.isSameSquareAs(new squarePos(2, 0))) != null)
                throw new AssertFailedException("Castling found through an enemy piece");
        }

        [TestMethod]
        public void testQueensideCastlingMoveIsExecutedCorrectly()
        {
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            square ourRook = ourBoard.addPiece(pieceType.rook, pieceColour.white, 0, 0);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 7, 7);

            // Make our castling move..
            move castlingMove = new move(ourKing, ourBoard[2, 0]);
            ourBoard.doMove(castlingMove);

            // Verify that the rook and king have both moved to their correct squares.
            Assert.IsTrue(ourBoard[2, 0] == ourKing);
            Assert.IsTrue(ourBoard[3, 0] == ourRook);
        }

        [TestMethod]
        public void testQueensideCastlingMoveIsUnExecutedCorrectly()
        {
            Board ourBoard = new Board(gameType.normal);
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            ourBoard.addPiece(pieceType.rook, pieceColour.white, 0, 0);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 7, 7);

            string origBoard = ourBoard.ToString();

            // Make out castling move
            move castlingMove = new move(ourKing, ourBoard[2, 0]);
            ourBoard.doMove(castlingMove);

            Assert.AreNotEqual(origBoard, ourBoard.ToString(), "Castling did not affect the board");

            // Now undo our castling and verify that we get back to the original position.
            ourBoard.undoMove(castlingMove);

            Assert.AreEqual(origBoard, ourBoard.ToString(), "Castling and then un-castling did not return the original board");
        }
    
    }
}