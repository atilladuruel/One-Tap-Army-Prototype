using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Text healthText;
    [SerializeField] private Text xpText;
    [SerializeField] private Text levelText;
    [SerializeField] private GameObject upgradeNotification;
    private Unit playerUnit;

    private void Start()
    {
        playerUnit = FindObjectOfType<Unit>();
        UpdateHUD();

        if (playerUnit != null)
        {
            playerUnit.OnLevelUp += UpdateHUD; // Level up event listener
        }
    }


    /// <summary>
    /// Updates the UI elements based on the player's unit stats.
    /// </summary>
    private void UpdateHUD()
    {
        if (playerUnit == null) return;

        healthText.text = $"Health: {playerUnit.Health}";
        levelText.text = $"Level: {playerUnit.GetLevel()}"; // Assuming GetLevel() is implemented
    }

    /// <summary>
    /// Displays an upgrade notification when an upgrade is available.
    /// </summary>
    public void ShowUpgradeNotification(bool show)
    {
        upgradeNotification.SetActive(show);
    }
}
