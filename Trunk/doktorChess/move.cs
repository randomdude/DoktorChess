using System;
using System.Text;

namespace doktorChess
{
    public class move
    {
        public squarePos srcPos;
        public squarePos dstPos;
        public bool isCapture = false;
        public square capturedSquare;
        private pieceType _type;
        private readonly square _srcSquare;

        public move(square src, square dst)
        {
            srcPos = src.position;
            dstPos = dst.position;
            _type = src.type;
            _srcSquare = src;

            // If we are capturing, fill in relevant info.
            if (dst.type != pieceType.none)
            {
                isCapture = true;
                capturedSquare = dst;
            }

        }

        public override string ToString()
        {
            return ToString(moveStringStyle.coord);
        }

        public string ToString(moveStringStyle chessNotation)
        {
            switch (chessNotation)
            {
                case moveStringStyle.coord:
                    return string.Format("[{0},{1}] -> [{2},{3}]", srcPos.x, srcPos.y, dstPos.x, dstPos.y);
                case moveStringStyle.chessNotation:
                    return toChessNotation();
                default:
                    throw new ArgumentOutOfRangeException("chessNotation");
            }
        }

        private string toChessNotation()
        {
            StringBuilder toRet = new StringBuilder();

            // Append a letter indicating the piece moving
            if (_srcSquare.type != pieceType.pawn)
                toRet.Append(_srcSquare.ToString());
            // Now append a chess-style coordinate.
            toRet.Append(dstPos.ToString(moveStringStyle.chessNotation));

            return toRet.ToString();
        }
    }
}