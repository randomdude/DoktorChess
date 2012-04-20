using System;
using System.Collections.Generic;
using System.Diagnostics;
using doktorChess;
using doktorChessGameEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class threatMapTests
    {
        [TestMethod]
        public void testThreatMapDeep_pawn()
        {
            Board ourBoard = new Board(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 4, 1);

            if (ourBoard.getCoverLevel( new squarePos(3,2), pieceColour.white ) != 1 ||
                ourBoard.getCoverLevel( new squarePos(5,2), pieceColour.white ) != 1   )
                throw new AssertFailedException("Threatmap created wrongly");

            if (ourPawn.coveredSquares.Count != 2)
                throw new AssertFailedException("Pawn created not covering two squares");

            if ((!ourPawn.coveredSquares[3, 2]) || 
                (!ourPawn.coveredSquares[5, 2])    )
                throw new AssertFailedException("Pawn created with incorrect .coveredSquares");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 1 ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 1   )
                throw new AssertFailedException("Pawn created with incorrect .piecesWhichThreatenSquare count");

            //if (  ourBoard[ ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2][ourPawn.position.flatten()] ] != ourPawn ||
            //      ourBoard[ ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[5, 2][ourPawn.position.flatten()] ] != ourPawn)
            //    throw new AssertFailedException("Pawn created with incorrect .piecesWhichThreatenSquare contents");

            move ourMove = new move(ourPawn, ourBoard[4, 3]);
            ourBoard.doMove(ourMove);

            if (ourBoard.getCoverLevel(new squarePos(3, 4), pieceColour.white) != 1 ||
                ourBoard.getCoverLevel(new squarePos(5, 4), pieceColour.white) != 1)
                throw new AssertFailedException("Threatmap did not update cover levels correctly");

            if (ourPawn.coveredSquares.Count != 2)
                throw new AssertFailedException("Pawn updated not covering two squares");

            if ((!ourPawn.coveredSquares[3, 4]) ||
                (!ourPawn.coveredSquares[5, 4])   )
                throw new AssertFailedException("Pawn updated with incorrect .coveredSquares");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 0 ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 0   )
                throw new AssertFailedException("Pawn's pre-update .piecesWhichThreatenSquare was not changed");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 4].Count != 1 ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 4].Count != 1)
                throw new AssertFailedException("Pawn updated with incorrect .piecesWhichThreatenSquare count");

            //if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 4][ourPawn.position.flatten()] != ourPawn ||
            //    ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[5, 4][ourPawn.position.flatten()] != ourPawn)
            //    throw new AssertFailedException("Pawn updated with incorrect .piecesWhichThreatenSquare contents");
        }

        [TestMethod]
        public void testThreatMapDeep_pawnCapture()
        {
            Board ourBoard = new Board(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 4, 1);
            square enemyPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.black, 3, 2);

            if (ourBoard.getCoverLevel(new squarePos(3, 2), pieceColour.white) != 1 ||
                ourBoard.getCoverLevel(new squarePos(5, 2), pieceColour.white) != 1)
                throw new AssertFailedException("Threatmap created wrongly");

            if (ourBoard.getCoverLevel(new squarePos(2, 1), pieceColour.white) != -1 ||
                ourBoard.getCoverLevel(new squarePos(4, 1), pieceColour.white) != -1)
                throw new AssertFailedException("Enemy pawn create with no threatened squares");

            if (ourPawn.coveredSquares.Count != 2)
                throw new AssertFailedException("Pawn created not covering two squares");

            if ((!ourPawn.coveredSquares[3, 2]) ||
                (!ourPawn.coveredSquares[5, 2])   )
                throw new AssertFailedException("Pawn created with incorrect .coveredSquares");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 1 ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 1)
                throw new AssertFailedException("Pawn created with incorrect .piecesWhichThreatenSquare count");

            //if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2][ourPawn.position.flatten()] != ourPawn ||
            //    ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[5, 2][ourPawn.position.flatten()] != ourPawn)
            //    throw new AssertFailedException("Pawn created with incorrect .piecesWhichThreatenSquare contents");

            move ourMove = new move(ourPawn, ourBoard[3, 2]);
            ourBoard.doMove(ourMove);

            if (ourBoard.getCoverLevel(new squarePos(2, 3), pieceColour.white) != 1 ||
                ourBoard.getCoverLevel(new squarePos(4, 3), pieceColour.white) != 1)
                throw new AssertFailedException("Threatmap did not update cover levels correctly");

            if (ourBoard.getCoverLevel(new squarePos(2, 1), pieceColour.white) != 0 ||
                ourBoard.getCoverLevel(new squarePos(4, 1), pieceColour.white) != 0)
                throw new AssertFailedException("Captured pawn still has threatened squares");

            if (ourPawn.coveredSquares.Count != 2)
                throw new AssertFailedException("Pawn updated not covering two squares");

            if (!ourPawn.coveredSquares[2, 3] ||
                !ourPawn.coveredSquares[4, 3]   )
                throw new AssertFailedException("Pawn updated with incorrect .coveredSquares");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 0 ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[3, 2].Count != 0)
                throw new AssertFailedException("Pawn's pre-update .piecesWhichThreatenSquare was not changed");

            if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[2, 3].Count != 1 ||
                ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[4, 3].Count != 1)
                throw new AssertFailedException("Pawn updated with incorrect .piecesWhichThreatenSquare count");

            //if (((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[2, 3][ourPawn.position.flatten()] != ourPawn ||
            //    ((threatMap)ourBoard.coverLevel).piecesWhichThreatenSquare[4, 3][ourPawn.position.flatten()] != ourPawn)
            //    throw new AssertFailedException("Pawn updated with incorrect .piecesWhichThreatenSquare contents");
        }

        [TestMethod]
        public void testThreatMapDeep_discovered()
        {
            Board ourBoard = new Board(gameType.queenAndPawns, boardSearchConfig.getDebugConfig()  );
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 4, 1);
            square ourRook = ourBoard.addPiece(pieceType.rook, pieceColour.white, 0, 1);

            move ourMove = new move(ourPawn, ourBoard[4, 2]);
            ourBoard.doMove(ourMove);

            // Observe the squares to the right of our pawn - they should now be accessible to the rook

            for (int x = 1; x < 7; x++)
            {
                if (ourBoard.getCoverLevel(new squarePos(x, 1), pieceColour.white) != 1)
                    throw new AssertFailedException("Threatmap did not update cover levels correctly");
            }
        }

        [TestMethod]
        public void testThreatMapDeep_discoveredPromotion()
        {
            Board ourBoard = new Board(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());
            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 3, 6);
            square enemyPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.black, 3, 7);
            square ourRook = ourBoard.addPiece(pieceType.rook, pieceColour.white, 0, 7);

            move ourMove = new move(ourPawn, ourBoard[3, 7], pieceType.queen);
            ourBoard.doMove(ourMove);

            // Observe the squares to the right of our pawn - they should not be accessible to the rook
            for (int x = 4; x < 7; x++)
            {
                if (ourBoard.getCoverLevel(new squarePos(x, 7), pieceColour.white) != 1)
                    throw new AssertFailedException("Threatmap did not update cover levels correctly");
            }

            // the pawn itself is protected once.
            if (ourBoard.getCoverLevel(new squarePos(3, 7), pieceColour.white) != 1)
                throw new AssertFailedException("Threatmap did not update cover levels of promoted piece correctly");
        }

        [TestMethod]
        public void testThreatMapDeep_discoveredCapture()
        {
            Board ourBoard = new Board(gameType.queenAndPawns, boardSearchConfig.getDebugConfig());

            square ourPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.white, 0, 1);
            square enemyPawn = ourBoard.addPiece(pieceType.pawn, pieceColour.black, 1, 2);
            square ourRook = ourBoard.addPiece(pieceType.rook, pieceColour.white, 0, 0);
            square enemyRook = ourBoard.addPiece(pieceType.rook, pieceColour.black, 0, 7);

            move ourMove = new move(ourPawn, enemyPawn);
            ourBoard.doMove(ourMove);

            // Squares between rooks our pawn should be threatened by both rooks, so should have a threat
            // level of 0, apart from the one at y=3, which is threatened by a pawn.
            for (int x = 1; x < 7; x++)
            {
                if (x == 3)
                {
                    if (ourBoard.getCoverLevel(new squarePos(0, x), pieceColour.black) != -1)
                        throw new AssertFailedException("Threatmap did not update cover levels correctly for discovered spaces");
                }
                else
                {
                    if (ourBoard.getCoverLevel(new squarePos(0, x), pieceColour.black) != 0)
                        throw new AssertFailedException("Threatmap did not update cover levels correctly for discovered spaces");
                }
            }

            // Both rooks should be threatened once, by the other rook.
            if (ourBoard.getCoverLevel(new squarePos(0, 0), pieceColour.black) !=  1 ||
                ourBoard.getCoverLevel(new squarePos(0, 7), pieceColour.white) !=  1   )
                throw new AssertFailedException("Threatmap did not update cover levels correctly for rooks");
        }
        
        [TestMethod]
        public void testMoveUndoingThreatmapWithCapture()
        {
            Board ourBoard = Board.makeFromFEN("8/8/8/8/4p3/5P2/8/8 b - - 0 1", gameType.normal, boardSearchConfig.getDebugConfig());
            square ourPawn = ourBoard[4, 3];
            square enemyPawn = ourBoard[5, 2];

            string origThreatMap = ourBoard.coverLevel.ToString();

            int[,] threatCounts = new int[Board.sizeX,Board.sizeY];
            for (int x = 0; x < Board.sizeX; x++)
            {
                for (int y = 0; y < Board.sizeY; y++)
                {
                    threatCounts[x, y] = ourBoard[x, y].coveredSquares.Count;
                }
            }

            move ourMove = new move(ourPawn, enemyPawn);
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
            boardSearchConfig cfg = boardSearchConfig.getDebugConfig();
            cfg.checkLots = true;
            cfg.checkThreatMapLots = true;
            Board ourBoard = Board.makeNormalStartPosition(cfg);
            string origThreatMap = ourBoard.coverLevel.ToString();

            int[,] threatCounts = new int[Board.sizeX, Board.sizeY];
            for (int x = 0; x < Board.sizeX; x++)
            {
                for (int y = 0; y < Board.sizeY; y++)
                {
                    threatCounts[x, y] = ourBoard[x, y].coveredSquares.Count;
                }
            }

            sizableArray<move> moves = ourBoard.getMoves(pieceColour.white);
            if (moves.Length == 0)
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
                        {
                            Debug.WriteLine("Troublesome move : " + thisMove.ToString());
                            throw new AssertFailedException("Threat count incorrect at " + x + ", " + y + ": expected " + threatCounts[x, y] + " actual " + ourBoard[x, y].coveredSquares.Count);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void testMoveUndoingThreatmap_Castling()
        {
            Board ourBoard = new Board(gameType.normal, boardSearchConfig.getDebugConfig());
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
            Board ourBoard = Board.makeQueenAndPawnsStartPosition(boardSearchConfig.getDebugConfig());

            string expectedThreatString =
                " -1 -1 -1  0 -1 -1 -1 -1\r\n  0  0 -1 -1 -1  0  0  0\r\n  0 -1  0 -1  0 -1  0  0\r\n -1  0  0 -1  0  0 -1  0\r\n  0  0  0 -1  0  0  0 -1\r\n  1  2  2  1  2  2  2  1\r\n  0  0  0 -1  0  0  0  0\r\n  0  0  0  0  0  0  0  0\r\n";

            string threatString = ourBoard.coverLevel.ToString();

            if (expectedThreatString != threatString)
                throw new Exception("Threat map was incorrect");
        }

        [TestMethod]
        public void testThreatMapNormalStartPosition()
        {
            Board ourBoard = Board.makeNormalStartPosition(boardSearchConfig.getDebugConfig());

            string expectedThreatString =
                "  0 -1 -1 -1 -1 -1 -1  0\r\n -1 -1 -1 -4 -4 -1 -1 -1\r\n -2 -2 -3 -2 -2 -3 -2 -2\r\n  0  0  0  0  0  0  0  0\r\n  0  0  0  0  0  0  0  0\r\n  2  2  3  2  2  3  2  2\r\n  1  1  1  4  4  1  1  1\r\n  0  1  1  1  1  1  1  0\r\n";

            string threatString = ourBoard.coverLevel.ToString();

            if (expectedThreatString != threatString)
                throw new Exception("bad threat map");
        }
    }
}