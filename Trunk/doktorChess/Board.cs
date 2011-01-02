using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace doktorChess
{
    public class Board 
    {
        public const int sizeX = 8;
        public const int sizeY = 8;

        public int searchDepth = 4;

        private readonly square[,] _squares = new square[sizeX, sizeY];

        /// <summary>
        /// Lookaside list of squares occupied by white pieces
        /// </summary>
        private List<square> whitePieceSquares = new List<square>();

        /// <summary>
        /// Lookaside list of squares occupied by black pieces
        /// </summary>
        private List<square> blackPieceSquares = new List<square>();

        // Keep some search stats in here
        public moveSearchStats stats;

        public Board ()
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    _squares[x, y] = new square(new squarePos(x,y));
                }
            }            
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

        public static Board makeQueenAndPawnsStartPosition()
        {
            Board newBoard = new Board();

            for (int x = 0; x < sizeX; x++)
                newBoard.addPiece(x, 1, pieceType.pawn, pieceColour.white);

            newBoard.addPiece(3, 7, pieceType.queen, pieceColour.black);

            return newBoard;
        }

        public square addPiece(int x, int y, pieceType newType, pieceColour newColour)
        {
            _squares[x, y] = square.makeSquare(newType, newColour, new squarePos(x, y));

            if (newColour == pieceColour.white)
                whitePieceSquares.Add(_squares[x, y]);
            else if (newColour == pieceColour.black)
                blackPieceSquares.Add(_squares[x, y]);
            else
                throw new ArgumentException();

            return _squares[x, y];
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

            BoardScorer scorer = new BoardScorer(myPieces, enemyPieces );
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
            // For queen-and-pawns, for now.

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

                if ( (piece.position.y == sizeY-1 && piece.colour == pieceColour.white ) ||
                     (piece.position.y == 0 && piece.colour == pieceColour.black ) )
                {
                    // We have won the game if it is ours., or lost if not.
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
            lineAndScore toRet = findBestMove(playerCol, true, searchDepth);
            watch.Stop();
            stats.totalSearchTime = watch.ElapsedMilliseconds;

            return toRet;
        }

        private lineAndScore findBestMove(pieceColour playerCol, bool usToPlay, int depthLeft)
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
                bestLineSoFar = new lineAndScore(new move[searchDepth + 1], int.MinValue);
            else
                bestLineSoFar = new lineAndScore(new move[searchDepth + 1], int.MaxValue);

            // Find a list of possible moves..
            List<move> movesToConsider = getMoves(toPlay);
            if (movesToConsider.Count == 0)
            {
                // This should totally and absolutely not happen after we've verified that the
                // game is still in progress.
                throw new ArgumentException();
            }

            // As we score each, keep the score in this var. We declare it outside the loop to
            // prevent costly re-instantiation.
// ReSharper disable TooWideLocalVariableScope
            int score;
// ReSharper restore TooWideLocalVariableScope
            foreach (move consideredMove in movesToConsider)
            {
                doMove(consideredMove);

                if (depthLeft == 0)
                {
                    stats.boardsScored++;
                    score = getScore(playerCol);

                    if ((usToPlay && (score >= bestLineSoFar.finalScore )) ||
                         (!usToPlay && (score <= bestLineSoFar.finalScore)))
                    {
                        bestLineSoFar.finalScore = score;
                        bestLineSoFar.line[searchDepth] = consideredMove;
                    }
                }
                else
                {
                    lineAndScore thisMove = findBestMove(playerCol, !usToPlay, depthLeft - 1);

                    if ((usToPlay && (thisMove.finalScore >= bestLineSoFar.finalScore)) ||
                        (!usToPlay && (thisMove.finalScore <= bestLineSoFar.finalScore)))
                    {
                        bestLineSoFar.finalScore = thisMove.finalScore;
                        bestLineSoFar.line[searchDepth - depthLeft] = consideredMove;
                        for (int index = 0; index < thisMove.line.Length; index++)
                        {
                            if (thisMove.line[index] != null)
                                bestLineSoFar.line[index] = thisMove.line[index];
                        }
                    }
                }

                undoMove(consideredMove);
            }

            return bestLineSoFar;
        }

        public void doMove(move move)
        {
            square src = this[move.srcPos];
            square dst = this[move.dstPos];

            square tmp = dst;
            this[move.dstPos] = this[move.srcPos];
            this[move.srcPos] = tmp;

            this[move.dstPos].position = move.dstPos;
            this[move.srcPos] = new square(move.srcPos);

            if (move.isCapture)
            {
                if (dst.colour == pieceColour.white)
                    whitePieceSquares.Remove(dst);
                else if (dst.colour == pieceColour.black)
                    blackPieceSquares.Remove(dst);
            }

            src.movedCount++;
        }

        private void undoMove(move move)
        {
            this[move.srcPos] = this[move.dstPos];
            this[move.srcPos].position = move.srcPos;

            if (move.isCapture)
            {
                this[move.dstPos] = move.capturedSquare;

                if (move.capturedSquare.colour == pieceColour.white)
                    whitePieceSquares.Add(move.capturedSquare);
                else if (move.capturedSquare.colour == pieceColour.black)
                    blackPieceSquares.Add(move.capturedSquare);
            }
            else
            {
                this[move.dstPos] = new square(move.dstPos);
            }

            this[move.dstPos].movedCount--;
        }

    }

    public class moveSearchStats
    {
        public int boardsScored;
        public long totalSearchTime;
        public long boardScoreTime;

        public double scoredPerSecond
        {
            get { return boardsScored/(totalSearchTime/1000.0); }
        }
    }
}