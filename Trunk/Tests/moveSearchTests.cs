using System.Diagnostics;
using doktorChess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class moveSearchTests
    {
        [TestMethod]
        public void testDepthTwoAsWhiteToPlay()
        {
            testAsWhiteToPlay(2, false);
        }

        [TestMethod]
        public void testDepthThreeAsWhiteToPlay()
        {
            testAsWhiteToPlay(3, false);
        }

        [TestMethod]
        public void testDepthFourAsWhiteToPlay()
        {
            testAsWhiteToPlay(4, false);
        }

        [TestMethod]
        public void testAlphaBetaDepthTwoAsWhiteToPlay()
        {
            testAsWhiteToPlay(2, true);
        }

        [TestMethod]
        public void testAlphaBetaDepthThreeAsWhiteToPlay()
        {
            testAsWhiteToPlay(3, true);
        }

        [TestMethod]
        public void testAlphaBetaDepthFourAsWhiteToPlay()
        {
            testAsWhiteToPlay(4, true);
        }

        [TestMethod]
        public void testAlphaBetaDepthFiveAsWhiteToPlay()
        {
            testAsWhiteToPlay(5, true);
        }

        public static void testAsWhiteToPlay(int depth, bool useAlphaBeta)
        {
            boardSearchConfig config = new boardSearchConfig();
            config.useThreatMap = false;
            config.killerHeuristic = false;
            config.useAlphaBeta = useAlphaBeta;
            config.searchDepth = depth;
            Board ourBoard = Board.makeQueenAndPawnsStartPosition(config);

            // We are white, and it is white's move. What is the (our) best move? It should
            // be D3, to prevent the pawn at D2 from being taken. Note that this relies on
            // the following move sequence being considered:
            // D3 QxD3 D4xQ
            // and thus needs a search depth that deep or more.
            lineAndScore bestLine = ourBoard.findBestMove(pieceColour.white);

            for (int i = 0; i < bestLine.line.Length; i++)
            {
                if (bestLine.line[i] != null)
                    Debug.WriteLine(bestLine.line[i].ToString());
            }

            Debug.WriteLine("Scored boards count: " + ourBoard.stats.boardsScored);

            // verify first move is from D1 to D2.
            Assert.IsTrue(bestLine.line[0].srcPos.isSameSquareAs(new squarePos(3, 1)));
            Assert.IsTrue(bestLine.line[0].dstPos.isSameSquareAs(new squarePos(3, 2)));
        }

        [TestMethod]
        public void testAsWinningMoveAsWhiteToPlay()
        {
            boardSearchConfig config = new boardSearchConfig();
            config.searchDepth = 0;
            Board ourBoard = new Board(gameType.queenAndPawns, config);
            ourBoard.addPiece(pieceType.queen, pieceColour.black, 3, 4);
            ourBoard.addPiece(pieceType.pawn, pieceColour.white, 0, 4);
            ourBoard.addPiece(pieceType.pawn, pieceColour.white, 1, 6);

            // We are white, and it is white's move. Analyze a situation in which pushing
            // a pawn will result in a win. Ensure that the AI does not push the wrong pawn.
            lineAndScore bestLine = ourBoard.findBestMove(pieceColour.white);

            for (int i = 0; i < bestLine.line.Length; i++)
            {
                if (bestLine.line[i] != null)
                    Debug.WriteLine(bestLine.line[i].ToString());
            }

            // verify first move is from A6 to A7.
            Assert.IsTrue(bestLine.line[0].srcPos.isSameSquareAs(new squarePos(1, 6)));
            Assert.IsTrue(bestLine.line[0].dstPos.isSameSquareAs(new squarePos(1, 7)));
        }

        [Ignore]
        [TestMethod]
        public void testSpecificPosition()
        {
            boardSearchConfig config = new boardSearchConfig();
            config.searchDepth = 3;
            // Test a  specific position which is giving me trouble.
            Board ourboard = new Board(gameType.queenAndPawns, config);

            ourboard.addPiece(pieceType.pawn, pieceColour.white, 0, 4).movedCount++;
            ourboard.addPiece(pieceType.pawn, pieceColour.white, 1, 1).movedCount++;
            ourboard.addPiece(pieceType.pawn, pieceColour.white, 3, 5).movedCount++;

            ourboard.addPiece(pieceType.queen, pieceColour.black, 3, 2).movedCount++;

            lineAndScore best = ourboard.findBestMove(pieceColour.black);

            Debug.WriteLine("Best line:");
            Debug.WriteLine(best.ToString());
        }
    }
}