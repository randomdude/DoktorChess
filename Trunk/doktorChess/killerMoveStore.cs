using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace doktorChess
{
    public class killerMoveStore
    {
        private readonly List<List<List<bool>>> killerMovesAtDepth = new List<List<List<bool>>>();
        //private readonly int[] listSizesForDepth = new int[] { 0, 100, 100, 500, 2000, 7000, 40000, 200000, 0 };

        public killerMoveStore(int searchDepth)
        {
            int boardSqCount = Board.sizeX*Board.sizeY;

            for (int depth = 0; depth < searchDepth + 1; depth++)
            {
                // For each depth, add a new list..
                killerMovesAtDepth.Add(new List<List<bool>>(boardSqCount));

                initTableAtDepth(depth);
            }
        }

        private void initTableAtDepth(int depth)
        {
            int boardSqCount = Board.sizeX * Board.sizeY;

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

        public void Clear()
        {
            foreach (List<List<bool>> depth in killerMovesAtDepth)
            {
                foreach (List<bool> srcSquareList in depth)
                {
                    for (int index = 0; index < srcSquareList.Count; index++)
                        srcSquareList[index] = false;
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
            //for (int i = 1; i < Board.sizeX * Board.sizeY; i++)
            //{
            //    killerMovesAtDepth[maxDepth][i].Clear();
            //    killerMovesAtDepth[maxDepth][i].Add(false);
            //}
        
        }
    }
}