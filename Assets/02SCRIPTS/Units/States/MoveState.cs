using UnityEngine;

public class MoveState : IUnitState
{
    private Vector3 target;
    public MoveState(Vector3 targetPosition)
    {
        target = targetPosition;
    }
    public void EnterState(Unit unit)
    {
        if (unit.agent != null)
        {
            unit.agent.SetDestination(target);
        }
    }
    public void UpdateState(Unit unit)
    {
        if (unit.agent != null && !unit.agent.pathPending && unit.agent.remainingDistance <= unit.agent.stoppingDistance)
        {
            unit.ChangeState(new IdleState());
        }
    }
    public void ExitState() { }
}
