using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action<int> OnPlayerLevelUp;

    public static void PlayerLevelUp(int playerID)
    {
        OnPlayerLevelUp?.Invoke(playerID);
    }
}
