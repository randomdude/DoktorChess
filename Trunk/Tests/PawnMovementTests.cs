using System;
using System.Collections.Generic;
using doktorChess;
using doktorChessGameEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class PawnMovementTests
    {
        [TestMethod]
        public void testInitialPawnMovement()
        {
            DoktorChessAIBoard ourBoard = new DoktorChessAIBoard(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());
            ourBoard.addPiece(pieceType.pawn, pieceColour.white, 1, 1);
            square ourPawn = ourBoard[1, 1];

            sizableArray<move> actual = ourPawn.getPossibleMoves(ourBoard);

            // We expect that the pawn can move two spaces forward, or one space forward.
            List<move> expected = new List<move>
                                      {
                                          new move(ourPawn, ourBoard[1, 2]),
                                          new move(ourPawn, ourBoard[1, 3])
                                      };

            VectorMovementTests.testListsAreOfSameMoves(expected, actual);

            if (ourPawn.movedCount != 0)
                throw new Exception("Pawn not move count not incremented");
        }

        [TestMethod]
        public void testNonInitialPawnMovement()
        {
            // Pawns can only move one square after their initial move.
            DoktorChessAIBoard ourBoard = new DoktorChessAIBoard(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 1, 1);

            // Mark pawn as having moved
            ourPawn.movedCount++;

            sizableArray<move> actual = ourPawn.getPossibleMoves(ourBoard);

            // We expect that the pawn can move one space forward only.
            List<move> expected = new List<move>
                                      {
                                          new move(ourPawn, ourBoard[1, 2])
                                      };

            VectorMovementTests.testListsAreOfSameMoves(expected, actual);
        }

        [TestMethod]
        public void testPawnPromotion()
        {
            // Pawns should be able to be promoted to any piece of their own colour, other than a
            // king.
            DoktorChessAIBoard ourBoard = new DoktorChessAIBoard(gameType.normal, boardSearchConfig.getDebugConfig());
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 1, 6);
            ourPawn.movedCount++;

            sizableArray<move> actual = ourPawn.getPossibleMoves(ourBoard);

            // We expect a number of moves forward, all of which are promotions.
            List<move> expected = new List<move>
                                      {
                                            new move(ourPawn, ourBoard[1, 7], pieceType.bishop),
                                            new move(ourPawn, ourBoard[1, 7], pieceType.knight),
                                            new move(ourPawn, ourBoard[1, 7], pieceType.queen),
                                            new move(ourPawn, ourBoard[1, 7], pieceType.rook)
                                      };

            VectorMovementTests.testListsAreOfSameMoves(expected, actual);
        }

        [TestMethod]
        public void testPawnPromotionAsBlack()
        {
            // Pawns should be able to be promoted to any piece of their own colour, other than a
            // king.
            DoktorChessAIBoard ourBoard = new DoktorChessAIBoard(gameType.normal, boardSearchConfig.getDebugConfig());
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.black, 1, 1);
            ourPawn.movedCount++;

            sizableArray<move> actual = ourPawn.getPossibleMoves(ourBoard);

            // We expect a number of moves forward, all of which are promotions.
            List<move> expected = new List<move>
                                      {
                                            new move(ourPawn, ourBoard[1, 0], pieceType.bishop),
                                            new move(ourPawn, ourBoard[1, 0], pieceType.knight),
                                            new move(ourPawn, ourBoard[1, 0], pieceType.queen),
                                            new move(ourPawn, ourBoard[1, 0], pieceType.rook)
                                      };

            VectorMovementTests.testListsAreOfSameMoves(expected, actual);
        }

        [TestMethod]
        public void testPawnPromotionWithCapture()
        {
            // Verify promotion occurs when capturing in to the back row.
            DoktorChessAIBoard ourBoard = new DoktorChessAIBoard(gameType.normal, boardSearchConfig.getDebugConfig());
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 1, 6);
            ourPawn.movedCount++;
            ourBoard.addPiece(pieceType.knight, pieceColour.black, 1, 7);
            ourBoard.addPiece(pieceType.knight, pieceColour.black, 2, 7);

            sizableArray<move> actual = ourPawn.getPossibleMoves(ourBoard);

            // We expect a number of moves forward, all of which are promotions.
            List<move> expected = new List<move>
                                      {
                                            new move(ourPawn, ourBoard[2, 7], pieceType.bishop),
                                            new move(ourPawn, ourBoard[2, 7], pieceType.knight),
                                            new move(ourPawn, ourBoard[2, 7], pieceType.queen),
                                            new move(ourPawn, ourBoard[2, 7], pieceType.rook)
                                      };

            VectorMovementTests.testListsAreOfSameMoves(expected, actual);
        }

        [TestMethod]
        public void testPawnMovementWithCaptureCol0()
        {
            // Spawn a black pawn at 0,3 and a white pawn at 1,4. Verify that the black
            // pawn can capture the white.

            DoktorChessAIBoard ourBoard = new DoktorChessAIBoard(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 0, 3);
            square enemyPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.black, 1, 4);

            sizableArray<move> possibleMoves = ourPawn.getPossibleMoves(ourBoard);

            checkContainsSingleCapture(possibleMoves.getArray(), ourPawn, enemyPawn);
        }

        [TestMethod]
        public void testPawnMovementWithCaptureCol7()
        {
            // Now test the same, at the other end of the board. Test on the edge.
            DoktorChessAIBoard ourBoard = new DoktorChessAIBoard(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 6, 3);
            square enemyPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.black, 7, 4);

            sizableArray<move> possibleMoves = ourPawn.getPossibleMoves(ourBoard);

            checkContainsSingleCapture(possibleMoves.getArray(), ourPawn, enemyPawn);
        }

        [TestMethod]
        public void testThatEnPassantOccursWhenItShouldAsBlack()
        {
            DoktorChessAIBoard ourBoard = new DoktorChessAIBoard(gameType.normal, boardSearchConfig.getDebugConfig());
            // En passant requires that the enemy pawn has just advanced two squares. Because of this, we make this move on a board and then check that en passant can occur.
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.black, 6, 3);
            square enemyPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 7, 1);

            // Advance the enemy pawn
            move advanceTwo = new move(enemyPawn, ourBoard[enemyPawn.position.up(2)]);
            ourBoard.doMove(advanceTwo);

            // Now verify that the enemy pawn is captured.
            sizableArray<move> possibleMoves = ourPawn.getPossibleMoves(ourBoard);
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
            DoktorChessAIBoard ourBoard = DoktorChessAIBoard.makeFromFEN("8/6p1/8/7P/8/8/8/8 b - - 0 1", gameType.normal, boardSearchConfig.getDebugConfig());

            square enemyPawn = ourBoard[6, 6];
            square ourPawn = ourBoard[7, 4];

            // Advance the enemy pawn
            move advanceTwo = new move(enemyPawn, ourBoard[enemyPawn.position.down(2)]);
            ourBoard.doMove(advanceTwo);

            // Now verify that the enemy pawn is captured.
            sizableArray<move> possibleMoves = ourPawn.getPossibleMoves(ourBoard);
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
            DoktorChessAIBoard ourBoard = DoktorChessAIBoard.makeFromFEN("8/8/8/8/6P1/8/7p/8 b - - 0 1", gameType.normal, boardSearchConfig.getDebugConfig());
            // Verify that we cannot en passant after our opponent has moved a pawn forward one, not two, squares.
            square ourPawn = ourBoard[6, 3];
            square enemyPawn = ourBoard[7, 1];

            square ourNewSquare = ourBoard[ourPawn.position.rightOne().upOne()];

            // Advance the enemy pawn twice
            move advanceOne = new move(enemyPawn, ourBoard[enemyPawn.position.upOne()]);
            ourBoard.doMove(advanceOne);
            ourBoard.colToMove = pieceColour.black;
            advanceOne = new move(enemyPawn, ourBoard[enemyPawn.position.upOne()]);
            ourBoard.doMove(advanceOne);

            sizableArray<move> possibleMoves = ourPawn.getPossibleMoves(ourBoard);
            if (Array.Find(possibleMoves.getArray(), a => a.isCapture == true) != null) 
                throw new AssertFailedException("Found en passant capture when none is legal");
        }

        [TestMethod]
        public void testThatEnPassantDoesNotOccurAfterExtraMove()
        {
            DoktorChessAIBoard ourBoard = DoktorChessAIBoard.makeFromFEN("8/8/8/8/6P1/8/1P5p/8 b - - 0 1", gameType.normal, boardSearchConfig.getDebugConfig());
            // Verify that we cannot en passant after we move a piece
            square ourPawn = ourBoard[6, 3];
            square ourKing = ourBoard[1, 1];
            square enemyPawn = ourBoard[7, 1];

            // Advance the enemy pawn two squares, and then move our king. This should cause en passant to be impossible.
            move advanceTwo = new move(enemyPawn, ourBoard[enemyPawn.position.up(2)]);
            ourBoard.doMove(advanceTwo);

            // Then, move our king
            move kingMove = new move(ourKing, ourBoard[ourKing.position.upOne()]);
            ourBoard.doMove(kingMove);

            // Now. Can we en passant?
            sizableArray<move> possibleMoves = ourPawn.getPossibleMoves(ourBoard);
            if (Array.Find(possibleMoves.getArray(), a => a.isCapture == true) != null)
                throw new AssertFailedException("Found en passant capture when none is legal");
        }

        /// <summary>
        /// Check a list of moves contains only one capture, which has the supplied src and dst/enemysquare.
        /// </summary>
        /// <param name="possibleMoves"></param>
        /// <param name="ourPiece"></param>
        /// <param name="enemyPiece"></param>
        /// <returns></returns>
        public move checkContainsSingleCapture(move[] possibleMoves, square ourPiece, square enemyPiece)
        {
            bool captureFound = false;
            move capture = null;

            foreach (move thisMove in possibleMoves)
            {
                if (thisMove.isCapture)
                {
                    Assert.IsTrue(thisMove.srcPos.isSameSquareAs(ourPiece.position));
                    Assert.IsTrue(thisMove.dstPos.isSameSquareAs(enemyPiece.position));
                    Assert.AreEqual(thisMove.dstPos, enemyPiece.position);
                    Assert.AreEqual(thisMove.capturedSquare, enemyPiece);

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