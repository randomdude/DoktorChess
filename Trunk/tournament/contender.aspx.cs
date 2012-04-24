using System;
using System.Collections.Generic;
using System.Reflection;
using doktorChessGameEngine;

namespace tournament
{
    public class contender
    {
        private readonly Type boardType;

        public string typeName
        {
            get { return boardType.Name; }
        }

        public bool isErrored = false;
        public string errorMessage = String.Empty;
        public float score = 0;
        public int wins;
        public int draws;
        public int losses;
        public int errorCount = 0;
        public readonly int ID;
        private static int nextID = 0;
        private object  nextIDLock = new object();
        public List<playedGame> gamesPlayed = new List<playedGame>();
        public Exception exception;

        public contender(Type contenderBoardType)
        {
            boardType = contenderBoardType;
            lock (nextIDLock)
            {
                ID = nextID;
                nextID++;
            }
        }

        public baseBoard makeNewBoard()
        {
            ConstructorInfo cstr = boardType.GetConstructor( new Type[] { typeof(gameType) } );
            baseBoard toRet = (baseBoard) cstr.Invoke(new object[] {gameType.normal});

            toRet.makeStartPosition();

            isErrored = false;
            errorMessage = String.Empty;
                
            return toRet;
        }
    }
}