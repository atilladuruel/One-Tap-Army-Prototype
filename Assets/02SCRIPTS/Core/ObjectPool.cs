using System.Collections.Generic;
using UnityEngine;

// Object Pooler for Units
public class ObjectPooler : MonoBehaviour
{
    private static ObjectPooler _instance;
    public static ObjectPooler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<ObjectPooler>();
            }
            return _instance;
        }
    }

    public List<GameObject> unitPrefabs;
    public int poolSize = 10;
    private Queue<GameObject> unitPool = new Queue<GameObject>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        for (int j = 0; j < unitPrefabs.Count; j++)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject unit = Instantiate(unitPrefabs[j]);
                unit.SetActive(false);
                unitPool.Enqueue(unit);
            }
        }
    }

    public GameObject GetUnit(UnitType unitType)
    {
        if (unitPool.Count > 0)
        {
            GameObject unit = unitPool.Dequeue();
            unit.SetActive(true);
            return unit;
        }
        else
        {
            return Instantiate(unitPrefabs[0]);
        }
    }

    public void ReturnUnit(GameObject unit)
    {
        unit.SetActive(false);
        unitPool.Enqueue(unit);
    }
}