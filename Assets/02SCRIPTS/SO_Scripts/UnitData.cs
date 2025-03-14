using UnityEngine;
using System.Collections.Generic;

public enum Stats { Health, Speed, Attack }

[CreateAssetMenu(fileName = "NewUnit", menuName = "Unit/UnitData")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public UnitType unitType;
    public List<Stats> upgradeType;
    public int health;
    public int speed;
    public int attack;
}
