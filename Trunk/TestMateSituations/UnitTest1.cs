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

        [TestMethod]
        public void testMateFinding_position004()
        {
            testMateFinding(@"8/R7/4kPP1/3ppp2/3B1P2/1K1P1P2/8/8 w - - 0 1", 3, null);
        }

        [TestMethod]
        public void testMateFinding_position005()
        {
            testMateFinding(@"r1bq2r1/b4pk1/p1pp1p2/1p2pP2/1P2P1PB/3P4/1PPQ2P1/R3K2R w - - 0 1", 2, null);
        }

        [TestMethod]
        public void testMateFinding_position006()
        {
            testMateFinding(@"3r1r1k/1p3p1p/p2p4/4n1NN/6bQ/1BPq4/P3p1PP/1R5K w - - 0 1", 3, null);
        }

        [TestMethod]
        public void testMateFinding_position007()
        {
            testMateFinding(@"k7/8/N1N5/3B4/K7/8/4p1r1/8 w – - 0 1", 3, null);
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
