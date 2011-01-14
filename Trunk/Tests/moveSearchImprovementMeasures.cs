using System.Diagnostics;
using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class moveSearchImprovementMeasures
    {
        /// <summary>
        /// Never run a test involving a minimax search at deeper than this ply.
        /// </summary>
        private const int maxDepthForMinimax = 5;

        /// <summary>
        /// Never run a test involving an AB search at deeper than this ply.
        /// </summary>
        private const int MaxDepthForAB = 7;

        [TestMethod]
        public void findImprovementAlphaBetaDepth()
        {
            for (int depth = 1; depth < maxDepthForMinimax; depth++)
            {
                moveSearchStats statsAB = runTest(depth, true, false);
                moveSearchStats statsMinimax = runTest(depth, false, false);

                double ratio = ((double)statsAB.boardsScored) / ((double)statsMinimax.boardsScored);

                Debug.WriteLine(string.Format("Depth {0}, scored {1} boards AB and {2} minimax, ratio {3}", depth, statsAB.boardsScored, statsMinimax.boardsScored, ratio));
            }
        }

        [TestMethod]
        public void findImprovementKillerDepth()
        {
            for (int depth = 1; depth < MaxDepthForAB; depth++)
            {
                moveSearchStats stats = runTest(depth, true, false);
                moveSearchStats statsKiller = runTest(depth, true, true);

                double ratio = ((double)statsKiller.boardsScored) / ((double)stats.boardsScored);

                Debug.WriteLine(string.Format("Depth {0}, scored {1} boards with killer heursitic and {2} without, ratio {3}", depth, statsKiller.boardsScored, stats.boardsScored, ratio));
            }
        }

        private static moveSearchStats runTest(int depth, bool useAlphaBeta, bool useKiller)
        {
            Board ourBoard = Board.makeQueenAndPawnsStartPosition();

            ourBoard.searchDepth = depth;
            ourBoard.alphabeta = useAlphaBeta;
            ourBoard.killerHeuristic = useKiller;

            ourBoard.findBestMove(pieceColour.white);

            return ourBoard.stats;
        }
    }
}