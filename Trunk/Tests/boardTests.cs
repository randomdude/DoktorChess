using System.Collections.Generic;
using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class boardTests
    {
        [TestMethod]
        public void testCheckDetection()
        {
            Board ourBoard = new Board(gameType.normal);
            ourBoard.addPiece(1, 1, pieceType.rook, pieceColour.white);
            ourBoard.addPiece(3, 1, pieceType.king, pieceColour.black);

            Assert.IsTrue(ourBoard.playerIsInCheck(pieceColour.white));
        }

        [TestMethod]
        public void testMoveUndoing()
        {
            Board ourBoard = Board.makeNormalStartPosition();

            string origBoard = ourBoard.ToString();

            List<move> moves = ourBoard.getMoves(pieceColour.white);

            ourBoard.doMove(moves[0]);

            if (ourBoard.ToString() == origBoard )
                throw new AssertFailedException("After a move, the board has not changed");

            ourBoard.undoMove(moves[0]);

            if (ourBoard.ToString() != origBoard)
                throw new AssertFailedException("After a move undo, the board has changed");
        }

        [TestMethod]
        public void testMoveUndoingEnPassant()
        {
            Board ourBoard = new Board(gameType.normal);
            square ourPawn = ourBoard.addPiece(7, 4, pieceType.pawn, pieceColour.white);
            square enemyPawn = ourBoard.addPiece(6, 6, pieceType.pawn, pieceColour.black);

            // Advance the enemy pawn, so we can capture it via en passant
            move advanceTwo = new move(enemyPawn, ourBoard[enemyPawn.position.down(2)]);
            ourBoard.doMove(advanceTwo);

            string origBoard = ourBoard.ToString();

            List<move> moves = ourBoard.getMoves(pieceColour.white);

            // Play our only capturing move
            move enPassant = moves.Find(a => a.isCapture == true);
            ourBoard.doMove(enPassant);

            if (ourBoard.ToString() == origBoard)
                throw new AssertFailedException("After a move, the board has not changed");

            ourBoard.undoMove(enPassant);

            if (ourBoard.ToString() != origBoard)
                throw new AssertFailedException("After a move undo, the board has changed");
        }
    }
}