using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public int aiID = 1; // AI identifier
    public Castle castle;
    public List<Unit> activeUnits = new List<Unit>();
    private UnitData selectedUnitData;
    private Coroutine spawnRoutine;

    [Header("Default Unit")]
    public UnitData defaultUnit; // The default unit AI will spawn

    private void Start()
    {
        if (defaultUnit != null)
        {
            SelectUnit(defaultUnit);
        }

        // Start AI decision-making loop
        StartCoroutine(AIBehaviorLoop());
    }

    /// <summary>
    /// Spawns a unit at the AI's spawn point.
    /// </summary>
    private void SpawnUnit()
    {
        if (selectedUnitData == null || GameManager.Instance.isGameOver) return;

        Vector3 spawnPosition = castle.spawnPoint.position;
        Quaternion spawnRotation = Quaternion.identity;

        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
        spawnPosition += randomOffset;

        GameObject newUnit = ObjectPooler.Instance.GetUnit(selectedUnitData.unitType);
        newUnit.transform.position = spawnPosition;
        newUnit.transform.rotation = spawnRotation;

        Unit unit = null;
        newUnit.TryGetComponent<Unit>(out unit);
        if (unit != null)
            activeUnits.Add(unit);
    }

    public void SelectUnit(UnitData newUnitData)
    {
        if (newUnitData == null || GameManager.Instance.isGameOver)
            return;

        selectedUnitData = newUnitData;

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        spawnRoutine = StartCoroutine(SpawnSelectedUnitContinuously());
    }

    private IEnumerator SpawnSelectedUnitContinuously()
    {
        while (!GameManager.Instance.isGameOver && selectedUnitData != null)
        {
            SpawnUnit();
            yield return new WaitForSeconds(selectedUnitData.spawnTime);
        }
    }

    /// <summary>
    /// AI decision-making loop: Decides whether to attack or defend.
    /// </summary>
    private IEnumerator AIBehaviorLoop()
    {
        while (!GameManager.Instance.isGameOver)
        {
            yield return new WaitForSeconds(3f); // AI makes a new decision every 3 seconds

            if (activeUnits.Count == 0)
            {
                continue; // Skip if AI has no units
            }

            if (IsUnderAttack()) // Check if the AI is under attack
            {
                MoveUnitsInFormation(castle.transform.position); // Defend the base
            }
            else
            {
                Vector3 targetPosition = FindAttackPosition();
                MoveUnitsInFormation(targetPosition); // Attack enemy base
            }
        }
    }

    /// <summary>
    /// Checks if AI's castle is under attack (example condition).
    /// </summary>
    private bool IsUnderAttack()
    {
        // Placeholder: Check if enemy units are near the AI castle
        Collider[] enemies = Physics.OverlapSphere(castle.transform.position, 10f, LayerMask.GetMask("Unit"));
        return enemies.Length > 3; // If more than 3 enemy units are near, consider it an attack
    }

    /// <summary>
    /// Finds an attack target (e.g., the enemy castle).
    /// </summary>
    private Vector3 FindAttackPosition()
    {
        Castle enemyCastle = FindObjectOfType<PlayerController>().castle;

        if (enemyCastle != null)
        {
            return enemyCastle.transform.position;
        }

        return transform.position;
    }

    /// <summary>
    /// Moves AI units in formation towards the target position.
    /// </summary>
    private void MoveUnitsInFormation(Vector3 targetPosition)
    {
        if (activeUnits.Count == 0) return;

        List<Vector3> formationPositions;

        if (activeUnits.Count > 5)
            formationPositions = GetGridFormation(targetPosition, activeUnits.Count);
        else
            formationPositions = GetFormationPositions(targetPosition, activeUnits.Count);

        for (int i = 0; i < activeUnits.Count; i++)
        {
            if (activeUnits[i] != null)
            {
                activeUnits[i].MoveTo(formationPositions[i]);
            }
        }
    }

    /// <summary>
    /// Generates a circular formation around the target position.
    /// </summary>
    private List<Vector3> GetFormationPositions(Vector3 targetPosition, int unitCount)
    {
        List<Vector3> positions = new List<Vector3>();
        float radius = 2.0f;
        float angleStep = 360f / unitCount;

        for (int i = 0; i < unitCount; i++)
        {
            float angle = i * angleStep;
            float x = targetPosition.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = targetPosition.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            positions.Add(new Vector3(x, targetPosition.y, z));
        }

        return positions;
    }

    /// <summary>
    /// Generates a grid formation around the target position.
    /// </summary>
    private List<Vector3> GetGridFormation(Vector3 targetPosition, int unitCount, int rowSize = 3, float spacing = 2.0f)
    {
        List<Vector3> positions = new List<Vector3>();

        int rows = Mathf.CeilToInt((float)unitCount / rowSize);
        int cols = Mathf.Min(unitCount, rowSize);
        int index = 0;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (index >= unitCount) break;

                float x = targetPosition.x + (c - (cols / 2)) * spacing;
                float z = targetPosition.z + (r - (rows / 2)) * spacing;
                positions.Add(new Vector3(x, targetPosition.y, z));

                index++;
            }
        }
        return positions;
    }
}
