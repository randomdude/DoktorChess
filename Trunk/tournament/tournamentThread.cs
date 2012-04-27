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
        public readonly List<contender> contenders = new List<contender>();
        public readonly List<tournamentGame> gameQueue = new List<tournamentGame>();

        private tournamentGame _currentGame = null;

        private static void staticThreadStart(Object instance)
        {
            ((tournamentThread)instance).threadStart();
        }

        private void threadStart()
        {
            lock (gameQueue)
            {
                lock (contenders)
                {
                    // Make a queue of potential matches!
                    foreach (contender contenderWhite in contenders)
                    {
                        foreach (contender contenderBlack in contenders)
                        {
                            // Don't make AIs play themselves
                            if (contenderWhite == contenderBlack)
                                continue;
                            // And don't let two house players play.
                            if (contenderWhite.isHousePlayer && contenderBlack.isHousePlayer)
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
                    _currentGame = pendingGames[0];
                    _currentGame.startInNewThread();
                }
            }
        }

        private void gameFinished(tournamentGame recentlyfinished, baseBoard fromWhitesView)
        {
            // Change the score of each player
            if (recentlyfinished.isErrored)
            {
                if (recentlyfinished.white.isErrored)
                {
                    recentlyfinished.white.errorCount += 1;

                    recentlyfinished.black.score += 1.0f;
                    recentlyfinished.black.wins++;
                }
                if (recentlyfinished.black.isErrored)
                {
                    recentlyfinished.black.errorCount += 1;

                    recentlyfinished.white.score += 1.0f;
                    recentlyfinished.white.wins++;
                }
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
            gameInfo.erroredSide = recentlyfinished.erroredSide;
            gameInfo.errorMessage = recentlyfinished.white.errorMessage;
            gameInfo.opponentID = recentlyfinished.black.ID;
            gameInfo.opponentTypeName = recentlyfinished.black.typeName;
            gameInfo.exception = recentlyfinished.white.exception;
            gameInfo.moveList = recentlyfinished.moveList;
            gameInfo.didWin = recentlyfinished.winningSide == pieceColour.white;
            gameInfo.col = pieceColour.white;
            gameInfo.board = fromWhitesView;
            recentlyfinished.white.gamesPlayed.Add(gameInfo);

            playedGame gameInfoBlack = new playedGame();
            gameInfoBlack.isDraw = recentlyfinished.isDraw;
            gameInfoBlack.isErrored = recentlyfinished.black.isErrored;
            gameInfoBlack.erroredSide = recentlyfinished.erroredSide;
            gameInfoBlack.errorMessage = recentlyfinished.black.errorMessage;
            gameInfoBlack.exception = recentlyfinished.black.exception;
            gameInfoBlack.opponentID = recentlyfinished.white.ID;
            gameInfoBlack.opponentTypeName = recentlyfinished.white.typeName;
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
                    _currentGame = pendingGames[0];
                    pendingGames[0].startInNewThread();
                }
                else
                {
                    _currentGame = null;
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
            lock (contenders)
            {
                contenders.Add(contender);
            }
        }

        public void abort()
        {
            lock (gameQueue)
            {
                if (_currentGame != null)
                {
                    _currentGame.abort();
                    _currentGame = null;
                }
            }
        }
    }
}