using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using doktorChessGameEngine;

namespace tournament
{
    public partial class watchGame : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int gameID = Convert.ToInt32(Request.QueryString["gameID"]);

            tournamentGame toWatch = runningTournament._tournament.gameQueue.Where(x => x.id == gameID).Single();

            lblGameDetail.Text = "Game: " + toWatch.white.typeName + " vs " + toWatch.black.typeName;
            lblWhite.Text = "White : " + toWatch.white.typeName;
            lblBlack.Text = "Black : " + toWatch.black.typeName;
            if (toWatch.isFinished)
            {
                if (toWatch.isErrored)
                {
                    lblStatus.Text = "<b>Game has aborted with an error!</b>";
                }
                else if (toWatch.isDraw)
                {
                    lblStatus.Text = "<b>Game is drawn!</b>";
                }
                else
                {
                    if (toWatch.winningSide == pieceColour.white)
                        lblStatus.Text = "<b>Game is won for white (" + toWatch.white.typeName + ")!</b>";
                    else
                        lblStatus.Text = "<b>Game is won for black (" + toWatch.black.typeName + ")!</b>";
                }
            }
            else
            {
                lblStatus.Text = "Game is in progress (" + toWatch.moveList.Count + " moves so far)";
                lblStatus.Text += "<br/>" + toWatch.colToMove + " to play.";
            }
            lblTheGame.Text = toWatch.boardRepresentation;
        }
    }
}
