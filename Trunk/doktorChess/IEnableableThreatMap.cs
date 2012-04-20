using doktorChessGameEngine;

namespace doktorChess
{
    public interface IEnableableThreatMap
    {
// ReSharper disable UnusedMemberInSuper.Global
        int this[squarePos position] { get; set; }
// ReSharper restore UnusedMemberInSuper.Global
        void add(int x, int y);
        void add(squarePos pos);
        void remove(square toRemove);

        bool checkStuff { set; }

        string ToString();
        bool isThreatened(square squareToCheck, pieceColour sideToExamine);
    }
}