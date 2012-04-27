using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using doktorChess;
using doktorChessGameEngine;

namespace tournament
{
    public static class utils
    {
        public static TableCell makeCellAndEscapeContents(string text)
        {
            return makeCell(HttpUtility.HtmlEncode(text));
        }

        public static TableCell makeCell(string text)
        {
            TableCell optCell = new TableCell();
            optCell.Text = text;
            return optCell;
        }

        public static TableCell makeCellAndEscapeContents(string text, string cssStyle)
        {
            TableCell optCell = makeCellAndEscapeContents(text);
            optCell.CssClass = cssStyle;
            return optCell;
        }

        public static string makeMoveListAndEscapeContents(List<move> gameToExamine)
        {
            try
            {
                StringBuilder resp = new StringBuilder();

                int moveIndex = 0;
                resp.Append("Move list: <br/>");
                foreach (move thisMove in gameToExamine)
                {
                    if (moveIndex % 2 == 0)
                        resp.Append((moveIndex / 2) + 1 + ". ");
                    try
                    {
                        resp.Append(HttpUtility.HtmlEncode(thisMove.ToString(moveStringStyle.chessNotation)));
                    }
                    catch (Exception e)
                    {
                        resp.Append(e.Message + " while serialising");
                    }
                    if (moveIndex % 2 == 0)
                        resp.Append(" ");
                    else
                        resp.Append("<br/>");

                    moveIndex++;
                }
                resp.Append("<br/>");

                return resp.ToString();
            }
            catch (Exception)
            {
                return "[unable to generate]";
            }
        }

        static public Table makeTableAndEscapeContents(baseBoard board)
        {
            try
            {
                Table htmlTable = new Table();
                htmlTable.CellPadding = 0;
                htmlTable.CellSpacing = 0;

                for (int y = baseBoard.sizeY - 1; y > -1; y--)
                {
                    TableRow newRow = new TableRow();

                    for (int x = 0; x < baseBoard.sizeX; x++)
                    {
                        TableCell newCell = new TableCell();

                        // Fill with our chess piece image if there's a piece there
                        pieceType pieceType = board.getPieceType(x, y);
                        if (pieceType != pieceType.none)
                        {
                            Image pieceImage = new Image();
                            newCell.Controls.Add(pieceImage);
                            pieceImage.AlternateText = HttpUtility.HtmlEncode(board.getPieceString(x, y));
                            pieceImage.ImageUrl = getImageForPiece(pieceType, board.getPieceColour(x, y));
                            pieceImage.CssClass = "piece";
                        }

                        // Get our nice checkerboard pattern via CSS. Set all of our cells to
                        // have the boardSquare CSS, too, so that jQuery can funkerise them.
                        newCell.CssClass = "boardSquare";
                        int offx = (y % 2 == 1) ? x : x + 1; // offset by 1 on every other row
                        if ((offx % 2 == 0))
                            newCell.CssClass += " boardSquareOdd";
                        else
                            newCell.CssClass += " boardSquareEven";

                        newCell.Attributes["x"] = x.ToString();
                        newCell.Attributes["y"] = y.ToString();

                        newRow.Cells.Add(newCell);
                    }

                    htmlTable.Rows.Add(newRow);
                }

                return htmlTable;
            }
            catch (Exception)
            {
                return new Table();
            }
        }

        private static string getImageForPiece(pieceType type, pieceColour colour)
        {
            string url = @"images/";

            switch (type)
            {
                case pieceType.none:
                    url += "none";
                    break;
                case pieceType.queen:
                    url += "queen";
                    break;
                case pieceType.pawn:
                    url += "pawn";
                    break;
                case pieceType.bishop:
                    url += "bishop";
                    break;
                case pieceType.rook:
                    url += "rook";
                    break;
                case pieceType.king:
                    url += "king";
                    break;
                case pieceType.knight:
                    url += "knight";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            url += "-" + colour;

            url += ".png";

            return url;
        }
    }
}