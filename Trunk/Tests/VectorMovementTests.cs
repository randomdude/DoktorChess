using System;
using System.Collections.Generic;
using System.Diagnostics;
using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class sizableArrayTests
    {
        [TestMethod]
        public void testIterator()
        {
            sizableArray<move> foo = new sizableArray<move>(5);
            
            move move1 = new move(new square(0,0), new square(1,1));
            move move2 = new move(new square(1,1), new square(2,2));

            foo.Add(move1);
            foo.Add(move2);

            bool move1seen = false;
            bool move2seen = false;

            foreach (move iterated in foo)
            {
                Debug.WriteLine("Iterated object " + iterated);
                if (iterated == move1)
                {
                    if (move1seen)
                        throw new Exception("Move one iterated twice");
                    move1seen = true;
                }
                else if (iterated == move2)
                {
                    if (move2seen)
                        throw new Exception("Move two iterated twice");
                    move2seen = true;
                }
                else if (iterated == null)
                    throw new Exception("Iterator returned null");
                else 
                    throw new Exception("Iterator returned something crazy");
            }

            if (!move1seen || !move2seen)
                throw new Exception("Iterator did not iterate over all elements");
        }

        [TestMethod]
        public void testIteratorOverEmptyCollection()
        {
            sizableArray<move> foo = new sizableArray<move>(5);

            if (foo.Length != 0)
                throw new Exception("Iterator length not zero while iterating over an empty collection");

            foreach (move thisMove in foo)
                throw new Exception("Iterator returned something while iterating over an empty collection");
        }
    }


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

            sizableArray<move> possibleMoves = queenie.getMovesForVector(null, ourBoard, dir, false);

            testListsAreOfSameMoves(expectedmoves, possibleMoves);

        }

        public static void testListsAreOfSameMoves(List<move> expectedmoves, sizableArray<move> actualMoves)
        {
            Assert.AreEqual(expectedmoves.Count, actualMoves.Length, "Incorrect amount of moves");

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
