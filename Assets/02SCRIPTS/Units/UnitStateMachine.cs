using UnityEngine;

public class UnitStateMachine
{
    private IUnitState currentState;
    private Unit unit;

    public UnitStateMachine(Unit unit)
    {
        this.unit = unit;
    }

    public void ChangeState(IUnitState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState(unit);
    }

    public void Update()
    {
        currentState?.UpdateState(unit);
    }
}
