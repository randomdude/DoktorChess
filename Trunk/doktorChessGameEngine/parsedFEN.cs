using System;
using System.Linq;
using doktorChessGameEngine;

namespace doktorChessGameEngine
{
    public class parsedFEN
    {
        public readonly char[,] boardRepresentation = new char[baseBoard.sizeX, baseBoard.sizeY];
        public pieceColour toPlay;
        public bool whiteCanCastleKingside ;
        public bool blackCanCastleKingside ;
        public bool whiteCanCastleQueenside ;
        public bool blackCanCastleQueenside ;

        public parsedFEN(string fenString)
        {
            string[] split = fenString.Split(' ');

            string piecePlacement = split[0];
            string activeColour = split[1];
            string castlingAbility = split[2];
            string enPassantSquare = split[3];
            string halfMovesSoFar = split[4];
            string fullMovesSoFar = split[5];

            parsePiecePlacement(piecePlacement);
            parseActiveColour(activeColour);
            parseCastling(castlingAbility);
        }

        private void parseCastling(string caslingAbility)
        {
            string trimmed = caslingAbility.Trim();

            if(trimmed.Contains('-'))
                return;

            if (trimmed.Contains('K'))
                whiteCanCastleKingside = true;
            if (trimmed.Contains('Q'))
                whiteCanCastleQueenside = true;
            if (trimmed.Contains('k'))
                blackCanCastleKingside = true;
            if (trimmed.Contains('q'))
                blackCanCastleQueenside = true;

        }

        private void parseActiveColour(string rawCol)
        {
            string trimmedCol = rawCol.ToUpper().Trim();

            if (trimmedCol == "W")
                toPlay = pieceColour.white;
            else if (trimmedCol == "B")
                toPlay = pieceColour.black;
            else
                throw new Exception();
        }

        void parsePiecePlacement(string piecePlacement)
        {
            // Extract rows..
            string[] piecePlacementInRows = piecePlacement.Split('/');

            // And extract individual pieces.
            for (int y = 0; y != baseBoard.sizeY; y++)
            {
                string thisRow = piecePlacementInRows[baseBoard.sizeY - 1 - y];

                int xCharPos = 0;
                for (int x = 0; x < baseBoard.sizeX; )
                {
                    char thisChar = thisRow[xCharPos];

                    // Check for any numbers, which indicate empty spaces
                    int xskip;
                    if( int.TryParse(thisChar.ToString(), out xskip) )
                    {
                        // Set this many empty spaces.
                        int lim = x + xskip;
                        for (; x < lim; x++)
                            boardRepresentation[x, y] = ' ';
                        xCharPos++;
                        continue;
                    }

                    // Not a number. It's piece.
                    boardRepresentation[x, y] = thisChar;
                    x++;
                    xCharPos++;
                }
            }
        }
    }
}