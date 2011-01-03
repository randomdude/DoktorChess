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
            testAsWhiteToPlay(2);
        }

        [TestMethod]
        public void testDepthThreeAsWhiteToPlay()
        {
            testAsWhiteToPlay(3);
        }

        [TestMethod]
        public void testDepthFourAsWhiteToPlay()
        {
            testAsWhiteToPlay(4);
        }

        public static void testAsWhiteToPlay(int depth)
        {
            Board ourBoard = Board.makeQueenAndPawnsStartPosition();

            // We are white, and it is white's move. What is the (our) best move? It should
            // be D3, to prevent the pawn at D2 from being taken. Note that this relies on
            // the following move sequence being considered:
            // D3 QxD3 D4xQ
            // and thus needs a search depth that deep or more.
            ourBoard.searchDepth = depth;
            lineAndScore bestLine = ourBoard.findBestMove(pieceColour.white);

            for (int i = 0; i < bestLine.line.Length; i++)
            {
                if (bestLine.line[i] != null)
                    Debug.WriteLine(bestLine.line[i].ToString());
            }

            // verify first move is from D1 to D2.
            Assert.IsTrue(bestLine.line[0].srcPos.isSameSquareAs(new squarePos(3, 1)));
            Assert.IsTrue(bestLine.line[0].dstPos.isSameSquareAs(new squarePos(3, 2)));
        }

        [TestMethod]
        public void testAsWinningMoveAsWhiteToPlay()
        {
            Board ourBoard = new Board(gameType.queenAndPawns);
            ourBoard.addPiece(3, 4, pieceType.queen, pieceColour.black);
            ourBoard.addPiece(0, 4, pieceType.pawn, pieceColour.white);
            ourBoard.addPiece(1, 6, pieceType.pawn, pieceColour.white);

            // We are white, and it is white's move. Analyze a situation in which pushing
            // a pawn will result in a win. Ensure that the AI does not push the wrong pawn.
            ourBoard.searchDepth = 0;
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

        [TestMethod]
        public void testSpecificPosition()
        {
            // Test a  specific position which is giving me trouble.
            Board ourboard = new Board(gameType.queenAndPawns);

            ourboard.addPiece(0, 4, pieceType.pawn, pieceColour.white).movedCount++;
            ourboard.addPiece(1, 1, pieceType.pawn, pieceColour.white).movedCount++;
            ourboard.addPiece(3, 5, pieceType.pawn, pieceColour.white).movedCount++;

            ourboard.addPiece(3, 2, pieceType.queen, pieceColour.black).movedCount++;

            ourboard.searchDepth = 3;
            lineAndScore best = ourboard.findBestMove(pieceColour.black);

            Debug.WriteLine("Best line:");
            Debug.WriteLine(best.ToString());
        }
    }
}