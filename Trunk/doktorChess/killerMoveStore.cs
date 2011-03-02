using System.Collections.Generic;

namespace doktorChess
{
    public class killerMoveStore
    {
        private readonly List<List<List<bool>>> killerMovesAtDepth = new List<List<List<bool>>>();

        public killerMoveStore(int searchDepth)
        {
            const int boardSqCount = Board.sizeX*Board.sizeY;

            for (int depth = 0; depth < searchDepth + 1; depth++)
            {
                // For each depth, add a new list..
                killerMovesAtDepth.Add(new List<List<bool>>(boardSqCount));

                initTableAtDepth(depth);
            }
        }

        private void initTableAtDepth(int depth)
        {
            const int boardSqCount = Board.sizeX * Board.sizeY;

            // Then add a list of 64 square lists to each.
            for (int n = 0; n < boardSqCount; n++)
            {
                killerMovesAtDepth[depth].Add(new List<bool>(boardSqCount));
                for (int m = 0; m < boardSqCount; m++)
                {
                    killerMovesAtDepth[depth][n].Add(false);
                }
            }
        }

        public bool contains(move toAdd, int depth)
        {
            List<List<bool>> thisDepth = killerMovesAtDepth[depth];
            List<bool> fromThisSq = thisDepth[toAdd.dstPos.flatten()];

            return fromThisSq[toAdd.dstPos.flatten()];
        }

        public void add(int depth, move toAdd)
        {
            if (contains(toAdd, depth))
                return;

            List<List<bool>> thisDepth = killerMovesAtDepth[depth];
            List<bool> fromThisSq = thisDepth[toAdd.dstPos.flatten()];

            fromThisSq[toAdd.dstPos.flatten()] = true;
        }

        public void advanceOne(int maxDepth)
        {
            // Notch all the other moves up.
            for (int i = 0; i < maxDepth - 1; i++)
            {
                killerMovesAtDepth[i + 1] = killerMovesAtDepth[i];
            }

            // Clear bottom move
            initTableAtDepth(1);
        }
    }
}