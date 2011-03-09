using System.Diagnostics;
using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMateSituations
{
    [TestClass]
    public class MateFindingTests
    {
        [TestMethod]
        public void testMateFinding_position001()
        {
            testMateFinding(@"8/8/8/8/8/8/4KP2/R1Qn3k w - - 0 0", 2, null);
        }

        [TestMethod]
        public void testMateFinding_position002()
        {
            testMateFinding(@"r5rk/5p1p/5R2/4B3/8/8/7P/7K w - - 0 0", 3, "Ra6,f6,BxP,Rg7,RxR");
        }

        [TestMethod]
        public void testMateFinding_position003()
        {
            testMateFinding(@"5B2/6P1/1p6/8/1N6/kP6/2K5/8 w - - 0 1", 3, null);
        }

        private static void testMateFinding(string testFEN, int ourmoves, string correctMoves)
        {
            boardSearchConfig cfg = boardSearchConfig.getDebugConfig();
            cfg.searchDepth = (ourmoves + (ourmoves - 1) ) - 1;
            Board testBoard = Board.makeNormalFromFEN(testFEN, cfg);

            lineAndScore bestLine = testBoard.findBestMove();

            Assert.IsTrue(bestLine.finalScore >= BoardScorer.highest, "Did not find a mate");

            if (correctMoves != null)
            {
                string[] correctMovesSplit = correctMoves.Split(',');
                for (int index = 0; index < correctMovesSplit.Length; index++)
                {
                    string oneCorrectMove = correctMovesSplit[index];
                    if (bestLine.line[index].ToString(moveStringStyle.chessNotation).ToUpper().Trim() != oneCorrectMove.ToUpper().Trim() )
                        throw new AssertFailedException("Incorrect mate found");
                }
            }
            else
            {
                foreach (move move in bestLine.line)
                    Debug.WriteLine(move.ToString(moveStringStyle.chessNotation));
            }
        }
    }
}
