using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using doktorChessGameEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace doktorChess
{
    public class Board : baseBoard
    {
        private bool _whiteKingCaptured = false;
        private bool _blackKingCaptured = false;
        public bool whiteHasCastled;
        public bool blackHasCastled;

        /// <summary>
        /// Lookaside of white material advantage
        /// </summary>
        public int whiteMaterialAdvantage;

        /// <summary>
        /// Lookaside of black material advantage
        /// </summary>
        public int blackMaterialAdvantage;

        private readonly boardSearchConfig _searchConfig;

        private killerMoveStore _killerStore;
        public readonly IEnableableThreatMap coverLevel;

        // Keep some search stats in here
        public moveSearchStats stats;

        public Board(gameType newType, boardSearchConfig newSearchConfig)
            : base(newType)
        {
            // Note that kings are captured, since none exist.
            _blackKingCaptured = true;
            _whiteKingCaptured = true;

            if (newSearchConfig.useThreatMap)
                coverLevel = new threatMap(this);
            else
                coverLevel = new disabledThreatMap();

            coverLevel.checkStuff = newSearchConfig.checkThreatMapLots;
            _searchConfig = newSearchConfig;
        }

        public static Board makeQueenAndPawnsStartPosition(boardSearchConfig searchConfig)
        {
            Board newBoard = new Board(gameType.queenAndPawns, searchConfig);

            newBoard.makeStartPosition();

            return newBoard;
        }

        public static Board makeNormalStartPosition()
        {
            boardSearchConfig config = new boardSearchConfig();
            Board newBoard = new Board(gameType.normal, config);

            newBoard.makeStartPosition();

            // Both kings are present in the new board.
            newBoard._blackKingCaptured = false;
            newBoard._whiteKingCaptured = false;

            return newBoard;
        }

        public static Board makeNormalStartPosition(boardSearchConfig searchConfig)
        {
            Board newBoard = new Board(gameType.normal, searchConfig);

            newBoard.makeStartPosition();

            // Both kings are present in the new board.
            newBoard._blackKingCaptured = false;
            newBoard._whiteKingCaptured = false;

            return newBoard;
        }

        public static Board makeNormalFromFEN(string FENString, boardSearchConfig searchConfig)
        {
            Board newBoard = new Board(gameType.normal, searchConfig);

            newBoard.makeFromFEN(FENString);

            return newBoard;
        }

        protected override void sanityCheck()
        {
            if (!_searchConfig.checkLots)
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
            int seenBlackMaterialScore = 0;
            int seenWhiteMaterialScore = 0;
            bool blackKingSeen = false;
            bool whiteKingSeen = false;
            foreach (square square in _squares)
            {
                if (square.type == pieceType.none)
                    continue;

                switch (square.colour)
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
                        seenBlackMaterialScore += BoardScorer.getMaterialAdvantage(square.type);
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
                        seenWhiteMaterialScore += BoardScorer.getMaterialAdvantage(square.type);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (seenWhite != whitePieceSquares.Count)
                throw new Exception("White piece list contains extra pieces");
            if (seenBlack != blackPieceSquares.Count)
                throw new Exception("Black piece list contains extra pieces");

            if (seenWhiteMaterialScore != whiteMaterialAdvantage)
                throw new Exception("White material score is wrong");
            if (seenBlackMaterialScore != blackMaterialAdvantage)
                throw new Exception("Black material score is wrong");

            if (blackKingSeen != !_blackKingCaptured)
                throw new Exception("Black king capture status incorrect");
            if (whiteKingSeen != !_whiteKingCaptured)
                throw new Exception("White king capture status incorrect");
        }

        public override string ToString()
        {
            StringBuilder toRet = new StringBuilder(sizeX * sizeY * 2);

            for (int y = sizeY - 1; y > -1; y--)
            {
                for (int x = 0; x < sizeX; x++)
                    toRet.Append(_squares[x, y].ToString());
                toRet.Append(Environment.NewLine);
            }
            toRet.Append(Environment.NewLine);

            return toRet.ToString();
        }

        protected override square addPiece(pieceType newType, pieceColour newColour, squarePos dstPos)
        {
            base.addPiece(newType, newColour, dstPos);

            addNewPieceToArrays(this[dstPos]);
            coverLevel.add(dstPos.x, dstPos.y);

            return this[dstPos];
        }

        protected override void addPiece(square newSquare, squarePos newSquarePos)
        {
            base.addPiece(newSquare, newSquarePos);

            addNewPieceToArrays(this[newSquarePos]);
            coverLevel.add(newSquarePos.x, newSquarePos.y);
        }

        protected override void removePiece(square toRemove)
        {
            coverLevel.remove(toRemove);
            removeNewPieceFromArrays(toRemove);
            
            base.removePiece(toRemove);
        }

        protected override void movePiece(square toMove, squarePos destPos)
        {
            squarePos srcPos = toMove.position;

            coverLevel.remove(toMove);
            this[destPos] = toMove;
            this[destPos].position = destPos;
            this[srcPos] = new square(srcPos);
            coverLevel.add(destPos);
        }

        protected override void unmovePiece(squarePos srcPos, squarePos dstPos)
        {
            // store the piece which is moving before we erase it
            square movingPiece = this[dstPos];

            // Erase the old destination
            this[dstPos] = new square(dstPos);
            coverLevel.remove(movingPiece);

            // Move our piece back to its source square
            this[srcPos] = movingPiece;
            this[srcPos].position = srcPos;
            coverLevel.add(srcPos);
        }

        private void addNewPieceToArrays(square newSq)
        {
            switch (newSq.colour)
            {
                case pieceColour.white:
                    whiteMaterialAdvantage += BoardScorer.getMaterialAdvantage(newSq);
                    break;
                case pieceColour.black:
                    blackMaterialAdvantage += BoardScorer.getMaterialAdvantage(newSq);
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
                    whiteMaterialAdvantage -= BoardScorer.getMaterialAdvantage(newSq);
                    break;
                case pieceColour.black:
                    blackMaterialAdvantage -= BoardScorer.getMaterialAdvantage(newSq);
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

        public override lineAndScore findBestMove()
        {
            stats = new moveSearchStats();
            Stopwatch watch = new Stopwatch();
            watch.Start();

            if ((_searchConfig.killerHeuristic && !_searchConfig.killerHeuristicPersists) ||
                (_searchConfig.killerHeuristic && _killerStore == null))
            {
                if (_searchConfig.searchDepth > 0)
                    _killerStore = new killerMoveStore(_searchConfig.searchDepth);
            }

            lineAndScore toRet = findBestMove(true, _searchConfig.searchDepth, int.MinValue, int.MaxValue);

            watch.Stop();
            stats.totalSearchTime = watch.ElapsedMilliseconds;

            advanceKillerTables();

            return toRet;
        }

        private lineAndScore findBestMove(bool usToPlay, int depthLeft, int min, int max)
        {
            if (getGameStatus(colToMove) != gameStatus.inProgress)
            {
                // The game is over. Return a score immediately, and no moves.
                stats.boardsScored++;
                BoardScorer scorer = new BoardScorer(this, colToMove, _searchConfig.scoreConfig);
                //return new lineAndScore(new move[] { }, scorer.getScore() - (searchConfig.searchDepth - depthLeft), scorer);
                int score = scorer.getScore() * -1;
                return new lineAndScore(new move[] { }, score, scorer);
            }

            // Find our best move and its score. Initialise the best score to max or min,
            // depending if we're searching for the minimum or maximum score.
            lineAndScore bestLineSoFar;
            if (usToPlay)
                bestLineSoFar = new lineAndScore(new move[_searchConfig.searchDepth + 1], int.MinValue, null);
            else
                bestLineSoFar = new lineAndScore(new move[_searchConfig.searchDepth + 1], int.MaxValue, null);

            // Find a list of possible moves..
            sizableArray<move> movesToConsider = getMoves(colToMove);

            if (_searchConfig.checkLots)
            {
                if (movesToConsider.Length == 0)
                {
                    // This should totally and absolutely not happen after we've verified that the
                    // game is still in progress.
                    throw new ArgumentException();
                }

                bool valid = false;
                foreach (move thisMove in movesToConsider)
                {
                    if (valid)
                        break;

                    doMove(thisMove);

                    if (!playerIsInCheck(colToMove))
                        valid = true;

                    undoMove(thisMove);
                }

                // Niether should this.
                if (!valid)
                    throw new ArgumentException();
            }

            // Move any good (possibly killer) moves to the top of our search 
            prioritizeMoves(movesToConsider, depthLeft);

            foreach (move consideredMove in movesToConsider)
            {
                // If we're checking heavily, we check that the board is restored correctly after we
                // undo any move.
                string origThreatMap = null;
                string origPieces = null;
                if (_searchConfig.checkLots)
                {
                    origThreatMap = coverLevel.ToString();
                    origPieces = ToString();
                }

                doMove(consideredMove);

                // If this move would leave us in check, we can ignore it
                // TODO: optimise this.
                if (playerIsInCheck(colToMove))
                {
                    undoMove(consideredMove);
                    continue;
                }

                if (depthLeft == 0)
                {
                    stats.boardsScored++;
                    BoardScorer scorer = new BoardScorer(this, getOtherSide(colToMove), _searchConfig.scoreConfig);
                    int score = scorer.getScore();

                    // Modify our score based on how deep we are, so that we prefer shallower moves over deeper
                    //score += (searchConfig.searchDepth - depthLeft) * (usToPlay ? +1 : -1);

                    if ((usToPlay && (score > bestLineSoFar.finalScore)) ||
                        (!usToPlay && (score < bestLineSoFar.finalScore)))
                    {
                        bestLineSoFar.finalScore = score;
                        bestLineSoFar.line[_searchConfig.searchDepth] = consideredMove;
                        bestLineSoFar._scorer = scorer;
                    }
                }
                else
                {
                    lineAndScore thisMove = findBestMove(!usToPlay, depthLeft - 1, min, max);

                    if ((usToPlay && (thisMove.finalScore > bestLineSoFar.finalScore)) ||
                        (!usToPlay && (thisMove.finalScore < bestLineSoFar.finalScore)))
                    {
                        bestLineSoFar.finalScore = thisMove.finalScore;
                        bestLineSoFar._scorer = thisMove._scorer;
                        bestLineSoFar.line[_searchConfig.searchDepth - depthLeft] = consideredMove;
                        for (int index = 0; index < thisMove.line.Length; index++)
                        {
                            if (thisMove.line[index] != null)
                                bestLineSoFar.line[index] = thisMove.line[index];
                        }
                    }
                }

                undoMove(consideredMove);
                colToMove = getOtherSide(colToMove);

                // Verify that the board has been restored to its former state correctly.
                if (_searchConfig.checkLots)
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

                if (_searchConfig.useAlphaBeta)
                {
                    if (depthLeft > 0)
                    {
                        if (usToPlay)
                        {
                            if (min < bestLineSoFar.finalScore)
                                min = bestLineSoFar.finalScore;

                            if (min >= max)
                            {
                                if (_searchConfig.killerHeuristic)
                                {
                                    _killerStore.add(depthLeft, consideredMove);
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
                                if (_searchConfig.killerHeuristic)
                                {
                                    _killerStore.add(depthLeft, consideredMove);
                                }
                                break;
                            }
                        }
                    }
                }

            }

            return bestLineSoFar;
        }

        public void advanceKillerTables()
        {
            if (_killerStore != null)
                _killerStore.advanceOne(_searchConfig.searchDepth);
        }

        private void prioritizeMoves(sizableArray<move> movesToConsider, int depthLeft)
        {
            // Bring any moves which are 'probably good' to the top of our list. Hold an array of bools, and set
            // each one which corresponds to a good move, so that we can avoid moving anything twice.
            bool[] movesReordered = new bool[movesToConsider.Length];
            int[] reorderedMovesToConsider = new int[movesToConsider.Length];
            int reorderedCount = 0;

            int n = 0;
            if (_killerStore != null)
            {
                // If one of these moves caused a cutoff last time, consider that move first.
                foreach (move consideredMove in movesToConsider)
                {
                    if (_killerStore.contains(consideredMove, depthLeft))
                    {
                        movesReordered[n] = true;
                        reorderedMovesToConsider[reorderedCount++] = n;
                    }
                    n++;
                }
            }

            // Consider any capturing or pawn promotions first, too
            n = 0;
            foreach (move thisMove in movesToConsider)
            {
                if (thisMove.isCapture ||      // captures are good.
                    thisMove.isPawnPromotion)  // pawn promotions are good.
                {
                    if (movesReordered[n] == false)
                    {
                        movesReordered[n] = true;
                        reorderedMovesToConsider[reorderedCount++] = n;
                    }
                }
                n++;
            }

            // Swap any good moves such that they are at the top
            int swapCount = 0;
            for (int i = 0; i < reorderedCount; i++)
                movesToConsider.bringToPosition(reorderedMovesToConsider[i], swapCount++);
        }

        public int getCoverLevel(square squareToCheck, pieceColour sideToExamine)
        {
            return getCoverLevel(squareToCheck.position, sideToExamine);
        }

        public int getCoverLevel(squarePos squareToCheck, pieceColour sideToExamine)
        {
            return sideToExamine == pieceColour.white ? coverLevel[squareToCheck] : -coverLevel[squareToCheck];
        }

        public override bool isThreatenedHint(square squareToCheck, pieceColour sideToExamine)
        {
            // Return true if the square is covered by at least one enemy piece. Ignore if we are 
            // covering it or not.
            return coverLevel.isThreatened(squareToCheck, sideToExamine);
        }

        public override void doMove(move move)
        {
            // If this move is a castling, update the rooks movedCount
            if (move.isACastling)
            {
                if (this[move.srcPos].colour == pieceColour.white)
                    whiteHasCastled = true;
                else if (this[move.srcPos].colour == pieceColour.black)
                    blackHasCastled = true;
                else
                    throw new Exception("Cannot identify colour");
            }

            base.doMove(move);
        }

        public override void undoMove(move move)
        {
            if (move.isACastling)
            {
                if (this[move.dstPos].colour == pieceColour.white)
                    whiteHasCastled = false;
                else if (this[move.dstPos].colour == pieceColour.black)
                    blackHasCastled = false;
                else
                    throw new Exception("Cannot identify colour");

            }

            base.undoMove(move);
        }

        protected override gameStatus getGameStatusForNormal(List<square> myPieces, List<square> enemyPieces)
        {
            // 'normal' rule games must always have one king for each side.
            if (_whiteKingCaptured || _blackKingCaptured)
                throw new Exception("Chess rules violated - no king is present for at least one side");

            return base.getGameStatusForNormal(myPieces, enemyPieces);
        }
    }
}