using UnityEngine;

public class DeathState : IUnitState
{
    public void EnterState(Unit unit)
    {
        Debug.Log(unit.name + " has died.");
        unit.HandleDeath();
    }

    public void UpdateState(Unit unit) { }

    public void ExitState() { }
}
