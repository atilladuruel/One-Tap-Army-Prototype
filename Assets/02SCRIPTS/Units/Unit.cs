using UnityEngine.AI;
using UnityEngine;
using System;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public int playerID;
    public List<Renderer> unitRenderer;
    public NavMeshAgent agent;
    public UnitData unitData;

    public Vector3 target;

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
        set { health = Math.Max(0, value); } // Ensure health doesn't go below 0
    }

    public float Speed
    {
        get { return speed; }
        set { speed = Math.Max(0, value); } // Ensure speed isn't negative
    }

    public int AttackPower
    {
        get { return attackPower; }
        set { attackPower = Math.Max(0, value); } // Ensure attack power isn't negative
    }

    private void Awake()
    {
        if (unitData != null) // Initialize unit stats from UnitData
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

        // If unit reaches its destination, switch to IdleState
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
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 2f, NavMesh.AllAreas))
        {
            //agent.SetDestination(hit.position);
            ChangeState(new MoveState(hit.position));
        }
    }

    /// <summary>
    /// Increases the unit's level and resets XP progress.
    /// </summary>
    private void LevelUp()
    {
        level++; // Increase level
        OnLevelUp?.Invoke(); // Notify UI or other systems

        Debug.Log($"{unitName} leveled up to {level}!");
    }

    /// <summary>
    /// Returns the current level of the unit.
    /// </summary>
    public int GetLevel()
    {
        return level;
    }

    public void Initialize(int ownerID)
    {
        playerID = ownerID;
        ApplyTeamColor();
    }

    private void ApplyTeamColor()
    {
        Color teamColor = PlayerManager.Instance.GetPlayerByID(playerID).teamColor;
        foreach (var renderer in unitRenderer)
        {
            renderer.material.color = teamColor;
        }
    }

    public bool IsEnemy(Unit other)
    {
        return playerID != other.playerID; // If they have different IDs, they are enemies
    }

    /// <summary>
    /// Takes damage from enemy attacks.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (health <= 0) return; // Already dead

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Handles unit death and returns to Object Pool.
    /// </summary>
    private void Die()
    {
        Debug.Log($"{unitName} has been destroyed!");

        // Return unit to Object Pool instead of disabling
        ObjectPooler.Instance.ReturnUnit(gameObject);
    }
}
