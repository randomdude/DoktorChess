using System;
using System.Web.UI.WebControls;
using doktorChess;
using doktorChessGameEngine;
using exampleMiniMax;

namespace WebFrontend
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack || !moveHandler.sessionValid(Context) )
            {
                // Initialise a new board
                //baseBoard newBoard = exampleMiniMaxBoard.makeNormalStartPosition();
                boardSearchConfig config = new boardSearchConfig();
                config.useThreatMap = false;
                config.searchDepth = 4;
                baseBoard newBoard = DoktorChessAIBoard.makeNormalStartPosition(config);

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
