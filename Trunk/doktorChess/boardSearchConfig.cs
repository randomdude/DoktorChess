using System;

namespace doktorChess
{
    [Serializable]
    public class boardSearchConfig
    {
// ReSharper disable ConvertToConstant.Global
        public bool useAlphaBeta = true;
        public bool killerHeuristic = true;
        public bool killerHeuristicPersists = false;
        public bool useThreatMap = true;
        public int searchDepth = 4;
        public bool checkLots = false;
        public bool checkThreatMapLots = false;

        public readonly scoreModifiers scoreConfig = new scoreModifiers();
// ReSharper restore ConvertToConstant.Global

        public static boardSearchConfig getDebugConfig()
        {
            boardSearchConfig toRet = new boardSearchConfig();

            toRet.checkLots = true;
            toRet.checkThreatMapLots = true;

            return toRet;
        }
    }
}