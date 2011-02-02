using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace doktorChess
{
    public class Board 
    {
        public const int sizeX = 8;
        public const int sizeY = 8;

        /// <summary>
        /// How many moves have been played on this board
        /// </summary>
        public int moveCount = 0;

        /// <summary>
        /// Rules in effect for this game
        /// </summary>
        private readonly gameType _type;

        private readonly square[,] _squares = new square[sizeX, sizeY];

        private bool _whiteKingCaptured = false;
        private bool _blackKingCaptured = false;
        public bool whiteHasCastled;
        public bool blackHasCastled;

        /// <summary>
        /// Lookaside list of squares occupied by white pieces
        /// </summary>
        private List<square> whitePieceSquares = new List<square>();

        /// <summary>
        /// Lookaside list of squares occupied by black pieces
        /// </summary>
        private List<square> blackPieceSquares = new List<square>();

        private boardSearchConfig searchConfig;

        public killerMoveStore killerStore;
        public IEnableableThreatMap coverLevel;

        // Keep some search stats in here
        public moveSearchStats stats;

        public Board (gameType newType, boardSearchConfig newSearchConfig)
        {
            // Set the game type
            _type = newType;

            // Spawn all our squares
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                    _squares[x, y] = new square(new squarePos(x,y));
            }

            // Note that kings are captured, since none exist.
            _blackKingCaptured = true;
            _whiteKingCaptured = true;

            if (newSearchConfig.useThreatMap)
                coverLevel = new threatMap(this);
            else
                coverLevel = new disabledThreatMap();

            coverLevel.checkStuff = newSearchConfig.checkThreatMapLots;
            searchConfig = newSearchConfig;
        }

        public square this[squarePos myPos]
        {
            get { return _squares[myPos.x, myPos.y]; }
            private set { _squares[myPos.x, myPos.y] = value; }
        }

        public square this[int x, int y]
        {
            get { return _squares[x, y]; }
            private set { _squares[x, y] = value; }
        }

        public static Board makeQueenAndPawnsStartPosition(boardSearchConfig searchConfig)
        {
            Board newBoard = new Board(gameType.queenAndPawns, searchConfig);

            for (int x = 0; x < sizeX; x++)
                newBoard.addPiece(pieceType.pawn, pieceColour.white, x, 1);

            newBoard.addPiece(pieceType.queen, pieceColour.black, 3, 7);

            return newBoard;
        }

        public static Board makeNormalStartPosition(boardSearchConfig searchConfig)
        {
            Board newBoard = new Board(gameType.normal, searchConfig);

            // Apply two rows of pawns
            for (int x = 0; x < sizeX; x++)
            {
                newBoard.addPiece(pieceType.pawn, pieceColour.white, x, 1);
                newBoard.addPiece(pieceType.pawn, pieceColour.black, x, 6);
            }

            // And now fill in the two end ranks.
            foreach (int y in new int[] { 0, 7 })
            {
                pieceColour col = (y == 0 ? pieceColour.white : pieceColour.black);

                newBoard.addPiece(pieceType.rook, col, 0, y);
                newBoard.addPiece(pieceType.knight, col, 1, y);
                newBoard.addPiece(pieceType.bishop, col, 2, y);
                newBoard.addPiece(pieceType.queen, col, 3, y);
                newBoard.addPiece(pieceType.king, col, 4, y);
                newBoard.addPiece(pieceType.bishop, col, 5, y);
                newBoard.addPiece(pieceType.knight, col, 6, y);
                newBoard.addPiece(pieceType.rook, col, 7, y);
            }

            newBoard._blackKingCaptured = false;
            newBoard._whiteKingCaptured = false;

            return newBoard;
        }

        public void sanityCheck()
        {
            if (!searchConfig.checkLots)
                return;

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    // Check the square's position is set to its location in the array
                    if (!this[x, y].position.isSameSquareAs(x, y))
                        throw new Exception("Square grid is broken");
                }
            }

            int seenBlack = 0;
            int seenWhite = 0;
            bool blackKingSeen = false;
            bool whiteKingSeen = false;
            foreach (square square in _squares)
            {
                if (square.type == pieceType.none)
                    continue;

                switch(square.colour)
                {
                    case pieceColour.black:
                        if (!blackPieceSquares.Contains(square))
                            throw new Exception("Black square list does not contain all black squares");
                        seenBlack++;
                        if (square.type == pieceType.king)
                        {
                            if (blackKingSeen)
                                throw new Exception("Multiple black kings on board");
                            blackKingSeen = true;
                        }
                        break;
                    case pieceColour.white:
                        if (!whitePieceSquares.Contains(square))
                            throw new Exception("White square list does not contain all white squares");
                        seenWhite++;
                        if (square.type == pieceType.king)
                        {
                            if (whiteKingSeen)
                                throw new Exception("Multiple white kings on board");
                            whiteKingSeen = true;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (seenWhite != whitePieceSquares.Count)
                throw new Exception("White piece list contains extra pieces");
            if (seenBlack != blackPieceSquares.Count)
                throw new Exception("Black piece list contains extra pieces");

            if (blackKingSeen != !_blackKingCaptured)
                throw new Exception("Black king capture status incorrect");
            if (whiteKingSeen != !_whiteKingCaptured)
                throw new Exception("White king capture status incorrect");
        }

        public override string ToString()
        {
            StringBuilder toRet = new StringBuilder(sizeX * sizeY * 2);

            for (int y = sizeY-1; y > -1; y--)
            {
                for (int x = 0; x < sizeX; x++)
                    toRet.Append(_squares[x,y].ToString());
                toRet.Append(Environment.NewLine);
            }
            toRet.Append(Environment.NewLine);

            return toRet.ToString();
        }

        private square addPiece(pieceType newType, pieceColour newColour, squarePos dstPos)
        {
            this[dstPos] = square.makeSquare(newType, newColour, dstPos);
            addNewPieceToArrays(this[dstPos]);
            coverLevel.add(dstPos.x, dstPos.y);

            return this[dstPos];
        }

        public square addPiece(pieceType newType, pieceColour newColour, int x, int y)
        {
            square toRet = addPiece(newType, newColour, new squarePos(x, y));

            return toRet;
        }

        private void addPiece(square newSquare, squarePos newSquarePos)
        {
            newSquare.position = newSquarePos;
            this[newSquarePos] = newSquare;
            addNewPieceToArrays(newSquare);
            coverLevel.add(newSquarePos);
        }

        private void removePiece(square toRemove)
        {
            coverLevel.remove(toRemove);
            removeNewPieceFromArrays(toRemove);
            this[toRemove.position] = new square(toRemove.position);
        }

        private void addNewPieceToArrays(square newSq)
        {
            if (whitePieceSquares.Contains(newSq) ||
                blackPieceSquares.Contains(newSq))
                throw new Exception("Duplicate square");

            switch (newSq.colour)
            {
                case pieceColour.white:
                    whitePieceSquares.Add(newSq);
                    break;
                case pieceColour.black:
                    blackPieceSquares.Add(newSq);
                    break;
                default:
                    throw new ArgumentException();
            }

            if (newSq.type == pieceType.king)
            {
                if (newSq.colour == pieceColour.black)
                    _blackKingCaptured = false;
                else if (newSq.colour == pieceColour.white)
                    _whiteKingCaptured = false;
                else
                    throw new ArgumentException();
            }
        }

        private void removeNewPieceFromArrays(square newSq)
        {
            switch (newSq.colour)
            {
                case pieceColour.white:
                    whitePieceSquares.Remove(newSq);
                    break;
                case pieceColour.black:
                    blackPieceSquares.Remove(newSq);
                    break;
                default:
                    throw new ArgumentException();
            }

            if (newSq.type == pieceType.king)
            {
                if (newSq.colour == pieceColour.black)
                    _blackKingCaptured = true;
                else if (newSq.colour == pieceColour.white)
                    _whiteKingCaptured = true;
                else
                    throw new ArgumentException();
            }
        }

        public List<move> getMoves(pieceColour toMoveColour)
        {
            List<square> occupiedSquares = getPiecesForColour(toMoveColour);

            List<move> possibleMoves = new List<move>(occupiedSquares.Count * 5);
            foreach (square occupiedSquare in occupiedSquares)
                possibleMoves.AddRange(occupiedSquare.getPossibleMoves(this));

            return possibleMoves;
        }

        public List<square> getPiecesForColour(pieceColour toMoveColour)
        {
            if (toMoveColour == pieceColour.white)
                return whitePieceSquares;
            else if (toMoveColour == pieceColour.black)
                return blackPieceSquares;
            else
                throw new ArgumentException();

        }

        public int getScore(pieceColour myPieceColour)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<square> myPieces = getPiecesForColour(myPieceColour);
            List<square> enemyPieces = getPiecesForColour(getOtherSide(myPieceColour));

            BoardScorer scorer = new BoardScorer(this, myPieces, enemyPieces );
            scorer.setGameStatus(getGameStatus(myPieces, enemyPieces));
            int toRet = scorer.getScore();
            watch.Stop();
            stats.boardScoreTime += watch.ElapsedMilliseconds;

            return toRet;
        }

        public static pieceColour getOtherSide(pieceColour ofThisSide)
        {
            switch (ofThisSide)
            {
                case pieceColour.black:
                    return pieceColour.white;
                case pieceColour.white:
                    return pieceColour.black;
                default:
                    throw new ArgumentException();
            }
        }

        public gameStatus getGameStatus(pieceColour myPieceColour)
        {
            List<square> myPieces = getPiecesForColour(myPieceColour);
            List<square> enemyPieces = getPiecesForColour(getOtherSide(myPieceColour));

            return getGameStatus(myPieces, enemyPieces);
        }

        public gameStatus getGameStatus(List<square> myPieces, List<square> enemyPieces)
        {
            if (_type == gameType.queenAndPawns)
                return getGameStatusForQueenAndPawns(myPieces, enemyPieces);
            else if (_type == gameType.normal)
                return getGameStatusForNormal(myPieces, enemyPieces);
            else
                throw  new ArgumentException();
        }

        private gameStatus getGameStatusForNormal(List<square> myPieces, List<square> enemyPieces)
        {
            // If either side has no pieces, the game is undefined.
            if (myPieces.Count == 0)
                return gameStatus.lost;
            if (enemyPieces.Count == 0)
                return gameStatus.won;

            // Now we are certain we have pieces, we can identify our side.
            pieceColour myCol = myPieces[0].colour;

            if (_whiteKingCaptured)
                return myCol == pieceColour.white ? gameStatus.lost : gameStatus.won;
            else if (_blackKingCaptured)
                return myCol == pieceColour.black ? gameStatus.lost : gameStatus.won;

            // If the current player cannot move, then the game is a draw.
            bool movesFound = false;
            foreach (square myPiece in myPieces)
            {
                if (myPiece.getPossibleMoves(this).Count != 0)
                {
                    movesFound = true;
                    break;
                }
            }

            if (!movesFound)
                return gameStatus.drawn;

            return gameStatus.inProgress;
        }

        private gameStatus getGameStatusForQueenAndPawns(List<square> myPieces, List<square> enemyPieces)
        {
            // The game is over if either side has no pieces left
            if (myPieces.Count == 0)
                return gameStatus.lost;
            if (enemyPieces.Count == 0)
                return gameStatus.won;

            // Now we are certain we have pieces, we can identify our side.
            pieceColour myCol = myPieces[0].colour;

            // If the current player cannot move, it is a draw.
            bool movesFound = false;
            foreach (square myPiece in myPieces)
            {
                if (myPiece.getPossibleMoves(this).Count != 0)
                {
                    movesFound = true;
                    break;
                }
            }

            if (!movesFound)
                return gameStatus.drawn;

            // The game is won if a pawn has made it to the other side of the board.
            List<square> allPieces = new List<square>( whitePieceSquares.Count + blackPieceSquares.Count );
            allPieces.AddRange(getPiecesForColour(pieceColour.white));
            allPieces.AddRange(getPiecesForColour(pieceColour.black));

            foreach (square piece in allPieces)
            {
                if (piece.type != pieceType.pawn)
                    continue;

                if ( (piece.position.y == sizeY - 1 && piece.colour == pieceColour.white ) ||
                     (piece.position.y == 0 && piece.colour == pieceColour.black ) )
                {
                    // We have won the game if it is ours, or lost if not.
                    if (myCol == piece.colour )
                        return gameStatus.won;
                    else
                        return gameStatus.lost;
                }
            }

            return gameStatus.inProgress;
        }

        public lineAndScore findBestMove(pieceColour playerCol)
        {
            stats = new moveSearchStats();
            // Assume it is our move.
            Stopwatch watch = new Stopwatch();
            watch.Start();

            if ((searchConfig.killerHeuristic && !searchConfig.killerHeuristicPersists) ||
                 (searchConfig.killerHeuristic && killerStore == null))
                killerStore = new killerMoveStore(searchConfig.searchDepth);

            lineAndScore toRet = findBestMove(playerCol, true, searchConfig.searchDepth, int.MinValue, int.MaxValue);

            watch.Stop();
            stats.totalSearchTime = watch.ElapsedMilliseconds;

            return toRet;
        }

        public void advanceKillerTables()
        {
            killerStore.advanceOne(searchConfig.searchDepth);
        }

        private lineAndScore findBestMove(pieceColour playerCol, bool usToPlay, int depthLeft, int min, int max)
        {
            pieceColour enemyCol = getOtherSide(playerCol);
            pieceColour toPlay = usToPlay ? playerCol : enemyCol;

            if (getGameStatus(toPlay) != gameStatus.inProgress)
            {
                // The game is over. Return a score immediately, and no moves.
                stats.boardsScored++;
                return new lineAndScore( new move[] {}, getScore(playerCol));
            }

            // Find our best move and its score. Initialise the best score to max or min,
            // depending if we're searching for the minimum or maximum score.
            lineAndScore bestLineSoFar;
            if (usToPlay)
                bestLineSoFar = new lineAndScore(new move[searchConfig.searchDepth + 1], int.MinValue);
            else
                bestLineSoFar = new lineAndScore(new move[searchConfig.searchDepth + 1], int.MaxValue);

            // Find a list of possible moves..
            List<move> movesToConsider = getMoves(toPlay);
            if (movesToConsider.Count == 0)
            {
                // This should totally and absolutely not happen after we've verified that the
                // game is still in progress.
                throw new ArgumentException();
            }

            if (searchConfig.killerHeuristic)
            {
                int killerMoves = 0;

                // If one of these moves caused a cutoff last time, consider that move first.
                List<move> reorderedMovesToConsider = new List<move>(movesToConsider.Count);
                foreach (move consideredMove in movesToConsider)
                {
                    if (killerStore.contains(consideredMove, depthLeft))
                    {
                        reorderedMovesToConsider.Add(consideredMove);
                        killerMoves++;
                    }
                }
                if (reorderedMovesToConsider.Count > 0)
                {
                    foreach (move move in movesToConsider)
                    {
                        if (!reorderedMovesToConsider.Contains(move))
                            reorderedMovesToConsider.Add(move);
                    }
                    movesToConsider = reorderedMovesToConsider;
                }
            }

            // As we score each, keep the score in this var. We declare it outside the loop to
            // prevent costly re-instantiation.
// ReSharper disable TooWideLocalVariableScope
            int score;
// ReSharper restore TooWideLocalVariableScope
            int moveIndex = 0;
            foreach (move consideredMove in movesToConsider)
            {
                // If we're checking heavily, we check that the board is restored correctly after we
                // undo any move.
                string origThreatMap = null;
                string origPieces = null;
                if (searchConfig.checkLots)
                {
                    origThreatMap = coverLevel.ToString();
                    origPieces = ToString();
                }

                doMove(consideredMove);

                if (depthLeft == 0)
                {
                    stats.boardsScored++;
                    score = getScore(playerCol);

                    if ((usToPlay && (score > bestLineSoFar.finalScore)) ||
                         (!usToPlay && (score < bestLineSoFar.finalScore)))
                    {
                        bestLineSoFar.finalScore = score;
                        bestLineSoFar.line[searchConfig.searchDepth] = consideredMove;
                    }
                }
                else
                {
                    lineAndScore thisMove = findBestMove(playerCol, !usToPlay, depthLeft - 1, min, max);

                    if ((usToPlay && (thisMove.finalScore > bestLineSoFar.finalScore)) ||
                        (!usToPlay && (thisMove.finalScore < bestLineSoFar.finalScore)))
                    {
                        bestLineSoFar.finalScore = thisMove.finalScore;
                        bestLineSoFar.line[searchConfig.searchDepth - depthLeft] = consideredMove;
                        for (int index = 0; index < thisMove.line.Length; index++)
                        {
                            if (thisMove.line[index] != null)
                                bestLineSoFar.line[index] = thisMove.line[index];
                        }
                    }
                }

                undoMove(consideredMove);

                // Verify that the board has been restored to its former state correctly.
                if (searchConfig.checkLots)
                {
                    if (origPieces != ToString())
                        throw new Exception("Board pieces were not restored correctly");

                    if (origThreatMap != coverLevel.ToString())
                    {
                        Debug.WriteLine("Move : " + consideredMove.ToString());
                        Debug.WriteLine("expected");
                        Debug.WriteLine(origThreatMap);
                        Debug.WriteLine("actual");
                        Debug.WriteLine(coverLevel.ToString());
                        throw new Exception("Threat map was not restored correctly");
                    }
                }

                if (searchConfig.useAlphaBeta)
                {
                    if (depthLeft > 0)
                    {
                        if (usToPlay)
                        {
                            if (min < bestLineSoFar.finalScore)
                                min = bestLineSoFar.finalScore;

                            if (min >= max)
                            {
                                if (searchConfig.killerHeuristic)
                                {
                                    killerStore.add(depthLeft, consideredMove);
                                }
                                break;
                            }
                        }
                        else
                        {
                            if (max > bestLineSoFar.finalScore)
                                max = bestLineSoFar.finalScore;

                            if (max <= min)
                            {
                                if (searchConfig.killerHeuristic)
                                {
                                    killerStore.add(depthLeft, consideredMove);
                                }
                                break;
                            }
                        }
                    }
                }
                moveIndex++;
            }

            return bestLineSoFar;
        }

        public void doMove(move move)
        {
            sanityCheck();

            // Update movedness count for the moving piece
            this[move.srcPos].moveNumbers.Push(moveCount);
            this[move.srcPos].movedCount++;
            
            square movingSquare = this[move.srcPos];

            // If this move is a castling, update the rooks movedCount
            if (move.isACastling())
            {
                if (this[move.srcPos].colour == pieceColour.white)
                    whiteHasCastled = true;
                else if (this[move.srcPos].colour == pieceColour.black)
                    blackHasCastled = true;
                else
                    throw new Exception("Cannot identify colour");

                rookSquare rook = move.findCastlingRook(this);
                move.castlingRook = rook;
                rook.movedCount++;
                rook.moveNumbers.Push(moveCount);                
            }

            // Update the number of moves on this board
            moveCount++;

            // Move our piece from the source to the destination, removing any piece that might be there
            if (this[move.dstPos].type != pieceType.none)
            {
                removePiece(this[move.dstPos]);
                if (!move.isCapture)
                    throw new Exception("Non-capture in to occupied square");
            }
            movePiece(movingSquare, move.dstPos);

            // If we're castling, move the rook appropriately
            if (move.isACastling())
            {
                square rook = move.findCastlingRook(this);
                movePiece(rook, move.findNewPosForCastlingRook() );
            }

            square captured = null;
            if (move.isCapture)
            {
                captured = move.capturedSquare;

                if (!captured.position.isSameSquareAs(move.dstPos))
                {
                    // The capture was not in to our piece's destination square. Set the captured square
                    // to be empty.
                    this[captured.position] = new square(captured.position);
                    removeNewPieceFromArrays(captured);
                    coverLevel.remove(captured);
                }
            }

            if (move.isPawnPromotion)
            {
                removePiece(movingSquare);
                this[move.dstPos] = addPiece(move.typeToPromoteTo, movingSquare.colour, move.dstPos);
                this[move.dstPos].pastLife = movingSquare;
            }


            sanityCheck();
        }

        private void movePiece(square toMove, squarePos destPos)
        {
            squarePos srcPos = toMove.position;

            coverLevel.remove(toMove);
            this[destPos] = toMove;
            this[destPos].position = destPos;
            this[srcPos] = new square(srcPos);
            coverLevel.add(destPos);
        }

        public void undoMove(move move)
        {
            sanityCheck();

            // Revert any promotion
            if (move.isPawnPromotion)
            {
                square promoted = this[move.dstPos];
                removePiece(promoted);
                this[move.dstPos] = promoted.pastLife;
                addNewPieceToArrays(promoted.pastLife);
            }

            // dec the move counters
            moveCount--;
            this[move.dstPos].movedCount--;
            if (this[move.dstPos].moveNumbers.Count == 0)
                this[move.dstPos].moveNumbers = this[move.dstPos].moveNumbers;
            Debug.Assert(this[move.dstPos].moveNumbers.Pop() == moveCount);

            // Dec the rook's move counter if this is a castling
            if (move.isACastling())
            {
                if (this[move.dstPos].colour == pieceColour.white)
                    whiteHasCastled = false;
                else if (this[move.dstPos].colour == pieceColour.black)
                    blackHasCastled = false;
                else
                    throw new Exception("Cannot identify colour");

                square rook = move.castlingRook;
                rook.movedCount--;
                Assert.AreEqual(moveCount, rook.moveNumbers.Pop());
            }

            // store the piece which is moving before we erase it
            square movingPiece = this[move.dstPos];

            // Erase the old destination
            this[move.dstPos] = new square(move.dstPos);
            coverLevel.remove(movingPiece);

            // Move our piece back to its source square
            this[move.srcPos] = movingPiece;
            this[move.srcPos].position = move.srcPos;
            coverLevel.add(move.srcPos); 

            // If a castling, move the rook back too.
            if (move.isACastling())
            {
                square rook = move.castlingRook;

                removePiece(rook);

                this[rook.position] = new square(rook.position);
                if (rook.position.x == 3)
                {
                    addPiece(rook, new squarePos(0, rook.position.y));
                    //this[0, rook.position.y] = rook;
                    //rook.position = new squarePos(0, rook.position.y);
                }
                else if (rook.position.x == 5)
                {
                    addPiece(rook, new squarePos(7, rook.position.y));
                    //this[7, rook.position.y] = rook;
                    //rook.position = new squarePos(7, rook.position.y);                    
                }
                else
                    Assert.Fail("While uncastling, could not find castled rook");
            }

            // Restore any captured piece
            if (move.isCapture)
                addPiece(move.capturedSquare, move.capturedSquarePos);

            sanityCheck();
        }


        public bool playerIsInCheck(pieceColour playerPossiblyInCheck)
        {
            List<move> moves = getMoves(getOtherSide(playerPossiblyInCheck));

            return moves.Exists(a => a.isCapture && a.capturedSquare.type == pieceType.king);
        }

        public bool wouldMovePutPlayerInCheck(pieceColour playerCol, move playersMove)
        {
            doMove(playersMove);
            bool toRet = playerIsInCheck(playerCol);
            undoMove(playersMove);

            return toRet;
        }

        public int getCoverLevel(square squareToCheck, pieceColour sideToExamine)
        {
            return getCoverLevel(squareToCheck.position, sideToExamine);
        }

        public int getCoverLevel(squarePos squareToCheck, pieceColour sideToExamine)
        {
            return sideToExamine == pieceColour.white ? coverLevel[squareToCheck] : -coverLevel[squareToCheck];
        }

        public bool isThreatened(square squareToCheck, pieceColour sideToExamine)
        {
            // Return true if the square is covered by at least one enemy piece. Ignore if we are 
            // covering it or not.
            return coverLevel.isThreatened(squareToCheck, sideToExamine);
        }
    }
}