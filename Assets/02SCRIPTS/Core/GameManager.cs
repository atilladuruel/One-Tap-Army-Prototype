using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Player;

namespace Game.Core
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        internal bool isGameOver;
        public bool IsGamePaused { get; private set; }

        private float gameDuration = 300f; // 5 minutes (300 seconds)
        private float remainingTime;

        public List<Castle> castles; // List of all castles in the game

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                // Ensure GameManager is a root object before making it persistent
                if (transform.parent != null)
                {
                    transform.SetParent(null);
                }

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject); // Prevent duplicate GameManager instances
            }
        }

        private void Start()
        {
            remainingTime = gameDuration;
            StartCoroutine(GameTimer());
        }

        private IEnumerator GameTimer()
        {
            while (remainingTime > 0 && !isGameOver)
            {
                yield return new WaitForSeconds(1f);
                remainingTime--;

                CheckGameOver();
            }

            if (!isGameOver)
            {
                EndGameByTime();
            }
        }

        public void CheckGameOver()
        {
            // Get alive castles
            List<Castle> aliveCastles = castles.Where(c => c.IsAlive()).ToList();

            if (aliveCastles.Count == 1) // If only one castle remains, declare winner
            {
                isGameOver = true;
                EndGame(aliveCastles[0]);
            }
        }

        private void EndGameByTime()
        {
            if (isGameOver) return;


            isGameOver = true;
            Castle winner = castles.OrderByDescending(c => c.GetHealth()).First();
            EndGame(winner);
        }

        private void EndGame(Castle winner)
        {
            isGameOver = true;
            Debug.Log($"Game Over! The winner is: {winner.ownerName} with {winner.GetHealth()} HP remaining.");


        }

        public void PauseGame()
        {
            IsGamePaused = true;
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            IsGamePaused = false;
            Time.timeScale = 1f;
        }
    }
}
