using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls;
using doktorChess;
using doktorChessGameEngine;
using exampleMiniMax;

namespace tournament
{
    public partial class _Default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (runningTournament._tournament == null)
            {
                lblStatus.Text = "No tournament in progress";
                return;
            }

            lock (runningTournament._tournamentLock)
            {
                IEnumerable<tournamentGame> queued = runningTournament._tournament.gameQueue.Where(x => !x.isRunning && !x.isFinished);

                lblStatus.Text = queued.Count() + " games in queue:" ;

                foreach (tournamentGame thisGame in queued)
                {
                    TableRow gameRow = new TableRow();
                    tblQueuedGames.Rows.Add(gameRow);

                    TableCell cell1 = new TableCell();
                    cell1.Text = thisGame.white.typeName;
                    gameRow.Cells.Add(cell1);

                    TableCell cell2 = new TableCell();
                    cell2.Text = thisGame.black.typeName;
                    gameRow.Cells.Add(cell2);
                }

                IEnumerable<tournamentGame> inProgress = runningTournament._tournament.gameQueue.Where(x => x.isRunning && !x.isFinished);

                if (inProgress.Count() > 0)
                {
                    StringBuilder resp = new StringBuilder();
                    tournamentGame thisGame = inProgress.First();
                    lblCurrentGameSummary.Text = thisGame.white.typeName + " vs " + thisGame.black.typeName + " (" + thisGame.moveList.Count + " moves so far)";

                    // lol xss
                    string moveListHTML = makeMoveList(thisGame.moveList);
                    resp.Append(
                        string.Format("<table><tr><td>{1}</td><td>{0}</td></tr></table>", 
                            moveListHTML, thisGame.boardRepresentation));
                    lblCurrentGame.Text = resp.ToString();
                }
            }

        }

        public static string makeMoveList( List<move> gameToExamine)
        {
            StringBuilder resp = new StringBuilder();

            int moveIndex = 0;
            resp.Append("Move list: <br/>");
            foreach (move thisMove in gameToExamine)
            {
                if (moveIndex % 2 == 0)
                    resp.Append((moveIndex / 2) + 1 + ". ");
                resp.Append(thisMove.ToString(moveStringStyle.chessNotation));
                if (moveIndex % 2 == 0)
                    resp.Append(" ");
                else
                    resp.Append("<br/>");

                moveIndex++;
            }
            resp.Append("<br/>");

            return resp.ToString();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            runningTournament.startNewTournament();
        }

    }
}
