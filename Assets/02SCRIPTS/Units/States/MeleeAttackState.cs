using UnityEngine;

namespace Game.Units.States
{
    public class MeleeAttackState : IUnitState
    {
        private Unit target;
        public MeleeAttackState(Unit enemy)
        {
            target = enemy;
        }
        public void EnterState(Unit unit)
        {
            unit.PlayAnimation("MeleeAttack");
            Debug.Log(unit.UnitName + " is attacking!");
        }
        public void UpdateState(Unit unit)
        {
            if (target == null || target.Health <= 0)
            {
                unit.ChangeState(new IdleState());
            }
        }
        public void ExitState() { }
    }
}
