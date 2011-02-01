using System;
using System.Collections.Generic;
using System.Diagnostics;
using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class boardTests
    {
        [TestMethod]
        public void testCheckDetectionAsBlack()
        {
            Board ourBoard = new Board(gameType.normal, new boardSearchConfig());
            ourBoard.addPiece(pieceType.rook, pieceColour.white, 1, 1);
            ourBoard.addPiece(pieceType.king, pieceColour.black, 3, 1);

            Assert.IsTrue(ourBoard.playerIsInCheck(pieceColour.black));
        }

        [TestMethod]
        public void testCheckDetectionAsWhite()
        {
            Board ourBoard = new Board(gameType.normal, new boardSearchConfig());
            ourBoard.addPiece(pieceType.rook, pieceColour.black, 1, 1);
            ourBoard.addPiece(pieceType.king, pieceColour.white, 3, 1);

            Assert.IsTrue(ourBoard.playerIsInCheck(pieceColour.white));
        }

        [TestMethod]
        public void testMoveUndoing()
        {
            Board ourBoard = Board.makeNormalStartPosition(new boardSearchConfig());

            string origBoard = ourBoard.ToString();

            List<move> moves = ourBoard.getMoves(pieceColour.white);
            if (moves.Count == 0)
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
        public void testMoveUndoingThreatmap()
        {
            Board ourBoard = Board.makeNormalStartPosition(new boardSearchConfig());

            string origThreatMap = ourBoard.coverLevel.ToString();

            int[,] threatCounts = new int[Board.sizeX, Board.sizeY];
            for (int x = 0; x < Board.sizeX; x++)
            {
                for (int y = 0; y < Board.sizeY; y++)
                {
                    threatCounts[x, y] = ourBoard[x,y].coveredSquares.Count;
                }
            }

            List<move> moves = ourBoard.getMoves(pieceColour.white);
            if (moves.Count == 0)
                Assert.Inconclusive("No moves found");

            foreach (move thisMove in moves)
            {
                ourBoard.doMove(thisMove);

                if (ourBoard.coverLevel.ToString() == origThreatMap)
                    throw new AssertFailedException("After a move, the threat map has not changed");

                ourBoard.undoMove(thisMove);

                if (ourBoard.coverLevel.ToString() != origThreatMap)
                {
                    Debug.WriteLine("Troublesome move : " + thisMove.ToString());
                    Debug.WriteLine("Expected:");
                    Debug.WriteLine(origThreatMap);
                    Debug.WriteLine("Actual:");
                    Debug.WriteLine(ourBoard.coverLevel.ToString());
                    throw new AssertFailedException("After a move undo, the threat map has changed");
                }

                for (int x = 0; x < Board.sizeX; x++)
                {
                    for (int y = 0; y < Board.sizeY; y++)
                    {
                        if (threatCounts[x, y] != ourBoard[x, y].coveredSquares.Count)
                            throw new AssertFailedException("Piece covered count incorrect");
                    }
                }
            }
        }

        [TestMethod]
        public void testMoveDoingUndoingWithPawnPromotion()
        {
            Board ourBoard = new Board(gameType.normal, new boardSearchConfig());
            ourBoard.addPiece(pieceType.pawn, pieceColour.white, 1, 6);

            string origBoard = ourBoard.ToString();

            List<move> potentialMoves = ourBoard.getMoves(pieceColour.white);

            if (potentialMoves.Count == 0)
                Assert.Inconclusive("No pawn moves found");

            // Find promotion moves
            List<move> promotionMoves = potentialMoves.FindAll(a => a.isPawnPromotion);

            if (promotionMoves.Count == 0)
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
            Board ourBoard = new Board(gameType.normal, new boardSearchConfig());
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 7, 4);
            square enemyPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.black, 6, 6);

            // Advance the enemy pawn, so we can capture it via en passant
            move advanceTwo = new move(enemyPawn, ourBoard[enemyPawn.position.down(2)]);
            ourBoard.doMove(advanceTwo);

            string origBoard = ourBoard.ToString();

            List<move> moves = ourBoard.getMoves(pieceColour.white);

            // Play our only capturing move
            move enPassant = moves.Find(a => a.isCapture == true);
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