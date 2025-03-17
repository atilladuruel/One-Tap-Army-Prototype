using UnityEngine;

namespace Game.Units.States
{
    public class RangedAttackState : IUnitState
    {
        private Unit target;

        public RangedAttackState(Unit enemy)
        {
            target = enemy;
        }

        public void EnterState(Unit unit)
        {
            unit.PlayAnimation("Shoot"); // Play ranged attack animation
            Debug.Log($"{unit.name} is shooting at {target.name}!");
            UnitAttack attack = unit.GetComponent<UnitAttack>();

            if (attack.arrowPrefab != null && attack.firePoint != null)
            {
                GameObject arrow = Object.Instantiate(attack.arrowPrefab, attack.firePoint.position, Quaternion.identity);
                Arrow arrowComponent = arrow.GetComponent<Arrow>();

                if (arrowComponent != null)
                {
                    arrowComponent.SetTarget(target.transform, unit.AttackPower);
                }
            }
            else
            {
                Debug.LogError("ArrowPrefab or FirePoint is not set for Archer!");
            }
        }

        public void UpdateState(Unit unit)
        {
            if (target == null || target.Health <= 0)
            {
                unit.ChangeState(new IdleState()); // Return to idle if target is dead
            }
        }

        public void ExitState()
        {
            Debug.Log("Exiting Ranged Attack State.");
        }
    }
}
