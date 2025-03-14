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
                _instance = FindObjectOfType<ObjectPooler>();
            }
            return _instance;
        }
    }

    public List<GameObject> unitPrefab;
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

        for (int j = 0; j < unitPrefab.Count; j++)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject unit = Instantiate(unitPrefab[j]);
                unit.SetActive(false);
                unitPool.Enqueue(unit);
            }
        }
    }

    public GameObject GetUnit()
    {
        if (unitPool.Count > 0)
        {
            GameObject unit = unitPool.Dequeue();
            unit.SetActive(true);
            return unit;
        }
        else
        {
            return Instantiate(unitPrefab[0]);
        }
    }

    public void ReturnUnit(GameObject unit)
    {
        unit.SetActive(false);
        unitPool.Enqueue(unit);
    }
}