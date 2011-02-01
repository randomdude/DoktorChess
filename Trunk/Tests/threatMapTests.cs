using System;
using System.Collections.Generic;
using System.Diagnostics;
using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class threatMapTests
    {
        [TestMethod]
        public void testThreatMapDeep_pawn()
        {
            Board ourBoard = new Board(gameType.queenAndPawns, new boardSearchConfig());
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 4, 1);

            if (ourBoard.getCoverLevel( new squarePos(3,2), pieceColour.white ) != 1 ||
                ourBoard.getCoverLevel( new squarePos(5,2), pieceColour.white ) != 1   )
                throw new AssertFailedException("Threatmap created wrongly");

            if (ourPawn.coveredSquares.Count != 2)
                throw new AssertFailedException("Pawn created not covering two squares");
            
            if ((!ourPawn.coveredSquares[0].isSameSquareAs(new squarePos(3,2))) || 
                (!ourPawn.coveredSquares[1].isSameSquareAs(new squarePos(5,2)))   )
                throw new AssertFailedException("Pawn created with incorrect .coveredSquares");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 1 ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 1   )
                throw new AssertFailedException("Pawn created with incorrect .piecesWhichThreatenSquare count");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2][ ourPawn.position.flatten() ] != ourPawn ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[5, 2][ourPawn.position.flatten() ] != ourPawn)
                throw new AssertFailedException("Pawn created with incorrect .piecesWhichThreatenSquare contents");

            move ourMove = new move(ourPawn, ourBoard[4, 3]);
            ourBoard.doMove(ourMove);

            if (ourBoard.getCoverLevel(new squarePos(3, 4), pieceColour.white) != 1 ||
                ourBoard.getCoverLevel(new squarePos(5, 4), pieceColour.white) != 1)
                throw new AssertFailedException("Threatmap did not update cover levels correctly");

            if (ourPawn.coveredSquares.Count != 2)
                throw new AssertFailedException("Pawn updated not covering two squares");

            if ((!ourPawn.coveredSquares[0].isSameSquareAs(new squarePos(3, 4))) ||
                (!ourPawn.coveredSquares[1].isSameSquareAs(new squarePos(5, 4))))
                throw new AssertFailedException("Pawn updated with incorrect .coveredSquares");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 0 ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 0   )
                throw new AssertFailedException("Pawn's pre-update .piecesWhichThreatenSquare was not changed");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 4].Count != 1 ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 4].Count != 1)
                throw new AssertFailedException("Pawn updated with incorrect .piecesWhichThreatenSquare count");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 4][ourPawn.position.flatten()] != ourPawn ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[5, 4][ourPawn.position.flatten()] != ourPawn)
                throw new AssertFailedException("Pawn updated with incorrect .piecesWhichThreatenSquare contents");
        }

        [TestMethod]
        public void testThreatMapDeep_pawnCapture()
        {
            Board ourBoard = new Board(gameType.queenAndPawns, new boardSearchConfig());
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 4, 1);
            square enemyPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.black, 3, 2);

            if (ourBoard.getCoverLevel(new squarePos(3, 2), pieceColour.white) != 1 ||
                ourBoard.getCoverLevel(new squarePos(5, 2), pieceColour.white) != 1)
                throw new AssertFailedException("Threatmap created wrongly");

            if (ourPawn.coveredSquares.Count != 2)
                throw new AssertFailedException("Pawn created not covering two squares");

            if ((!ourPawn.coveredSquares[0].isSameSquareAs(new squarePos(3, 2))) ||
                (!ourPawn.coveredSquares[1].isSameSquareAs(new squarePos(5, 2))))
                throw new AssertFailedException("Pawn created with incorrect .coveredSquares");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 1 ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 1)
                throw new AssertFailedException("Pawn created with incorrect .piecesWhichThreatenSquare count");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2][ourPawn.position.flatten()] != ourPawn ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[5, 2][ourPawn.position.flatten()] != ourPawn)
                throw new AssertFailedException("Pawn created with incorrect .piecesWhichThreatenSquare contents");

            move ourMove = new move(ourPawn, ourBoard[3, 2]);
            ourBoard.doMove(ourMove);

            if (ourBoard.getCoverLevel(new squarePos(2, 3), pieceColour.white) != 1 ||
                ourBoard.getCoverLevel(new squarePos(4, 3), pieceColour.white) != 1)
                throw new AssertFailedException("Threatmap did not update cover levels correctly");

            if (ourPawn.coveredSquares.Count != 2)
                throw new AssertFailedException("Pawn updated not covering two squares");

            if ((!ourPawn.coveredSquares[0].isSameSquareAs(new squarePos(2, 3))) ||
                (!ourPawn.coveredSquares[1].isSameSquareAs(new squarePos(4, 3))))
                throw new AssertFailedException("Pawn updated with incorrect .coveredSquares");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 0 ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 0)
                throw new AssertFailedException("Pawn's pre-update .piecesWhichThreatenSquare was not changed");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[2, 3].Count != 1 ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[4, 3].Count != 1)
                throw new AssertFailedException("Pawn updated with incorrect .piecesWhichThreatenSquare count");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[2, 3][ourPawn.position.flatten()] != ourPawn ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[4, 3][ourPawn.position.flatten()] != ourPawn)
                throw new AssertFailedException("Pawn updated with incorrect .piecesWhichThreatenSquare contents");
        }

        [TestMethod]
        public void testMoveUndoingThreatmapWithCapture()
        {
            Board ourBoard = new Board(gameType.normal, new boardSearchConfig());
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 4, 1);
            square enemyPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.black, 5, 2);

            string origThreatMap = ourBoard.coverLevel.ToString();

            int[,] threatCounts = new int[Board.sizeX,Board.sizeY];
            for (int x = 0; x < Board.sizeX; x++)
            {
                for (int y = 0; y < Board.sizeY; y++)
                {
                    threatCounts[x, y] = ourBoard[x, y].coveredSquares.Count;
                }
            }

            move ourMove = new move(ourPawn, ourBoard[5, 2]);
            ourBoard.doMove(ourMove);

            if (ourBoard.coverLevel.ToString() == origThreatMap)
                throw new AssertFailedException("After a move, the threat map has not changed");

            ourBoard.undoMove(ourMove);

            if (ourBoard.coverLevel.ToString() != origThreatMap)
            {
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
                    threatCounts[x, y] = ourBoard[x, y].coveredSquares.Count;
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
        public void testMoveUndoingThreatmap_Castling()
        {
            Board ourBoard = new Board(gameType.normal, new boardSearchConfig());
            square ourKing = ourBoard.addPiece(pieceType.king, pieceColour.white, 4, 0);
            square ourRook = ourBoard.addPiece(pieceType.rook, pieceColour.white, 7, 0);

            square enemyPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.black, 6, 6);

            string origThreatMap = ourBoard.coverLevel.ToString();

            int[,] threatCounts = new int[Board.sizeX,Board.sizeY];
            for (int x = 0; x < Board.sizeX; x++)
            {
                for (int y = 0; y < Board.sizeY; y++)
                {
                    threatCounts[x, y] = ourBoard[x, y].coveredSquares.Count;
                }
            }

            move castling = new move(ourKing, ourBoard[6, 0]);

            ourBoard.doMove(castling);

            if (ourBoard.coverLevel.ToString() == origThreatMap)
                throw new AssertFailedException("After a move, the threat map has not changed");

            ourBoard.undoMove(castling);

            if (ourBoard.coverLevel.ToString() != origThreatMap)
            {
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

        [TestMethod]
        public void testThreatMapQueenPawnsStartPosition()
        {
            Board ourBoard = Board.makeQueenAndPawnsStartPosition(new boardSearchConfig());

            string expectedThreatString =
                " -1 -1 -1  0 -1 -1 -1 -1\r\n  0  0 -1 -1 -1  0  0  0\r\n  0 -1  0 -1  0 -1  0  0\r\n -1  0  0 -1  0  0 -1  0\r\n  0  0  0 -1  0  0  0 -1\r\n  1  2  2  1  2  2  2  1\r\n  0  0  0 -1  0  0  0  0\r\n  0  0  0  0  0  0  0  0\r\n";

            string threatString = ourBoard.coverLevel.ToString();

            if (expectedThreatString != threatString)
                throw new Exception("Threat map was incorrect");
        }

        [TestMethod]
        public void testThreatMapNormalStartPosition()
        {
            Board ourBoard = Board.makeNormalStartPosition(new boardSearchConfig());

            //string expectedThreatString =
            //    "  0  0  0  0  0  0  0  0\r\n  0  0  0 -1  0  0  0  0\r\n  1  2  2  1  2  2  2  1\r\n  0  0  0 -1  0  0  0 -1\r\n -1  0  0 -1  0  0 -1  0\r\n  0 -1  0 -1  0 -1  0  0\r\n  0  0 -1 -1 -1  0  0  0\r\n -1 -1 -1  0 -1 -1 -1 -1\r\n";

            string threatString = ourBoard.coverLevel.ToString();

            //if (expectedThreatString != threatString)
            //    throw new Exception("bad threat map");
        }
    }
}