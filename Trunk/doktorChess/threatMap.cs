using System.Collections.Generic;
using System.Linq;
using System.Text;
using doktorChessGameEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace doktorChess
{
    public class threatMap : IEnableableThreatMap
    {
        /// <summary>
        /// The actual threat map, from whites POV
        /// </summary>
        private readonly int[,] threats = new int[Board.sizeX,Board.sizeY];

        /// <summary>
        /// 64 lists of squarePos, each holding a list of squares which cover it
        /// </summary>
        public readonly speedySquareList[,] piecesWhichThreatenSquare = new speedySquareList[Board.sizeX, Board.sizeY];
        private readonly Board _parentBoard;

        /// <summary>
        /// Check internal consistency frequently if set
        /// </summary>
        public bool checkStuff { private get; set; }

        public int this[squarePos position]
        {
            get { return threats[position.x, position.y]; }
            set { threats[position.x, position.y] = value; }
        }

        public threatMap(Board newBoard)
        {
            _parentBoard = newBoard;

            for (int y = 0; y < Board.sizeY; y++)
            {
                for (int x = 0; x < Board.sizeX; x++)
                {
                    // Spawn lists of pieces which threaten each square
                    piecesWhichThreatenSquare[x, y] = new speedySquareList();
                }
            }
        }

        public void add(int x, int y)
        {
            // This is slightly complicated by the fact that any new piece could block other pieces
            // from threatening squares. Because of this, we keep per-piece 'threat lists' containing
            // the a list of squares threatened by the given piece, and if a piece is placed in to a
            // square threatened by another piece, we then re-evaluate the threatening pieces.

            // Find squares which threaten the square we are placing in to, and stash them in another
            // list
            sizableArray<int> piecesToRecalc = new sizableArray<int>(piecesWhichThreatenSquare[x, y].Count);
            piecesToRecalc.AddRange( piecesWhichThreatenSquare[x, y] );

            // Now, add the new pieces threatened squares
            sizableArray<square> potentialDestSquares = _parentBoard[x, y].getCoveredSquares(_parentBoard);

            // Since our threat map is always stored from white's viewpoint, we should add or subtract
            // depending if threatened pieces are white or black.
            int mapAddition = _parentBoard[x, y].colour == pieceColour.white ? 1 : -1;

            // Now cycle through each move and apply them to our map, and to the piece.
            //_parentBoard[x, y].coveredSquares.Clear();
            foreach (square potentialDstSq in potentialDestSquares)
            {
                // Add the threatened squares to our threat map
                this[potentialDstSq.position] += mapAddition;

                speedySquareList piecesWhichThreatenThisSq = piecesWhichThreatenSquare[potentialDstSq.position.x, potentialDstSq.position.y];
                // update our list of pieces threatening each square
                piecesWhichThreatenThisSq[squarePos.flatten(x, y)] = true;

                // and our list of squares covered by piece
                _parentBoard[x, y].coveredSquares[potentialDstSq.position.flatten()] = true;
            }

            // and then recalculate pieces that need it. To save time, we don't re-evaluate everything-
            // we just remove extra squares based on the piece.
            foreach (int toRecalcPos in piecesToRecalc)
            {
                square toRecalc = _parentBoard[toRecalcPos];

                int toRecalcAddition = toRecalc.colour == pieceColour.white ? 1 : -1;

                // Knights can never be blocked from accessing squares.
                if (toRecalc.type == pieceType.knight)
                    continue;

                 //Pawns can always access their two attack squares.
                if (toRecalc.type == pieceType.pawn)
                    continue;

                // Remove covered squares which are on the other 'side' of us - for example, if a rook
                // is to our left, remove squares to our right from it.

                int offX = toRecalc.position.x - x;
                int offY = toRecalc.position.y - y;

                int sx;
                if (offX < 0)
                    sx = +1;
                else if (offX > 0)
                    sx = -1;
                else
                    sx = 0;

                int sy;
                if (offY < 0)
                    sy = +1;
                else if (offY > 0)
                    sy = -1;
                else
                    sy = 0;

                // Look right up to the edge for all pieces apart from the king, which can look only
                // one square in each direction.
                int limitx = Board.sizeX;
                int limity = Board.sizeY;
                if (toRecalc.type == pieceType.king)
                {
                    limitx = toRecalc.position.x + (sx * 2);
                    limity = toRecalc.position.x + (sy * 2);

                    if (limitx > Board.sizeX)
                        limitx = Board.sizeX;
                    if (limity > Board.sizeY)
                        limity = Board.sizeY;
                }

                //Debug.WriteLine( toRecalc.type + toRecalc.position + ":");
                int removex = x + sx;
                int removey = y + sy;
                while(removex >= 0 && removex < limitx &&
                    removey >= 0 && removey < limity )
                {
                    squarePos toRemoveSqPos = new squarePos(removex, removey);

                    this[toRemoveSqPos] -= toRecalcAddition;

                    speedySquareList piecesWhichThreatenThisSq = piecesWhichThreatenSquare[removex, removey];

                    piecesWhichThreatenThisSq[toRecalc.position] = false;
                    toRecalc.coveredSquares[ toRemoveSqPos ] = false;

                    //Debug.WriteLine("Removed now-blocked " + pos);

                    // we can threaten one piece, but no farther.
                    if (_parentBoard[removex, removey].type != pieceType.none)
                        break;

                    removex += sx;
                    removey += sy;

                }
            }
            sanityCheck();
        }

        public void add(squarePos pos)
        {
            add(pos.x, pos.y);
        }

        public void remove(square toRemove)
        {
            int posDirection = toRemove.colour == pieceColour.white ? 1 : -1;

            // Remove the actual piece, and what it threatens.
            foreach (int threatenedSquareFlat in toRemove.coveredSquares )
            {
                squarePos threatenedSquare = squarePos.unflatten(threatenedSquareFlat);

                this[threatenedSquare] -= posDirection;

                // The removed piece no longer threatens this square.
                piecesWhichThreatenSquare[threatenedSquare.x, threatenedSquare.y][toRemove.position] = false;
            }
            toRemove.coveredSquares.Clear();
            
            // and now force a re-evaluation of things that threatened this square.
            sizableArray<int> piecesToRecalc = new sizableArray<int>(piecesWhichThreatenSquare[toRemove.position.x, toRemove.position.y].Count);
            piecesToRecalc.AddRange(piecesWhichThreatenSquare[toRemove.position.x, toRemove.position.y]);

            foreach (int toRecalcFlat in piecesToRecalc)
            {
                square toRecalc = _parentBoard[squarePos.unflatten(toRecalcFlat)];

                // Knights can never be blocked from accessing squares.
                if (toRecalc.type == pieceType.knight)
                    continue;

                //Pawns can always access their two attack squares.
                if (toRecalc.type == pieceType.pawn)
                    continue;

                int toRecalcAddition = toRecalc.colour == pieceColour.white ? 1 : -1;

                // Remove covered squares which are on the other 'side' of us - for example, if a rook
                // is to our left, remove squares to our right from it.
                int offX = toRecalc.position.x - toRemove.position.x;
                int offY = toRecalc.position.y - toRemove.position.y;

                int sx;
                if (offX < 0)
                    sx = +1;
                else if (offX > 0)
                    sx = -1;
                else
                    sx = 0;

                int sy;
                if (offY < 0)
                    sy = +1;
                else if (offY > 0)
                    sy = -1;
                else
                    sy = 0;

                // Look right up to the edge for all pieces apart from the king, which can look only
                // one square in each direction.
                int limitx = Board.sizeX;
                int limity = Board.sizeY;
                if (toRecalc.type == pieceType.king)
                {
                    limitx = toRecalc.position.x + (2 * sx);
                    limity = toRecalc.position.x + (2 * sx);

                    if (limitx > Board.sizeX)
                        limitx = Board.sizeX;
                    if (limity > Board.sizeY)
                        limity = Board.sizeY;
                }

                //Debug.WriteLine(toRecalc.type + " @ " + toRecalc.position + ":");
                int removex = toRemove.position.x + sx;
                int removey = toRemove.position.y + sy;
                while(removex >= 0 && removex < limitx &&
                      removey >= 0 && removey < limity   )
                {
                    squarePos pos = new squarePos(removex, removey);

                    this[pos] += toRecalcAddition;
                    piecesWhichThreatenSquare[removex, removey][toRecalc.position] = true;
                    toRecalc.coveredSquares[pos.flatten()] = true;

                    //Debug.WriteLine("Added discovered " + pos);

                    // we can threaten one piece, but no farther.
                    if (_parentBoard[removex, removey].type != pieceType.none)
                        break;

                    removex += sx;
                    removey += sy;

                }
            }
            sanityCheck();
        }

        public override string ToString()
        {
            StringBuilder toRet = new StringBuilder(Board.sizeY * (Board.sizeY * 2));

            for (int y = Board.sizeY - 1; y > -1; y--)
            {
                for (int x = 0; x < Board.sizeX; x++)
                    toRet.Append(threats[x, y].ToString().PadLeft(3, ' '));
                toRet.AppendLine();
            }

            return toRet.ToString();
        }

        public bool isThreatened(square squareToCheck, pieceColour sideToExamine)
        {
            // Return true if the square is covered by at least one enemy piece. Ignore if we are 
            // covering it or not.
            pieceColour otherSide = Board.getOtherSide(sideToExamine);
            return piecesWhichThreatenSquare[squareToCheck.position.x, squareToCheck.position.y].Any(sp => _parentBoard[squarePos.unflatten(sp)].colour == otherSide);
        }

        private void sanityCheck()
        {
            if (!checkStuff)
                return;

            for (int y = Board.sizeY - 1; y > -1; y--)
            {
                for (int x = 0; x < Board.sizeX; x++)
                {
                    int calculated = 0;
                    foreach (int flatSqPos in piecesWhichThreatenSquare[x, y])
                    {
                        square sq = _parentBoard[squarePos.unflatten(flatSqPos)];
                        if (sq.colour == pieceColour.white)
                            calculated++;
                        else if (sq.colour == pieceColour.black)
                            calculated--;
                        else
                            throw new AssertFailedException("square threat has strange colour");
                    }
                    if (calculated != threats[x,y])
                        throw new AssertFailedException("Threat map is wrong");
                }
            }

        }

    }
}