using UnityEngine;

public class Castle
{
    [SerializeField] private Transform _spawnPoint; //Unitlerin kaleden çýktýktan sonra toplanma noktasý
    public int level = 1;                          // Kale seviyesi
    private int health = 3000;                      // Kaleye ait can miktarý
    private int maxHealth = 3000;                   // Maksimum can miktarý
    private float xp = 0;                           // Kalenin kazandýðý deneyim puaný
    private float xpThreshold = 100;                // Seviye atlamak için gereken XP

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            DestroyCastle();
        }
    }

    public void GainXP(float amount)
    {
        xp += amount;
        if (xp >= xpThreshold)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        xp = 0;
        level++;
        maxHealth += 500;  // Seviye baþýna ekstra can
        health = maxHealth;
        Debug.Log("Castle leveled up to: " + level);
    }

    public void DestroyCastle()
    {
        Debug.Log("Castle has been destroyed!");
        // Burada oyunu bitirme veya diðer oyuncuya zafer kazandýrma iþlemleri eklenebilir.
    }
}
