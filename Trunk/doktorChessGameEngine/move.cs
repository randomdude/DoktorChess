using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace doktorChessGameEngine
{
    [Serializable]
    public class move
    {
        public readonly squarePos srcPos;
        public readonly squarePos dstPos;
        public readonly squarePos capturedSquarePos;
        public readonly bool isCapture = false;
        public bool isACastling = false;
        public readonly square capturedSquare;
        private readonly square _srcSquare;
        public squarePos castlingRookSrcPos;
        public squarePos castlingRookDstPos;
        public readonly bool isPawnPromotion;
        public readonly pieceType typeToPromoteTo;

        public static move fromJSON(string JSON, baseBoard parentBoard)
        {
            minimalMove json = new JavaScriptSerializer().Deserialize<minimalMove>(JSON);

            // Don't forget that the board is inverted, as we show it to the user
            json.srcSquarePos = new squarePos(json.srcSquarePos.x, 7 - json.srcSquarePos.y);
            json.dstSquarePos = new squarePos(json.dstSquarePos.x, 7 - json.dstSquarePos.y);

            move toRet = new move( parentBoard[json.srcSquarePos], parentBoard[json.dstSquarePos]);

            return toRet;
        }

        public move(square src, square dst)
        {
            srcPos = src.position;
            dstPos = dst.position;
            _srcSquare = src;

            // If we are capturing, fill in relevant info.
            if (dst.type != pieceType.none)
            {
                isCapture = true;
                capturedSquare = dst;
                capturedSquarePos = dst.position;
            }

            if (src.type == pieceType.king)
                findCastlingRookPositions();
        }

        public move(square src, square dst, square captured)
        {
            srcPos = src.position;
            dstPos = dst.position;
            _srcSquare = src;

            Debug.Assert(dst.type == pieceType.none);

            if (captured.type != pieceType.none)
            {
                isCapture = true;
                capturedSquare = captured;
                capturedSquarePos = captured.position;
            }

            if (src.type == pieceType.king)
                findCastlingRookPositions();
        }

        public move(square src, square dst, pieceType toPromoteTo)
        {
            srcPos = src.position;
            dstPos = dst.position;
            _srcSquare = src;

            if (src.type != pieceType.pawn)
                throw new Exception("Attempt to promote non-pawn");
            if (toPromoteTo == pieceType.pawn)
                throw new Exception("Attempt to promote a pawn to a pawn");
            if (toPromoteTo == pieceType.none)
                throw new Exception("Attempt to promote a pawn to an empty space");

            isPawnPromotion = true;
            typeToPromoteTo = toPromoteTo;

            if (dst.type != pieceType.none)
            {
                isCapture = true;
                capturedSquare = dst;
                capturedSquarePos = dst.position;
            }

            if (src.type == pieceType.king)
                findCastlingRookPositions();
        }

        /// <summary>
        /// Is this move legal according to the rules of chess?
        /// </summary>
        /// <param name="ourBoard">Board to examine</param>
        /// <returns></returns>
        public bool isLegal(baseBoard ourBoard)
        {
            if (_srcSquare.type == pieceType.none)
                return false;

            // Is the move possible according to the piece?
            sizableArray<move> possibleMovesWithMovingPiece = _srcSquare.getPossibleMoves(ourBoard);

            return possibleMovesWithMovingPiece.Any(
                possibleMove => possibleMove.srcPos.isSameSquareAs(srcPos) && 
                                possibleMove.dstPos.isSameSquareAs(dstPos)                      );
        }

        /// <summary>
        /// Add any extra data to the move so it makes sense
        /// </summary>
        /// <param name="ourBoard"></param>
        /// <returns>null if move is illegal</returns>
        public move sanitize(baseBoard ourBoard)
        {
            sizableArray<move> possibleMovesWithMovingPiece = _srcSquare.getPossibleMoves(ourBoard);
            IEnumerable<move> casted = possibleMovesWithMovingPiece.Cast<move>();

            return casted.FirstOrDefault(a => a.srcPos.isSameSquareAs(srcPos) && a.dstPos.isSameSquareAs(dstPos));
        }

        private void setCastling()
        {
            // Castlings are denoted by a king moving two spaces.
            if (_srcSquare.type == pieceType.king &&
                    (
                     srcPos.x - dstPos.x == 2 ||
                     dstPos.x - srcPos.x == 2
                    )
                )
                isACastling = true;
            else
                isACastling = false;
        }

        private void findCastlingRookPositions()
        {
            setCastling();

            if (!isACastling)
                return;

            if (dstPos.x > srcPos.x)
            {
                castlingRookSrcPos = new squarePos(7, dstPos.y) ;
                castlingRookDstPos = new squarePos(dstPos.x - 1, dstPos.y);
            }
            else if (srcPos.x > dstPos.x)
            {
                castlingRookSrcPos = new squarePos(0, dstPos.y);
                castlingRookDstPos = new squarePos(dstPos.x + 1, dstPos.y);
            }
            else
                throw new Exception("Malformed castling");
        }

        public rookSquare findCastlingRook(baseBoard theBoard)
        {
            if (!isACastling)
                throw new Exception("Asked to find castling rook of a move not a castle");

            if (dstPos.x > srcPos.x)
                return (rookSquare) theBoard[7, dstPos.y];
            else if (srcPos.x > dstPos.x)
                return (rookSquare) theBoard[0, dstPos.y];

            throw new Exception("Malformed castling");
        }

        public squarePos findNewPosForCastlingRook()
        {
            if (!isACastling)
                throw new Exception("Asked to find new pos of a castling rook of a move not a castle");

            // The rook moves one space past the king in the direction of travel.
            if (dstPos.x > srcPos.x)
                return dstPos.leftOne();
            else if (srcPos.x > dstPos.x)
                return dstPos.rightOne();

            throw new Exception("Malformed castling");
        }

        public override string ToString()
        {
            return ToString(moveStringStyle.coord);
        }

        public string ToString(moveStringStyle style)
        {
            switch (style)
            {
                case moveStringStyle.coord:
                    return string.Format("[{0},{1}] -> [{2},{3}]", srcPos.x, srcPos.y, dstPos.x, dstPos.y);
                case moveStringStyle.chessNotation:
                    return toChessNotation();
                default:
                    throw new ArgumentOutOfRangeException("style");
            }
        }

        private string toChessNotation()
        {
            StringBuilder toRet = new StringBuilder();

            if (isACastling)
            {
                if (dstPos.x > srcPos.x)
                    return "O-O";
                else if (srcPos.x > dstPos.x)
                    return "O-O-O";
                else
                    throw new Exception("Unrecognised castle");
            }

            if (!isCapture)
            {
                // A normal move, eg BE2

                // Append a letter indicating the piece moving
                if (_srcSquare.type != pieceType.pawn)
                    toRet.Append(_srcSquare.ToString().ToUpper());

                // Now append a chess-style coordinate.
                toRet.Append(dstPos.ToString(moveStringStyle.chessNotation).ToLower());
            }
            else
            {
                // A capture, eg BxR
                toRet.Append(_srcSquare.ToString().ToUpper());
                toRet.Append("x");
                toRet.Append(capturedSquare.ToString().ToUpper());
            }
            return toRet.ToString();
        }

    }
}