using UnityEngine;

public class AttackState : IUnitState
{
    public void EnterState(Unit unit)
    {
        Debug.Log(unit.unitName + " is attacking!");
    }
    public void UpdateState(Unit unit)
    {
        // Implement attack logic here
    }
    public void ExitState() { }
}
