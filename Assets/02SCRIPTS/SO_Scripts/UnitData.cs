using UnityEngine;
using System.Collections.Generic;
using Game.Units;

namespace Game.Scriptables
{
    public enum Stats { Health, Speed, Attack }

    [CreateAssetMenu(fileName = "NewUnitData", menuName = "Unit/UnitData")]
    public class UnitData : ScriptableObject
    {
        public string unitName;
        public UnitType unitType;
        public List<Stats> upgradeType;
        public int health;
        public float speed;
        public int attack;
        public float attackRange = 0.5f; // Melee attack range
        public float rangedAttackRange = 1.5f; // Archer's ranged attack range
        public float awarenessRange = 2f; // Unit awareness range
        public float attackCooldown = 0.5f;
        public float spawnTime;
        public float spawnTimeDecrease;
    }
}
