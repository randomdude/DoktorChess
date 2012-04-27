using System;
using doktorChessGameEngine;

namespace exampleMiniMax
{
    [chessAIAttribute]
    [Serializable]
    public class exampleRandom : baseBoard
    {
        Random rnd = new Random();

        public exampleRandom(gameType newType) : base(newType)
        {
        }
        
        public override lineAndScore findBestMove()
        {
            sizableArray<move> movesToConsider = getMoves(colToMove);

            // Filter out any moves in to check
            sizableArray<move> movesNotIntoCheck = new sizableArray<move>(movesToConsider.Length);
            pieceColour movingCol = colToMove;
            foreach (move consideredMove in movesToConsider)
            {
                doMove(consideredMove);

                if (!isPlayerInCheck(movingCol))
                    movesNotIntoCheck.Add(consideredMove);

                undoMove(consideredMove);
            }

            // and then select a random move.
            int rndNum = rnd.Next(movesNotIntoCheck.Length);
            move randomMove = movesNotIntoCheck[rndNum];

            return new lineAndScore(new move[] { randomMove }, 0, null);
        }
    }
}