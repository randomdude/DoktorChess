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

                    newRow.Cells.Add(playerDetail.makeCell(thisContender.score.ToString()));

                    newRow.Cells.Add(playerDetail.makeCell(thisContender.wins.ToString()));
                    newRow.Cells.Add(playerDetail.makeCell(thisContender.losses.ToString()));
                    newRow.Cells.Add(playerDetail.makeCell(thisContender.draws.ToString()));

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
                TableCell spacer = new TableCell();
                spacer.CssClass = "spacer";
                legendRow.Cells.Add(spacer);
                foreach (contender contRow in containersByName)
                {
                    TableCell cell = new TableCell();
                    cell.Text = contRow.typeName;
                    cell.CssClass = "legendRow";
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
                    legendCell.CssClass = "legendCol";
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
                            cell.CssClass = "disabled";
                            continue;
                        }

                        // Now hunt down the game between the two
                        IEnumerable<playedGame> intersection = contRow.gamesPlayed.Where(x => x.opponentID == ContCol.ID && x.col == pieceColour.white);
                        if (intersection.Count() == 0)
                        {
                            // Game is pending
                            cell.CssClass = "pending";
                            cell.Text = "...";
                        }
                        else
                        {
                            playedGame gameRes = intersection.Single();
                            if (gameRes.isDraw)
                            {
                                cell.CssClass = "draw";
                                cell.Text = "Draw";
                            }
                            else
                            {
                                if (gameRes.didWin)
                                {
                                    cell.CssClass = "whitewin";
                                    cell.Text = "White win";
                                }
                                else
                                {
                                    cell.CssClass = "whiteloss";
                                    cell.Text = "White loss";
                                }
                            }
                        }
                    }
                }

                // Now the league table.
                IOrderedEnumerable<tournamentGame> gamesByWhiteName = _Default._tournament.gameQueue.OrderBy(x => x.white.typeName);

                foreach (tournamentGame game in gamesByWhiteName)
                {
                    TableRow newRow = new TableRow();
                    matchList.Rows.Add(newRow);

                    TableCell whiteRes, blackRes;
                    if (!game.isFinished)
                    {
                        if (game.isRunning)
                        {
                            whiteRes = playerDetail.makeCell("Playing now");
                            blackRes = playerDetail.makeCell("Playing now");
                        }
                        else
                        {
                            whiteRes = playerDetail.makeCell("...");
                            blackRes = playerDetail.makeCell("...");                            
                        }
                    }
                    else
                    {
                        // The game is finished.
                        if (game.isDraw)
                        {
                            whiteRes = playerDetail.makeCell("½", "resultCellDraw");
                            blackRes = playerDetail.makeCell("½", "resultCellDraw");
                        }
                        else
                        {
                            if (game.winningSide == pieceColour.white)
                            {
                                whiteRes = playerDetail.makeCell("1", "resultCellWin");
                                blackRes = playerDetail.makeCell("0", "resultCellLoss");
                            }
                            else
                            {
                                whiteRes = playerDetail.makeCell("0", "resultCellLoss");
                                blackRes = playerDetail.makeCell("1", "resultCellWin");
                                
                            }
                        }
                    }

                    newRow.Cells.Add(playerDetail.makeCell(game.white.typeName));
                    newRow.Cells.Add(whiteRes);
                    newRow.Cells.Add(playerDetail.makeCell(" - "));
                    newRow.Cells.Add(blackRes);
                    newRow.Cells.Add(playerDetail.makeCell(game.black.typeName));
                }
            }
        }

    }
}
