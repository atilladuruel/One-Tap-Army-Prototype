using UnityEngine;

public class UnitAttack : MonoBehaviour
{
    private Unit unit;
    private int damage;
    public float attackRange = 1f; // Melee attack range
    public float rangedAttackRange = 3f; // Archer's ranged attack range
    public float awarenessRange = 5f; // Archer's ranged attack range
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;
    public GameObject arrowPrefab; // Projectile for ranged attack
    public Transform firePoint; // Where the arrow is fired from (for archers)

    private void Awake()
    {
        unit = GetComponent<Unit>();
        if (unit != null && unit.unitData != null)
        {
            damage = unit.unitData.attack;
        }
        else
        {
            Debug.LogError($"{gameObject.name} is missing Unit or UnitData!");
        }
    }

    private void Update()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            FindAndAttackEnemy();
        }
    }

    private void FindAndAttackEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, awarenessRange, LayerMask.GetMask("Unit"));

        //Debug.Log($"{unit.UnitName} is scanning for enemies, found: {colliders.Length}");

        foreach (Collider col in colliders)
        {
            Unit targetUnit = col.GetComponent<Unit>();

            if (targetUnit != null && unit.IsEnemy(targetUnit))
            {
                float distance = Vector3.Distance(transform.position, targetUnit.transform.position);

                if (unit.unitData.unitType == UnitType.Archer && distance > attackRange && distance <= rangedAttackRange)
                {
                    RangedAttack(targetUnit); // Archer shoots an arrow if within ranged range
                }
                else if (distance <= attackRange)
                {
                    Attack(targetUnit); // Melee attack
                }

                lastAttackTime = Time.time;
                break;
            }
        }
    }

    private void Attack(Unit target)
    {
        Debug.Log($"{unit.UnitName} is attacking {target.UnitName} with {damage} damage!");
        target.TakeDamage(damage);
    }

    private void RangedAttack(Unit target)
    {
        Debug.Log($"{unit.UnitName} is shooting an arrow at {target.UnitName}!");

        if (arrowPrefab != null && firePoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
            Arrow arrowComponent = arrow.GetComponent<Arrow>();
            if (arrowComponent != null)
            {
                arrowComponent.SetTarget(target.transform, damage);
            }
        }
        else
        {
            Debug.LogError("ArrowPrefab or FirePoint is not set for Archer!");
        }
    }
}
