using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using doktorChessGameEngine;

namespace tournament
{
    public static class runningTournament
    {
        public static tournamentThread _tournament = null;
        public static object _tournamentLock = new object();

        public static List<assemblyNameAndType> _tournamentPlayerTypes = new List<assemblyNameAndType>(50);

        private static List<assemblyNameAndType> housePlayers;

        static runningTournament()
        {
            Assembly housePlayersAsm = Assembly.GetAssembly(typeof(exampleMiniMax.exampleMiniMaxBoard));
    
            // Copy the assembly housing the house players (lol) in to a new temp folder
            string path = Path.GetRandomFileName();
            string assemblyFolder = Path.Combine(Path.GetTempPath(), path);
            Directory.CreateDirectory(assemblyFolder);
            string asmName = Path.GetFileName(housePlayersAsm.Location);
            string assemblyPath = Path.Combine(assemblyFolder, asmName);
            File.Copy(housePlayersAsm.Location, assemblyPath);

            housePlayers = scoreboard.processFile(assemblyPath, "House");

            foreach (assemblyNameAndType housePlayer in housePlayers)
            {
                housePlayer.isHousePlayer = true;
            }
        }

        public static void startNewTournament()
        {
            if (_tournament != null)
                _tournament.abort();

            _tournament = new tournamentThread();

            foreach (assemblyNameAndType playerType in _tournamentPlayerTypes)
            {
                _tournament.addContender(new contender(playerType));
            }

            _tournament.startInNewThread();
        }

        public static void endAndStartWithNewPlayers(List<assemblyNameAndType> newPlayerTypes)
        {
            // We have loaded one or more players. We should stop the current tournament and start a new one
            // including this player.
            lock (_tournamentLock)
            {
                // End current tournament
                if (_tournament != null)
                {
                    _tournament.abort();
                }
                else
                {
                    // If starting a new one, add house players.
                    addHousePlayers();
                }

                // Add our new Type
                foreach (assemblyNameAndType newPlayerType in newPlayerTypes)
                {
                    _tournamentPlayerTypes.Add(newPlayerType);
                }

                // And start a new tournament.
                startNewTournament();
            }

        }

        public static void clearPlayers()
        {
            lock (_tournamentLock)
            {
                if (_tournament != null)
                    _tournament.abort();

                _tournamentPlayerTypes.Clear();

                // Add any 'house' players
                addHousePlayers();

                startNewTournament();
            }
        }

        private static void addHousePlayers()
        {
            foreach (assemblyNameAndType housePlayer in housePlayers)
            {
                _tournamentPlayerTypes.Add(housePlayer);
            }
        }

        public static void restart()
        {
            lock (_tournamentLock)
            {
                // End current tournament
                if (_tournament != null)
                    _tournament.abort();

                // And start a new tournament.
                startNewTournament();
            }
            
        }

        public static int getPlayerCount()
        {
            lock (_tournamentLock)
            {
                return _tournamentPlayerTypes.Count;
            }
        }

        public static void clearPlayers(string playerName)
        {
            lock (_tournamentLock)
            {
                if (_tournament != null)
                    _tournament.abort();

                _tournamentPlayerTypes.RemoveAll(x => x.ownerName == playerName);

                // Add any 'house' players
                addHousePlayers();

                startNewTournament();
            }
        }
    }
}