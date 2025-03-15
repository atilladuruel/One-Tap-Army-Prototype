using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Castle : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    public int playerID;
    public string ownerName; // ✅ Oyuncu veya AI ismi
    public List<Renderer> castleRenderer;
    public Transform spawnPoint => _spawnPoint;
    public int level = 1;
    private int health = 3200;
    private int maxHealth = 3200;

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
    /// Kalenin hayatta olup olmadığını kontrol eder.
    /// </summary>
    public bool IsAlive()
    {
        return health > 0;
    }

    /// <summary>
    /// Kalenin mevcut can değerini döndürür.
    /// </summary>
    public int GetHealth()
    {
        return health;
    }

    /// <summary>
    /// Hasar alındığında çağrılır, kale yıkılırsa GameManager'a haber verir.
    /// </summary>
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            DestroyCastle();
        }
    }

    /// <summary>
    /// Kalenin seviyesini yükseltir.
    /// </summary>
    public void LevelUp()
    {
        level++;
        maxHealth += 500;  // Seviye başına ekstra can
        health = maxHealth; // Yeni seviyede canı doldur
        Debug.Log($"{ownerName}'s Castle leveled up to: {level}");
    }

    /// <summary>
    /// Kale yıkıldığında çağrılır, GameManager'a haber verir.
    /// </summary>
    private void DestroyCastle()
    {
        Debug.Log($"{ownerName}'s Castle has been destroyed!");

        gameObject.SetActive(false); // Kalenin yok olduğunu göster
        GameManager.Instance.CheckGameOver(); // GameManager'a bildir
    }
}
