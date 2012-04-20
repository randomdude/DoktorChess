using System;
using System.Web.UI.WebControls;
using doktorChess;
using doktorChessGameEngine;

namespace WebFrontend
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack || !moveHandler.sessionValid(Context) )
            {
                // Initialise a new board
                boardSearchConfig config = new boardSearchConfig();
                config.searchDepth = 4;
                //Board newBoard = Board.makeNormalFromFEN(@"8/8/8/8/8/8/4KP2/R1Qn3k w - - 0 0", config);
                Board newBoard = Board.makeNormalStartPosition(config);

                Session["playerCol"] = pieceColour.white;
                Session["board"] = newBoard;

                Table boardAsTable = moveHandler.makeTable(newBoard);

                // Copy data in to our table
                while (boardAsTable.Rows.Count > 0)
                    board.Rows.Add(boardAsTable.Rows[0]);

                return;
            }
        }
    }
}
