using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using doktorChessGameEngine;

namespace tournament
{
    public class tournamentThread
    {
        private Thread _thread;
        public readonly List<contender> _contenders = new List<contender>();
        public readonly List<tournamentGame> gameQueue = new List<tournamentGame>();

        private tournamentGame currentGame = null;

        private static void staticThreadStart(Object instance)
        {
            ((tournamentThread)instance).threadStart();
        }

        private void threadStart()
        {
            lock (gameQueue)
            {
                lock (_contenders)
                {
                    // Make a queue of potential matches!
                    foreach (contender contenderWhite in _contenders)
                    {
                        foreach (contender contenderBlack in _contenders)
                        {
                            // Don't make engines play themselves!
                            if (contenderWhite == contenderBlack)
                                continue;
                            gameQueue.Add(new tournamentGame(contenderWhite, contenderBlack)
                                              {OnGameFinished = gameFinished});
                        }
                    }
                }

                // Now start the first game.
                tournamentGame[] pendingGames = gameQueue.Where(x => !x.isRunning && !x.isFinished).ToArray();
                if (pendingGames.Length > 0)
                {
                    currentGame = pendingGames[0];
                    currentGame.startInNewThread();
                }
            }
        }

        private void gameFinished(tournamentGame recentlyfinished, baseBoard fromWhitesView)
        {
            // Change the score of each player
            if (recentlyfinished.isErrored)
            {
                if (recentlyfinished.white.isErrored)
                    recentlyfinished.white.errorCount += 1;
                if (recentlyfinished.black.isErrored)
                    recentlyfinished.black.errorCount += 1;
            }
            else if (recentlyfinished.isDraw)
            {
                recentlyfinished.white.score += 0.5f;
                recentlyfinished.black.score += 0.5f;

                recentlyfinished.white.draws++;
                recentlyfinished.black.draws++;
            }
            else
            {
                if (recentlyfinished.winningSide == pieceColour.white)
                {
                    recentlyfinished.white.score += 1.0f;

                    recentlyfinished.white.wins++;
                    recentlyfinished.black.losses++;
                }
                else
                {
                    recentlyfinished.black.score += 1.0f;

                    recentlyfinished.black.wins++;
                    recentlyfinished.white.losses++;
                }
            }

            // Make the historic game info
            playedGame gameInfo = new playedGame();
            gameInfo.isDraw = recentlyfinished.isDraw;
            gameInfo.isErrored = recentlyfinished.white.isErrored;
            gameInfo.errorMessage = recentlyfinished.white.errorMessage;
            gameInfo.opponentID = recentlyfinished.black.ID;
            gameInfo.opponentName = recentlyfinished.black.typeName;
            gameInfo.exception = recentlyfinished.white.exception;
            gameInfo.moveList = recentlyfinished.moveList;
            gameInfo.didWin = recentlyfinished.winningSide == pieceColour.white;
            gameInfo.col = pieceColour.white;
            gameInfo.board = fromWhitesView;
            recentlyfinished.white.gamesPlayed.Add(gameInfo);

            playedGame gameInfoBlack = new playedGame();
            gameInfoBlack.isDraw = recentlyfinished.isDraw;
            gameInfoBlack.isErrored = recentlyfinished.black.isErrored;
            gameInfoBlack.errorMessage = recentlyfinished.black.errorMessage;
            gameInfoBlack.exception = recentlyfinished.black.exception;
            gameInfoBlack.opponentID = recentlyfinished.white.ID;
            gameInfoBlack.opponentName = recentlyfinished.white.typeName;
            gameInfoBlack.moveList = recentlyfinished.moveList;
            gameInfoBlack.didWin = recentlyfinished.winningSide == pieceColour.black;
            gameInfoBlack.col = pieceColour.black;
            gameInfoBlack.board = fromWhitesView;
            recentlyfinished.black.gamesPlayed.Add(gameInfoBlack);

            // Start the next!
            lock (gameQueue)
            {
                tournamentGame[] pendingGames = gameQueue.Where(x => !x.isRunning && !x.isFinished).ToArray();
                if (pendingGames.Length > 0)
                {
                    currentGame = pendingGames[0];
                    pendingGames[0].startInNewThread();
                }
                else
                {
                    currentGame = null;
                }
            }
        }

        public void startInNewThread()
        {
            _thread = new Thread(staticThreadStart);
            _thread.Name = "Tournament thread";
            _thread.Start(this);
        }

        public void addContender(contender contender)
        {
            lock (_contenders)
            {
                _contenders.Add(contender);
            }
        }

        public void abort()
        {
            lock (gameQueue)
            {
                if (currentGame != null)
                {
                    currentGame.abort();
                    currentGame = null;
                }
            }
        }
    }

    public class playedGame
    {
        public int opponentID;
        public bool isErrored;
        public bool isDraw;
        public List<move> moveList;
        public string errorMessage;
        public string opponentName;
        public bool didWin;
        public Exception exception;
        public pieceColour col;
        public baseBoard board;
    }
}