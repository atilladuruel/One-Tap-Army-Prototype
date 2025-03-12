using UnityEngine;

public class MoveState : IUnitState
{
    public void EnterState(Unit unit)
    {
        Debug.Log(unit.name + " moving.");
    }

    public void UpdateState(Unit unit)
    {
        // Birim hedefe ilerlesin
    }

    public void ExitState() { }
}
