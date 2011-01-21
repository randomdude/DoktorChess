using System;
using System.Web.UI.WebControls;
using doktorChess;

namespace WebFrontend
{
    public partial class _Default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack || !moveHandler.sessionValid(Context) )
            {
                // Initialise a new board
                Board newBoard = Board.makeNormalStartPosition();
                newBoard.searchDepth = 4;

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
