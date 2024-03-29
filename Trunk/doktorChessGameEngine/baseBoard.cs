﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;

namespace doktorChessGameEngine
{
    public class chessAIAttribute : Attribute
    {
        
    }

    [Serializable]
    public abstract class baseBoard : System.MarshalByRefObject
    {
        /// <summary>
        /// How many squares wide the board is
        /// </summary>
        public const int sizeX = 8;

        /// <summary>
        /// How many squares high the board is
        /// </summary>
        public const int sizeY = 8;

        /// <summary>
        /// How many moves have been played on this board
        /// </summary>
        public int moveCount;

        /// <summary>
        /// The top element is how many consecutive moves have elapsed without a pawn move or a capture. 
        /// </summary>
        public Stack<int> fiftyMoveCounter = new Stack<int>(300);

        /// <summary>
        /// A stack of every board position that has occured in this game
        /// </summary>
        private Stack<string> positionsSoFar = new Stack<string>(100);

        /// <summary>
        /// Ruleset in effect for this game
        /// </summary>
        protected gameType _type { get; private set; }

        /// <summary>
        /// How many moves have been played on this board
        /// </summary>
        public int getMoveCount
        {
            get { return moveCount; }
        }

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

        /// <summary>
        /// How many milliseconds may elapse before the game is declared lost?
        /// </summary>
        public int timeLeftMS = Int32.MaxValue;

        /// <summary>
        /// Create a new empty board using the supplied rule set.
        /// </summary>
        /// <param name="newType">gameType to make</param>
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

        /// <summary>
        /// Get/Set the square at the given position
        /// </summary>
        /// <param name="myPos">Position to examine</param>
        /// <returns></returns>
        public square this[squarePos myPos]
        {
            // ReSharper disable UnusedMember.Local
            get { return _squares[myPos.x, myPos.y]; }
            protected set { _squares[myPos.x, myPos.y] = value; }
            // ReSharper restore UnusedMember.Local
        }

        /// <summary>
        /// Get/Set the square at the given position
        /// </summary>
        /// <param name="flatPos">Flat position to examine ( ie, x+(y*sizeY))</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get/Set the square at the given position
        /// </summary>
        /// <param name="x">X position to examine</param>
        /// <param name="y">Y position to examine</param>
        /// <returns></returns>
        public square this[int x, int y]
        {
            // ReSharper disable UnusedMember.Local
            get { return _squares[x, y]; }
            private set { _squares[x, y] = value; }
            // ReSharper restore UnusedMember.Local
        }

        /// <summary>
        /// Create the initial board set up
        /// </summary>
        public void makeStartPosition()
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

            positionsSoFar.Clear();
            positionsSoFar.Push(this.ToString());
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

        /// <summary>
        /// Create a board with the FEN-specified layout
        /// </summary>
        /// <param name="FENString">FEN of board to create</param>
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
                            throw new ArgumentException();
                    }
                }
            }

            // Pull in castling rights
            this[7, 7].excludeFromCastling = !fen.blackCanCastleKingside;
            this[0, 7].excludeFromCastling = !fen.blackCanCastleQueenside;
            this[7, 0].excludeFromCastling = !fen.whiteCanCastleKingside;
            this[0, 0].excludeFromCastling = !fen.whiteCanCastleQueenside;

            positionsSoFar.Clear();
            positionsSoFar.Push(this.ToString()); 
            
            this.colToMove = fen.toPlay;
        }

        /// <summary>
        /// Is the game won/lost/drawn?
        /// </summary>
        /// <param name="toPlay">Side to play</param>
        /// <returns></returns>
        public gameStatus getGameStatus(pieceColour toPlay)
        {
            List<square> myPieces = getPiecesForColour(toPlay);
            List<square> enemyPieces = getPiecesForColour(getOtherSide(toPlay));

            return getGameStatus(myPieces, enemyPieces);
        }

        /// <summary>
        /// Is the game won/lost/drawn?
        /// </summary>
        /// <param name="myPieces">The pieces of the side to play</param>
        /// <param name="enemyPieces">The pieces of the side not to play</param>
        /// <returns></returns>
        public gameStatus getGameStatus(List<square> myPieces, List<square> enemyPieces)
        {
            if (_type == gameType.queenAndPawns)
                return getGameStatusForQueenAndPawns(myPieces, enemyPieces);
            else if (_type == gameType.normal)
                return getGameStatusForNormal(myPieces, enemyPieces);
            else
                throw new ArgumentException();
        }

        /// <summary>
        /// Is the 'normal rule' game won/lost/drawn?
        /// </summary>
        /// <param name="myPieces">The pieces of the side to play</param>
        /// <param name="enemyPieces">The pieces of the side not to play</param>
        /// <returns></returns>
        protected virtual gameStatus getGameStatusForNormal(List<square> myPieces, List<square> enemyPieces)
        {
#if DEBUG
            // If either side has no pieces, the game is undefined.
            if (myPieces.Count == 0 || enemyPieces.Count == 0)
                throw new chessRuleViolationException("Chess rules violated - a side has no pieces");
#endif
            // Now we are certain we have pieces, we can identify our side.
            pieceColour myCol = myPieces[0].colour;

            // If 50 moves have passed without pawn move or check, the game is a stalemate.
            // A 'move' here is a pair of white/back moves, so this is 100 half-moves
            if (moveCount > 0 && !(fiftyMoveCounter.Peek() < 100))
                return gameStatus.drawn;

            // If this position has been seen three times, then the game is a draw.
            // Note that this does not _exactly_ match the FIDE rules, since in order for a 
            // position to be considered 'the same' under FIDE, it must be the same side's 
            // move, and the range of possible moves must be the same (ie, if a pawn could be
            // captured via en passant and cannot later, that is not the same move).
            string thisPos = this.ToString();
            if (positionsSoFar != null && positionsSoFar.Contains(thisPos))
            {
                if (positionsSoFar.Count(x => x == thisPos) > 2)
                    return gameStatus.drawn;
            }

#if DEBUG
            // It is nonsensical to move out of check.
            if (isPlayerInCheck(getOtherSide(colToMove)))
                throw new chessRuleViolationException("Player has moved in to check");
#endif
            // Since it is likely that most pieces will have moves, we don't call getMoves, which
            // would create a move list involving each piece. Instead, we check each piece in turn
            // which avoids listing moves for many pieces.
            bool nonCheckMoves = false;
            pieceColour movingSide = colToMove;
            square[] occupiedSquares = getPiecesForColour(colToMove).ToArray();
            foreach (square occupiedSquare in occupiedSquares)
            {
                sizableArray<move> moves = occupiedSquare.getPossibleMoves(this);
                foreach (move thisMove in moves)
                {
                    doMove(thisMove);

                    if (!isPlayerInCheck(movingSide))
                    {
                        nonCheckMoves = true;
                        undoMove(thisMove);
                        break;
                    }

                    undoMove(thisMove);
                }
                if (nonCheckMoves)
                    break;
            }

            if (!nonCheckMoves)
            {
                // If  the player to move is in check, then the game is finished if they have no 
                // moves which get out of check.
                if (isPlayerInCheck(movingSide))
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

        /// <summary>
        /// Retrieve squares containing pieces of the specified colour
        /// </summary>
        /// <param name="toMoveColour">Colour of piecs to return</param>
        /// <returns></returns>
        public List<square> getPiecesForColour(pieceColour toMoveColour)
        {
            if (toMoveColour == pieceColour.white)
                return whitePieceSquares;
            else if (toMoveColour == pieceColour.black)
                return blackPieceSquares;
            else
                throw new ArgumentException();

        }

        /// <summary>
        /// Return the opponent of the supplied side
        /// </summary>
        /// <param name="ofThisSide">Side to return opponent of</param>
        /// <returns></returns>
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

        /// <summary>
        /// TRUE if the player specified is in check, FALSE otherwise.
        /// </summary>
        /// <param name="playerPossiblyInCheck">The colour to examine</param>
        /// <returns></returns>
        public bool isPlayerInCheck(pieceColour playerPossiblyInCheck)
        {
            // Check does not exist in queen-and-pawns.
            if (_type == gameType.queenAndPawns)
                return false;

            sizableArray<move> moves = getMoves(getOtherSide(playerPossiblyInCheck));

            return moves.Exists(a => a.isCapture && a.capturedSquare.type == pieceType.king);
        }

        /// <summary>
        /// Would the specified move put the player of the specified colour in to check?
        /// </summary>
        /// <param name="playerCol">Colour which may move in ot check</param>
        /// <param name="playersMove">Move to examine</param>
        /// <returns></returns>
        public bool wouldMovePutPlayerInCheck(pieceColour playerCol, move playersMove)
        {
            doMove(playersMove);
            bool toRet = isPlayerInCheck(playerCol);
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

            addToArrays(this[dstPos]);

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
                throw new Exception("Duplicate square at " + newSquare.position.ToString());
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
        /// Remove the specified piece from the board without updating board status arrays
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

        /// <summary>
        /// Perform the specified move
        /// </summary>
        /// <param name="move">The move to play</param>
        public virtual void doMove(move move)
        {
            sanityCheck();

#if DEBUG
            if (this[move.srcPos].type == pieceType.none)
                throw new ArgumentException("Moving empty space");
            if (this[move.srcPos].colour != colToMove)
                throw new ArgumentException("Moving peice of wrong colour");
#endif
            // If this is a pawn move or a capture, reset the fifty-move rule counter.
            if (this[move.srcPos].type == pieceType.pawn || move.isCapture)
            {
                fiftyMoveCounter.Push(0);
            }
            else
            {
                if (moveCount > 0)
                    fiftyMoveCounter.Push(fiftyMoveCounter.Peek() + 1);
                else
                    fiftyMoveCounter.Push(1);
            }

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

            // Store this new position on our stack
            if (positionsSoFar != null)
                positionsSoFar.Push(this.ToString());

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
            //sanityCheck();

#if DEBUG
            if (this[move.dstPos].colour == colToMove)
                throw new ArgumentException("Unmoving peice of wrong colour");
#endif
            // undo our fifty-move counter
            fiftyMoveCounter.Pop();

            // remove this position from our list of played positions
            if (positionsSoFar != null)
            {
                string oldPos = positionsSoFar.Pop();
#if DEBUG
                if(oldPos != this.ToString())
                    throw new Exception("positionsSoFar contains incorrect pos");
#endif
            }

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

            int popped = this[move.dstPos].moveNumbers.Pop();
#if DEBUG
            if (popped != moveCount)
                throw new Exception("moveNumbers contains wrong move count");
#endif

            // Dec the rook's move counter if this is a castling
            square castlingRook = null;
            if (move.isACastling)
            {
                castlingRook = this[move.castlingRookDstPos];
                castlingRook.movedCount--;
                int poppedRook = castlingRook.moveNumbers.Pop();
#if DEBUG
                if (moveCount != poppedRook)
                    throw new Exception("moveNumbers contains wrong move count after popping uncastling rook");
#endif
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
                {
                    throw new Exception("While uncastling, could not find castled rook");
                }
            }

            // Restore any captured piece
            if (move.isCapture)
                addPiece(move.capturedSquare, move.capturedSquarePos);

            colToMove = getOtherSide(colToMove);

            sanityCheck();
        }

        /// <summary>
        /// Return a collecetion of all moves one player may be able to make.
        /// Note that moves in to check will also be returned.
        /// </summary>
        /// <param name="toMoveColour">Side to examine</param>
        /// <returns></returns>
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

        /// <summary>
        /// Find the best move for the colour to plau, and return the expected best Line and it's score.
        /// </summary>
        /// <returns></returns>
        public abstract lineAndScore findBestMove();

        /// <summary>
        /// Create an ASCII representation of the board.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Disable the three-fold repetition rule.
        /// </summary>
        public void disableThreeFoldRule()
        {
            positionsSoFar = null;
        }

        // These methods are here so we can avoid exposing the speedySquareList to the remoting boundary.

        /// <summary>
        /// Get the type of the piece at the specified board location.
        /// </summary>
        /// <param name="x">X co-ordinate</param>
        /// <param name="y">Y co-ordinate</param>
        /// <returns></returns>
        public pieceType getPieceType(int x, int y)
        {
            return this[x, y].type;
        }

        /// <summary>
        /// Get the colour of the piece at the specified board location.
        /// </summary>
        /// <param name="x">X co-ordinate</param>
        /// <param name="y">Y co-ordinate</param>
        /// <returns></returns>
        public pieceColour getPieceColour(int x, int y)
        {
            return this[x, y].colour;
        }

        /// <summary>
        /// Get the string representation of the piece at the specified board location.
        /// </summary>
        /// <param name="x">X co-ordinate</param>
        /// <param name="y">Y co-ordinate</param>
        /// <returns></returns>
        public string getPieceString(int x, int y)
        {
            return this[x, y].ToString();
        }

        /// <summary>
        /// This object should be kept alive forever.
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}