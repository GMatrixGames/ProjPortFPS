using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utils
{
    public class LeaderboardStorage
    {
        private static LeaderboardStorage _instance;
        private static readonly object _lock = new();

        public static LeaderboardStorage instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new LeaderboardStorage();
                }
            }
        }

        public List<string> leaderboard = new();
        private readonly string settingsPath;

        private LeaderboardStorage()
        {
            settingsPath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
            Load(); // Load settings on instantiation
        }

        public void Save()
        {
            var json = JsonUtility.ToJson(leaderboard, true);
            File.WriteAllText(settingsPath, json);
        }

        public void Load()
        {
            if (File.Exists(settingsPath))
            {
                var json = File.ReadAllText(settingsPath);
                leaderboard = JsonUtility.FromJson<List<string>>(json);
            }
            else
            {
                Save(); // Save default settings for next start
            }
        }
    }
}