using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using doktorChessGameEngine;

namespace tournament
{
    [Serializable]
    public class contender
    {
        private readonly Type boardType;

        public string typeName
        {
            get { return boardType.Name; }
        }

        /// <summary>
        /// The name of the contianing assembly
        /// </summary>
        public readonly string assemblyName;

        /// <summary>
        /// The directory path containing the assembly
        /// </summary>
        public readonly string assemblyPath;

        public bool isErrored = false;
        public string errorMessage = String.Empty;
        public float score = 0;
        public int wins;
        public int draws;
        public int losses;
        public int errorCount = 0;
        public readonly int ID;
        private static int _nextID = 0;
        private readonly object  nextIDLock = new object();
        public readonly List<playedGame> gamesPlayed = new List<playedGame>();
        public Exception exception;
        public int timeLeft;

        public contender(assemblyNameAndType AssAndType)
        {
            boardType = AssAndType.theType;
            assemblyName = AssAndType.AssemblyName;
            assemblyPath = AssAndType.AssemblyFolder;
            lock (nextIDLock)
            {
                ID = _nextID;
                _nextID++;
            }
        }

        public baseBoard makeNewBoard(AppDomain appDomain)
        {
            // Args to pass to the constructor of the board
            object[] cstrArgs = new object[] {gameType.normal};

            // Create the new board instance!
            AssemblyName asmName = AssemblyName.GetAssemblyName(Path.Combine(assemblyPath, assemblyName));
            asmName.CodeBase = null;
            baseBoard toRet = (baseBoard)appDomain.CreateInstanceAndUnwrap(asmName.FullName, boardType.FullName, false,
                                                                        BindingFlags.CreateInstance, null,
                                                                        cstrArgs, null, new object[0], null );

            if (!RemotingServices.IsTransparentProxy(toRet))
                throw new Exception("The unwrapped object is not a proxy!");

            toRet.makeStartPosition();

            isErrored = false;
            errorMessage = String.Empty;
                
            return toRet;
        }
    }

}