using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace doktorChess
{
    public class threatMap : IEnableableThreatMap
    {
        private int[,] threats = new int[Board.sizeX,Board.sizeY];
        public Dictionary<int, square>[,] piecesWhichThreatenSquare = new Dictionary<int, square>[Board.sizeX, Board.sizeY];
        private Board _parentBoard;

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
                    piecesWhichThreatenSquare[x, y] = new Dictionary<int, square>();
                }
            }
        }

        public void add(int x, int y)
        {
            // This is slightly complicated by the fact that any new piece could block other pieces
            // from threatening squares. Because of this, we keep per-piece 'threat lists' containing
            // the a list of squares threatened by the given piece, and if a piece is placed in to a
            // square threatened by another piece, we then re-evaluate that piece.

            // Find squares which threaten the square we are placing in to, and stash them in another
            // list
            List<square> piecesToRecalc = new List<square>(piecesWhichThreatenSquare[x, y].Count);
            piecesToRecalc.AddRange( piecesWhichThreatenSquare[x, y].Values );

            // Now, add the new pieces threatened squares
            List<move> potentialMoves = _parentBoard[x, y].getCoveredSquares(_parentBoard);

            // Since our threat map is always stored from white's viewpoint, we should add or subtract
            // depending if threatened pieces are white or black.
            int mapAddition = _parentBoard[x, y].colour == pieceColour.white ? 1 : -1;

            // Now cycle through each move and apply them to our map, and to the piece.
            _parentBoard[x, y].coveredSquares.Clear();
            foreach (move potentialMove in potentialMoves)
            {
                // Add the threatened squares to our threat map
                this[potentialMove.dstPos] += mapAddition;

                // update our list of pieces threatening each square
                //if (piecesWhichThreatenSquare[potentialMove.dstPos.x, potentialMove.dstPos.y].ContainsKey(x + (y * Board.sizeX)))
                //    throw new AssertFailedException("Duplicate threatened square " + potentialMove.dstPos.ToString());

                piecesWhichThreatenSquare[potentialMove.dstPos.x, potentialMove.dstPos.y].Add( x + (y * Board.sizeX), _parentBoard[x, y]);

                // and our list of squares covered by piece
                _parentBoard[x, y].coveredSquares.Add(potentialMove.dstPos);
            }

            // and then recalculate pieces that need it.
            foreach (square toRecalc in piecesToRecalc)
            {
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

                int sx = 0;
                if (offX < 0)
                    sx = +1;
                else if (offX > 0)
                    sx = -1;
                else
                    sx = 0;

                int sy = 0;
                if (offY < 0)
                    sy = +1;
                else if (offY > 0)
                    sy = -1;
                else
                    sy = 0;

                int removex = toRecalc.position.x + sx;
                int removey = toRecalc.position.y + sy;
                while(removex >= 0 && removex < Board.sizeX &&
                    removey >= 0 && removey < Board.sizeY   &&
                    _parentBoard[removex, removey].type == pieceType.none)
                {
                    squarePos pos = new squarePos(removex, removey);

                    this[pos] -= mapAddition;
                    piecesWhichThreatenSquare[removex, removey].Remove(toRecalc.position.x + (toRecalc.position.y * Board.sizeX));
                    toRecalc.coveredSquares.Remove(pos);


                    removex += sx;
                    removey += sy;

                }
            }
        }

        public void add(squarePos pos)
        {
            add(pos.x, pos.y);
        }

        public void remove(square toRemove)
        {
            int posDirection = toRemove.colour == pieceColour.white ? 1 : -1;

            foreach (squarePos threatenedSquare in toRemove.coveredSquares )
            {
                this[threatenedSquare] -= posDirection;

                // The removed piece no longer threatens this square.
                piecesWhichThreatenSquare[threatenedSquare.x, threatenedSquare.y].Remove(toRemove.position.flatten());
            }
            toRemove.coveredSquares.Clear();
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
            foreach (KeyValuePair<int, square> squareKVP in piecesWhichThreatenSquare[squareToCheck.position.x, squareToCheck.position.y])
            {
                if (squareKVP.Value.colour == otherSide)
                    return true;
            }
            return false;
        }
    }
}