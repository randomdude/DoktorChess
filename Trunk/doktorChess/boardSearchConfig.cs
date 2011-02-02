namespace doktorChess
{
    public class boardSearchConfig
    {
        public bool useAlphaBeta = true;
        public bool killerHeuristic = true;
        public bool killerHeuristicPersists = false;
        public bool useThreatMap = true;
        public int searchDepth = 4;
        public bool checkLots = false;
        public bool checkThreatMapLots = false;

        public static boardSearchConfig getDebugConfig()
        {
            boardSearchConfig toRet = new boardSearchConfig();

            toRet.checkLots = true;
            toRet.checkThreatMapLots = true;

            return toRet;
        }
    }
}