using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web.Script.Serialization;

namespace doktorChess
{
    public class move
    {
        public readonly squarePos srcPos;
        public readonly squarePos dstPos;
        public readonly squarePos capturedSquarePos;
        public readonly bool isCapture = false;
        public readonly square capturedSquare;
        private pieceType _type;
        private readonly square _srcSquare;

        public static move fromJSON(string JSON, Board parentBoard)
        {
            minimalMove json = new JavaScriptSerializer().Deserialize<minimalMove>(JSON);

            // Don't forget that the board is inverted, as we show it to the user
            json.srcSquarePos.y = 7 - json.srcSquarePos.y;
            json.dstSquarePos.y = 7 - json.dstSquarePos.y;

            move toRet = new move( parentBoard[json.srcSquarePos], parentBoard[json.dstSquarePos] );

            return toRet;
        }

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
                capturedSquarePos = dst.position;
            }
        }

        public move(square src, square dst, square captured)
        {
            srcPos = src.position;
            dstPos = dst.position;
            _type = src.type;
            _srcSquare = src;

            Debug.Assert(dst.type == pieceType.none);
                
            if (captured.type != pieceType.none)
            {
                isCapture = true;
                capturedSquare = captured;
                capturedSquarePos = captured.position;
            }            
        }

        /// <summary>
        /// Check for the same start and end square co-ordinates
        /// </summary>
        /// <param name="toCompare"></param>
        /// <returns></returns>
        public bool isSameSquaresAs(move toCompare)
        {
            return (toCompare.srcPos.isSameSquareAs(srcPos) && toCompare.dstPos.isSameSquareAs(dstPos));
        }

        /// <summary>
        /// Is this move legal according to the rules of chess?
        /// </summary>
        /// <param name="ourBaord"></param>
        /// <returns></returns>
        public bool isLegal(Board ourBoard)
        {
            if (_srcSquare.type == pieceType.none)
                return false;

            List<move> possibleMovesWithMovingPiece = _srcSquare.getPossibleMoves(ourBoard);

            foreach (move possibleMove in possibleMovesWithMovingPiece)
            {
                if (possibleMove.srcPos.isSameSquareAs(srcPos) &&
                    possibleMove.dstPos.isSameSquareAs(dstPos)) 
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add any extra data to the move so it makes sense
        /// </summary>
        /// <param name="ourBoard"></param>
        /// <returns>null if move is illegal</returns>
        public move sanitize(Board ourBoard)
        {
            List<move> possibleMovesWithMovingPiece = _srcSquare.getPossibleMoves(ourBoard);

            foreach (move possibleMove in possibleMovesWithMovingPiece)
            {
                if (possibleMove.srcPos.isSameSquareAs(srcPos) &&
                    possibleMove.dstPos.isSameSquareAs(dstPos))
                {
                    return  possibleMove;
                }
            }
            return null;
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