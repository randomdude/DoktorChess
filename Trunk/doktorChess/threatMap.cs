using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace doktorChess
{
    public class threatMap : IEnableableThreatMap
    {
        private int[,] threats = new int[Board.sizeX,Board.sizeY];
        public List<square>[,] piecesWhichThreatenSquare = new List<square>[Board.sizeX, Board.sizeY];
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
                    piecesWhichThreatenSquare[x, y] = new List<square>(20);

                }
            }
        }

        public void add(int x, int y)
        {
            //Debug.WriteLine(_parentBoard.ToString());

            // This is slightly complicated by the fact that any new piece could block other pieces
            // from threatening squares. Because of this, we keep per-piece 'threat lists' containing
            // the a list of squares threatened by the given piece, and if a piece is placed in to a
            // square threatened by another piece, we then re-evaluate that piece.

            if (_parentBoard[x, y].type == pieceType.knight)
                x = x;

            // Find squares which threaten the square we are placing in to, and stash them in another
            // list
            List<square> piecesToRecalc = new List<square>(piecesWhichThreatenSquare[x, y].Count);
            piecesToRecalc.AddRange( piecesWhichThreatenSquare[x, y] );

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
                piecesWhichThreatenSquare[potentialMove.dstPos.x, potentialMove.dstPos.y].Add(_parentBoard[x, y]);

                // and our list of squares covered by piece
                _parentBoard[x, y].coveredSquares.Add(potentialMove.dstPos);
            }

            // and then recalculate pieces that need it.
            foreach (square toRecalc in piecesToRecalc)
            {
                List<move> recalcMoves = toRecalc.getCoveredSquares(_parentBoard);

                // Remove existing covered squares 
                foreach (squarePos coveredSquare in toRecalc.coveredSquares)
                {
                    this[coveredSquare] -= mapAddition;
                    piecesWhichThreatenSquare[toRecalc.position.x, toRecalc.position.y].Remove(_parentBoard[ coveredSquare ]);
                }
                toRecalc.coveredSquares.Clear();

                // then add new ones.
                foreach (move potentialRecalcMoves in recalcMoves)
                {
                    this[potentialRecalcMoves.dstPos] += mapAddition;
                    piecesWhichThreatenSquare[toRecalc.position.x, toRecalc.position.y].Remove(_parentBoard[potentialRecalcMoves.dstPos]);
                    //piecesWhichThreatenSquare[potentialRecalcMoves.dstPos.x, potentialRecalcMoves.dstPos.y].Add(toRecalc);
                    toRecalc.coveredSquares.Add(potentialRecalcMoves.dstPos);
                }
            }

            //Debug.WriteLine(this.ToString());
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
                piecesWhichThreatenSquare[toRemove.position.x, toRemove.position.y].Remove( _parentBoard[threatenedSquare]);
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
            return piecesWhichThreatenSquare[squareToCheck.position.x, squareToCheck.position.y]
                .Find(x => x.colour == otherSide) != null;
        }
    }
}