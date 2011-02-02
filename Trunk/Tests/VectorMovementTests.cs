using System;
using System.Collections.Generic;
using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class VectorMovementTests
    {
        public void testVectorMovement(List<squarePos> expectedPos, vectorDirection dir)
        {
            Board ourBoard = new Board(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());
            squarePos srcSquare = new squarePos(3, 3);

            ourBoard.addPiece(pieceType.queen, pieceColour.white, srcSquare.x, srcSquare.y);
            queenSquare queenie = (queenSquare)ourBoard[srcSquare];

            List<move> expectedmoves = new List<move>(expectedPos.Count);

            foreach (squarePos thisPos in expectedPos)
                expectedmoves.Add(new move(queenie, ourBoard[thisPos]));

            List<move> possibleMoves = queenie.getMovesForVector(ourBoard, dir, false);

            testListsAreOfSameMoves(expectedmoves, possibleMoves);

        }

        public static void testListsAreOfSameMoves(List<move> expectedmoves, List<move> actualMoves)
        {
            Assert.AreEqual(expectedmoves.Count, actualMoves.Count, "Incorrect amount of moves");

            foreach (move thisPossibleMove in actualMoves)
            {
                for (int i = 0; i < expectedmoves.Count; i++)
                {
                    move thisExpectedMove = expectedmoves[i];

                    if ((thisExpectedMove.srcPos.isSameSquareAs(thisPossibleMove.srcPos) &&
                         (thisExpectedMove.dstPos.isSameSquareAs(thisPossibleMove.dstPos))))
                    {
                        expectedmoves.Remove(thisExpectedMove);
                        break;
                    }
                }
            }

            Assert.AreEqual(0, expectedmoves.Count, "Unexpected move found");
        }

        [TestMethod]
        public void testVectorMovementUp()
        {
            List<squarePos> expected = new List<squarePos>
            {
                new squarePos(3, 4),
                new squarePos(3, 5),
                new squarePos(3, 6),
                new squarePos(3, 7)
            };
            testVectorMovement(expected , vectorDirection.up);
        }

        [TestMethod]
        public void testVectorMovementDown()
        {
            List<squarePos> expected = new List<squarePos>
            {
                new squarePos(3, 2),
                new squarePos(3, 1),
                new squarePos(3, 0)
            };
            testVectorMovement(expected, vectorDirection.down);
        }

        [TestMethod]
        public void testVectorMovementLeft()
        {
            List<squarePos> expected = new List<squarePos>
            {
                new squarePos(2, 3),
                new squarePos(1, 3),
                new squarePos(0, 3)
            };
            testVectorMovement(expected, vectorDirection.left);
        }

        [TestMethod]
        public void testVectorMovementRight()
        {
            List<squarePos> expected = new List<squarePos>
            {
                new squarePos(4, 3),
                new squarePos(5, 3),
                new squarePos(6, 3),
                new squarePos(7, 3)
            };
            testVectorMovement(expected, vectorDirection.right);
        }

        [TestMethod]
        public void testVectorMovementLeftUp()
        {
            List<squarePos> expected = new List<squarePos>
            {
                new squarePos(2, 4),
                new squarePos(1, 5),
                new squarePos(0, 6)
            };
            testVectorMovement(expected, vectorDirection.leftup);
        }

        [TestMethod]
        public void testVectorMovementRightUp()
        {
            List<squarePos> expected = new List<squarePos>
            {
                new squarePos(4, 4),
                new squarePos(5, 5),
                new squarePos(6, 6),
                new squarePos(7, 7)
            };
            testVectorMovement(expected, vectorDirection.rightup);
        }

        [TestMethod]
        public void testVectorMovementLeftDown()
        {
            List<squarePos> expected = new List<squarePos>
            {
                new squarePos(2, 2),
                new squarePos(1, 1),
                new squarePos(0, 0)
            };
            testVectorMovement(expected, vectorDirection.leftdown);
        }

        [TestMethod]
        public void testVectorMovementRightDown()
        {
            List<squarePos> expected = new List<squarePos>
            {
                new squarePos(4, 2),
                new squarePos(5, 1),
                new squarePos(6, 0)
            };
            testVectorMovement(expected, vectorDirection.rightdown);
        }
    }
}
