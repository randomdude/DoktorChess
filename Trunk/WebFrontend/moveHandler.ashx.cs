﻿using System;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using doktorChess;

namespace WebFrontend
{
    [WebService(Namespace = "http://gamesfairy.co.uk")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class moveHandler : IHttpHandler, IRequiresSessionState 
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        readonly JavaScriptSerializer ser = new JavaScriptSerializer();

        public void ProcessRequest(HttpContext context)
        {
            Board theBoard = (Board) context.Session["board"];
            pieceColour playerCol = (pieceColour) context.Session["playerCol"];
            pieceColour computerCol = Board.getOtherSide(playerCol);
            moveResponse resp = new moveResponse();

            // Find the move that the user has just input.
            string playersMoveString = context.Request["ourMove"];
            move playersMove = move.fromJSON(playersMoveString, theBoard);

            // if the players move is for the wrong side, or is illegal, return 
            // early, otherwise play it.
            if (!playersMove.isLegal(theBoard) || theBoard[playersMove.srcPos].colour != playerCol )
            {
                resp.isValid = false;
                resp.loadBoardTable(makeTable(theBoard));

                context.Response.Write(ser.Serialize(resp));
                return;
            }
            resp.isValid = true;
            theBoard.doMove(playersMove);
                
            // Now, find our best move, and play it
            move bestMove = theBoard.findBestMove(computerCol).line[0];
            theBoard.doMove(bestMove);

            // and send the move and new board to the client.
            resp.movedPieceSrc = bestMove.srcPos;
            resp.movedPieceDst = bestMove.dstPos;
            resp.loadBoardTable(makeTable(theBoard));

            context.Response.Write(ser.Serialize(resp));
        }

        static public Table makeTable(Board board)
        {
            Table htmlTable = new Table();
            htmlTable.CellPadding = 0;
            htmlTable.CellSpacing = 0;

            for (int y = Board.sizeY - 1; y > -1; y--)
            {
                TableRow newRow = new TableRow();

                for (int x = 0; x < Board.sizeX; x++)
                {
                    TableCell newCell = new TableCell();

                    // Fill with our chess piece image if there's a piece there
                    square piece = board[x, y];
                    if (piece.type != pieceType.none)
                    {
                        Image pieceImage = new Image();
                        newCell.Controls.Add(pieceImage);
                        pieceImage.AlternateText = piece.ToString();
                        pieceImage.ImageUrl = getImageForPiece(piece.type, piece.colour);
                        pieceImage.CssClass = "piece";
                    }

                    // Get our nice checkerboard pattern via CSS. Set all of our cells to
                    // have the boardSquare CSS, too, so that jQuery can funkerise them.
                    newCell.CssClass = "boardSquare";
                    int offx = (y%2 == 1) ? x : x + 1;  // offset by 1 on every other row
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
                    break;
                case pieceType.rook:
                    break;
                case pieceType.king:
                    break;
                case pieceType.knight:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            url += ".jpg";

            return url;
        }

        public static bool sessionValid(HttpContext context)
        {
            if (context.Session["board"] == null ||
                context.Session["playerCol"] == null)
                return false;

            return true;
        }
    }
}
