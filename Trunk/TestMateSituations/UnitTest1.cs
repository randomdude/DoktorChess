using System;
using System.Diagnostics;
using doktorChess;
using doktorChessGameEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMateSituations
{
    public class mateSituation
    {
        public int movesToMate;
        public string FEN;

        public mateSituation(int newMovesToMate, string newFEN)
        {
            movesToMate = newMovesToMate;
            FEN = newFEN;
        }
    }

    [TestClass]
    public class MateFindingTests_bulk
    {
        private mateSituation[] mates = new mateSituation[]
                                            {
new mateSituation(2, "7n/3NR3/1P3p2/1p1kbN1B/1p6/1K6/6b1/1Q6 w – - 0 1"), 
new mateSituation(2, "3R4/4K3/5p2/5p2/8/3BkNQ1/8/8 w – - 0 1"), 
new mateSituation(2, "3K4/4B3/3Rp3/8/4pk2/1Qp1Np2/2p2P2/2R5 w – - 0 1"), 
new mateSituation(2, "6r1/5Q2/1n1p2pB/4k2b/3b3r/8/1NBRp2N/1K2R3 w – - 0 1"), 
new mateSituation(2, "8/1Rp5/K3P3/2B2Q2/n1kP4/P3r3/P3PN2/1N2bB2 w – - 0 1"), 
new mateSituation(2, "3KN3/2B2R2/6n1/8/4k2n/4p3/4B3/3R4 w – - 0 1"), 
new mateSituation(2, "5B2/3p1n2/R2p4/1P1NRBQ1/1KPkrb2/1p6/2Pp1Pn1/4r3 w – - 0 1"), 
new mateSituation(2, "rqr5/1B1N3p/2n1p2K/b2P4/RB1Nkn1R/1P6/2P2PP1/1Q6 w – - 0 1"), 
new mateSituation(2, "8/3k1P2/1K3B2/3B4/8/8/8/8 w – - 0 1"), 
new mateSituation(2, "2B5/8/4pN1K/R1B1qkP1/4p3/7p/5P1P/4Q3 w – - 0 1"), 
new mateSituation(2, "5Q2/8/8/8/8/5p2/2N1Np2/2K2k2 w – - 0 1"), 
new mateSituation(2, "8/8/1R6/2p5/8/2Bk1NRN/3P4/K6B w – - 0 1")
                                            };

        [TestMethod]
        public void testMateFinding_position_Bulk_001()
        {
            for (int n = 0; n < mates.Length; n++ )
                MateFindingTests.testMateFinding(mates[n].FEN, mates[n].movesToMate, null);
        }
    
    }

    [TestClass]
    public class MateFindingTests
    {
        [TestMethod]
        public void testMateFinding_position001()
        {
            testMateFinding(@"8/8/8/8/8/8/4KP2/R1Qn3k w - - 0 0", 2, null);
        }

        [TestMethod]
        public void testMateFinding_position001_asBlack()
        {
            testMateFinding(@"r1qN3K/4kp2/8/8/8/8/8/8 b - - 0 0", 2, null);
        }
        
        [TestMethod]
        public void testMateFinding_position002()
        {
            testMateFinding(@"r5rk/5p1p/5R2/4B3/8/8/7P/7K w - - 0 0", 3, "Ra6,f6,BxP,Rg7,RxR");
        }

        [TestMethod]
        public void testMateFinding_position002_asBlack()
        {
            testMateFinding(@"7k/7p/8/8/4b3/5r2/5P1P/R5RK b - - 0 0", 3, "Ra3,f3,BxP,Rg2,RxR");
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

        public static void testMateFinding(string testFEN, int ourmoves, string correctMoves)
        {
            boardSearchConfig cfg = boardSearchConfig.getDebugConfig();
            cfg.searchDepth = (ourmoves + (ourmoves - 1) ) - 1;
            Board testBoard = Board.makeNormalFromFEN(testFEN, cfg);

            testBoard.disableThreeFoldRule();

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
