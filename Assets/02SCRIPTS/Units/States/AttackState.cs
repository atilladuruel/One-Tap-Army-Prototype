using UnityEngine;
using Game.Player;

namespace Game.Units.States
{
    public class AttackState : IUnitState
    {
        private IDamageable target;
        private bool isRanged;

        public AttackState(IDamageable enemy, bool ranged)
        {
            target = enemy;
            isRanged = ranged;
        }

        public void EnterState(Unit unit)
        {
            if (isRanged)
            {
                unit.PlayAnimation("Shoot");
                Debug.Log($"{unit.name} is shooting at {target}!");
                RangedAttack(unit);
            }
            else
            {
                unit.PlayAnimation("MeleeAttack");
                Debug.Log($"{unit.name} is attacking {target}!");
                MeleeAttack(unit);
            }
        }

        public void UpdateState(Unit unit)
        {
            if (target == null || !target.IsAlive())
            {
                unit.ChangeState(new IdleState()); // Switch to Idle when the target is dead
            }
        }

        public void ExitState()
        {
            Debug.Log("Exiting Attack State.");
        }

        private void MeleeAttack(Unit unit)
        {
            target.TakeDamage(unit.AttackPower);
        }

        private void RangedAttack(Unit unit)
        {
            UnitAttack attack = unit.GetComponent<UnitAttack>();

            if (attack.arrowPrefab != null && attack.firePoint != null)
            {
                GameObject arrow = Object.Instantiate(attack.arrowPrefab, attack.firePoint.position, Quaternion.identity);
                Arrow arrowComponent = arrow.GetComponent<Arrow>();

                if (arrowComponent != null)
                {
                    arrowComponent.SetTarget(((Unit)target).transform, unit.AttackPower);
                }
            }
            else
            {
                Debug.LogError("ArrowPrefab or FirePoint is not set for Archer!");
            }
        }
    }
}
