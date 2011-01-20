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
            Board ourBoard = new Board(gameType.queenAndPawns);
            ourBoard.addPiece(1,1, pieceType.pawn, pieceColour.white);
            square ourPawn = ourBoard[1, 1];

            List<move> actual = (List<move>) ourPawn.getPossibleMoves(ourBoard);

            // We expect that the pawn can move two spaces forward, or one space forward.
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
            Board ourBoard = new Board(gameType.queenAndPawns);
            square ourPawn = ourBoard.addPiece(1, 1, pieceType.pawn, pieceColour.white );

            // Mark pawn as having moved
            ourPawn.movedCount++;

            List<move> actual = ourPawn.getPossibleMoves(ourBoard);

            // We expect that the pawn can move one space forward only.
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

            Board ourBoard = new Board(gameType.queenAndPawns);
            square ourPawn = ourBoard.addPiece(0, 3, pieceType.pawn, pieceColour.white);
            square enemyPawn = ourBoard.addPiece(1, 4, pieceType.pawn, pieceColour.black);

            List<move> possibleMoves = ourPawn.getPossibleMoves(ourBoard);

            checkContainsSingleCapture(possibleMoves, ourPawn, enemyPawn);
        }

        [TestMethod]
        public void testPawnMovementWithCaptureCol7()
        {
            // Now test the same, at the other end of the board. Test on the edge.
            Board ourBoard = new Board(gameType.queenAndPawns);
            square ourPawn = ourBoard.addPiece(6, 3, pieceType.pawn, pieceColour.white);
            square enemyPawn = ourBoard.addPiece(7, 4, pieceType.pawn, pieceColour.black);

            List<move> possibleMoves = ourPawn.getPossibleMoves(ourBoard);

            checkContainsSingleCapture(possibleMoves, ourPawn, enemyPawn);
        }

        [TestMethod]
        public void testThatEnPassantOccursWhenItShouldAsBlack()
        {
            Board ourBoard = new Board(gameType.normal);
            // En passant requires that the enemy pawn has just advanced two squares. Because of this, we make this move on a board and then check that en passant can occur.
            square ourPawn = ourBoard.addPiece(6, 3, pieceType.pawn, pieceColour.black);
            square enemyPawn = ourBoard.addPiece(7, 1, pieceType.pawn, pieceColour.white);

            // Advance the enemy pawn
            move advanceTwo = new move(enemyPawn, ourBoard[enemyPawn.position.up(2)] );
            ourBoard.doMove(advanceTwo);

            // Now verify that the enemy pawn is captured.
            List<move> possibleMoves = ourPawn.getPossibleMoves(ourBoard);
            move enPassantCapture = null; 
            foreach (move thisMove in possibleMoves)
            {
                if (thisMove.isCapture)
                {
                    if (enPassantCapture != null)
                        throw new AssertFailedException("More than one capture was found");

                    // Note that our dest square is not the enemy square here. 
                    Assert.IsTrue(thisMove.srcPos.isSameSquareAs(ourPawn.position));
                    Assert.IsTrue(thisMove.dstPos.isSameSquareAs(ourPawn.position.rightOne().downOne() ));
                    Assert.AreSame(thisMove.capturedSquare, enemyPawn);

                    enPassantCapture = thisMove;
                }
            }
            if (enPassantCapture == null)
                throw new AssertFailedException("En passant capture did not occur");

            // Make sure that the en passant capture ends up putting our pawn in the square above the enemy pawn
            Assert.IsTrue(enemyPawn.position.downOne().isSameSquareAs( enPassantCapture.dstPos) );
        }

        [TestMethod]
        public void testThatEnPassantOccursWhenItShouldAsWhite()
        {
            Board ourBoard = new Board(gameType.normal);
            square ourPawn = ourBoard.addPiece(7, 4, pieceType.pawn, pieceColour.white);
            square enemyPawn = ourBoard.addPiece(6, 6, pieceType.pawn, pieceColour.black);

            // Advance the enemy pawn
            move advanceTwo = new move(enemyPawn, ourBoard[enemyPawn.position.down(2)]);
            ourBoard.doMove(advanceTwo);

            // Now verify that the enemy pawn is captured.
            List<move> possibleMoves = ourPawn.getPossibleMoves(ourBoard);
            move enPassantCapture = null;
            foreach (move thisMove in possibleMoves)
            {
                if (thisMove.isCapture)
                {
                    if (enPassantCapture != null)
                        throw new AssertFailedException("More than one capture was found");

                    // Note that our dest square is not the enemy square here. 
                    Assert.IsTrue(thisMove.srcPos.isSameSquareAs(ourPawn.position));
                    Assert.IsTrue(thisMove.dstPos.isSameSquareAs(ourPawn.position.leftOne().upOne()));
                    Assert.AreSame(thisMove.capturedSquare, enemyPawn);

                    enPassantCapture = thisMove;
                }
            }
            if (enPassantCapture == null)
                throw new AssertFailedException("En passant capture did not occur");

            // Make sure that the en passant capture ends up putting our pawn in the square above the enemy pawn
            Assert.IsTrue(enemyPawn.position.upOne().isSameSquareAs(enPassantCapture.dstPos));
        }

        [TestMethod]
        public void testThatEnPassantDoesNotOccurAfterTwoPawnAdvances()
        {
            Board ourBoard = new Board(gameType.normal);
            // Verify that we cannot en passant after our opponent has moved a pawn forward one, not two, squares.
            square ourPawn = ourBoard.addPiece(6, 3, pieceType.pawn, pieceColour.white);
            square enemyPawn = ourBoard.addPiece(7, 1, pieceType.pawn, pieceColour.black);

            square ourNewSquare = ourBoard[ourPawn.position.rightOne().upOne()];

            // Advance the enemy pawn twice
            move advanceOne = new move(enemyPawn, ourBoard[enemyPawn.position.upOne()]);
            ourBoard.doMove(advanceOne);
            advanceOne = new move(enemyPawn, ourBoard[enemyPawn.position.upOne()]);
            ourBoard.doMove(advanceOne);

            List<move> possibleMoves = ourPawn.getPossibleMoves(ourBoard);
            if (possibleMoves.Find(a => a.isCapture == true) != null) 
                throw new AssertFailedException("Found en passant capture when none is legal");
        }

        [TestMethod]
        public void testThatEnPassantDoesNotOccurAfterExtraMove()
        {
            Board ourBoard = new Board(gameType.normal);
            // Verify that we cannot en passant after we move a piece
            square ourPawn = ourBoard.addPiece(6, 3, pieceType.pawn, pieceColour.white);
            square ourKing = ourBoard.addPiece(1, 1, pieceType.pawn, pieceColour.white);
            square enemyPawn = ourBoard.addPiece(7, 1, pieceType.pawn, pieceColour.black);

            // Advance the enemy pawn two squares, and then move our king. This should cause en passant to be impossible.
            move advanceTwo = new move(enemyPawn, ourBoard[enemyPawn.position.up(2)]);
            ourBoard.doMove(advanceTwo);

            // Then, move our king
            move kingMove = new move(ourKing, ourBoard[ourKing.position.upOne()]);
            ourBoard.doMove(kingMove);

            // Now. Can we en passant?
            List<move> possibleMoves = ourPawn.getPossibleMoves(ourBoard);
            if (possibleMoves.Find(a => a.isCapture == true) != null)
                throw new AssertFailedException("Found en passant capture when none is legal");
        }

        /// <summary>
        /// Check a list of moves contains only one capture, which has the supplied src and dst/enemysquare.
        /// </summary>
        /// <param name="possibleMoves"></param>
        /// <param name="ourPiece"></param>
        /// <param name="enemyPiece"></param>
        /// <returns></returns>
        public move checkContainsSingleCapture(List<move> possibleMoves, square ourPiece, square enemyPiece)
        {
            bool captureFound = false;
            move capture = null;

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
                    capture = thisMove;
                }
            }
            if (!captureFound)
                throw new AssertFailedException("No captures found, one expected");
            return capture;
        }

    }
}