using UnityEngine;
using Game.Player;

namespace Game.Units.States
{
    /// <summary>
    /// State for attacking enemy castles.
    /// </summary>
    public class AttackCastleState : IUnitState
    {
        private Castle targetCastle;
        private float attackCooldown = 1.5f;
        private float lastAttackTime = 0f;

        public AttackCastleState(Castle castle)
        {
            targetCastle = castle;
        }

        public void EnterState(Unit unit)
        {
            unit.PlayAnimation("Attack"); // Play attack animation
            Debug.Log($"{unit.name} is attacking {targetCastle.ownerName}'s Castle!");
        }

        public void UpdateState(Unit unit)
        {
            if (targetCastle == null || !targetCastle.IsAlive()) // If castle is destroyed
            {
                unit.ChangeState(new IdleState()); // Return to idle state
                return;
            }

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                targetCastle.TakeDamage(unit.AttackPower); // Inflict damage on the castle
                lastAttackTime = Time.time;
            }
        }

        public void ExitState()
        {
            Debug.Log("Exiting Attack Castle State.");
        }
    }
}
