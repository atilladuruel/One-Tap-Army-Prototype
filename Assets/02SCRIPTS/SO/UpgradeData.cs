using UnityEngine;

public enum UpgradeType { Health, Speed, Attack }

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public UpgradeType upgradeType;
    public int cost;
    public int effectValue;
}
