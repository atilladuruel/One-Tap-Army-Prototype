using UnityEngine;

public class DeathState : IUnitState
{
    public void EnterState(Unit unit)
    {
        unit.gameObject.SetActive(false);
        ObjectPooler.Instance?.ReturnUnit(unit.gameObject);
    }
    public void UpdateState(Unit unit) { }
    public void ExitState() { }
}
