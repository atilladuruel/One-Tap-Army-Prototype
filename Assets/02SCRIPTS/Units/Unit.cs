using UnityEngine.AI;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    public NavMeshAgent agent;
    public UnitData unitData;

    private int level = 1;
    private string unitName;
    private int health;
    private float speed;
    private int attackPower;
    private IUnitState currentState;

    public event System.Action OnLevelUp; // Event to notify level up

    public int Level
    {
        get { return level; }
        set { if (value > 0) level = value; }
    }

    public string UnitName
    {
        get { return unitName; }
        set { unitName = value; }
    }

    public int Health
    {
        get { return health; }
        set { health = Math.Max(0, value); } // Saðlýk 0'ýn altýna düþmesin
    }

    public float Speed
    {
        get { return speed; }
        set { speed = Math.Max(0, value); } // Hýz negatif olamaz
    }

    public int AttackPower
    {
        get { return attackPower; }
        set { attackPower = Math.Max(0, value); } // Saldýrý gücü negatif olamaz
    }

    private void Awake()
    {
        if (unitData != null) // Eðer birim verisi atanmýþsa, baþlangýç deðerlerini set et
        {
            unitName = unitData.unitName;
            health = unitData.health;
            speed = unitData.speed;
            attackPower = unitData.attack;
        }

        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        ChangeState(new IdleState());
    }

    private void Update()
    {
        currentState?.UpdateState(this);

        // Eðer hedefe ulaþýldýysa, IdleState'e geç
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            ChangeState(new IdleState());
        }
    }

    public void ChangeState(IUnitState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState(this);
    }

    public void MoveTo(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);
        ChangeState(new MoveState(targetPosition));
    }

    /// <summary>
    /// Increases the unit's level and resets XP progress.
    /// </summary>
    private void LevelUp()
    {
        level++; // Level artýr
        OnLevelUp?.Invoke(); // UI veya baþka sistemlere haber ver

        Debug.Log($"{unitName} leveled up to {level}!");
    }

    /// <summary>
    /// Returns the current level of the unit.
    /// </summary>
    public int GetLevel()
    {
        return level;
    }
}
