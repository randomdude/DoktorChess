using System.Collections.Generic;
using doktorChessGameEngine;

namespace doktorChess
{
    public class killerMoveStore
    {
        private readonly List<List<List<bool>>> _killerMovesAtDepth = new List<List<List<bool>>>();

        public killerMoveStore(int searchDepth)
        {
            const int boardSqCount = Board.sizeX * Board.sizeY;

            for (int depth = 0; depth < searchDepth + 1; depth++)
            {
                // For each depth, add a new list..
                _killerMovesAtDepth.Add(new List<List<bool>>(boardSqCount));

                initTableAtDepth(depth);
            }
        }

        private void initTableAtDepth(int depth)
        {
            const int boardSqCount = Board.sizeX * Board.sizeY;

            // Then add a list of 64 square lists to each.
            for (int n = 0; n < boardSqCount; n++)
            {
                _killerMovesAtDepth[depth].Add(new List<bool>(boardSqCount));
                for (int m = 0; m < boardSqCount; m++)
                {
                    _killerMovesAtDepth[depth][n].Add(false);
                }
            }
        }

        public bool contains(move toAdd, int depth)
        {
            List<List<bool>> thisDepth = _killerMovesAtDepth[depth];
            List<bool> fromThisSq = thisDepth[toAdd.dstPos.flatten()];

            return fromThisSq[toAdd.dstPos.flatten()];
        }

        public void add(int depth, move toAdd)
        {
            if (contains(toAdd, depth))
                return;

            List<List<bool>> thisDepth = _killerMovesAtDepth[depth];
            List<bool> fromThisSq = thisDepth[toAdd.dstPos.flatten()];

            fromThisSq[toAdd.dstPos.flatten()] = true;
        }

        public void advanceOne(int maxDepth)
        {
            // Notch all the other moves up.
            for (int i = 0; i < maxDepth - 1; i++)
            {
                _killerMovesAtDepth[i + 1] = _killerMovesAtDepth[i];
            }

            // Clear bottom move
            initTableAtDepth(1);
        }
    }
}