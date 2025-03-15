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
    public UnitData defaultUnit; // Oyun baþladýðýnda ilk spawnlanacak unit

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

    public void Initialize(Transform spawnPosition)
    {
        transform.position = spawnPosition.position;
    }

    /// <summary>
    /// Spawns a unit of the selected type at the castle's spawn point.
    /// </summary>
    public void SpawnUnit()
    {
        if (selectedUnitData == null || GameManager.Instance.isGameOver) return;

        GameObject newUnit = ObjectPooler.Instance.GetUnit(selectedUnitData.unitType);
        Unit unit = null;
        newUnit.TryGetComponent<Unit>(out unit);
        if (unit != null)
            activeUnits.Add(unit);
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
            if (hit.collider.CompareTag("Unit")) // If a unit is tapped/clicked, select it
            {
                SelectUnit(hit.collider.GetComponent<Unit>().unitData);
            }
            else if (selectedUnitData != null) // If ground is tapped/clicked, move units
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

        // Eðer zaten bir spawn süreci çalýþýyorsa durdur
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        // Yeni spawn sürecini baþlat
        spawnRoutine = StartCoroutine(SpawnSelectedUnitContinuously());
    }

    /// <summary>
    /// Spawns the selected unit at regular intervals.
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
    /// Moves all selected units to the given target position.
    /// </summary>
    public void MoveSelectedUnits(Vector3 targetPosition)
    {
        foreach (Unit unit in activeUnits)
        {
            if (unit != null)
            {
                unit.MoveTo(targetPosition);
            }
        }
    }

    /// <summary>
    /// Called when a unit levels up. Can be used to update UI or give bonuses.
    /// </summary>
    private void HandleUnitLevelUp()
    {
        Debug.Log("A unit has leveled up!");
    }
}
