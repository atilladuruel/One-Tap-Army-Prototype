using UnityEngine;

public class AttackState : IUnitState
{
    public void EnterState(Unit unit)
    {
        Debug.Log(unit.name + " attacking.");
    }

    public void UpdateState(Unit unit)
    {
        // Birim saldýrý yapsýn
    }

    public void ExitState() { }
}
