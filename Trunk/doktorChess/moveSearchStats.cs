namespace doktorChess
{
    public class moveSearchStats
    {
// ReSharper disable UnusedMember.Global
        public int boardsScored;
        public long totalSearchTime;
        public long boardScoreTime;

        public double scoredPerSecond
        {
            get { return boardsScored/(totalSearchTime/1000.0); }
        }
// ReSharper restore UnusedMember.Global
    }
}