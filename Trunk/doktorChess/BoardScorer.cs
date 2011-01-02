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

        public BoardScorer(List<square> myPieces, List<square> enemyPieces)
        {
            commonConstructorStuff(myPieces, enemyPieces);
        }

        public BoardScorer(Board toScore, pieceColour viewpoint)
        {
            List<square> myPieces = toScore.getPiecesForColour(viewpoint);
            List<square> enemyPieces = toScore.getPiecesForColour(viewpoint == pieceColour.black ? pieceColour.white : pieceColour.black);

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

        private int getMaterialAdvantage(pieceType pieces)
        {
            switch (pieces)
            {
                case pieceType.none:
                    return 0;
                case pieceType.queen:
                    return 8;
                case pieceType.pawn:
                    return 1;
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
                    return int.MaxValue;
                case gameStatus.drawn:
                    return 0;
                case gameStatus.lost:
                    return int.MinValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return _myMaterialAdvantage - _myMaterialDisadvantage;
        }

        public void setGameStatus(gameStatus newGameStatus)
        {
            _status = newGameStatus;
        }
    }
}