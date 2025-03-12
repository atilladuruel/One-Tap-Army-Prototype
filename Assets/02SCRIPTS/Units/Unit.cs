using UnityEngine;

public class Unit : MonoBehaviour
{
    public PlayerController owner;
    public UnitStateMachine stateMachine;
    public UnitType unitType;
    public bool isDead = false;

    private void Awake()
    {
        stateMachine = new UnitStateMachine(this);
    }

    public void Initialize(PlayerController player, Vector3 spawnPosition)
    {
        owner = player;
        transform.position = spawnPosition;
        stateMachine.ChangeState(new IdleState());
    }

    private void Update()
    {
        stateMachine.Update();
    }

    public void MoveTo(Vector3 destination)
    {
        if (!isDead)
            stateMachine.ChangeState(new MoveState());
    }

    public void Attack()
    {
        if (!isDead)
            stateMachine.ChangeState(new AttackState());
    }

    public void Die()
    {
        if (!isDead)
        {
            isDead = true;
            stateMachine.ChangeState(new DeathState());
        }
    }

    public void HandleDeath()
    {
        // Birim ölüm animasyonu oynatabilir
        // Birim oyun dünyasýndan kaldýrýlabilir
        Destroy(gameObject, 2f); // 2 saniye sonra birimi yok et
    }
}
