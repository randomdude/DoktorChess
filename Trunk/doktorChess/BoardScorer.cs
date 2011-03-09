using System;
using System.Collections.Generic;
using System.Text;

namespace doktorChess
{
    public class BoardScorer
    {
        private readonly int _myMaterialAdvantage;
        private readonly int _myMaterialDisadvantage;
        private readonly gameStatus _status;
        private readonly Board parentBoard;
        private readonly pieceColour viewpoint;

        private int castleAdvantage = 0;
        private int danglingAdvantage = 0;
        private int danglingDisadvantage = 0;
        public readonly scoreModifiers modifiers;

        // Don't use min and maxval, because they are used by searches. This keeps things clean.
        public const int lowest = int.MinValue + 1000;
        public const int highest = int.MaxValue - 1000;
     
        public BoardScorer(Board toScore, pieceColour newViewpoint, scoreModifiers newModifiers)
        {
            modifiers = newModifiers;
            viewpoint = newViewpoint;
            List<square> myPieces = toScore.getPiecesForColour(viewpoint);
            List<square> enemyPieces = toScore.getPiecesForColour(viewpoint == pieceColour.black ? pieceColour.white : pieceColour.black);

            parentBoard = toScore;

            if (viewpoint == pieceColour.black)
            {
                _myMaterialAdvantage = toScore.blackMaterialAdvantage;
                _myMaterialDisadvantage = toScore.whiteMaterialAdvantage;
            } else if (viewpoint == pieceColour.white)
            {
                _myMaterialAdvantage = toScore.whiteMaterialAdvantage;
                _myMaterialDisadvantage = toScore.blackMaterialAdvantage;
            }

            _status = toScore.getGameStatus(myPieces, enemyPieces);
        }

        public static int getMaterialAdvantage(square square)
        {
            return getMaterialAdvantage(square.type);
        }

        public static int getMaterialAdvantage(pieceType pieces)
        {
            switch (pieces)
            {
                case pieceType.none:
                    return 0;
                case pieceType.pawn:
                    return 1;
                case pieceType.bishop:
                    return 3;
                case pieceType.knight:
                    return 3;
                case pieceType.rook:
                    return 5;
                case pieceType.queen:
                    return 8;
                case pieceType.king:
                    // Do not return int.Maxval, in order to avoid overflow. Situations with no king
                    // are already handled as wins or losses.
                    return 0;   
                default:
                    throw new Exception("Unrecognised piece");
            }
        }

        public int getScore()
        {
            // If we have won/lost/drawn, return a special value
            switch (_status)
            {
                case gameStatus.inProgress:
                    break;
                case gameStatus.won:
                    return highest;
                case gameStatus.drawn:
                    return 0;
                case gameStatus.lost:
                    return lowest;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (parentBoard.whiteHasCastled && viewpoint == pieceColour.white) castleAdvantage += 3;
            if (parentBoard.blackHasCastled && viewpoint == pieceColour.black) castleAdvantage += 3;

            // Find any dangling pieces, and give them a modifier
            List<square> myPieces = parentBoard.getPiecesForColour(viewpoint);

            foreach (square myPiece in myPieces)
            {
                if (parentBoard.getCoverLevel(myPiece, viewpoint) < 0)
                    danglingDisadvantage += (getMaterialAdvantage(myPiece.type));
            }

            List<square> enemyPieces = parentBoard.getPiecesForColour(Board.getOtherSide(viewpoint));
            foreach (square enemyPiece in enemyPieces)
            {
                if (parentBoard.getCoverLevel(enemyPiece, Board.getOtherSide(viewpoint)) < 0)
                    danglingAdvantage += (getMaterialAdvantage(enemyPiece.type));
            }

            return
                ((_myMaterialAdvantage - _myMaterialDisadvantage) * modifiers.materialModifier) 
                + castleAdvantage
                + ((danglingAdvantage - danglingDisadvantage) * modifiers.danglingModifier);
        }

        public override string ToString()
        {
            StringBuilder toRet = new StringBuilder();

            toRet.AppendLine("final score: " + getScore());
            toRet.AppendLine("material advantage: " + _myMaterialAdvantage);
            toRet.AppendLine("material disadvantage: " + _myMaterialDisadvantage);
            toRet.AppendLine("dangling advantage: " + danglingAdvantage);
            toRet.AppendLine("dangling disadvantage: " + danglingDisadvantage);

            return toRet.ToString();
        }

    }

    public class scoreModifiers
    {
        public int danglingModifier = 2;
        public int materialModifier = 10;
    }
}