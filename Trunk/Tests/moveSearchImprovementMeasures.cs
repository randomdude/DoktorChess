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
        private const int maxDepthForMinimax = 4;

        /// <summary>
        /// Never run a test involving an AB search at deeper than this ply.
        /// </summary>
        private const int MaxDepthForAB = 5;

        [TestMethod]
        public void findImprovementAlphaBetaDepth()
        {
            boardSearchConfig configMinimax = new boardSearchConfig() {useAlphaBeta = false, killerHeuristic = false, useThreatMap = false};
            boardSearchConfig configAB = new boardSearchConfig() {useAlphaBeta = true, killerHeuristic = false, useThreatMap = false};

            for (int depth = 1; depth < maxDepthForMinimax; depth++)
            {
                configAB.searchDepth = depth;
                configMinimax.searchDepth = depth;

                moveSearchStats statsAB = runTest(configAB);
                moveSearchStats statsMinimax = runTest(configMinimax);

                double ratio = ((double)statsAB.boardsScored) / ((double)statsMinimax.boardsScored);
                double timeSpeedup = statsAB.totalSearchTime / (double)statsMinimax.totalSearchTime;

                Debug.WriteLine(string.Format("Depth {0}, scored {1} boards AB and {2} minimax, ratio {3} : time {4} / {5}, ratio {6}", depth, statsAB.boardsScored, statsMinimax.boardsScored, ratio, statsAB.totalSearchTime, statsMinimax.totalSearchTime, timeSpeedup));

                // Check against some previous run estimates for all but the first run; the first run
                // is too fast to time accurately.
                if (depth == 1)
                    continue;
                double expected = 1;
                switch (depth)
                {
                    case 2:
                        expected = 0.5;
                        break;
                    case 3:
                        expected = 0.1;
                        break;
                    case 4:
                        expected = 0.1;
                        break;
                }

                if (ratio > expected)
                    throw new AssertFailedException("AB run seemed to take a long time compared to minimax");
            }
        }

        [TestMethod]
        public void findImprovementKillerDepth()
        {
            boardSearchConfig configKiller = new boardSearchConfig() {useAlphaBeta = true, killerHeuristic = true, useThreatMap = false};
            boardSearchConfig configNonKiller = new boardSearchConfig() {useAlphaBeta = true, killerHeuristic = false, useThreatMap = false};

            for (int depth = 1; depth < MaxDepthForAB; depth++)
            {
                configKiller.searchDepth = depth;
                configNonKiller.searchDepth = depth;

                moveSearchStats statsNonKiller = runTest(configNonKiller);
                moveSearchStats statsKiller = runTest(configKiller);

                double ratio = ((double)statsKiller.boardsScored) / ((double)statsNonKiller.boardsScored);
                double timeSpeedup = statsKiller.totalSearchTime / (double)statsNonKiller.totalSearchTime;

                Debug.WriteLine(string.Format("Depth {0}, scored {1} boards killer and {2} without, ratio {3} : time {4} / {5}, ratio {6}", depth, statsKiller.boardsScored, statsNonKiller.boardsScored, ratio, statsKiller.totalSearchTime, statsNonKiller.totalSearchTime, timeSpeedup));
            }
        }

        [TestMethod]
        public void findSlowdownThreatMappingDepth()
        {
            boardSearchConfig configThreatMap = new boardSearchConfig() { useAlphaBeta = true, killerHeuristic = true, useThreatMap = true };
            boardSearchConfig configNonThreatMap = new boardSearchConfig() { useAlphaBeta = true, killerHeuristic = true, useThreatMap = false };

            for (int depth = 0; depth < MaxDepthForAB; depth++)
            {
                configThreatMap.searchDepth = depth;
                configNonThreatMap.searchDepth = depth;

                moveSearchStats statsThreatMap = runTest(configThreatMap);
                moveSearchStats statsNonThreatMap = runTest(configNonThreatMap);

                double timeSpeedup = statsNonThreatMap.totalSearchTime / (double)statsThreatMap.totalSearchTime;

                Debug.WriteLine(string.Format("Depth {0} : time {1} without / {2} with threatmap, ratio {3}", depth, statsNonThreatMap.totalSearchTime, statsThreatMap.totalSearchTime, timeSpeedup));

                //if (timeSpeedup < 0.9)
                //    throw new AssertFailedException("Threat mapping is too slow");
            }
        }

        private static moveSearchStats runTest(boardSearchConfig searchConfig)
        {
            Board ourBoard = Board.makeNormalStartPosition(searchConfig);

            ourBoard.findBestMove(pieceColour.white);

            return ourBoard.stats;
        }
    }
}