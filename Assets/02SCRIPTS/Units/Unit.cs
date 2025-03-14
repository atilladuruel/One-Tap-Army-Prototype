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

    public int Level
    {
        get { return level; }
        set
        {
            if (value > 0)
                level = value;
        }
    }

    public string UnitName
    {
        get { return unitName; }
        set { unitName = value; }
    }

    public int Health
    {
        get { return health; }
        set
        {
            if (value < 0) health = 0; // Saðlýk 0'ýn altýna düþmesin
            else health = value;
        }
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


    public event System.Action OnLevelUp; // Event to notify level up

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        ChangeState(new IdleState());
    }

    private void Update()
    {
        currentState?.UpdateState(this);
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
        level++; // Increase required XP for next level
        OnLevelUp?.Invoke(); // Notify listeners (e.g., HUD)

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
