using UnityEngine;

public class UnitAttack : MonoBehaviour
{
    public Unit unit;
    public float attackRange = 2f;
    public int damage = 10;

    private void Update()
    {
        FindAndAttackEnemy();
    }

    private void FindAndAttackEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Unit"));
        foreach (Collider col in colliders)
        {
            Unit targetUnit = col.GetComponent<Unit>();

            if (targetUnit != null && unit.IsEnemy(targetUnit)) // Only attack enemies
            {
                Attack(targetUnit);
                break;
            }
        }
    }

    private void Attack(Unit target)
    {
        //target.TakeDamage(damage);
    }
}
