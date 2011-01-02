using System;
using System.Collections.Generic;
using System.Text;

namespace doktorChess
{
    public class lineAndScore
    {
        /// <summary>
        /// List of moves we represent
        /// </summary>
        public move[] line;

        /// <summary>
        /// Score of the final position when all moves have been played
        /// </summary>
        public int finalScore;

        public lineAndScore(move[] newLine, int newFinalScore)
        {
            line = newLine;
            finalScore = newFinalScore;
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
            toRet.AppendLine("Score " + finalScore);

            return toRet.ToString();
        }
    }
}