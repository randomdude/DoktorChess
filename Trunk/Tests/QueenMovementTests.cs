using System;
using System.Collections.Generic;
using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class QueenMovementTests
    {
        private List<move> getExpectedMoveSquares(square srcSquare)
        {
            List<move> expectedmoves = new List<move>
                                           {
                                               new move(srcSquare, new square(3, 7)),
                                               new move(srcSquare, new square(3, 6)),
                                               new move(srcSquare, new square(3, 5)),
                                               new move(srcSquare, new square(3, 4)),
                                               new move(srcSquare, new square(3, 2)),
                                               new move(srcSquare, new square(3, 1)),
                                               new move(srcSquare, new square(3, 0)),
                                               new move(srcSquare, new square(0, 3)),
                                               new move(srcSquare, new square(1, 3)),
                                               new move(srcSquare, new square(2, 3)),
                                               new move(srcSquare, new square(4, 3)),
                                               new move(srcSquare, new square(5, 3)),
                                               new move(srcSquare, new square(6, 3)),
                                               new move(srcSquare, new square(7, 3)),
                                               new move(srcSquare, new square(0, 0)),
                                               new move(srcSquare, new square(1, 1)),
                                               new move(srcSquare, new square(2, 2)),
                                               new move(srcSquare, new square(4, 4)),
                                               new move(srcSquare, new square(5, 5)),
                                               new move(srcSquare, new square(6, 6)),
                                               new move(srcSquare, new square(7, 7)),
                                               new move(srcSquare, new square(0, 6)),
                                               new move(srcSquare, new square(1, 5)),
                                               new move(srcSquare, new square(2, 4)),
                                               new move(srcSquare, new square(4, 2)),
                                               new move(srcSquare, new square(5, 1)),
                                               new move(srcSquare, new square(6, 0))
                                           };
            return expectedmoves;
        }

        [TestMethod]        
        public void testQueenMovement()
        {
            Board ourBoard = new Board(gameType.queenAndPawns, new boardSearchConfig());
            squarePos srcSquare = new squarePos(3, 3);

            ourBoard.addPiece(pieceType.queen, pieceColour.white, srcSquare.x, srcSquare.y);
            queenSquare queenie = (queenSquare) ourBoard[srcSquare];

            List<move> possibleMoves = queenie.getPossibleMoves(ourBoard);
            List<move> expectedmoves = getExpectedMoveSquares(queenie);

            VectorMovementTests.testListsAreOfSameMoves(expectedmoves, possibleMoves);
        }

        [TestMethod]
        public void testQueenMovementWithCapture()
        {
            Board ourBoard = new Board(gameType.queenAndPawns, new boardSearchConfig());
            squarePos srcSquare = new squarePos(3, 3);

            ourBoard.addPiece(pieceType.queen, pieceColour.white, srcSquare.x, srcSquare.y);
            queenSquare queenie = (queenSquare)ourBoard[srcSquare];

            // Place a black pawn on the board, and ensure that we can capture it, and
            // that we cannot move through it.
            ourBoard.addPiece(pieceType.pawn, pieceColour.black, 1, 1);

            List<move> possibleMoves = queenie.getPossibleMoves(ourBoard);
            List<move> expectedmoves = getExpectedMoveSquares(queenie);

            // We don't expect to be able to move to (0,0), since that square is behind an
            // enemy pawn..
            bool found = false;
            for (int i = 0; i < expectedmoves.Count; i++)
            {
                if (expectedmoves[i].dstPos.x == 0 && expectedmoves[i].dstPos.y == 0)
                {
                    expectedmoves.RemoveAt(i);
                    found = true;
                    break;
                }
            }
            if (!found)
                throw new ArgumentOutOfRangeException();

            VectorMovementTests.testListsAreOfSameMoves(expectedmoves, possibleMoves);
        }
    }
}