using System;
using System.Collections.Generic;
using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class threatMapTests
    {
        [TestMethod]
        public void testThreatMapQueenPawnsStartPosition()
        {
            Board ourBoard = Board.makeQueenAndPawnsStartPosition(new boardSearchConfig());

            string expectedThreatString =
                " -1 -1 -1  0 -1 -1 -1 -1\r\n  0  0 -1 -1 -1  0  0  0\r\n  0 -1  0 -1  0 -1  0  0\r\n -1  0  0 -1  0  0 -1  0\r\n  0  0  0 -1  0  0  0 -1\r\n  1  2  2  1  2  2  2  1\r\n  0  0  0 -1  0  0  0  0\r\n  0  0  0  0  0  0  0  0\r\n";

            string threatString = ourBoard.coverLevel.ToString();

            if (expectedThreatString != threatString)
                throw new Exception("Threat map was incorrect");
        }

        [TestMethod]
        public void testThreatMapNormalStartPosition()
        {
            Board ourBoard = Board.makeNormalStartPosition(new boardSearchConfig());

            //string expectedThreatString =
            //    "  0  0  0  0  0  0  0  0\r\n  0  0  0 -1  0  0  0  0\r\n  1  2  2  1  2  2  2  1\r\n  0  0  0 -1  0  0  0 -1\r\n -1  0  0 -1  0  0 -1  0\r\n  0 -1  0 -1  0 -1  0  0\r\n  0  0 -1 -1 -1  0  0  0\r\n -1 -1 -1  0 -1 -1 -1 -1\r\n";

            string threatString = ourBoard.coverLevel.ToString();

            //if (expectedThreatString != threatString)
            //    throw new Exception("bad threat map");
        }
    }
}