using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using doktorChessGameEngine;

namespace tournament
{

    public partial class scoreboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lock (runningTournament._tournamentLock)
            {
                if (runningTournament._tournament == null || runningTournament.getPlayerCount() == 0)
                {
                    results.Visible = false;
                    PanelNoTournament.Visible = true;
                }
                else
                {
                    IOrderedEnumerable<contender> containersByScore =
                        runningTournament._tournament.contenders.OrderByDescending(x => x.score);
                    IOrderedEnumerable<contender> containersByName =
                        runningTournament._tournament.contenders.OrderByDescending(x => x.score);
                    IOrderedEnumerable<tournamentGame> gamesByWhiteName =
                        runningTournament._tournament.gameQueue.OrderBy(x => x.white.typeName);

                    // fill the top leaderboard
                    fillLeaderBoard(containersByScore);

                    // And then the all-play-all table.
                    fillResultGrid(containersByName);

                    // Now the league table.
                    fillFixtureList(gamesByWhiteName);

                    // Now, populate the 'tournament status' box
                    fillTournamentStatus();
                }
            }

            // Don't show the player upload div if the user isn't logged in.
            if (HttpContext.Current.User == null ||
                !HttpContext.Current.User.Identity.IsAuthenticated)
            {
                lblYouAre.Text = "You not logged in.";
                UploadPanel.Visible = false;
                lnkLogin.Visible = true;
                cmdLogout.Visible = false;
                authOnly.Visible = false;
            }
            else
            {
                // User _is_ logged in.
                lblYouAre.Text = "You are logged in as " + HttpContext.Current.User.Identity.Name;
                lnkLogin.Visible = false;
                cmdLogout.Visible = true;
                authOnly.Visible = true;
            }
        }

        private void fillTournamentStatus()
        {
            int finishedGames = runningTournament._tournament.gameQueue.Count(x => x.isFinished);
            int totalGames = runningTournament._tournament.gameQueue.Count();
            tournamentGame currentGame = runningTournament._tournament.gameQueue.SingleOrDefault(x => x.isRunning && !x.isFinished);
            if (finishedGames == totalGames)
            {
                lblTourneyStatus.Text = "All games finished";
                lblCurrentGameStatus.Text = "";
            }
            else
            {
                lblTourneyStatus.Text = String.Format("{0} of {1} games finished", finishedGames, totalGames);
                if (currentGame == null)
                {
                    lblCurrentGameStatus.Text = "First game starting..";
                }
                else
                {
                    lblCurrentGameStatus.Text = String.Format("Current game is {0} (white) vs {1} (black)",
                                                              currentGame.white.typeName, currentGame.black.typeName);
                    lblCurrentGameLink.Text =
                        String.Format("<a href=watchGame.aspx?gameID={0}>Watch current game</a>", currentGame.id);
                }
            }
        }

        private void fillFixtureList(IOrderedEnumerable<tournamentGame> gamesByWhiteName)
        {
            foreach (tournamentGame game in gamesByWhiteName)
            {
                TableRow newRow = new TableRow();
                matchList.Rows.Add(newRow);

                TableCell whiteRes, blackRes = null;
                bool middleCellsMerged = false;
                if (!game.isFinished)
                {
                    if (game.isRunning)
                    {
                        string cellText = String.Format("Playing now ({1} moves) <a href=watchGame.aspx?gameID={0}>Watch</a>", game.id, game.moveList.Count);

                        whiteRes = utils.makeCell(cellText);
                        whiteRes.ColumnSpan = 3;
                        middleCellsMerged = true;
                    }
                    else
                    {
                        whiteRes = utils.makeCellAndEscapeContents("...");
                        blackRes = utils.makeCellAndEscapeContents("...");                            
                    }
                }
                else
                {
                    // The game is finished.
                    if (game.isErrored)
                    {
                        if (game.erroredSide == pieceColour.white)
                        {
                            whiteRes = utils.makeCellAndEscapeContents("Error", "resultCellError");
                            blackRes = utils.makeCellAndEscapeContents("1", "resultCellWin");
                        }
                        else
                        {
                            whiteRes = utils.makeCellAndEscapeContents("1", "resultCellWin");
                            blackRes = utils.makeCellAndEscapeContents("Error", "resultCellError");
                        }
                    }
                    else if (game.isDraw)
                    {
                        whiteRes = utils.makeCellAndEscapeContents("½", "resultCellDraw");
                        blackRes = utils.makeCellAndEscapeContents("½", "resultCellDraw");
                    }
                    else
                    {
                        if (game.winningSide == pieceColour.white)
                        {
                            whiteRes = utils.makeCellAndEscapeContents("1", "resultCellWin");
                            blackRes = utils.makeCellAndEscapeContents("0", "resultCellLoss");
                        }
                        else
                        {
                            whiteRes = utils.makeCellAndEscapeContents("0", "resultCellLoss");
                            blackRes = utils.makeCellAndEscapeContents("1", "resultCellWin");
                                
                        }
                    }
                }

                newRow.Cells.Add(utils.makeCellAndEscapeContents(game.white.typeName));
                newRow.Cells.Add(whiteRes);
                if (!middleCellsMerged)
                {
                    newRow.Cells.Add(utils.makeCellAndEscapeContents(" - "));
                    newRow.Cells.Add(blackRes);
                }
                newRow.Cells.Add(utils.makeCellAndEscapeContents(game.black.typeName));
            }
        }

        private void fillResultGrid(IOrderedEnumerable<contender> containersByName)
        {
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
                        if (gameRes.isErrored)
                        {
                            if (gameRes.erroredSide == pieceColour.white)
                            {
                                cell.CssClass = "draw";
                                cell.Text = "White error";
                            }
                            else
                            {
                                cell.CssClass = "draw";
                                cell.Text = "Black error";
                            }
                        }
                        else if (gameRes.isDraw)
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
        }

        private void fillLeaderBoard(IOrderedEnumerable<contender> containersByScore)
        {
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

                newRow.Cells.Add(utils.makeCellAndEscapeContents(thisContender.score.ToString()));

                newRow.Cells.Add(utils.makeCellAndEscapeContents(thisContender.wins.ToString(), "resultCellWin"));
                newRow.Cells.Add(utils.makeCellAndEscapeContents(thisContender.draws.ToString(), "resultCellDraw"));
                newRow.Cells.Add(utils.makeCellAndEscapeContents(thisContender.losses.ToString(), "resultCellLoss"));

                if (thisContender.errorCount == 0)
                    newRow.Cells.Add(utils.makeCellAndEscapeContents(thisContender.errorCount.ToString()));
                else
                    newRow.Cells.Add(utils.makeCellAndEscapeContents(thisContender.errorCount.ToString(), "resultCellError"));

                Leaderboard.Rows.Add(newRow);
                n++;
            }
        }

        protected void cmdLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("scoreboard.aspx");
        }


        protected void cmdEmptyTournament_Click(object sender, EventArgs e)
        {
            if (HttpContext.Current.User == null ||
                !HttpContext.Current.User.Identity.IsAuthenticated)
            {
                Response.Redirect("/login.aspx");
                return;
            }

            runningTournament.clearPlayers();

            Response.Redirect("/scoreboard.aspx");
        }

        protected void cmdRestartTournament_Click(object sender, EventArgs e)
        {
            if (HttpContext.Current.User == null ||
                !HttpContext.Current.User.Identity.IsAuthenticated)
            {
                Response.Redirect("/login.aspx");
                return;
            }

            runningTournament.restart();

            Response.Redirect("/scoreboard.aspx");
        }

        protected void cmdChangePass_Click(object sender, EventArgs e)
        {
            if (HttpContext.Current.User == null ||
                !HttpContext.Current.User.Identity.IsAuthenticated)
            {
                Response.Redirect("/login.aspx");
                return;
            }

            Response.Redirect("/login.aspx?changepass=true");
        }

        protected void cmdUploadAI_Click(object sender, EventArgs e)
        {
            if (HttpContext.Current.User == null ||
                !HttpContext.Current.User.Identity.IsAuthenticated)
            {
                Response.Redirect("/login.aspx");
                return;
            }

            if (!FileUpload.HasFile)
            {
                Response.Write("No file supplied");
                return;
            }

            // Save the assembly in a temp folder. We need to do this to enable the appdomain to be rooted
            // at this folder.
            string path = Path.GetRandomFileName();
            string assemblyFolder = Path.Combine(Path.GetTempPath(), path);
            Directory.CreateDirectory(assemblyFolder);
            string assemblyPath = Path.Combine(assemblyFolder, FileUpload.FileName);
            FileUpload.SaveAs(assemblyPath);

            // Copy the doktorChessGameEngine assembly in to the new folder, since it is referenced by the
            // new assebly
            string toCopy = Assembly.GetAssembly(typeof (baseBoard)).Location;
            string dokChessPath = Path.Combine(assemblyFolder, "doktorChessGameEngine.dll");
            File.Copy(toCopy, dokChessPath );

            // Load the new assembly in to a new AppDomain
            AppDomain domain = tournamentGame.createAppDomain(assemblyFolder, "AI Interrogation");
            try
            {
                AssemblyName asmName = AssemblyName.GetAssemblyName(assemblyPath);
                asmName.CodeBase = null;
                Assembly ass = domain.Load(asmName.FullName);

                // Find types which are tagged with our chessAIAttribute
                List<assemblyNameAndType> newPlayerTypes = new List<assemblyNameAndType>(5);
                Module[] modules = ass.GetModules();
                foreach (Module module in modules)
                {
                    foreach (Type typeToInterrogate in module.GetTypes())
                    {
                        if (typeToInterrogate.GetCustomAttributes(typeof(chessAIAttribute), true).Length > 0)
                        {
                            newPlayerTypes.Add(new assemblyNameAndType(assemblyPath, assemblyFolder, typeToInterrogate));
                        }
                    }
                }

                if (newPlayerTypes.Count == 0)
                {
                    Response.Write("No [chessAI]-tagged classes found in the supplied assembly");
                    return;
                }

                // We have loaded one or more players. We should stop the current tournament and start a new one
                // including this player.
                runningTournament.endAndStartWithNewPlayers(newPlayerTypes);

                Response.Redirect("/scoreboard.aspx");
            }
            //catch(Exception)
            //{
            //    // TODO: Nice exception handling (dialogs, etc).
            //}
            finally 
            {
                try
                {
                    AppDomain.Unload(domain);
                }
                catch (CannotUnloadAppDomainException)
                {
                    // ... bah.
                }
           }
        }
    }
}
