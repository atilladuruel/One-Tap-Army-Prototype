using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private Dictionary<UnitType, Queue<Unit>> unitPool = new Dictionary<UnitType, Queue<Unit>>();
    public List<Unit> unitPrefabs;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var unitPrefab in unitPrefabs)
        {
            unitPool[unitPrefab.unitType] = new Queue<Unit>();
        }
    }

    public Unit GetPooledObject(UnitType type)
    {
        if (unitPool[type].Count > 0)
        {
            Unit unit = unitPool[type].Dequeue();
            unit.gameObject.SetActive(true);
            return unit;
        }
        else
        {
            Unit newUnit = Instantiate(unitPrefabs.Find(u => u.unitType == type));
            return newUnit;
        }
    }

    public void ReturnToPool(Unit unit)
    {
        unit.gameObject.SetActive(false);
        unitPool[unit.unitType].Enqueue(unit);
    }
}
