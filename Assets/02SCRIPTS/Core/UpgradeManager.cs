using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [SerializeField] private List<UnitData> availableUpgrades;

    public event Action OnUpgradeApplied;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ApplyUpgrade(UnitData upgrade, Unit unit)
    {
        if (unit == null || upgrade == null) return;

        //switch (upgrade.upgradeType)
        //{
        //    case Stats.Health:
        //        unit.health += upgrade.effectValue;
        //        break;
        //    case Stats.Speed:
        //        unit.speed += upgrade.effectValue;
        //        unit.agent.speed = unit.speed; // Apply new speed to NavMeshAgent
        //        break;
        //    case Stats.Attack:
        //        unit.attackPower += upgrade.effectValue;
        //        break;
        //}

        OnUpgradeApplied?.Invoke(); // Notify UI or other systems
    }

    public List<UnitData> GetAvailableUpgrades()
    {
        return availableUpgrades;
    }
}
