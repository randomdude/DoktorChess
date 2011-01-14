namespace doktorChess
{
    public class moveSearchStats
    {
        public int boardsScored;
        public long totalSearchTime;
        public long boardScoreTime;

        public double scoredPerSecond
        {
            get { return boardsScored/(totalSearchTime/1000.0); }
        }
    }
}