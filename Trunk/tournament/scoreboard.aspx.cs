using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using doktorChessGameEngine;

namespace tournament
{
    public partial class scoreboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Default._tournament == null)
            {
                Response.Write("No tournament in progress");
                return;
            }

            lock(_Default._tournamentLock)
            {
                // fill the top leaderboard
                IOrderedEnumerable<contender> containersByScore = _Default._tournament._contenders.OrderByDescending(x => x.score);

                int n = 1;
                foreach (contender thisContender in containersByScore)
                {
                    TableRow newRow = new TableRow();

                    TableCell cell0 = new TableCell();
                    cell0.Text = n.ToString();
                    newRow.Cells.Add(cell0);

                    TableCell cell1 = new TableCell();
                    HyperLink playerDetailLink = new HyperLink();
                    playerDetailLink.NavigateUrl = "/playerDetail.aspx?playerID=" + thisContender.ID;
                    playerDetailLink.Text = thisContender.typeName;
                    cell1.Controls.Add(playerDetailLink);
                    newRow.Cells.Add(cell1);

                    newRow.Cells.Add(makeCell(thisContender.score.ToString()));

                    newRow.Cells.Add(makeCell(thisContender.wins.ToString()));
                    newRow.Cells.Add(makeCell(thisContender.losses.ToString()));
                    newRow.Cells.Add(makeCell(thisContender.draws.ToString()));

                    TableCell errorCell = new TableCell();
                    errorCell.Text = thisContender.errorCount.ToString();
                    if (thisContender.errorCount > 0)
                        errorCell.BackColor = Color.Pink;
                    newRow.Cells.Add(errorCell);

                    Leaderboard.Rows.Add(newRow);
                    n++;
                }

                // And then the all-play-all table.
                IOrderedEnumerable<contender> containersByName = _Default._tournament._contenders.OrderByDescending(x => x.score);

                // Top legend
                TableRow legendRow = new TableRow();
                resultGrid.Rows.Add(legendRow);
                legendRow.Cells.Add(new TableCell());
                foreach (contender contRow in containersByName)
                {
                    TableCell cell = new TableCell();
                    cell.Text = contRow.typeName;
                    legendRow.Cells.Add(cell);
                }

                int contRowIndex = 0;
                foreach (contender contRow in containersByName)
                {
                    TableRow row = new TableRow();
                    resultGrid.Rows.Add(row);

                    // Add the left-hand legend
                    TableCell legendCell = new TableCell();
                    legendCell.Text = contRow.typeName;
                    row.Cells.Add(legendCell);
                    
                    contRowIndex++;
                    int contColIndex = 0;
                    foreach (contender ContCol in containersByName)
                    {
                        contColIndex++;

                        TableCell cell = new TableCell();
                        row.Cells.Add(cell);

                        if (contColIndex == contRowIndex)
                        {
                            cell.BackColor = Color.LightGray;
                            continue;
                        }

                        // Now hunt down the game between the two
                        IEnumerable<playedGame> intersection = contRow.gamesPlayed.Where(x => x.opponentID == ContCol.ID && x.col == pieceColour.white);
                        if (intersection.Count() == 0)
                        {
                            // Game is pending
                            cell.Text = "...";
                        }
                        else
                        {
                            playedGame gameRes = intersection.Single();
                            if (gameRes.isDraw)
                                cell.Text = "Draw";
                            else
                                cell.Text = gameRes.didWin ? "White win" : "White loss";
                        }
                    }
                }
            }
        }


        private TableCell makeCell(string text)
        {
            TableCell optCell = new TableCell();
            optCell.Text = text;
            return optCell;
        }
    }
}
