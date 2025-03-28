using Game.Player;
using Game.Scriptables;
using Game.Units.States;
using UnityEngine;

namespace Game.Units
{
    public class UnitAttack : MonoBehaviour
    {
        private Unit _unit;
        private UnitData _unitData;
        private int _damage;
        private float _lastAttackTime = 0f;
        public GameObject arrowPrefab; // Projectile for ranged attack
        public Transform firePoint; // Where the arrow is fired from (for archers)

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            if (_unit != null && _unit.unitData != null)
            {
                _unitData = _unit.unitData;
                _damage = _unit.unitData.attack;
            }
            else
            {
                Debug.LogError($"{gameObject.name} is missing Unit or UnitData!");
            }
        }

        private void Update()
        {
            if (Time.time >= _lastAttackTime + _unitData.attackCooldown)
            {
                FindAndAttackEnemy();
            }
        }

        private void FindAndAttackEnemy()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _unitData.unitType == UnitType.Archer? _unitData.rangedAttackRange : _unitData.attackRange, LayerMask.GetMask("Unit", "Castle"));

            foreach (Collider col in colliders)
            {
                Unit targetUnit = col.GetComponent<Unit>();
                Castle targetCastle = col.GetComponent<Castle>();

                IDamageable target = null;

                if (targetUnit != null && _unit.IsEnemy(targetUnit))
                {
                    target = targetUnit;
                }
                else if (targetCastle != null && targetCastle.playerID != _unit.playerID)
                {
                    target = targetCastle;
                }

                if (target != null && target.IsAlive())
                {
                    float distance = Vector3.Distance(transform.position, ((MonoBehaviour)target).transform.position);

                    bool isRanged = _unit.unitData.unitType == UnitType.Archer && distance > _unitData.attackRange && distance <= _unitData.rangedAttackRange;

                    _unit.ChangeState(new AttackState(target, isRanged));
                    _lastAttackTime = Time.time;
                    return;
                }
            }
        }




    }
}
