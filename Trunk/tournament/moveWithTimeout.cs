using System;
using System.Threading;
using doktorChessGameEngine;

namespace tournament
{
    public class moveWithTimeout
    {
        private baseBoard findBestMoveBoard;
        private lineAndScore bestLine;
        private AutoResetEvent evnt;
        private Exception ex;

        public lineAndScore findBestMoveWithTimeout(baseBoard boardToMove, int timeout)
        {
            findBestMoveBoard = boardToMove;
            Thread findThread = new Thread(findBestMoveWithTimeoutThread);
            evnt = new AutoResetEvent(false);
            evnt.Reset();
            findThread.Start();
            if (evnt.WaitOne(timeout, false))
            {
                if (ex != null)
                    throw ex;
                return bestLine;
            }

            findThread.Abort();

            throw new Exception("Player timeout");
        }

        private void findBestMoveWithTimeoutThread()
        {
            try
            {
                bestLine = findBestMoveBoard.findBestMove();
            }
            catch (Exception newEx)
            {
                ex = newEx;
                evnt.Set();

                return;
            }

            evnt.Set();
        }
    }
}