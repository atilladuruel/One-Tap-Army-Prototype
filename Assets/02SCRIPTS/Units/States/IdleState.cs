using UnityEngine;
namespace Game.Units.States
{
    // Unit States
    public class IdleState : IUnitState
    {
        public void EnterState(Unit unit)
        {
            unit.PlayAnimation("Idle"); // Play Idle animation
            Debug.Log($"{unit.name} has entered Idle State.");
        }
        public void UpdateState(Unit unit) { }
        public void ExitState(Unit unit) { }
    }
}