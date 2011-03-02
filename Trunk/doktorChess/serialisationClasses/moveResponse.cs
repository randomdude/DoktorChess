using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace doktorChess
{
    /// <summary>
    /// This class is used by serialisation stuff.
    /// </summary>
    public class moveResponse
    {
// ReSharper disable UnaccessedField.Global
// ReSharper disable MemberCanBePrivate.Global
        public bool isValid;

        public string newBoardHTML;
        public squarePos movedPieceSrc;
        public squarePos movedPieceDst;
        public bool hasExtraCaptureSquare;
        public squarePos extraCaptureSquarePos;
        public bool gameFinished = false;
        public string gameResult;
        public bool forceBoardReload;
        public string whiteMove;
        public string blackMove;
        public string moveNum;
        public string bestLine;
// ReSharper restore MemberCanBePrivate.Global
// ReSharper restore UnaccessedField.Global

        public void loadBoardTable(Table theTable)
        {
            TextWriter ourTextWriter2 = new StringWriter();
            HtmlTextWriter ourHtmlWriter2 = new HtmlTextWriter(ourTextWriter2);
            theTable.RenderControl(ourHtmlWriter2);
            newBoardHTML = ourTextWriter2.ToString();            
        }

        public void initFromMove(move initWith)
        {
            movedPieceSrc = initWith.srcPos;
            movedPieceDst = initWith.dstPos;            

            // If this is an en passant capture, we need to express that the capture square is not the dest.
            if (initWith.capturedSquare == null || 
                initWith.capturedSquare.position.isSameSquareAs(initWith.dstPos))
            {
                hasExtraCaptureSquare = false;
            }
            else
            {
                hasExtraCaptureSquare = true;
                extraCaptureSquarePos = initWith.capturedSquare.position;
            }
        }
    }
}