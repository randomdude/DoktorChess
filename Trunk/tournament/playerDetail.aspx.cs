using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using doktorChessGameEngine;

namespace tournament
{
    public partial class playerDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lock(_Default._tournamentLock)
            {
                int id = Convert.ToInt32(Request.QueryString["playerID"]);
                contender player = _Default._tournament._contenders.Where(x => x.ID == id).First();

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
                    string moveText = _Default.makeMoveList(gameResult.moveList);
                    string linkCaption = "Click to expand " + gameResult.moveList.Count + " moves";
                    moveList.Text = "<div><a href=\"#\" class=\"expandable\">" + linkCaption + "</a><p class=\"expandableChild\">" + moveText + "</p></div>"; 
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
