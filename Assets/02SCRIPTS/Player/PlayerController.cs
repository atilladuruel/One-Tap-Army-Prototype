using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerID;
    public Castle castle;
    public List<Unit> activeUnits = new List<Unit>();
    public PlayerLevel playerLevel;

    private void Awake()
    {
        playerLevel = new PlayerLevel(this);
    }

    public void Initialize(Transform spawnPosition)
    {
        transform.position = spawnPosition.position;
    }

    public void SpawnUnit(UnitType unitType)
    {
        //Unit newUnit = ObjectPool.Instance.GetPooledObject(unitType);
        //newUnit.Initialize(this, castle._spawnPoint.position);
        //activeUnits.Add(newUnit);
    }
}


