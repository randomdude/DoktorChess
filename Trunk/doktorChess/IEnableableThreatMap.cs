﻿using System;

namespace doktorChess
{
    public interface IEnableableThreatMap
    {
        int this[squarePos position] { get; set; }
        void add(int x, int y);
        void add(squarePos pos);
        void remove(square toRemove);

        string ToString();
        bool isThreatened(square squareToCheck, pieceColour sideToExamine);
    }
}