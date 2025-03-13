using UnityEngine.AI;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int health;
    public float speed;
    public int attackPower;
    public NavMeshAgent agent;

    private IUnitState currentState;

    // New XP & Level system
    private int xp = 0;
    private int level = 1;
    private int xpToNextLevel = 100; // XP required for next level

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
    /// Grants experience points to the unit and checks for level-up.
    /// </summary>
    public void GainXP(int amount)
    {
        xp += amount;
        if (xp >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    /// <summary>
    /// Increases the unit's level and resets XP progress.
    /// </summary>
    private void LevelUp()
    {
        level++;
        xp -= xpToNextLevel;
        xpToNextLevel += 50; // Increase required XP for next level
        OnLevelUp?.Invoke(); // Notify listeners (e.g., HUD)

        Debug.Log($"{unitName} leveled up to {level}!");
    }

    /// <summary>
    /// Returns the current XP value.
    /// </summary>
    public int GetXP()
    {
        return xp;
    }

    /// <summary>
    /// Returns the current level of the unit.
    /// </summary>
    public int GetLevel()
    {
        return level;
    }
}
