using System;
using System.Collections.Generic;
using System.Diagnostics;
using doktorChess;
using doktorChessGameEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class boardTests
    {
        [TestMethod]
        public void testCheckDetectionAsBlack()
        {
            DoktorChessAIBoard ourBoard = new DoktorChessAIBoard(gameType.normal, boardSearchConfig.getDebugConfig());
            ourBoard.addPiece(pieceType.rook, pieceColour.white, 1, 1);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 3, 1);

            Assert.IsTrue(ourBoard.isPlayerInCheck(pieceColour.black));
        }

        [TestMethod]
        public void testCheckDetectionAsWhite()
        {
            DoktorChessAIBoard ourBoard = new DoktorChessAIBoard(gameType.normal, boardSearchConfig.getDebugConfig());
            ourBoard.addPiece(pieceType.rook, pieceColour.black, 1, 1);
            ourBoard.addPiece(pieceType.king, pieceColour.white, 3, 1);

            Assert.IsTrue(ourBoard.isPlayerInCheck(pieceColour.white));
        }

        [TestMethod]
        public void testMoveUndoing()
        {
            DoktorChessAIBoard ourBoard = DoktorChessAIBoard.makeNormalStartPosition(boardSearchConfig.getDebugConfig());

            string origBoard = ourBoard.ToString();

            sizableArray<move> moves = ourBoard.getMoves(pieceColour.white);
            if (moves.Length == 0)
                Assert.Inconclusive("No moves found");

            foreach (move thisMove in moves)
            {
                ourBoard.doMove(thisMove);

                if (ourBoard.ToString() == origBoard)
                    throw new AssertFailedException("After a move, the board has not changed");

                ourBoard.undoMove(thisMove);

                if (ourBoard.ToString() != origBoard)
                    throw new AssertFailedException("After a move undo, the board has changed");
            }
        }


        [TestMethod]
        public void testMoveDoingUndoingWithPawnPromotion()
        {
            DoktorChessAIBoard ourBoard = new DoktorChessAIBoard(gameType.normal, boardSearchConfig.getDebugConfig());
            ourBoard.addPiece(pieceType.pawn, pieceColour.white, 1, 6);

            string origBoard = ourBoard.ToString();

            sizableArray<move> potentialMoves = ourBoard.getMoves(pieceColour.white);

            if (potentialMoves.Length == 0)
                Assert.Inconclusive("No pawn moves found");

            // Find promotion moves
            move[] promotionMoves = Array.FindAll(potentialMoves.getArray(), a => a.isPawnPromotion);

            if (promotionMoves.Length == 0)
                Assert.Inconclusive("No promotion moves found");

            foreach (move thisMove in promotionMoves)
            {
                ourBoard.doMove(thisMove);

                if (ourBoard.ToString() == origBoard)
                    throw new AssertFailedException("After a pawn promotion move, the board has not changed");

                // Additionally, verify that the pawn has been promoted
                if (ourBoard[thisMove.dstPos].type != thisMove.typeToPromoteTo)
                    throw new AssertFailedException("Pawn was not promoted");
                if (ourBoard[thisMove.dstPos].GetType() == typeof(pawnSquare))
                    throw new AssertFailedException("Pawn was not promoted, but type has changed");
                if (ourBoard[thisMove.dstPos].colour != pieceColour.white)
                    throw new AssertFailedException("Pawn was promoted to wrong colour");

                ourBoard.undoMove(thisMove);

                if (ourBoard.ToString() != origBoard)
                    throw new AssertFailedException("After a pawn promotion move undo, the board has changed");
            }
        }

        [TestMethod]
        public void testMoveUndoingEnPassant()
        {
            DoktorChessAIBoard ourBoard = DoktorChessAIBoard.makeFromFEN("8/6p1/8/7P/8/8/8/8 b - - 0 1", gameType.normal, boardSearchConfig.getDebugConfig());

            square enemyPawn = ourBoard[6, 6];

            // Advance the enemy pawn, so we can capture it via en passant
            move advanceTwo = new move(enemyPawn, ourBoard[enemyPawn.position.down(2)]);
            ourBoard.doMove(advanceTwo);

            string origBoard = ourBoard.ToString();

            sizableArray<move> moves = ourBoard.getMoves(pieceColour.white);

            // Play our only capturing move
            move enPassant = Array.Find(moves.getArray(), a => a.isCapture == true);
            if (enPassant == null)
                Assert.Inconclusive("No en passant move found");
            ourBoard.doMove(enPassant);

            if (ourBoard.ToString() == origBoard)
                throw new AssertFailedException("After a move, the board has not changed");

            ourBoard.undoMove(enPassant);

            if (ourBoard.ToString() != origBoard)
                throw new AssertFailedException("After a move undo, the board has changed");
        }
    }
}