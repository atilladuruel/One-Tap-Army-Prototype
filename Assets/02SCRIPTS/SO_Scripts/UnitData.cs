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
        public float spawnTime;
        public float spawnTimeDecrease;
    }
}
