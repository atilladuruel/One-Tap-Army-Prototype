using UnityEngine;

public class Castle
{
    [SerializeField] private Transform _spawnPoint; //Unitlerin kaleden ��kt�ktan sonra toplanma noktas�
    public int level = 1;                          // Kale seviyesi
    private int health = 3000;                      // Kaleye ait can miktar�
    private int maxHealth = 3000;                   // Maksimum can miktar�
    private float xp = 0;                           // Kalenin kazand��� deneyim puan�
    private float xpThreshold = 100;                // Seviye atlamak i�in gereken XP

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
        maxHealth += 500;  // Seviye ba��na ekstra can
        health = maxHealth;
        Debug.Log("Castle leveled up to: " + level);
    }

    public void DestroyCastle()
    {
        Debug.Log("Castle has been destroyed!");
        // Burada oyunu bitirme veya di�er oyuncuya zafer kazand�rma i�lemleri eklenebilir.
    }
}
