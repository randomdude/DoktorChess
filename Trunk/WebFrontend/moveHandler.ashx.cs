using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using doktorChess;
using doktorChessGameEngine;

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
            baseBoard theBoard = (baseBoard)context.Session["board"];
            pieceColour playerCol = (pieceColour) context.Session["playerCol"];
            pieceColour computerCol = baseBoard.getOtherSide(playerCol);
            moveResponse resp = new moveResponse();

            // Find the move that the user has just input.
            string playersMoveString = context.Request["ourMove"];
            move playersMove = move.fromJSON(playersMoveString, theBoard);

            // Some moves - like en passant - do not have enough information coming in from the 
            // frontend to represent properly, but have enough information to infer everything.
            // Add any data like that.
            playersMove = playersMove.sanitize(theBoard);

            // if the players move is for the wrong side, or is illegal, return 
            // early, otherwise play it.
            if (playersMove == null || 
                theBoard[playersMove.srcPos].colour != playerCol  ||
                theBoard.wouldMovePutPlayerInCheck(playerCol, playersMove) )
            {
                resp.isValid = false;
                resp.loadBoardTable(makeTable(theBoard));

                context.Response.Write(ser.Serialize(resp));
                return;
            }
            resp.isValid = true;
            theBoard.doMove(playersMove);

            // If the players move has resulted in an en passant capture, then we need to tell the JS to reload
            // the board, since it does not detect en passant.
            if (playersMove.isCapture &&
                !playersMove.capturedSquarePos.isSameSquareAs(playersMove.dstPos))
                resp.forceBoardReload = true;

            // If the player has castled, force a board reload too, since the JS does not detect this yet.
            if (playersMove.isACastling)
                resp.forceBoardReload = true;

            resp.moveNum = theBoard.moveCount.ToString();
            resp.whiteMove = playersMove.ToString(moveStringStyle.chessNotation);

            // Check that player has not finished the game
            gameStatus status = theBoard.getGameStatus(computerCol);
            if (status != gameStatus.inProgress)
            {
                resp.gameFinished = true;
                resp.gameResult = theBoard.getGameStatus(playerCol).ToString();
                resp.loadBoardTable(makeTable(theBoard));
                context.Response.Write(ser.Serialize(resp));
                return;
            }

            // Now, find our best move, and play it
            lineAndScore bestLine = theBoard.findBestMove();
            move bestMove = bestLine.line[0];
            theBoard.doMove(bestMove);
            resp.blackMove = bestMove.ToString(moveStringStyle.chessNotation);

            for (int index = 0; index < bestLine.line.Length; index++)
            {
                move lineMove = bestLine.line[index];
                resp.bestLine += lineMove.ToString(moveStringStyle.chessNotation) + " ";
                if (index % 2 != 0)
                    resp.bestLine += "\n";
            }
            resp.bestLine += "\nScore: " + bestLine._scorer.ToString();

            if (bestMove.isCapture &&
                !bestMove.capturedSquarePos.isSameSquareAs(bestMove.dstPos))
                resp.forceBoardReload = true;
            if (bestMove.isACastling)
                resp.forceBoardReload = true;

            // and send the move and new board to the client.
            resp.initFromMove(bestMove);
            resp.loadBoardTable(makeTable(theBoard));

            // If we have finished the game, let the UI know
            gameStatus newstatus = theBoard.getGameStatus(playerCol);
            if (newstatus != gameStatus.inProgress)
            {
                resp.gameFinished = true;
                resp.gameResult = newstatus.ToString();
            }

            context.Response.Write(ser.Serialize(resp));
        }

        static public Table makeTable(baseBoard board)
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

        public static bool sessionValid(HttpContext context)
        {
            if (context.Session["board"] == null ||
                context.Session["playerCol"] == null)
                return false;

            return true;
        }
    }
}
