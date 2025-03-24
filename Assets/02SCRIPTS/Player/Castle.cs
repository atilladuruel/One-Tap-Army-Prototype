using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.Core;

namespace Game.Units
{
    public struct CastleData
    {
        public int health;
        public int maxHealth;
        public bool isDestroyed;
    }

    /// <summary>
    /// Represents a player's or AI's castle.
    /// </summary>
    public class Castle : MonoBehaviour,IDamageable
    {
        [SerializeField] private Transform _spawnPoint;
        public int playerID;
        public string ownerName; // Player or AI name
        public List<Renderer> castleRenderer;
        public Transform spawnPoint => _spawnPoint;
        public int level = 1;
        private int health = 3200;
        private int maxHealth = 3200;
        private bool isDestroyed = false; // New flag to prevent multiple destructions

        private void Start()
        {
            ApplyTeamColor();
        }

        private void ApplyTeamColor()
        {
            Color teamColor = PlayerManager.Instance.GetPlayerByID(playerID).teamColor;
            foreach (Renderer r in castleRenderer)
            {
                r.material.color = teamColor;
            }
        }

        /// <summary>
        /// Checks if the castle is still alive.
        /// </summary>
        public bool IsAlive()
        {
            return health > 0;
        }

        /// <summary>
        /// Returns the current health of the castle.
        /// </summary>
        public int GetHealth()
        {
            return health;
        }

        /// <summary>
        /// Called when the castle takes damage.
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (isDestroyed) return; // Prevents taking damage after destruction

            health -= damage;
            Debug.Log($"🏰 {ownerName}'s Castle took {damage} damage! Remaining health: {health}");

            if (health <= 0)
            {
                health = 0;
                DestroyCastle();
            }
        }

        /// <summary>
        /// Levels up the castle, increasing max health.
        /// </summary>
        public void LevelUp()
        {
            level++;
            maxHealth += 500;
            health = maxHealth;
            Debug.Log($"{ownerName}'s Castle leveled up to: {level}");
        }

        /// <summary>
        /// Handles castle destruction and notifies the GameManager.
        /// </summary>
        private void DestroyCastle()
        {
            if (isDestroyed) return; // Prevents multiple calls

            isDestroyed = true;
            Debug.Log($"🔥 {ownerName}'s Castle has been destroyed!");

            gameObject.SetActive(false); // Hides the castle
            Game.Core.GameManager.Instance.CheckGameOver(); // Notifies GameManager
        }
    }
}
