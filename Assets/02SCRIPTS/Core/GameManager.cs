using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<PlayerController> players;
    public Transform[] spawnPoints;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Oyuncular� olu�tur
        InitializePlayers();
    }

    void InitializePlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].Initialize(spawnPoints[i]);
        }
    }
}
