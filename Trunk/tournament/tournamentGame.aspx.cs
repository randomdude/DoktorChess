using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web.UI;
using doktorChessGameEngine;
using WebFrontend;

namespace tournament
{
    public class tournamentGame
    {
        public readonly contender white;
        public readonly contender black;
        public bool isFinished = false;
        public bool isRunning = false;
        public bool isErrored { get; private set; }
        public bool isDraw { get; private set; }
        public pieceColour winningSide { get; private set; }
        private Thread _gameThread;
        public readonly List<move> moveList = new List<move>();
        public string boardRepresentation { get; private set; }

        public delegate void gameFinishedDelegate(tournamentGame recentlyFinished);
        public gameFinishedDelegate OnGameFinished;

        public tournamentGame(contender contenderWhite, contender contenderBlack)
        {
            white = contenderWhite;
            black = contenderBlack;
            boardRepresentation = string.Empty;
        }

        public void startInNewThread()
        {
            isRunning = true;

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
            catch (PlayerException e)
            {
                e.culprit.isErrored = true;
                e.culprit.errorMessage = e.Message;
                e.culprit.exception = e.InnerException;
                isErrored = true;
                isDraw = false;
                winningSide = e.culpritCol == pieceColour.white ? pieceColour.black : pieceColour.white;

                isFinished = true;
                isRunning = false;

                gameFinished();
            }
        }

        private void ThreadStartInner()
        {
            pieceColour colToMove = pieceColour.white;

            // Make new players
            baseBoard gameBoardWhite;
            baseBoard gameBoardBlack;

            try
            {
                gameBoardWhite = white.makeNewBoard();
            }
            catch (Exception e)
            {
                throw new PlayerException(white, pieceColour.white, "While attempting to make board: ", e);
            }

            try
            {
                gameBoardBlack = black.makeNewBoard();
            }
            catch (Exception e)
            {
                throw new PlayerException(black, pieceColour.black, "While attempting to make board: ", e);
            }

            while (true)
            {
                // Do some quick sanity checks to ensure that both AIs have consistant views of
                // the situation
                if (gameBoardWhite.colToMove != colToMove)
                    throw new PlayerException(white, pieceColour.white, "Player has incorrect the 'next player' value");

                if (gameBoardBlack.colToMove != colToMove)
                    throw new PlayerException(black, pieceColour.black, "Player has incorrect the 'next player' value");

                // Okay, checks are ok, so lets play a move!
                baseBoard boardToMove = colToMove == pieceColour.white ? gameBoardWhite : gameBoardBlack;
                contender player = colToMove == pieceColour.white ? white : black;

                move bestMove;
                try
                {
                    lineAndScore bestLine = boardToMove.findBestMove();
                    bestMove = bestLine.line[0];
                    moveList.Add(bestMove);
                }
                catch(Exception e)
                {
                    throw new PlayerException(player, colToMove, "During move search: ", e);                    
                }

                // Now play the move on both boards
                foreach(baseBoard thisContener in new [] {gameBoardBlack, gameBoardWhite} )
                {
                    try
                    {
                        thisContener.doMove(bestMove);
                    }
                    catch (Exception e)
                    {
                        contender culprit = thisContener == gameBoardBlack ? black : white;
                        pieceColour culpritCol = thisContener == gameBoardBlack ? pieceColour.black : pieceColour.white;
                        throw new PlayerException(culprit, culpritCol, "While playing move: ", e);
                    }
                }

                pieceColour colJustMoved = colToMove;
                colToMove = colToMove == pieceColour.white ? pieceColour.black : pieceColour.white;

                // Extract a graphical representation of the board so we don't need to query it
                // while the game is running
                TextWriter ourTextWriter2 = new StringWriter();
                HtmlTextWriter ourHtmlWriter2 = new HtmlTextWriter(ourTextWriter2);
                moveHandler.makeTable(gameBoardWhite).RenderControl(ourHtmlWriter2);
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
                    throw new PlayerException(white, pieceColour.white, "While evaluating game", e);
                }

                try
                {
                    statusBlack = gameBoardBlack.getGameStatus(colJustMoved);
                }
                catch (Exception e)
                {
                    throw new PlayerException(black, pieceColour.black, "While evaluating game ", e);
                }

                if (statusBlack != statusWhite)
                {
                    black.errorMessage = "Players disagree on status of game";
                    black.isErrored = true;
                    isErrored = true;
                    break;
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

            gameFinished();
        }

        private void gameFinished()
        {
            if (OnGameFinished != null)
                OnGameFinished.Invoke(this);
        }
    }

    public class PlayerException : Exception
    {
        public readonly contender culprit;
        public readonly pieceColour culpritCol;

        public PlayerException(contender newCulprit, pieceColour responsible)
        {
            culprit = newCulprit;
            culpritCol = responsible;
        }

        public PlayerException(contender newCulprit, pieceColour responsible, string why)
            : base(why)
        {
            culprit = newCulprit;
            culpritCol = responsible;
        }

        public PlayerException(contender newCulprit, pieceColour responsible, string why, Exception e)
            : base(why, e)
        {
            culprit = newCulprit;
            culpritCol = responsible;
        }
    }

}