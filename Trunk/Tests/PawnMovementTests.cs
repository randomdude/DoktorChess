using System.Collections.Generic;
using System.Diagnostics;
using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class PawnMovementTests
    {
        [TestMethod]
        public void testInitialPawnMovement()
        {
            Board ourBoard = new Board();
            ourBoard.addPiece(1,1, pieceType.pawn, pieceColour.black);
            square ourPawn = ourBoard[1, 1];

            List<move> actual = (List<move>) ourPawn.getPossibleMoves(ourBoard);

            // We expect that the pawn can move two spaces forward, or one space foward.
            List<move> expected = new List<move>
                                      {
                                          new move(ourPawn, ourBoard[1, 2]),
                                          new move(ourPawn, ourBoard[1, 3])
                                      };

            VectorMovementTests.testListsAreOfSameMoves(expected, actual);
        }

        [TestMethod]
        public void testNonInitialPawnMovement()
        {
            // Pawns can only move one square after their initial move.
            Board ourBoard = new Board();
            ourBoard.addPiece(1, 1, pieceType.pawn, pieceColour.black);
            square ourPawn = ourBoard[1, 1];

            // Mark pawn as having moved
            ourPawn.movedCount++;

            List<move> actual = ourPawn.getPossibleMoves(ourBoard);

            // We expect that the pawn can move one space foward only.
            List<move> expected = new List<move>
                                      {
                                          new move(ourPawn, ourBoard[1, 2])
                                      };

            VectorMovementTests.testListsAreOfSameMoves(expected, actual);
        }

        [TestMethod]
        public void testPawnMovementWithCaptureCol0()
        {
            // Spawn a black pawn at 0,3 and a white pawn at 1,4. Verify that the black
            // pawn can capture the white.

            Board ourBoard = new Board();
            square ourPawn = ourBoard.addPiece(0, 3, pieceType.pawn, pieceColour.black);
            square enemyPawn = ourBoard.addPiece(1, 4, pieceType.pawn, pieceColour.white);

            List<move> possibleMoves = ourPawn.getPossibleMoves(ourBoard);

            checkContainsSingleCapture(possibleMoves, ourPawn, enemyPawn);
        }

        [TestMethod]
        public void testPawnMovementWithCaptureCol7()
        {
            // Now test the same, at the other end of the board. Test on the edge.
            Board ourBoard = new Board();
            square ourPawn = ourBoard.addPiece(6, 3, pieceType.pawn, pieceColour.black);
            square enemyPawn = ourBoard.addPiece(7, 4, pieceType.pawn, pieceColour.white);

            List<move> possibleMoves = ourPawn.getPossibleMoves(ourBoard);

            checkContainsSingleCapture(possibleMoves, ourPawn, enemyPawn);
        }

        public void checkContainsSingleCapture(List<move> possibleMoves, square ourPiece, square enemyPiece)
        {
            bool captureFound = false;
            foreach (move thisMove in possibleMoves)
            {
                if (thisMove.isCapture)
                {
                    Assert.IsTrue(thisMove.srcPos.isSameSquareAs(ourPiece.position));
                    Assert.IsTrue(thisMove.dstPos.isSameSquareAs(enemyPiece.position));
                    Assert.AreSame(thisMove.dstPos, enemyPiece.position);
                    Assert.AreSame(thisMove.capturedSquare, enemyPiece);

                    if (captureFound)
                        throw new AssertFailedException("Multiple captures found, one expected");
                    captureFound = true;
                }
            }
            if (!captureFound)
                throw new AssertFailedException("No captures found, one expected");
        }

    }
}