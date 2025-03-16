using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerID = 0;
    public Castle castle;
    public List<Unit> activeUnits = new List<Unit>();
    public PlayerLevel playerLevel;
    private Camera mainCamera;
    private UnitData selectedUnitData;
    private Coroutine spawnRoutine;

    [Header("Default Unit")]
    public UnitData defaultUnit; // The default unit to be spawned at game start

    private void Awake()
    {
        playerLevel = new PlayerLevel(this);
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (defaultUnit != null)
        {
            SelectUnit(defaultUnit);
        }
    }

    /// <summary>
    /// Spawns a unit of the selected type at the castle's spawn point.
    /// </summary>
    public void SpawnUnit()
    {
        if (selectedUnitData == null || GameManager.Instance.isGameOver) return;

        // Get the spawn position from the castle
        Vector3 spawnPosition = castle.spawnPoint.position;
        Quaternion spawnRotation = Quaternion.identity;

        // Add slight random offset to prevent unit overlap
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
        spawnPosition += randomOffset;

        // Get a unit from the object pool and set its position
        GameObject newUnit = ObjectPooler.Instance.GetUnit(selectedUnitData.unitType, spawnPosition);
        newUnit.transform.position = spawnPosition;
        newUnit.transform.rotation = spawnRotation;

        // Add the unit to the active list
        Unit unit = null;
        newUnit.TryGetComponent<Unit>(out unit);
        if (unit != null)
        {
            unit.Initialize(playerID); // Call Initialize() with the player's ID
            activeUnits.Add(unit);
        }
    }

    private void Update()
    {
        // Handle touch input for mobile devices
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                HandleTouchOrClick(touch.position);
            }
        }
        // Handle mouse click input for PC
        else if (Input.GetMouseButtonDown(0))
        {
            HandleTouchOrClick(Input.mousePosition);
        }
    }

    /// <summary>
    /// Handles both touch and mouse click input to select or move units.
    /// </summary>
    private void HandleTouchOrClick(Vector3 screenPosition)
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Unit")) // Select a unit if clicked/tapped
            {
                SelectUnit(hit.collider.GetComponent<Unit>().unitData);
            }
            else if (selectedUnitData != null) // If ground is clicked, move units in formation
            {
                MoveSelectedUnits(hit.point);
            }
        }
    }

    /// <summary>
    /// Selects a unit type from the UI and starts continuous spawning for that unit.
    /// </summary>
    public void SelectUnit(UnitData newUnitData)
    {
        if (newUnitData == null || GameManager.Instance.isGameOver)
            return;

        selectedUnitData = newUnitData;

        // Stop the previous spawn routine if it's running
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        // Start spawning the selected unit continuously
        spawnRoutine = StartCoroutine(SpawnSelectedUnitContinuously());
    }

    /// <summary>
    /// Continuously spawns the selected unit type at regular intervals.
    /// </summary>
    private IEnumerator SpawnSelectedUnitContinuously()
    {
        while (!GameManager.Instance.isGameOver && selectedUnitData != null)
        {
            SpawnUnit();
            yield return new WaitForSeconds(selectedUnitData.spawnTime);
        }
    }

    /// <summary>
    /// Moves all selected units to a formation around the given target position.
    /// </summary>
    public void MoveSelectedUnits(Vector3 targetPosition)
    {
        if (activeUnits.Count == 0) return;

        List<Vector3> formationPositions;

        // Choose the formation type dynamically
        if (activeUnits.Count > 5)
            formationPositions = GetGridFormation(targetPosition, activeUnits.Count);
        else
            formationPositions = GetFormationPositions(targetPosition, activeUnits.Count);

        for (int i = 0; i < activeUnits.Count; i++)
        {
            if (activeUnits[i] != null)
            {
                activeUnits[i].MoveTo(formationPositions[i]); // Move each unit to its designated spot
            }
        }
    }

    /// <summary>
    /// Generates a circular formation around the target position.
    /// </summary>
    private List<Vector3> GetFormationPositions(Vector3 targetPosition, int unitCount)
    {
        List<Vector3> positions = new List<Vector3>();
        float radius = 1.0f; // Distance from the center
        float angleStep = 360f / unitCount; // Spread units evenly in a circle

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
    private List<Vector3> GetGridFormation(Vector3 targetPosition, int unitCount, int rowSize = 3, float spacing = .5f)
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

    /// <summary>
    /// Called when a unit levels up. Can be used to update UI or give bonuses.
    /// </summary>
    private void HandleUnitLevelUp()
    {
        Debug.Log("A unit has leveled up!");
    }
}
