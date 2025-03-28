using Game.Core;
using UnityEngine;
namespace Game.Units.States
{
    public class DeathState : IUnitState
    {
        public void EnterState(Unit unit)
        {
            unit.agent.enabled = false;
            unit.PlayAnimation("Death");
            Debug.Log($"{unit.name} has died.");
        }
        public void UpdateState(Unit unit) { }
        public void ExitState(Unit unit)
        {
            Debug.Log("Exiting Death State.");
        }
    }
}
