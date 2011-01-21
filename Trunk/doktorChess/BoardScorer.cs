using System;
using System.Collections.Generic;

namespace doktorChess
{
    public class BoardScorer
    {
        private int _myPieceCount;
        private int _enemyPieceCount;
        private int _myMaterialAdvantage;
        private int _myMaterialDisadvantage;
        private gameStatus _status;
        private Board parentBoard;
        private pieceColour viewpoint;

        // Don't use min and maxval, because they are used by searches. This keeps things clean.
        public const int lowest = int.MinValue + 1;
        public const int highest = int.MaxValue - 1;

        public BoardScorer(Board toScore, List<square> myPieces, List<square> enemyPieces)
        {
            parentBoard = toScore;
            viewpoint = myPieces[0].colour;
            commonConstructorStuff(myPieces, enemyPieces);
        }

        public BoardScorer(Board toScore, pieceColour newViewpoint)
        {
            viewpoint = newViewpoint;
            List<square> myPieces = toScore.getPiecesForColour(viewpoint);
            List<square> enemyPieces = toScore.getPiecesForColour(viewpoint == pieceColour.black ? pieceColour.white : pieceColour.black);

            parentBoard = toScore;
            commonConstructorStuff(myPieces, enemyPieces);

            setGameStatus(toScore.getGameStatus(myPieces, enemyPieces) );
        }

        private void commonConstructorStuff(List<square> myPieces, List<square> enemyPieces)
        {
            _myPieceCount = myPieces.Count;
            _enemyPieceCount = enemyPieces.Count;
            _myMaterialAdvantage = getMaterialAdvantage(myPieces);
            _myMaterialDisadvantage = getMaterialAdvantage(enemyPieces);

        }

        private int getMaterialAdvantage(List<square> pieces)
        {
            int toRet = 0;

            foreach (square thisSq in pieces)
            {
                toRet += getMaterialAdvantage(thisSq.type);
            }

            return toRet;
        }

        private static int getMaterialAdvantage(pieceType pieces)
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
                    return highest;
                default:
                    throw new ArgumentOutOfRangeException("Unrecognised piece");
            }
        }

        public int getScore()
        {
            // If we have won/lost/drawn, return a special value
            switch(_status)
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

            int castleAdvantage = 0;
            if (parentBoard != null)
            {
                if (parentBoard.whiteHasCastled && viewpoint == pieceColour.white) castleAdvantage += 50;
                if (parentBoard.blackHasCastled && viewpoint == pieceColour.black) castleAdvantage += 50;
            }

            return (_myMaterialAdvantage - _myMaterialDisadvantage) + castleAdvantage;
        }

        public void setGameStatus(gameStatus newGameStatus)
        {
            _status = newGameStatus;
        }
    }
}