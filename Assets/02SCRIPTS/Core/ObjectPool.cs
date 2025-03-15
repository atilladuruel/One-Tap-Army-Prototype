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

    public List<GameObject> unitPrefabs; // All unit prefabs
    public int poolSize = 10;

    private Dictionary<UnitType, Queue<GameObject>> unitPools = new Dictionary<UnitType, Queue<GameObject>>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        InitializePools();
    }

    /// <summary>
    /// Initializes a separate queue for each unit type and organizes them in child objects.
    /// </summary>
    private void InitializePools()
    {
        foreach (var prefab in unitPrefabs)
        {
            Unit unitComponent = prefab.GetComponent<Unit>();
            if (unitComponent != null)
            {
                UnitType unitType = unitComponent.unitData.unitType;

                // Create a parent GameObject for this unit type if it doesn't exist
                GameObject unitParent = new GameObject(unitType.ToString());
                unitParent.transform.SetParent(transform); // Attach to ObjectPooler

                // Initialize the queue for this unit type
                unitPools[unitType] = new Queue<GameObject>();

                for (int i = 0; i < poolSize; i++)
                {
                    GameObject unit = Instantiate(prefab, unitParent.transform); // Parent under its type group
                    unit.SetActive(false);
                    unitPools[unitType].Enqueue(unit);
                }
            }
            else
            {
                Debug.LogError($"Prefab {prefab.name} is missing Unit component!");
            }
        }
    }


    /// <summary>
    /// Retrieves a unit of the requested type from the pool, or instantiates a new one if needed.
    /// </summary>
    public GameObject GetUnit(UnitType unitType)
    {
        if (unitPools.ContainsKey(unitType) && unitPools[unitType].Count > 0)
        {
            GameObject unit = unitPools[unitType].Dequeue();
            unit.SetActive(true);
            return unit;
        }
        else
        {
            // Find the correct unit prefab
            GameObject prefab = unitPrefabs.Find(p => p.GetComponent<Unit>().unitData.unitType == unitType);
            if (prefab != null)
            {
                return Instantiate(prefab);
            }
            else
            {
                Debug.LogError($"No prefab found for unit type {unitType}");
                return null;
            }
        }
    }

    /// <summary>
    /// Returns a unit back to the correct pool.
    /// </summary>
    public void ReturnUnit(GameObject unit)
    {
        Unit unitComponent = unit.GetComponent<Unit>();
        if (unitComponent != null && unitPools.ContainsKey(unitComponent.unitData.unitType))
        {
            unit.SetActive(false);
            unitPools[unitComponent.unitData.unitType].Enqueue(unit);
        }
        else
        {
            Debug.LogError("Returned unit does not match any pool, destroying instead.");
            Destroy(unit);
        }
    }
}
