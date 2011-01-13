using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace doktorChess
{
    public class moveResponse
    {
        public bool isValid;

        public string newBoardHTML;
        public squarePos movedPieceSrc;
        public squarePos movedPieceDst;

        public void loadBoardTable(Table theTable)
        {
            TextWriter ourTextWriter2 = new StringWriter();
            HtmlTextWriter ourHtmlWriter2 = new HtmlTextWriter(ourTextWriter2);
            theTable.RenderControl(ourHtmlWriter2);
            newBoardHTML = ourTextWriter2.ToString();            
        }
    }
}