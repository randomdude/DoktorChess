﻿using System;

namespace doktorChess
{
    class disabledThreatMap : IEnableableThreatMap
    {
        public int this[squarePos position]
        {
            get { return 0;  }
            set {  }
        }

        public void add(int x, int y)
        {
        }

        public void add(squarePos pos)
        {
        }

        public void remove(square toRemove)
        {
        }

        public bool isThreatened(square squareToCheck, pieceColour sideToExamine)
        {
            return false;
        }
    }
}