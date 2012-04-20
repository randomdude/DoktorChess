using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace doktorChessGameEngine
{
    public abstract class baseBoard
    {
        public const int sizeX = 8;
        public const int sizeY = 8;

        /// <summary>
        /// How many moves have been played on this board
        /// </summary>
        public int moveCount;

        /// <summary>
        /// Ruleset in effect for this game
        /// </summary>
        protected gameType _type { get; private set; }

        /// <summary>
        /// Whose move it is
        /// </summary>
        public pieceColour colToMove = pieceColour.white;

        /// <summary>
        /// All the squares that our board comprises
        /// </summary>
        protected readonly square[,] _squares = new square[sizeX, sizeY];

        /// <summary>
        /// Lookaside list of squares occupied by white pieces
        /// </summary>
        public readonly List<square> whitePieceSquares = new List<square>(20);

        /// <summary>
        /// Lookaside list of squares occupied by black pieces
        /// </summary>
        public readonly List<square> blackPieceSquares = new List<square>(20);
        protected baseBoard(gameType newType)
        {
            // Set the game type
            _type = newType;

            // Spawn all our squares
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                    _squares[x, y] = new square(new squarePos(x, y));
            }
        }

        public square this[squarePos myPos]
        {
            // ReSharper disable UnusedMember.Local
            get { return _squares[myPos.x, myPos.y]; }
            protected set { _squares[myPos.x, myPos.y] = value; }
            // ReSharper restore UnusedMember.Local
        }

        public square this[int flatPos]
        {
            // ReSharper disable UnusedMember.Local
            get
            {
                squarePos pos = squarePos.unflatten(flatPos);
                return _squares[pos.x, pos.y];
            }
            protected set
            {
                squarePos pos = squarePos.unflatten(flatPos);
                _squares[pos.x, pos.y] = value;
            }
            // ReSharper restore UnusedMember.Local
        }

        public square this[int x, int y]
        {
            // ReSharper disable UnusedMember.Local
            get { return _squares[x, y]; }
            private set { _squares[x, y] = value; }
            // ReSharper restore UnusedMember.Local
        }

        protected void makeStartPosition()
        {
            switch (_type)
            {
                case gameType.normal:
                    makeNormalStartPosition();
                    break;
                case gameType.queenAndPawns:
                    makeQueenAndPawnsStartPosition();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void makeQueenAndPawnsStartPosition()
        {
            for (int x = 0; x < sizeX; x++)
                this.addPiece(pieceType.pawn, pieceColour.white, x, 1);

            this.addPiece(pieceType.queen, pieceColour.black, 3, 7);

            colToMove = pieceColour.white;
        }

        private void makeNormalStartPosition()
        {
            // Apply two rows of pawns
            for (int x = 0; x < sizeX; x++)
            {
                addPiece(pieceType.pawn, pieceColour.white, x, 1);
                addPiece(pieceType.pawn, pieceColour.black, x, 6);
            }

            // And now fill in the two end ranks.
            foreach (int y in new[] { 0, 7 })
            {
                pieceColour col = (y == 0 ? pieceColour.white : pieceColour.black);

                addPiece(pieceType.rook, col, 0, y);
                addPiece(pieceType.knight, col, 1, y);
                addPiece(pieceType.bishop, col, 2, y);
                addPiece(pieceType.queen, col, 3, y);
                addPiece(pieceType.king, col, 4, y);
                addPiece(pieceType.bishop, col, 5, y);
                addPiece(pieceType.knight, col, 6, y);
                addPiece(pieceType.rook, col, 7, y);
            }

            colToMove = pieceColour.white;
        }

        protected void makeFromFEN(string FENString)
        {
            parsedFEN fen = new parsedFEN(FENString);

            // Now create the board from the parsed output.
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    switch (fen.boardRepresentation[x, y])
                    {
                        case 'p':
                            addPiece(pieceType.pawn, pieceColour.black, x, y);
                            break;
                        case 'r':
                            addPiece(pieceType.rook, pieceColour.black, x, y);
                            break;
                        case 'n':
                            addPiece(pieceType.knight, pieceColour.black, x, y);
                            break;
                        case 'b':
                            addPiece(pieceType.bishop, pieceColour.black, x, y);
                            break;
                        case 'q':
                            addPiece(pieceType.queen, pieceColour.black, x, y);
                            break;
                        case 'k':
                            addPiece(pieceType.king, pieceColour.black, x, y);
                            break;
                        case 'P':
                            addPiece(pieceType.pawn, pieceColour.white, x, y);
                            break;
                        case 'R':
                            addPiece(pieceType.rook, pieceColour.white, x, y);
                            break;
                        case 'N':
                            addPiece(pieceType.knight, pieceColour.white, x, y);
                            break;
                        case 'B':
                            addPiece(pieceType.bishop, pieceColour.white, x, y);
                            break;
                        case 'Q':
                            addPiece(pieceType.queen, pieceColour.white, x, y);
                            break;
                        case 'K':
                            addPiece(pieceType.king, pieceColour.white, x, y);
                            break;
                        case ' ':
                            break;
                        default:
                            throw new Exception();
                    }
                }
            }

            // Pull in castling rights
            this[7, 7].excludeFromCastling = !fen.blackCanCastleKingside;
            this[0, 7].excludeFromCastling = !fen.blackCanCastleQueenside;
            this[7, 0].excludeFromCastling = !fen.whiteCanCastleKingside;
            this[0, 0].excludeFromCastling = !fen.whiteCanCastleQueenside;

            this.colToMove = fen.toPlay;
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
                throw new ArgumentException();
        }

        protected virtual gameStatus getGameStatusForNormal(List<square> myPieces, List<square> enemyPieces)
        {
            // If either side has no pieces, the game is undefined.
            if (myPieces.Count == 0 || enemyPieces.Count == 0)
                throw new Exception("Chess rules violated - a side has no pieces");

            // Now we are certain we have pieces, we can identify our side.
            pieceColour myCol = myPieces[0].colour;

            // Todo: optimise lots.
            bool nonCheckMoves = false;
            sizableArray<move> moves = getMoves(colToMove);
            foreach (move thisMove in moves)
            {
                doMove(thisMove);

                if (!playerIsInCheck(colToMove))
                {
                    nonCheckMoves = true;
                    undoMove(thisMove);
                    break;
                }

                undoMove(thisMove);
            }

            if (!nonCheckMoves)
            {
                // urghhhh. If  the player to move is in check, then the game is finished if they have no moves
                // which get out of check.
                if (playerIsInCheck(colToMove))
                    return (colToMove == myCol) ? gameStatus.lost : gameStatus.won;

                // If the current player cannot move without being in check, then the game is a draw.
                return gameStatus.drawn;
            }

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
            bool movesFound;
            if (colToMove == myCol)
                movesFound = myPieces.Any(myPiece => myPiece.getPossibleMoves(this).Length != 0);
            else
                movesFound = enemyPieces.Any(myPiece => myPiece.getPossibleMoves(this).Length != 0);

            if (!movesFound)
                return gameStatus.drawn;

            // The game is won if a pawn has made it to the other side of the board.
            List<square> allPieces = new List<square>(whitePieceSquares.Count + blackPieceSquares.Count);
            allPieces.AddRange(getPiecesForColour(pieceColour.white));
            allPieces.AddRange(getPiecesForColour(pieceColour.black));
            List<square> pawns = allPieces.FindAll(a => a.type == pieceType.pawn);

            foreach (square piece in pawns)
            {
                if ((piece.position.y == sizeY - 1 && piece.colour == pieceColour.white) ||
                    (piece.position.y == 0 && piece.colour == pieceColour.black))
                {
                    // We have won the game if it is ours, or lost if not.
                    if (myCol == piece.colour)
                        return gameStatus.won;
                    else
                        return gameStatus.lost;
                }
            }

            return gameStatus.inProgress;
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

        public bool playerIsInCheck(pieceColour playerPossiblyInCheck)
        {
            // Check does not exist in queen-and-pawns.
            if (_type == gameType.queenAndPawns)
                return false;

            sizableArray<move> moves = getMoves(getOtherSide(playerPossiblyInCheck));

            return moves.Exists(a => a.isCapture && a.capturedSquare.type == pieceType.king);
        }

        public bool wouldMovePutPlayerInCheck(pieceColour playerCol, move playersMove)
        {
            doMove(playersMove);
            bool toRet = playerIsInCheck(playerCol);
            undoMove(playersMove);

            return toRet;
        }

        /// <summary>
        /// Add a piece to the board
        /// </summary>
        /// <param name="newType">Type of piece to add</param>
        /// <param name="newColour">Colour of piece to add</param>
        /// <param name="dstPos">Location of piece to add</param>
        /// <returns></returns>
        protected virtual square addPiece(pieceType newType, pieceColour newColour, squarePos dstPos)
        {
            this[dstPos] = square.makeSquare(newType, newColour, dstPos);

            return this[dstPos];
        }

        /// <summary>
        /// Add a piece to the board
        /// </summary>
        /// <param name="newType">Type of piece to add</param>
        /// <param name="newColour">Colour of piece to add</param>
        /// <param name="x">X location of piece to add</param>
        /// <param name="y">Y location of piece to add</param>
        /// <returns></returns>
        public virtual square addPiece(pieceType newType, pieceColour newColour, int x, int y)
        {
            square newSquare = addPiece(newType, newColour, new squarePos(x, y));

            addToArrays(newSquare);

            return newSquare;
        }

        /// <summary>
        /// Add a piece to this board
        /// </summary>
        /// <param name="newSquare">square containing piece to add</param>
        /// <param name="newSquarePos">location to add the specified square at</param>
        protected virtual void addPiece(square newSquare, squarePos newSquarePos)
        {
            newSquare.position = newSquarePos;
            this[newSquarePos] = newSquare;

            addToArrays(newSquare);
        }

        /// <summary>
        /// Add the given piece to the whitePieceSquares or blackPieceSquares array.
        /// </summary>
        /// <param name="newSquare"></param>
        private void addToArrays(square newSquare)
        {
#if DEBUG
            if (whitePieceSquares.Contains(newSquare) ||
                blackPieceSquares.Contains(newSquare))
                throw new Exception("Duplicate square");
#endif

            switch (newSquare.colour)
            {
                case pieceColour.white:
                    whitePieceSquares.Add(newSquare);
                    break;
                case pieceColour.black:
                    blackPieceSquares.Add(newSquare);
                    break;
                default:
                    throw new ArgumentException();
            }            
        }

        /// <summary>
        /// Remove the specified piece from the board 
        /// </summary>
        /// <param name="toRemove"></param>
        protected virtual void removePiece(square toRemove)
        {
            this[toRemove.position] = new square(toRemove.position);

            switch (toRemove.colour)
            {
                case pieceColour.white:
                    whitePieceSquares.Remove(toRemove);
                    break;
                case pieceColour.black:
                    blackPieceSquares.Remove(toRemove);
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public virtual void doMove(move move)
        {
            sanityCheck();

#if DEBUG
            if (this[move.srcPos].colour != colToMove)
                throw new ArgumentException("Moving peice of wrong colour");
#endif

            // Update movedness count for the moving piece
            this[move.srcPos].moveNumbers.Push(moveCount);
            this[move.srcPos].movedCount++;

            square movingSquare = this[move.srcPos];

            // If this move is a castling, update the rooks movedCount
            if (move.isACastling)
            {
                this[move.castlingRookSrcPos].movedCount++;
                this[move.castlingRookSrcPos].moveNumbers.Push(moveCount);
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
            if (move.isACastling)
            {
                square rook = move.findCastlingRook(this);
                movePiece(rook, move.findNewPosForCastlingRook());
            }

            square captured;
            if (move.isCapture)
            {
                captured = move.capturedSquare;

                if (!captured.position.isSameSquareAs(move.dstPos))
                {
                    // The capture was not in to our piece's destination square. Set the captured 
                    // square to be empty.
                    removePiece(captured);
                }
            }

            if (move.isPawnPromotion)
            {
                removePiece(movingSquare);
                this[move.dstPos] = addPiece(move.typeToPromoteTo, movingSquare.colour, move.dstPos);
                this[move.dstPos].pastLife = movingSquare;
            }

            colToMove = getOtherSide(colToMove);

            sanityCheck();
        }

        /// <summary>
        /// Move a piece on the board, without performing the move itself (eg castling, or a capture)
        /// </summary>
        /// <param name="toMove">The piece to move</param>
        /// <param name="destPos">The location to move to</param>
        protected virtual void movePiece(square toMove, squarePos destPos)
        {
            squarePos srcPos = toMove.position;

            this[destPos] = toMove;
            this[destPos].position = destPos;
            this[srcPos] = new square(srcPos);
        }

        /// <summary>
        /// Move a piece on the board back to its original position, without reverting the entire
        /// move.
        /// </summary>
        /// <param name="srcPos">Square to move from</param>
        /// <param name="dstPos">Square to move to</param>
        protected virtual void unmovePiece(squarePos srcPos, squarePos dstPos)
        {
            // store the piece which is moving before we erase it
            square movingPiece = this[dstPos];

            // Erase the old destination
            this[dstPos] = new square(dstPos);

            // Move our piece back to its source square
            this[srcPos] = movingPiece;
            this[srcPos].position = srcPos;
        }

        public virtual void undoMove(move move)
        {
            sanityCheck();

#if DEBUG
            if (this[move.dstPos].colour == colToMove)
                throw new ArgumentException("Unmoving peice of wrong colour");
#endif

            // Revert any promotion
            if (move.isPawnPromotion)
            {
                square promoted = this[move.dstPos];
                removePiece(promoted);
                this[move.dstPos] = promoted.pastLife;
                addPiece(promoted.pastLife, move.dstPos);
            }

            // dec the move counters
            moveCount--;
            this[move.dstPos].movedCount--;
            if (this[move.dstPos].moveNumbers.Count == 0)
                this[move.dstPos].moveNumbers = this[move.dstPos].moveNumbers;
            Debug.Assert(this[move.dstPos].moveNumbers.Pop() == moveCount);

            // Dec the rook's move counter if this is a castling
            square castlingRook = null;
            if (move.isACastling)
            {
                castlingRook = this[move.castlingRookDstPos];
                castlingRook.movedCount--;
                Assert.AreEqual(moveCount, castlingRook.moveNumbers.Pop());
            }

            unmovePiece(move.srcPos, move.dstPos);

            // If a castling, move the rook back too.
            if (move.isACastling)
            {
                removePiece(castlingRook);

                // ReSharper disable PossibleNullReferenceException
                // castlingRook cannot be null at this point, as we set it if there is a castling.
                this[castlingRook.position] = new square(castlingRook.position);
                // ReSharper restore PossibleNullReferenceException
                if (castlingRook.position.x == 3)
                {
                    addPiece(castlingRook, new squarePos(0, castlingRook.position.y));
                }
                else if (castlingRook.position.x == 5)
                {
                    addPiece(castlingRook, new squarePos(7, castlingRook.position.y));
                }
                else
                    Assert.Fail("While uncastling, could not find castled rook");
            }

            // Restore any captured piece
            if (move.isCapture)
                addPiece(move.capturedSquare, move.capturedSquarePos);

            colToMove = getOtherSide(colToMove);

            sanityCheck();
        }

        public sizableArray<move> getMoves(pieceColour toMoveColour)
        {
            List<square> occupiedSquares = getPiecesForColour(toMoveColour);

            // Generously guess the size of this array
            sizableArray<move> possibleMoves = new sizableArray<move>(occupiedSquares.Count * 50);

            // Add all moves from all pieces
            foreach (square occupiedSquare in occupiedSquares)
                possibleMoves.AddRange(occupiedSquare.getPossibleMoves(this));

            return possibleMoves;
        }

        /// <summary>
        /// Check the board for consistancy. Called at various points during piece moving.
        /// </summary>
        protected virtual void sanityCheck()
        {
            
        }

        /// <summary>
        /// Is the specified square 'covered' by the specified colour? Note that this is just
        /// a hint, and may be incorrect under certain circumstances.
        /// </summary>
        /// <param name="squareToCheck"></param>
        /// <param name="sideToExamine"></param>
        /// <returns></returns>
        public virtual bool isThreatenedHint(square squareToCheck, pieceColour sideToExamine)
        {
            // This base class does not provide an implementation.
            return false;
        }

        public abstract lineAndScore findBestMove();
    }
}