using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
                    // No tournament is in progress!
                    return;
                }

                int id = Convert.ToInt32(Request.QueryString["playerID"]);
                contender player = runningTournament._tournament.contenders.Where(x => x.ID == id).First();

                lblplayerName.Text = player.typeName;

                foreach (playedGame gameResult in player.gamesPlayed)
                {
                    TableRow row = new TableRow();
                    gameTable.Rows.Add(row);

                    row.Cells.Add(utils.makeCellAndEscapeContents(gameResult.col.ToString()));
                    row.Cells.Add(utils.makeCellAndEscapeContents(gameResult.opponentTypeName));
                    if (gameResult.isErrored)
                    {
                        string errStr =  gameResult.errorMessage + Environment.NewLine;
                        if (gameResult.exception != null)
                        {
                            errStr += gameResult.exception.Message;
                            errStr += gameResult.exception.StackTrace;
                        }

                        string errStrSafe = HttpUtility.HtmlEncode(errStr);
                        string errlinkCaption = "Error : Click to show details";
                        string errCellHTML = "<div><a href=\"#\" class=\"expandable\">" + errlinkCaption + "</a><p class=\"expandableChild\">" + errStrSafe + "</p></div>";

                        row.Cells.Add(utils.makeCell(errCellHTML));
                    }
                    else if (gameResult.isDraw)
                    {
                        row.Cells.Add(utils.makeCellAndEscapeContents("Draw", "resultCellDraw"));
                    }
                    else if (gameResult.didWin)
                    {
                        row.Cells.Add(utils.makeCellAndEscapeContents("Win", "resultCellWin"));
                    }
                    else
                    {
                        row.Cells.Add(utils.makeCellAndEscapeContents("Loss", "resultCellLoss"));
                    }

                    row.Cells.Add(utils.makeCellAndEscapeContents(gameResult.moveList.Count.ToString()));
                    TableCell moveList = new TableCell();
                    
                    // Make our collapsed, expandable moves list
                    string moveText = utils.makeMoveListAndEscapeContents(gameResult.moveList);
                    string linkCaption = "Click to expand " + gameResult.moveList.Count + " moves";
                    string moveListFull = "<div><a href=\"#\" class=\"expandable\">" + linkCaption + "</a><p class=\"expandableChild\">" + moveText + "</p></div>";
                    
                    // Make our collapsed end-game position
                    Table endPositionTable = utils.makeTableAndEscapeContents(gameResult.board);
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
    }
}
