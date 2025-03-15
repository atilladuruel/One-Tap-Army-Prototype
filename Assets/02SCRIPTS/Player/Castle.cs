using UnityEngine;

public class Castle : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    public Transform spawnPoint => _spawnPoint;
    public int level = 1;
    private int health = 3200;
    private int maxHealth = 3200;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            DestroyCastle();
        }
    }

    public void LevelUp()
    {
        level++;
        maxHealth += 500;  // Seviye baþýna ekstra can
        //health = maxHealth;
        Debug.Log("Castle leveled up to: " + level);
    }

    public void DestroyCastle()
    {
        Debug.Log("Castle has been destroyed!");
        // Burada oyunu bitirme veya diðer oyuncuya zafer kazandýrma iþlemleri eklenebilir.
    }
}
