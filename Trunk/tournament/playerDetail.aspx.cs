using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using doktorChessGameEngine;
using WebFrontend;

namespace tournament
{
    public partial class playerDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lock (runningTournament._tournamentLock)
            {
                if (runningTournament._tournament == null)
                {
                    Response.Write("No tournament in progress");
                    return;
                }

                int id = Convert.ToInt32(Request.QueryString["playerID"]);
                contender player = runningTournament._tournament._contenders.Where(x => x.ID == id).First();

                lblplayerName.Text = player.typeName;

                foreach (playedGame gameResult in player.gamesPlayed)
                {
                    TableRow row = new TableRow();
                    gameTable.Rows.Add(row);

                    row.Cells.Add(makeCell(gameResult.opponentName));
                    if (gameResult.isErrored)
                    {
                        string errStr = "Error : " + gameResult.errorMessage + Environment.NewLine;
                        if (gameResult.exception != null)
                        {
                            errStr += gameResult.exception.Message;
                            errStr += gameResult.exception.StackTrace;
                        }
                        row.Cells.Add(makeCell(errStr));
                    }
                    else if (gameResult.isDraw)
                    {
                        row.Cells.Add(makeCell("Draw", "resultCellDraw"));
                    }
                    else if (gameResult.didWin)
                    {
                        row.Cells.Add(makeCell("Win", "resultCellWin"));
                    }
                    else
                    {
                        row.Cells.Add(makeCell("Loss", "resultCellLoss"));
                    }

                    row.Cells.Add(makeCell(gameResult.moveList.Count.ToString()));
                    TableCell moveList = new TableCell();
                    
                    // Make our collapsed, expandable moves list
                    string moveText = _Default.makeMoveList(gameResult.moveList);
                    string linkCaption = "Click to expand " + gameResult.moveList.Count + " moves";
                    string moveListFull = "<div><a href=\"#\" class=\"expandable\">" + linkCaption + "</a><p class=\"expandableChild\">" + moveText + "</p></div>";
                    
                    // Make our collapsed end-game position
                    Table endPositionTable = moveHandler.makeTable(gameResult.board);
                    TextWriter ourTextWriter2 = new StringWriter();
                    HtmlTextWriter ourHtmlWriter2 = new HtmlTextWriter(ourTextWriter2);
                    endPositionTable.RenderControl(ourHtmlWriter2);
                    string endPositionHTML = ourTextWriter2.ToString();            

                    string finishedPosFull = "<div><a href=\"#\" class=\"dialogable\">Click to show end position</a><div class=\"dialogChild\">" + endPositionHTML + "</div></div>";

                    // and put them both in the cell.
                    moveList.Text = moveListFull + finishedPosFull;
                    row.Cells.Add(moveList);
                }
            }

        }

        public static TableCell makeCell(string text)
        {
            TableCell optCell = new TableCell();
            optCell.Text = text;
            return optCell;
        }

        public static TableCell makeCell(string text, string cssStyle)
        {
            TableCell optCell = makeCell(text);
            optCell.CssClass = cssStyle;
            return optCell;
        }
    }
}
