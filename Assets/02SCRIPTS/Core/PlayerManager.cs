using System.Collections.Generic;
using UnityEngine;
using Game.Player;
using Game.Units;

namespace Game.Core
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance;

        [System.Serializable]
        public class PlayerData
        {
            public string Name;
            public int playerID;
            public Color teamColor;
            public Castle castle;
            public List<Unit> units = new List<Unit>();
        }

        public List<PlayerData> players = new List<PlayerData>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        /// <summary>
        /// Get player by ID.
        /// </summary>
        public PlayerData GetPlayerByID(int id)
        {
            return players.Find(player => player.playerID == id);
        }

        /// <summary>
        /// Get a random enemy for the given player.
        /// </summary>
        public PlayerData GetEnemyForPlayer(int id)
        {
            List<PlayerData> enemies = players.FindAll(player => player.playerID != id);
            return enemies.Count > 0 ? enemies[Random.Range(0, enemies.Count)] : null;
        }
    }
}
