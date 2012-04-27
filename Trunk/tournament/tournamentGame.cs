using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;
using System.Web.UI;
using doktorChessGameEngine;
using WebFrontend;

namespace tournament
{
    [Serializable]
    public class tournamentGame
    {
        public readonly contender white;
        public readonly contender black;
        public bool isFinished = false;
        public bool isRunning = false;

        /// <summary>
        /// Was teh game aborted early due to an error?
        /// </summary>
        public bool isErrored { get; private set; }

        /// <summary>
        /// Did the game result in a draw?
        /// </summary>
        public bool isDraw { get; private set; }

        /// <summary>
        /// Which side won the game? Undefined if isErrored is false or isDraw is true.
        /// </summary>
        public pieceColour winningSide { get; private set; }

        /// <summary>
        /// Which side caused an error? Undefined if isErrored is false.
        /// </summary>
        public pieceColour erroredSide { get; private set; }

        public pieceColour colToMove { get; private set; }
        private Thread _gameThread;
        public readonly List<move> moveList = new List<move>();
        public string boardRepresentation { get; private set; }
        private baseBoard gameBoardWhite;
        private baseBoard gameBoardBlack;

        public readonly int id;

        private static int _nextID = 0;
        private static readonly object _nextIDLock = new object();

        AppDomain appDomainWhite;
        AppDomain appDomainBlack;

        public delegate void gameFinishedDelegate(tournamentGame recentlyFinished, baseBoard fromWhitesView);
        public gameFinishedDelegate OnGameFinished;

        public tournamentGame(contender contenderWhite, contender contenderBlack)
        {
            white = contenderWhite;
            black = contenderBlack;
            boardRepresentation = string.Empty;

            lock (_nextIDLock)
            {
                id = _nextID;
                _nextID++;
            }
        }

        public void startInNewThread()
        {
            _gameThread = new Thread(staticThreadStart);
            _gameThread.Name = "Tournament thread: " + white.typeName + " vs " + black.typeName;
            _gameThread.Priority = ThreadPriority.BelowNormal;
            _gameThread.Start(this);
        }

        private static void staticThreadStart(object game)
        {
            ((tournamentGame)game).ThreadStart();
        }

        private void ThreadStart()
        {
            try
            {
                ThreadStartInner();
            }
            catch (playerException e)
            {
                e.culprit.isErrored = true;
                e.culprit.errorMessage = e.Message;
                e.culprit.exception = e.InnerException;
                isErrored = true;
                isDraw = false;
                winningSide = e.culpritCol == pieceColour.white ? pieceColour.black : pieceColour.white;
                erroredSide = e.culpritCol;

                isFinished = true;
                isRunning = false;

                gameFinished(gameBoardWhite);
            }
        }

        private void ThreadStartInner()
        {
            gameThread();
        }

        public static AppDomain createAppDomain(string path, string appDomainName)
        {
            AppDomainSetup setup = new AppDomainSetup
                                       {
                                           ApplicationBase = path
                                       };

            PermissionSet permissions = new PermissionSet(null);
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
//            permissions.AddPermission(new FileIOPermission(PermissionState.Unrestricted));

            return AppDomain.CreateDomain(appDomainName, null, setup, permissions);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        private void gameThread()
        {
            // Make a new AppDomain for each player.
            appDomainWhite = createAppDomain(white.assemblyPath, "Tournament game AppDomain (white player)");
            appDomainBlack = createAppDomain(black.assemblyPath, "Tournament game AppDomain (black player)");

            colToMove = pieceColour.white;

            // Make new players
            try
            {
                gameBoardWhite = white.makeNewBoard(appDomainWhite);
            }
            catch (Exception e)
            {
                throw new playerException(white, pieceColour.white, "While attempting to make board: ", e);
            }

            try
            {
                gameBoardBlack = black.makeNewBoard(appDomainBlack);
            }
            catch (Exception e)
            {
                throw new playerException(black, pieceColour.black, "While attempting to make board: ", e);
            }

            // Fill in our board HTML
            TextWriter ourTextWriter = new StringWriter();
            HtmlTextWriter ourHtmlWriter = new HtmlTextWriter(ourTextWriter);
            utils.makeTableAndEscapeContents(gameBoardWhite).RenderControl(ourHtmlWriter);
            boardRepresentation = ourTextWriter.ToString();

            // Initialise player times (ms)
            white.timeLeft = black.timeLeft = 5*60*1000;
            isRunning = true;

            while (true)
            {
                // Do some quick sanity checks to ensure that both AIs have consistant views of
                // the situation
                if (gameBoardWhite.colToMove != colToMove)
                    throw new playerException(white, pieceColour.white, "Player has incorrect the 'next player' value");

                if (gameBoardBlack.colToMove != colToMove)
                    throw new playerException(black, pieceColour.black, "Player has incorrect the 'next player' value");

                // Okay, checks are ok, so lets play a move!
                baseBoard boardToMove = colToMove == pieceColour.white ? gameBoardWhite : gameBoardBlack;
                contender player = colToMove == pieceColour.white ? white : black;

                int timeLeft = colToMove == pieceColour.white ? white.timeLeft : black.timeLeft;

                move bestMove;
                try
                {
                    moveWithTimeout mwo = new moveWithTimeout();
                    boardToMove.timeLeftMS = timeLeft;
                    lineAndScore bestLine = mwo.findBestMoveWithTimeout(boardToMove, timeLeft);
                    bestMove = bestLine.line[0];
                    moveList.Add(bestMove);
                }
                catch (Exception e)
                {
                    throw new playerException(player, colToMove, "During move search: ", e);
                }

                // Now play the move on both boards
                foreach (baseBoard thisContener in new[] { gameBoardBlack, gameBoardWhite })
                {
                    try
                    {
                        thisContener.doMove(bestMove);
                    }
                    catch (Exception e)
                    {
                        contender culprit = thisContener == gameBoardBlack ? black : white;
                        pieceColour culpritCol = thisContener == gameBoardBlack ? pieceColour.black : pieceColour.white;
                        throw new playerException(culprit, culpritCol, "While playing move as " + culpritCol + ": ", e);
                    }
                }

                pieceColour colJustMoved = colToMove;
                colToMove = colToMove == pieceColour.white ? pieceColour.black : pieceColour.white;

                // Extract a graphical representation of the board so we don't need to query it
                // while the game is running
                TextWriter ourTextWriter2 = new StringWriter();
                HtmlTextWriter ourHtmlWriter2 = new HtmlTextWriter(ourTextWriter2);
                utils.makeTableAndEscapeContents(gameBoardWhite).RenderControl(ourHtmlWriter2);
                boardRepresentation = ourTextWriter2.ToString();

                // Check that game is still in progrss
                gameStatus statusWhite;
                gameStatus statusBlack;

                try
                {
                    statusWhite = gameBoardWhite.getGameStatus(colJustMoved);
                }
                catch (Exception e)
                {
                    throw new playerException(white, pieceColour.white, "While evaluating game", e);
                }

                try
                {
                    statusBlack = gameBoardBlack.getGameStatus(colJustMoved);
                }
                catch (Exception e)
                {
                    throw new playerException(black, pieceColour.black, "While evaluating game ", e);
                }

                if (statusBlack != statusWhite)
                {
                    // This could be white or black's fault - we can't tell for sure here.
                    throw new playerException(white, pieceColour.white, "While evaluating game ", new Exception("White and Black disagree on game status"));
                }

                if (statusWhite != gameStatus.inProgress)
                {
                    // OK, the game is over!
                    if (statusWhite == gameStatus.drawn)
                    {
                        isDraw = true;
                    }
                    else
                    {
                        isDraw = false;
                        winningSide = colJustMoved;
                    }
                    break;
                }
            }

            isFinished = true;
            isRunning = false;

            gameFinished(gameBoardWhite);
        }

        private void gameFinished(baseBoard board)
        {
            if (OnGameFinished != null)
                OnGameFinished.Invoke(this, board);
        }

        public void abort()
        {
            try
            {
                if (appDomainWhite != null)
                 AppDomain.Unload(appDomainWhite);
            }
            catch (Exception e1)
            {
                e1 = null;
            }

            try
            {
                if (appDomainBlack != null)
                    AppDomain.Unload(appDomainBlack);
            }
            catch (Exception e)
            {
                e = null;
            }

            try
            {
                // HEERURTTT! 
                // We shouldn't Thread.Abort, in an ideal world, but I think it is inevitable for now. 
                _gameThread.Abort();
            }
            catch (Exception)
            {
            }
        }

    }
}