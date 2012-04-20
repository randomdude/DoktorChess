using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using doktorChessGameEngine;

namespace exampleMiniMax
{
    public class boardScorer
    {
        private int score;
        public boardScorer(exampleMiniMaxBoard board, pieceColour viewpoint)
        {
            int whiteMaterial = addUpMaterial(board.whitePieceSquares);
            int blackMaterial = addUpMaterial(board.blackPieceSquares);

            if (viewpoint == pieceColour.white)
                score = whiteMaterial - blackMaterial;
            else
                score = blackMaterial - whiteMaterial;
        }

        private int addUpMaterial(List<square> sq)
        {
            int toRet = 0;

            foreach (square thisSquare in sq)
            {
                switch (thisSquare.type)
                {
                    case pieceType.none:
                        break;
                    case pieceType.queen:
                        toRet += 8;
                        break;
                    case pieceType.pawn:
                        toRet += 1;
                        break;
                    case pieceType.bishop:
                        toRet += 3;
                        break;
                    case pieceType.rook:
                        toRet += 5;
                        break;
                    case pieceType.king:
                        break;
                    case pieceType.knight:
                        toRet += 3;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return toRet;
        }

        public int getScore()
        {
            return score;
        }

        public override string ToString()
        {
            return score.ToString();
        }
    }
}
