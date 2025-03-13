using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button[] upgradeButtons;
    private List<UpgradeData> availableUpgrades;

    private void Start()
    {
        HideUpgradePanel();
    }

    /// <summary>
    /// Displays upgrade choices to the player.
    /// </summary>
    public void ShowUpgradePanel()
    {
        availableUpgrades = UpgradeManager.Instance.GetAvailableUpgrades();
        upgradePanel.SetActive(true);

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (i < availableUpgrades.Count)
            {
                upgradeButtons[i].gameObject.SetActive(true);
                upgradeButtons[i].GetComponentInChildren<Text>().text = availableUpgrades[i].upgradeName;
                int index = i;
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(availableUpgrades[index]));
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Hides the upgrade panel.
    /// </summary>
    public void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
    }

    /// <summary>
    /// Applies the selected upgrade and updates the UI.
    /// </summary>
    private void ApplyUpgrade(UpgradeData upgrade)
    {
        Unit playerUnit = FindObjectOfType<Unit>();
        if (playerUnit != null)
        {
            UpgradeManager.Instance.ApplyUpgrade(upgrade, playerUnit);
        }
        HideUpgradePanel();
    }
}
