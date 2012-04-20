using System;
using System.Text;

namespace doktorChessGameEngine
{
    public class lineAndScore
    {
        /// <summary>
        /// List of moves we represent
        /// </summary>
        public readonly move[] line;

        /// <summary>
        /// Score of the final position when all moves have been played
        /// </summary>
        public int finalScore;

        public Object _scorer;

        public lineAndScore(move[] newLine, int newFinalScore, Object scorer)
        {
            line = newLine;
            finalScore = newFinalScore;
            _scorer = scorer;
        }

        public override string ToString()
        {
            return ToString(moveStringStyle.coord);
        }

        public string ToString(moveStringStyle notationStyle)
        {
            StringBuilder toRet = new StringBuilder();

            foreach (move move in line)
            {
                if (move != null)
                    toRet.AppendLine(move.ToString(notationStyle));
            }
            toRet.AppendLine(_scorer.ToString());

            return toRet.ToString();
        }
    }
}